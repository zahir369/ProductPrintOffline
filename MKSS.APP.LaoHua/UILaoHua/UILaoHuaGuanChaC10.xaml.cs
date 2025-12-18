using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.Util;
using Microsoft.Win32;
using MKSS.APP.LaoHua.UILaoHua;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Threading;
using MKSS.APP.UserControls;

namespace MKSS.APP.LaoHua
{

    /// <summary>
    ///  UILaoHuaGuanChaC10.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaC10 : UserControl
    {

        UILaoHuaGuanChaC10Model _model = null;
        DispatcherTimer timer = new DispatcherTimer();
        public UILaoHuaGuanChaC10()
        {
            InitializeComponent();

            if (UILaoHuaGuanChaModel.IsInDesignMode(this)) return;//设计模式直接返回
            this.BtnFinishBoard.IsEnabled = false;
            BtnFinishBoardExt.IsEnabled = false;
            _model = base.DataContext as UILaoHuaGuanChaC10Model;
            _model.InitPage(this);

            this.TxtBatchList.Items.Clear();
            IniData(null);

            timer.Interval = new TimeSpan(0, 0, 1);//设置的间隔为一分钟
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();

        }

        DateTime PreExecute = DateTime.MinValue;
        Batch SelectBatch { get; set; }
        public bool TimerRefreshIng { get { return _model.BoardCaseTable.status == sta_job_status.statistcing; } }
        public void Timer_Tick(object sender, EventArgs e)
        {

            if (UILaoHuaGuanChaModel.IsInDesignMode(this)) return;//设计模式直接返回
            DateTime d = DateTime.Now;
            TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss")+(TimerRefreshIng?"...":"");
             
            if (PreExecute==DateTime.MinValue || (DateTime.Now - PreExecute).TotalSeconds > 10000) {
                PreExecute = DateTime.Now;
                Batch batch = SelectBatch;
                if (batch != null && batch.EnumAgingStatus == EnumAgingStatus.InAging)
                {
                    _model.QueryMQTT.Query(batch);
                    TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss...");
                }
            }

        }

        public Batch CurrentBatch { get; set; }
        public void IniData(Batch his)
        {
            List<Batch> bs = UILaoHuaGuanChaData.Instance.DBListBatch.ToList();
            if (his != null) {
                bs.Add(his);
            }
            this.TxtBatchList.ItemsSource = bs;
            if (bs.Count > 0 && (TxtBatchList.SelectedItem==null || (TxtBatchList.SelectedItem != null && !bs.Contains(TxtBatchList.SelectedItem)))) {
                TxtBatchList.SelectedItem = his==null ? bs[0]: his;
            }
            if (his != null)
            {
                TxtBatchList.SelectedItem = his;
            }
            if (bs.Count == 0) {
                TxtBatchList.SelectedItem = null;
                this.BtnFinishBoard.IsEnabled = false;
                BtnFinishBoardExt.IsEnabled = false;
            }
            PreExecute = DateTime.MinValue;//重置数据刷新
            CurrentBatch = TxtBatchList.SelectedItem as Batch;

        }

        private void TxtBatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            Batch batch = this.TxtBatchList.SelectedItem as Batch;
            if (batch != null) {
                this.BtnFinishBoard.IsEnabled = batch.EnumAgingStatus== EnumAgingStatus.InAging;
                BtnFinishBoardExt.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
            }
            SelectBatch = batch;
            _model.RefreshAddress(batch);
            _model.DataTimerRefresh(batch);
        }

        public Batch Batch { get; set; }
        public void SetData(List<PageBoardItem> data, Batch _batch)
        {

            Batch = _batch;
            int selectIndex = gridPager.SelectedIndex;
            if (selectIndex < 0) selectIndex = 0;
            int app = 4;
            gridPager.Items.Clear();
            List<UILaoHuaGuanChaPage> pagedatas = new List<UILaoHuaGuanChaPage>();
            for (int i = 0; i < data.Count; i = i + app)
            {
                int from = i;
                int to = data.Count - 1 >= i + app ? i + app : data.Count - 1;
                UILaoHuaGuanChaPage d = new UILaoHuaGuanChaPage() { };
                d.From = data[from];
                d.To = data[to];
                d.Data = data.GetRange(from, to - from).ToList();
                pagedatas.Add(d);
                gridPager.Items.Add(d);
            }
            if (selectIndex >= gridPager.Items.Count) selectIndex = 0;
            if (gridPager.Items.Count > 0) gridPager.SelectedItem = gridPager.Items[selectIndex];

        }


