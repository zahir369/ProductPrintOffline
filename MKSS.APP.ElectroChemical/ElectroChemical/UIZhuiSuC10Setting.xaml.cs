using Microsoft.Win32;
using MKSS.Model;
using MKSS.Service.ElectroChemical;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace MKSS.APP.ElectroChemical
{


    /// <summary>
    /// UIZhuiSuC10Setting.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Setting : UserControl
    {

        UIZhuiSuC10Model _model {
            get { return UIZhuiSuC10Model.Instance;
            }
        }
        public UIZhuiSuC10SettingModel SettingModel;
        public UIZhuiSuC10Setting()
        {
            InitializeComponent();
            if (_model == null) return;
            SettingModel = this.DataContext as UIZhuiSuC10SettingModel;
            _model.SettingModel = SettingModel;

            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (SettingModel !=null && item == SettingModel.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }
            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count > 0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }

            ProductConfig selP = null;
            this.TxtProductList.Items.Clear();
            SettingModel.TxtProductList = ElectroChemicalConfgig.Instance.ProductList;
            foreach (ProductConfig item in ElectroChemicalConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == SettingModel.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                    selP = item;
                }
            }
            if (selP==null && ElectroChemicalConfgig.Instance.Product.Count > 0) {
                this.TxtProductList.SelectedItem = ElectroChemicalConfgig.Instance.Product[0];
                selP = ElectroChemicalConfgig.Instance.Product[0];
            }
            if (selP!=null) {
                this.SettingModel.TxtTotalSpan = selP.TimeTotal;
                this.SettingModel.TxtGradingTimePoint = selP.GradingTimePoint;
                this.SettingModel.TxtTestTimePoints = selP.TestTimePoints;
            }

            this.SettingModel.TxtSerialPorts = ElectroChemicalConfgig.Instance.SerialPorts;
            this.SettingModel.TxtSensorGrougAddress = ElectroChemicalConfgig.Instance.SensorGrougAddress;
            
            IniData(null,null);

        }

        public void RefreshButtons() {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.BtnPause.IsEnabled = this.SettingModel.CanBtnPause;
                this.BtnStart.IsEnabled = this.SettingModel.CanBtnStart;
                this.BtnFinish.IsEnabled = this.SettingModel.CanBtnFinish;
                this.BtnExcelExport.IsEnabled = this.SettingModel.CanBtnExcelExport;
                this.BtnPause.Content = UIZhuiSuC10Model.Instance.ECService.Pause ? "继续" : "暂停";
            }));
            
        }
        public Batch BatchCurrent { get; set; }
        public List<Sensor> SensorsCurrent { get; set; }
        public void IniData(Batch batch, List<Sensor> sens)
        {
            RefreshButtons();

            if (sens == null) sens = new List<Sensor>();

            BatchCurrent = batch;
            SensorsCurrent = sens;
            if (batch != null)
            {
                this.BtnFinish.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
                this.BtnPause.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
            }
            if (_model.View == null) return;
            _model.View.SelectBatch = batch; 
            _model.DataTimerRefresh(batch);
            _model.View.UIZhuiSuC10Chart.SetBatch(batch);
            _model.View.UIZhuiSuC10Grid.IniTask(batch, sens);
            _model.View.UIZhuiSuC10Sensors.IniTask(batch, sens);
            _model.View.UIZhuiSuC10Setting.IniTask(batch, sens);
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(this.SettingModel.TxtTestTimePointsArr);
            List<PosEnum> sel = UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Sensors.SelectSensorIds;
            _model.View.UIZhuiSuC10Chart.ShowSensors(sel);

            if (batch != null) {
                 
            }
        }


        Batch Batch { get; set; }
        public TimeSpan F_AddTime { get; set; }
        public void SetData(Model.Batch _Batch, SensorGroupData   datas, TimeSpan _F_AddTime)
        {
            Batch = _Batch;
            F_AddTime = _F_AddTime;
        }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;

        }

        void SaveConfig()
        {
            if (this.SettingModel == null) return;
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig; 
            ElectroChemicalConfgig.Instance.SerialPorts = this.SettingModel.TxtSerialPorts;
            ElectroChemicalConfgig.Instance.SensorGrougAddress = this.SettingModel.TxtSensorGrougAddress;
            if (this.SettingModel.TxtTotalSpan > 0) c.TimeTotal = this.SettingModel.TxtTotalSpan;
            if (this.SettingModel.TxtGradingTimePoint>0) c.GradingTimePoint = this.SettingModel.TxtGradingTimePoint;
            if (!string.IsNullOrEmpty(TxtTestTimePoints.Text)) c.TestTimePoints = this.SettingModel.TxtTestTimePoints;
            ElectroChemicalConfgig.Instance.ProductList = c.Code;
            ElectroChemicalConfgig.Save();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            SaveConfig();

            if (this.TxtSerialPorts.SelectedItem == null) {
                MessageBox.Show("请选择通信串口。");
                return;
            }

            var testAddr = SettingModel.Address();
            if (testAddr==null || testAddr.Count==0)
            {
                MessageBox.Show("输入有效的通道 "+ SettingModel .TxtSensorGrougAddress+ " 无效。");
                return;
            }

            //测试通道是否正常
            try
            {
                var testSucess = _model.ECService.TestConn(SettingModel.TxtSerialPorts, testAddr);
                if (!testSucess)
                {
                    MessageBox.Show("通道 " + SettingModel.TxtSensorGrougAddress + " 测试无效。");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("通道 " + SettingModel.TxtSensorGrougAddress + " 测试无效："+ ex.Message);
                return;
            }
            

            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null)
            {
                MessageBox.Show("请选择产品名称。");
                return;
            }

            if (SettingModel.TxtTotalSpan <= 10)
            {
                MessageBox.Show("测试总时长最短10秒。");
                return;
            }

            UIZhuiSuAddModel w = new UIZhuiSuAddModel() {
                TxtProductCode = c.Code.ToString(),
                TxtProductName = c.Name,
                TxtTimeTotal = SettingModel.TxtTotalSpan
            };
            w.AddBatch();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetSpan(0,0, SettingModel.TxtTotalSpan);
            List<Sensor> sens = w.SelectSensors;
            _model.ECService.StartTask(w.SelectBatch, w.SelectSensors, c, SettingModel.TxtSerialPorts, testAddr).Start();
            BatchCurrent = w.SelectBatch;
            SettingModel.Started = true;
            this._model.View.UIZhuiSuC10Sensors.SetFontColor();
            UIZhuiSuData.Instance.RefreshData();
            IniData(BatchCurrent,  sens);

        }


        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            SaveConfig();
            Batch _Batch = BatchCurrent;
            if (_Batch != null)
            {

                //if (MessageBox.Show(
                //    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                //    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UIZhuiSuC10Model.Instance.ECService.StopTask();
                    UIZhuiSuData.Instance.FinishBatch(_Batch, F_AddTime.TotalSeconds);
                    UIZhuiSuC10Model.Instance.RefreshAddress(_Batch);
                    SettingModel.Started = false;
                    IniData(BatchCurrent, SensorsCurrent);
                }

            }

        }
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            if (UIZhuiSuC10Model.Instance.ECService.Pause)
            {
                UIZhuiSuC10Model.Instance.ECService.ResumeTask();
                this.BtnPause.Content = "暂停";
            }
            else {
                UIZhuiSuC10Model.Instance.ECService.PauseTask();
                this.BtnPause.Content = "继续";
            }
            
        }

        private void BtnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null) return;
            SaveConfig();
            Batch _Batch = BatchCurrent;
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
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Visible;
                        _model.ExportExcel(dlg.FileName, _Batch);
                        Cursor = Cursors.Arrow;
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }
            }
        }

        private void ButtonColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            Button _Button = sender as Button;
            if (_Button == null) return;
            ColorBoxTest.ColorSelect _ColorSelect = new ColorBoxTest.ColorSelect((_Button.Background));
            
            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {
                _Button.Background = _ColorSelect.SelectColor;
                ProductGrade _ProductGrade = _Button.Tag as ProductGrade;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                _ProductGrade.Color = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A,c1.R,c1.G,c1.B));
                this._model.View.UIZhuiSuC10Sensors.Refresh();


                ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
                if (c!= null)
                {
                    foreach (var item in c.Grades)
                    {
                        if (item.Grade == _ProductGrade.Grade) {
                            item.Color = _ProductGrade.Color;
                        }
                    }
                }
                
            }
        }

        private void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            SettingModel.ProductConfig = c;
            this.BtnGrade1.Tag = c.Grades[0];
            this.BtnGrade2.Tag = c.Grades[1];
            this.BtnGrade3.Tag = c.Grades[2];
            this.BtnGrade4.Tag = c.Grades[3];
            this.BtnGrade5.Tag = c.Grades[4];
            this.BtnGrade6.Tag = c.Grades[5];

            this.SettingModel.TxtTotalSpan = c.TimeTotal;
            this.SettingModel.TxtGradingTimePoint = c.GradingTimePoint;
            this.SettingModel.TxtTestTimePoints = c.TestTimePoints;

            if(UIZhuiSuC10Model.Instance.View!=null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits( );



        }

        private void TxtGrading_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._model.SettingModel == null) return;
            TextBox txt = sender as TextBox;

            try
            {
                if (txt.Name == "TxtGrading1From")
                {
                    this._model.SettingModel.TxtGrading1From = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading2From")
                {
                    this._model.SettingModel.TxtGrading2From = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading3From")
                {
                    this._model.SettingModel.TxtGrading3From = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading4From")
                {
                    this._model.SettingModel.TxtGrading4From = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading5From")
                {
                    this._model.SettingModel.TxtGrading5From = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading6From")
                {
                    this._model.SettingModel.TxtGrading6From = int.Parse(txt.Text);
                }

                if (txt.Name == "TxtGrading1To")
                {
                    this._model.SettingModel.TxtGrading1To = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading2To")
                {
                    this._model.SettingModel.TxtGrading2To = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading3To")
                {
                    this._model.SettingModel.TxtGrading3To = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading4To")
                {
                    this._model.SettingModel.TxtGrading4To = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading5To")
                {
                    this._model.SettingModel.TxtGrading5To = int.Parse(txt.Text);
                }
                if (txt.Name == "TxtGrading6To")
                {
                    this._model.SettingModel.TxtGrading6To = int.Parse(txt.Text);
                }


                TxtGrading1From.Text = this._model.SettingModel.TxtGrading1From.ToString();
                TxtGrading2From.Text = this._model.SettingModel.TxtGrading2From.ToString();
                TxtGrading3From.Text = this._model.SettingModel.TxtGrading3From.ToString();
                TxtGrading4From.Text = this._model.SettingModel.TxtGrading4From.ToString();
                TxtGrading5From.Text = this._model.SettingModel.TxtGrading5From.ToString();
                TxtGrading6From.Text = this._model.SettingModel.TxtGrading6From.ToString();


                TxtGrading1To.Text = this._model.SettingModel.TxtGrading1To.ToString();
                TxtGrading2To.Text = this._model.SettingModel.TxtGrading2To.ToString();
                TxtGrading3To.Text = this._model.SettingModel.TxtGrading3To.ToString();
                TxtGrading4To.Text = this._model.SettingModel.TxtGrading4To.ToString();
                TxtGrading5To.Text = this._model.SettingModel.TxtGrading5To.ToString();
                TxtGrading6To.Text = this._model.SettingModel.TxtGrading6To.ToString();

            }
            catch (Exception)
            {
                 
            }
            

            if (this._model.View!=null) this._model.View.UIZhuiSuC10Sensors.Refresh();
             

        }

        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._model.SettingModel == null) return;
            if (this._model.View == null) return;
            int last = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanLatest;
            if ((int)SliderProgress.Value != last)
            {
                this._model.View.UIZhuiSuC10Chart.SetSpan((int)SliderProgress.Value,0, (int)SliderProgress.Maximum);
                MKSS.Model.SensorGroupData new_data = this._model.ECService[TimeSpan.FromSeconds(SliderProgress.Value)];
                if (new_data != null) {
                    this._model.View.UIZhuiSuC10Sensors.SetData(BatchCurrent, new_data);
                }
            }
        }

        private void TxtTestTimePoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._model.SettingModel == null) return;
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null || string.IsNullOrEmpty(TxtTestTimePoints.Text)) return;
            c.TestTimePoints = TxtTestTimePoints.Text;
             
        }

        private void TxtTotalSpan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._model.SettingModel == null) return;
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null || string.IsNullOrEmpty(TxtTestTimePoints.Text)) return;
            try
            {
                c.TimeTotal = int.Parse(TxtTotalSpan.Text);
            }
            catch (Exception)
            {
                 
            }
        }
    }

}
