using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using InteractiveDataDisplay.WPF;
using System.Windows.Controls;
using WpfChartTest.Model;
using MKSS.APP.ECTester.UserCommon;

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TestData _TestData = new TestData();
        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;

        List<Sensor> SensorList = null;
        /// <summary>
        ///  参考线
        /// </summary>
        Dictionary<string, LineGraph> LineGraphStd { get; set; }
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, LineGraph> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        List<LagendObject> LegendsA = new List<LagendObject>();


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            InitializeComponent();
            _TestData.OnNewData += _TestData_OnNewData;

            ShowModeDY.IsChecked = Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa;
            ShowModeAD.IsChecked = Model.SensorGroupData.ShowMode == ShowModeEnum.AD;
            ShowModeAD.Foreground = ECTesterService.IndustryMode ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);
            ShowModeDY.Foreground = !ECTesterService.IndustryMode ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);
            ShowModeAD.FontSize = ECTesterService.IndustryMode ? 19 : 12;
            ShowModeDY.FontSize = !ECTesterService.IndustryMode ? 12 : 19;

            //时间放到初始值之后，避免事件呗激发
            ShowModeDY.Checked += ShowMode_Checked;
            ShowModeAD.Checked += ShowMode_Checked;

            this.SetAxisLimits(-3, 150);
            LockX_Checked(null, null);

            if (LineGraphDic == null)
            {

                SensorList = new List<Sensor>();
                LegendsA = new List<LagendObject>();//图例
                LineGraphStd = new Dictionary<string, LineGraph>();//参考线
                LineGraphDic = new Dictionary<PosEnum, LineGraph>();
                foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                {
                    LineGraphDic.Add(sid, AddLineGraph(sid));
                    CacheData.Add(sid, new VoltagePointCollection());
                    LegendsA.Add(new LagendObject(LineGraphDic[sid], sid, this.lines1));
                }
                this.legengA.ItemsSource = LegendsA;

            }
             
        }

        private void TestData_Click(object sender, RoutedEventArgs e)
        {
            _TestData.StartTask();
        }

        private void _TestData_OnNewData(SensorGroupData datas_history, double F_AddTime)
        {
            SetDataRealTime(datas_history);
        }

        TimeSpan MinInterval = TimeSpan.FromMilliseconds(500);
        DateTime SetDataRealTime_OnGetMQDataRefreshUI = DateTime.MinValue;
        Queue<SensorGroupData> Buffer = new Queue<SensorGroupData>();
        public void SetDataRealTime(  SensorGroupData d)
        {
            Buffer.Enqueue(d);
            try
            {
                SetDataRealTime_OnGetMQDataRefreshUI = DateTime.Now;//1 秒刷一次界面
                List<SensorGroupData> datasList = new List<SensorGroupData>();
                while (Buffer.Count > 0)
                {
                    SensorGroupData g = Buffer.Dequeue(); datasList.Add(g);//禁止上次任务的加入
                }
                SetDataRealTimeBatch(datasList);
            }
            catch (Exception ex)
            {
                ULogger.Info(ex.Message);
                ULogger.Info(ex.StackTrace);
            }
        }

        DateTime SetDataRealTimeLastRender = DateTime.MinValue;//避免刷新太快了，卡界面
        public void SetDataRealTimeBatch( List<SensorGroupData> datasList)
        { 
            if (LineGraphDic != null)
            {

                Dictionary<PosEnum, SensorDataItem> refreshes = new Dictionary<PosEnum, SensorDataItem>();
                foreach (SensorGroupData datas in datasList)
                {
                    foreach (SensorDataItem data in datas.Data.Values)
                    {
                        //检测中批次历史数据加上实时数据
                        if (!data.F_DataValueEmp && CacheData.ContainsKey(data.PosEnum))
                        {
                            if (!data.F_DataValueEmp)
                            {
                                CacheData[data.PosEnum].Add(data);
                                if (!refreshes.ContainsKey(data.PosEnum))
                                {
                                    refreshes.Add(data.PosEnum, data);
                                }
                            }
                        }
                    }
                }

                if (DateTime.Now - SetDataRealTimeLastRender < MinInterval) return;
                SetDataRealTimeLastRender = DateTime.Now;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        foreach (PosEnum pose in refreshes.Keys)
                        {
                            var sel = CacheData[pose];
                            LineGraph _LineGraph = LineGraphDic[pose];
                            _LineGraph.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
                        }
                    }
                    catch (Exception ex)
                    {
                        ULogger.Info(ex.Message);
                        ULogger.Info(ex.StackTrace);
                    }
                }));

            }


        }


        LineGraph AddLineGraph(PosEnum PosEnum)
        {

            var lg = new LineGraph();
            lines1.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(ColorUtil.Of.RandomNoDark);
            lg.Description = String.Format("{0}", PosEnum);
            lg.StrokeThickness = defalut_thickness;
            if (!CacheData.ContainsKey(PosEnum)) return lg;
            var sel = CacheData[PosEnum];
            if (sel.Count == 0) return lg;
            lg.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
            return lg;

        }


        public void RefreshPage()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (LineGraphDic == null) return;
                foreach (var pose in LineGraphDic.Keys)
                {
                    LineGraph _LineGraph = LineGraphDic[pose];
                    var sel = CacheData[pose];
                    _LineGraph.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
                }
            }));

        }



        private void ShowMode_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            if (r == null) return;
            string con = r.Content + "";
            foreach (ShowModeEnum item in Enum.GetValues(typeof(ShowModeEnum)))
            {
                string str = SensorGroupData.ShowModeString(item);
                if (str == con)
                {
                    SensorGroupData.ShowMode = item;

                    this.SetAxisLimits(0, (ECTesterService.TxtTotalSpan) * 1.3);
                    this.RefreshPage();

                }
            }
        }


        public void ShowMode_Next()
        {

            List<RadioButton> list = new List<RadioButton>();
            for (int c = 0; c < 3; c++)
            {
                for (int i = 0; i < PAnelControl.Children.Count; i++)
                {
                    var item = PAnelControl.Children[i];
                    if (item is RadioButton)
                    {
                        list.Add(item as RadioButton);
                    }
                }
            }

            RadioButton pre = null;
            RadioButton nex = null;
            RadioButton cur = null;
            string str = SensorGroupData.ShowModeString(SensorGroupData.ShowMode);
            for (int i = 1; i < list.Count; i++)
            {
                RadioButton r = list[i];
                string con = r.Content + "";
                if (str == con)
                {
                    pre = list[i - 1];
                    nex = list[i + 1];
                    cur = list[i + 0];
                    break;
                }
            }

            nex.IsChecked = true;

        }



        public void ShowMode_Pre()
        {

            List<RadioButton> list = new List<RadioButton>();
            for (int c = 0; c < 3; c++)
            {
                for (int i = 0; i < PAnelControl.Children.Count; i++)
                {
                    var item = PAnelControl.Children[i];
                    if (item is RadioButton)
                    {
                        list.Add(item as RadioButton);
                    }
                }
            }

            RadioButton pre = null;
            RadioButton nex = null;
            RadioButton cur = null;
            string str = SensorGroupData.ShowModeString(SensorGroupData.ShowMode);
            for (int i = 2; i < list.Count; i++)
            {
                RadioButton r = list[i];
                string con = r.Content + "";
                if (str == con)
                {
                    pre = list[i - 1];
                    nex = list[i + 1];
                    cur = list[i + 0];
                    break;
                }
            }

            pre.IsChecked = true;

        }

        LineGraph LineGraphT90Focus = null;
        /// <summary>
        ///  T90参考线
        /// </summary>
        Dictionary<string, LineGraph> LineGraphT90 { get; set; } = new Dictionary<string, LineGraph>();
        public void ShowStdLine(string key, double v)
        {

            try
            {

                if (v <= 0)
                {
                    if (LineGraphT90.ContainsKey(key))
                    {
                        lines1.Children.Remove(LineGraphT90[key]);
                        LineGraphT90.Remove(key);
                    }
                }

                if (!LineGraphT90.ContainsKey(key))
                {
                    var lg = new LineGraph();
                    lines1.Children.Add(lg);
                    bool is_zeroline = key.EndsWith("T90") || key.EndsWith("T10");
                    lg.Stroke = !is_zeroline ? new SolidColorBrush(ColorUtil.Of.Yellow) : new SolidColorBrush(ColorUtil.Of.OrangeRed);
                    lg.StrokeThickness = 1.6 * 2;
                    lg.StrokeDashArray = new DoubleCollection() { 6, 9 };

                    lg.Description = String.Format("{0}秒", v);
                    //lg.ShowInLegand = false;
                    lg.Plot(new double[] { v, v }, new double[] { 0, 5000 });
                    LineGraphT90.Add(key, lg);
                    if (LineGraphT90Focus != null && LineGraphT90Focus != lg)
                    {
                        LineGraphT90Focus.StrokeThickness = LineGraphT90Focus.StrokeThickness / 2;
                    }
                    LineGraphT90Focus = lg;
                }
                else
                {
                    var lg = LineGraphT90[key];
                    bool is_zeroline = key.EndsWith("T90") || key.EndsWith("T10");
                    lg.Stroke = !is_zeroline ? new SolidColorBrush(ColorUtil.Of.Yellow) : new SolidColorBrush(ColorUtil.Of.OrangeRed);
                    lg.StrokeThickness = 1.6 * 2;
                    lg.StrokeDashArray = new DoubleCollection() { 6, 9 };

                    lg.Description = String.Format("{0}秒", v);
                    //lg.ShowInLegand = false;
                    lg.Plot(new double[] { v, v }, new double[] { 0, 5000 });

                    if (LineGraphT90Focus != null && LineGraphT90Focus != lg)
                    {
                        LineGraphT90Focus.StrokeThickness = LineGraphT90Focus.StrokeThickness / 2;
                    }
                    LineGraphT90Focus = lg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FocusSensor(PosEnum pos, bool foucus)
        {

            if (LineGraphDic == null || !LineGraphDic.ContainsKey(pos) || LineGraphDic[pos] == null) return;

            if (pos == CurrentFocus && CurrentFocusDoing)
            {
                return;
            }

            if (foucus)
            {

                Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (CurrentFocus != pos)
                    {
                        var sel_old = LineGraphDic[CurrentFocus];
                        if (sel_old != null)
                        {
                            sel_old.StrokeThickness = defalut_thickness;
                            var data_old = CacheData[CurrentFocus];
                            if (data_old.Count > 0)
                            {
                                sel_old.Plot(data_old.Select(w => w.F_AddTime), data_old.Select(w => w.F_LoadDataValue));
                            }
                        }

                    }

                    var sel_new = LineGraphDic[pos];
                    var data_new = CacheData[pos];
                    if (data_new.Count > 0)
                    {
                        sel_new.StrokeThickness = this.focus_thickness;
                        sel_new.Plot(data_new.Select(w => w.F_AddTime), data_new.Select(w => w.F_LoadDataValue));
                        CurrentFocusDoing = true;
                        CurrentFocus = pos;
                    }


                }));

            }
            else
            {

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var sel_old = LineGraphDic[pos];
                    sel_old.StrokeThickness = defalut_thickness;
                    var data_old = CacheData[pos];
                    if (data_old.Count > 0)
                    {
                        sel_old.Plot(data_old.Select(w => w.F_AddTime), data_old.Select(w => w.F_LoadDataValue));
                        CurrentFocusDoing = false;
                    }
                }));

            }




        }

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue)
        {
            if (xMin != double.MinValue && xMax != double.MinValue) plotter1.PlotOriginX = xMin;
            else plotter1.PlotOriginX = 0;


            if (xMin != double.MinValue && xMax != double.MinValue) plotter1.PlotWidth = xMax - xMin;
            else plotter1.PlotWidth = 120;

            switch (SensorGroupData.ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    plotter1.PlotOriginY
                        = 0;
                    plotter1.PlotHeight
                        = 5;
                    break;
                case ShowModeEnum.YuLiu:
                    plotter1.PlotOriginY
                        = 0;
                    plotter1.PlotHeight
                        = 100;
                    break;
                case ShowModeEnum.AD:
                    plotter1.PlotOriginY
                        = 0;
                    plotter1.PlotHeight
                        = 1200;
                    break;
                default:
                    break;
            }
        }



        public void SetPageBoardItem(Batch _batch, SensorGroupData data)
        {

        }


        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            
        }


        private void LockX_Checked(object sender, RoutedEventArgs e)
        {
            if (this.LockX != null) plotter1.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            if (this.LockY != null) plotter1.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {

           
        }

        private void plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            plotter_MouseWheel(null, null);
        }



        private void TextPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                FrameworkElement ele = sender as FrameworkElement;
                if (ele == null) return;
                this.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), false);
            }
            catch (Exception)
            {

            }


        }

        private void TextPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {

                FrameworkElement ele = sender as FrameworkElement;
                if (ele == null) return;
                FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);
            }
            catch (Exception)
            {

            }

        }


        private void TextPanel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                FrameworkElement ele = sender as FrameworkElement;
                if (ele == null) return;
                FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);

            }
            catch (Exception)
            {

            }

        }


        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LagendObject> lss = LegendsA;
            foreach (var item in lss)
            {
                item.Checked = true;
            }
        }

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LagendObject> lss = LegendsA;
            foreach (var item in lss)
            {
                item.Checked = false;
            }
        }

        private void btnRevSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LagendObject> lss = LegendsA;
            foreach (var item in lss)
            {
                item.Checked = !item.Checked;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshPage();
        }

    }

}
