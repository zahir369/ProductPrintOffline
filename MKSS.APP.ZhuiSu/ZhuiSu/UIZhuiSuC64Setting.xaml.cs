using Microsoft.Win32;
using MKSS.APP.ZhuiSu.UserControls;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC64Setting.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC64Setting : UserControl
    {
        UIZhuiSuC64Model _model {
            get { return UIZhuiSuC64Model.Instance;
            }
        }
        public UIZhuiSuC64Setting()
        {
            InitializeComponent();
            if (_model == null) return;

            this.BtnFinishBoard.IsEnabled = false;
            this.TxtBatchList.Items.Clear();
            IniData(null);
        } 

        public int[] TxtTestTimePointsArr
        {
            get
            {
                List<int> ret = new List<int>();
                string[] arr = (TxtTestTimePoints.Text + "").Split(",");
                foreach (var item in arr)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    ret.Add(int.Parse(item));
                }
                return ret.ToArray();
            }
        }

        public void IniData(Batch his)
        {
            List<Batch> bs = UIZhuiSuData.Instance.DBListBatch.ToList();
            if (his != null)
            {
                bs.Add(his);
            }
            this.TxtBatchList.ItemsSource = bs;
            if (bs.Count > 0 && (TxtBatchList.SelectedItem == null || (TxtBatchList.SelectedItem != null && !bs.Contains(TxtBatchList.SelectedItem))))
            {
                TxtBatchList.SelectedItem = his == null ? bs[0] : his;
            }
            if (his != null)
            {
                TxtBatchList.SelectedItem = his;
            }
            if (bs.Count == 0)
            {
                TxtBatchList.SelectedItem = null;
                this.BtnFinishBoard.IsEnabled = false;
            }

        }

        Batch Batch { get; set; }
        List<PageSensorModel> Sensors { get; set; }
        public void SetData(PageBoardItem data, PageBoardItem dataB, Batch _batch)
        {
            Batch = _batch; 
            this.DataContext = data;
            BoardCase _BoardCase = null;
            Board _Board = null;
            Batch _Batch = UIZhuiSuData.Instance.OfBatch(data, ref _BoardCase, ref _Board);
            _Batch = Batch;

            List<PageSensorModel> tables = new List<PageSensorModel>();
            tables.AddRange(data.ProductTable);
            tables.AddRange(dataB.ProductTable);
            Sensors = tables;

        }


        private void TxtBatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TxtBatchList.SelectedItem == null && Batch != null) {
                //this.TxtBatchList.SelectedItem = Batch;
            }
            if (_model == null) return;
            Batch batch = this.TxtBatchList.SelectedItem as Batch;
            if (batch != null)
            {
                this.BtnFinishBoard.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
                this.TxtTestTimePoints.Text = batch.F_TestTimePoints;
            }
            if(_model.View==null) return;
            _model.View.SelectBatch = batch;
            //更新列配置
            PageSensorBase.Seconds = UIZhuiSuC64Model.Instance.View.UIZhuiSuC64Setting1.TxtTestTimePointsArr;
            _model.RefreshAddress(batch);
            _model.DataTimerRefresh(batch);  
             
        }

        private void BtnAddBatch_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            UIZhuiSuAdd w = new UIZhuiSuAdd();
            w.Topmost = true;
            bool? sucess = w.ShowDialog();
            if (sucess != null && sucess.Value)
            {

            }
            UIZhuiSuData.Instance.RefreshData();
            IniData(null);

        }


        private void BtnRegisterSensors_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            Batch batch = Batch;
            if (batch != null)
            {

            }
            if (batch == null || _model.View == null) return;

            UIZhuiSuC64SensorsAdd w = new UIZhuiSuC64SensorsAdd();
            w.Topmost = true;
            w.SetData(batch, Sensors);
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bool? sucess = w.ShowDialog();
            if (sucess != null && sucess.Value)
            {

            }

        }
        private void BtnFinishBoard_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            Batch _Batch = Batch;
            if (_Batch != null)
            {

                if (MessageBox.Show(
                    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UIZhuiSuData.Instance.FinishBatch(_Batch);
                    UIZhuiSuC64Model.Instance.RefreshAddress(_Batch);
                    IniData(null);
                }

            }
        }

        private void BtnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            Batch _Batch = Batch;
            if (_Batch != null)
            {
                string name = _Batch.F_BatchName.Replace("#", "-");
                var dlg = new SaveFileDialog()
                {
                    Title = _Batch.F_BatchName + "-另存为",
                    DefaultExt = "txt",
                    Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                    FileName = name
                };
                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        Cursor = Cursors.Wait;
                        //_model.View.ProgressNow.Visibility = System.Windows.Visibility.Visible;
                        WaitWindow.ShowWindow("正在导出", "正在导出成果，请稍候......",this);
                        _model.ExportExcel(dlg.FileName, _Batch);
                        Cursor = Cursors.Arrow;
                        WaitWindow.CloseWindow( this);
                        //_model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        WaitWindow.CloseWindow(this);
                        //_model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }
            }
        }

    }
}