        private void gridPager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UILaoHuaGuanChaPage page = gridPager.SelectedItem as UILaoHuaGuanChaPage;
            if (page == null) return;
            int x = 0;
            foreach (UIElement item in grid.Children)
            {
                if (item is UILaoHuaGuanChaC10Grid)
                {
                    UILaoHuaGuanChaC10Grid g = (UILaoHuaGuanChaC10Grid)item;
                    if (page.Data.Count > x)
                    {
                        g.SetData(page.Data[x], Batch);
                        x++;
                    } 
                }
            }
        }

        private void BtnAddBatch_Click(object sender, RoutedEventArgs e)
        {
            UILaoHuaGuanChaAdd w = new UILaoHuaGuanChaAdd();
            w.Topmost = true;
            bool? sucess = w.ShowDialog();
            if (sucess!=null && sucess.Value) {

            }
            UILaoHuaGuanChaData.Instance.RefreshData();
            IniData(null);

        }

        private void BtnFinishBoard_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.TxtBatchList.SelectedItem as Batch;
            if(_Batch != null)
            {

                if (MessageBox.Show(
                    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UILaoHuaGuanChaData.Instance.FinishBatch(_Batch);
                    UILaoHuaGuanChaC10Model.Instance.RefreshAddress(_Batch);
                    IniData(null);
                }
                
            }
        }
        private void BtnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.TxtBatchList.SelectedItem as Batch;
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
                        WaitWindow.ShowWindow("正在导出", "正在导出批次" + _Batch.F_BatchName + "......", this);
                        ProgressNow.Visibility = System.Windows.Visibility.Visible;
                        _model.ExportExcel(dlg.FileName, _Batch);
                        Cursor = Cursors.Arrow;
                        ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        
                WaitWindow.CloseWindow(this);
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show(ex.Message, "导出出错");
                    } 
                }
            }
        }

        private void BtnExcelExportDetail_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.TxtBatchList.SelectedItem as Batch;
            if (_Batch != null)
            {
                string name = _Batch.F_BatchName.Replace("#", "-");
                var dlg = new SaveFileDialog()
                {
                    Title = _Batch.F_BatchName + "-另存为",
                    DefaultExt = "txt",
                    Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                    FileName = name+"明细"
                };
                if (dlg.ShowDialog() == true)
                {
                    try
                    {

                        Cursor = Cursors.Wait;
                        WaitWindow.ShowWindow("正在导出", "正在导出批次" + _Batch.F_BatchName + "......", this);
                        ProgressNow.Visibility = System.Windows.Visibility.Visible;
                        _model.ExportBoardExcelV2ALL(dlg.FileName, _Batch);
                        Cursor = Cursors.Arrow;
                        ProgressNow.Visibility = System.Windows.Visibility.Hidden;

                        WaitWindow.CloseWindow(this);
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }
            }
        }

        private void BtnSetSerialNo_Click(object sender, RoutedEventArgs e)
        {
            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }
             

            if (MessageBox.Show(
                    string.Format("确定要生成批次" + en.F_BatchName + "的所有串号？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            DateTime test = new DateTime(1000, 1, 1);
            EnumSerialCoded sql = EnumSerialCoded.CodeAll;
            string msgSucess = "";
            sql = EnumSerialCoded.CodeAll;
            msgSucess = "已开始生成批次" + en.F_BatchName + "的所有串号，串号将在10分钟后生成完成，请十分钟后刷新结果。";
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_SerialCoded=" + (int)sql + ",F_SerialCodedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else
            {

                MessageBox.Show("生成串号启动失败");
                return;
            }
        }

        private void BtnSetTime_Click(object sender, RoutedEventArgs e)
        {
            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }



            if (MessageBox.Show(
                    string.Format("确定要生成批次" + en.F_BatchName + "的所有设备时间？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }


            DateTime test = new DateTime(1000, 1, 1);
            EnumDeviceTimed sql = EnumDeviceTimed.CodeAll;
            string msgSucess = "";
            sql = EnumDeviceTimed.CodeAll;
            msgSucess = "已开始生成批次" + en.F_BatchName + "的所有设备时间，设备时间将在10分钟后生成完成，请十分钟后刷新结果。";
             
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimed=" + (int)sql + ",F_DeviceTimedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }
        }


        private void BtnSetSerialNoPart_Click(object sender, RoutedEventArgs e)
        {
            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }


            if (MessageBox.Show(
                    string.Format("确定要生成批次" + en.F_BatchName + "的所有无效串号？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }


            DateTime test = new DateTime(1000, 1, 1);
            EnumSerialCoded sql = EnumSerialCoded.CodeInvalid;
            string msgSucess = "";
            sql = EnumSerialCoded.CodeInvalid;
            msgSucess = "将开始生成批次" + en.F_BatchName + "的所有无效串号，串号将在10分钟后生成完成，请十分钟后刷新结果。";
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_SerialCoded=" + (int)sql + ",F_SerialCodedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else
            {

                MessageBox.Show("生成串号启动失败");
                return;
            }
        }

        private void BtnSetTimePart_Click(object sender, RoutedEventArgs e)
        {

            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }


            if (MessageBox.Show(
                    string.Format("确定要生成批次" + en.F_BatchName + "的所有无效设备时间吗？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }


            DateTime test = new DateTime(1000, 1, 1);
            EnumDeviceTimed sql = EnumDeviceTimed.CodeInvalid;
            string msgSucess = "";
            sql = EnumDeviceTimed.CodeInvalid;
            msgSucess = "将开始生成批次" + en.F_BatchName + "的所有无效设备时间，设备时间将在10分钟后生成完成，请十分钟后刷新结果。";
             
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimed=" + (int)sql + ",F_DeviceTimedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }

        }


        private void BtnReadSerial_Click(object sender, RoutedEventArgs e)
        {

            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }

            if (MessageBox.Show(
                    string.Format("确定要读取批次" + en.F_BatchName + "的所有设备串号吗？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            DateTime test = new DateTime(1000, 1, 1);
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_SerialCodedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show("已开始读取批次" + en.F_BatchName + "的所有设备串号，设备串号将在10分钟后读取完成，请十分钟后刷新结果。");
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }

        }

        private void BtnReadTime_Click(object sender, RoutedEventArgs e)
        {

            Batch en = CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }

            if (MessageBox.Show(
                    string.Format("确定要读取批次" + en.F_BatchName + "的所有设备时间吗？"),
                    "确定窗口", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            DateTime test = new DateTime(1000, 1, 1);
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimedUpdateTime='" + test.ToString() + "' where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show("已开始读取批次" + en.F_BatchName + "的所有设备时间，设备时间将在10分钟后读取完成，请十分钟后刷新结果。");
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }

        }

    }

}
