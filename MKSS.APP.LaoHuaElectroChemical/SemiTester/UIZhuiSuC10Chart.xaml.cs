using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using DeviceDataMonitorWPF.UserCommon;
using InteractiveDataDisplay.WPF;
using MKSS.APP.SemiTester;
using MKSS.Model;
using MKSS.Service;
using MKSS.Util.Log;

namespace MKSS.APP.SemiTesterOld
{

    /// <summary>
    /// UIZhuiSuC10ChartV1.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Chart : UserControl
    {

        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;

        List<Sensor> SensorList = null;
        /// <summary>
        ///  参考线
        /// </summary>
        Dictionary<string,LineGraph> LineGraphStd { get; set; }
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, LineGraph> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        List<LagendObject> LegendsA = new List<LagendObject>();
        List<LagendObject> LegendsB = new List<LagendObject>();
        public UIZhuiSuC10Chart()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            this.SetAxisLimits(-3, 25);
            LockX_Checked(null, null);

            if (LineGraphDic == null)
            {
                 
                SensorList = new List<Sensor>();
                LegendsA = new List<LagendObject>();//图例
                LegendsB = new List<LagendObject>();//图例
                LineGraphStd = new Dictionary<string, LineGraph>();//参考线
                LineGraphDic = new Dictionary<PosEnum, LineGraph>();
                foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                {
                    LineGraphDic.Add(sid, AddLineGraph(sid));
                    CacheData.Add(sid, new VoltagePointCollection());
                    LegendsA.Add(new LagendObject(LineGraphDic[sid]));
                }
                this.legengA.ItemsSource = LegendsA;

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
                            if (sel_old != null) {
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

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue, double yMin = double.MinValue, double yMax = double.MinValue)
        {
            //if (UIZhuiSuC10Model.Instance.SettingModel== null || UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
            //if (xMin != double.MinValue && xMax != double.MinValue) plotter1.PlotOriginX = xMin;
            //else plotter1.PlotOriginX = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin;

            //if (yMin != double.MinValue && yMax != double.MinValue) plotter1.PlotOriginY = yMin;
            //else plotter1.PlotOriginY = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;

            //if (xMin != double.MinValue && xMax != double.MinValue) plotter1.PlotWidth = xMax - xMin;
            //else plotter1.PlotWidth = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMax - UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin;

            //if (yMin != double.MinValue && yMax != double.MinValue) plotter1.PlotHeight = yMax - yMin;
            //else plotter1.PlotHeight = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax - UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;

            //if (xMin != double.MinValue && xMax != double.MinValue) plotter2.PlotOriginX = xMin;
            //else plotter2.PlotOriginX = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin;

            //if (yMin != double.MinValue && yMax != double.MinValue) plotter2.PlotOriginY = yMin;
            //else plotter2.PlotOriginY = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;

            //if (xMin != double.MinValue && xMax != double.MinValue) plotter2.PlotWidth = xMax - xMin;
            //else plotter2.PlotWidth = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMax - UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin;

            //if (yMin != double.MinValue && yMax != double.MinValue) plotter2.PlotHeight = yMax - yMin;
            //else plotter2.PlotHeight = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax - UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;

        }


        Batch BatchSetting { get; set; }
        public void SetBatch(Batch _batch, List<PosEnum> sendor_ids, Stopwatch watcher)
        {

             
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        ULogger.Info("Chart SetBatch Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        if (_batch == null) return;
                        if (_batch == BatchSetting) return;//批次未变化
                        BatchSetting = _batch;

                        ULogger.Info("Chart SetBatch Reset:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        //清除历史数据
                        foreach (var item in LineGraphDic.Values)
                        {
                            item.Points = new PointCollection();
                        }
                        foreach (PosEnum pos in Enum.GetValues<PosEnum>())
                        {
                            CacheData[pos].Clear();
                        }

                        SensorList.Clear(); 

                        //ULogger.Info("Chart SetBatch 重绘参考线:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        ////缓存重绘参考线
                        //Chart[] plots = new Chart[] { plotter1, plotter2 };
                        //Grid[] grids = new Grid[] { lines1, lines2 };
                        //List<string> StdList = new List<string>();//记录添加项目
                        //foreach (var p in UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr)
                        //{
                        //    if (p == 0) continue;
                        //    for (int i = 0; i < plots.Length; i++)
                        //    {
                        //        string key = string.Format("{0}_{1}", i, p);
                        //        StdList.Add(key);
                        //        if (!LineGraphStd.ContainsKey(key))
                        //        {
                        //            var lg = new LineGraph();
                        //            grids[i].Children.Add(lg);
                        //            bool is_zeroline = (p == UIZhuiSuC10Model.Instance.SettingModel.TxtZeroPoint)
                        //                || (p == UIZhuiSuC10Model.Instance.SettingModel.TxtSpanPoint);
                        //            lg.Stroke = !is_zeroline ? new SolidColorBrush(ColorUtil.Of.LightBlue) : new SolidColorBrush(ColorUtil.Of.Red);
                        //            lg.StrokeThickness = !is_zeroline ? 0.8 : 1.6;
                        //            lg.StrokeDashArray = new DoubleCollection() { 6, 9 };

                        //            lg.Description = String.Format("{0}秒", p);
                        //            //lg.ShowInLegand = false;
                        //            lg.Plot(new double[] { p, p }, new double[] { plots[i].PlotOriginY - 100, plots[i].PlotOriginY + plots[i].PlotHeight + 100 });
                        //            LineGraphStd.Add(key, lg);
                        //        }
                        //    }


                        //}

                        ////清除绘制参考线
                        //foreach (var item in LineGraphStd.Keys.ToArray())
                        //{
                        //    if (!StdList.Contains(item))
                        //    {
                        //        LineGraphStd.Remove(item);
                        //        if (lines1.Children.Contains(LineGraphStd[item])) lines1.Children.Add(LineGraphStd[item]);
                        //        if (lines2.Children.Contains(LineGraphStd[item])) lines2.Children.Add(LineGraphStd[item]);
                        //    }
                        //}

                        //ULogger.Info("Chart SetBatch 加载传感器:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        ////加载传感器
                        //foreach (Sensor s in UIZhuiSuC10Model.Instance.ECService.BatchSensorDictionary.Values)
                        //{
                        //    SensorList.Add(s);
                        //}
                         
                        ULogger.Info("Chart SetBatch Finish:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

                    }
                    catch (Exception ex)
                    {
                        ULogger.Info(ex.Message);
                        ULogger.Info(ex.StackTrace);
                    }
                }));

          

            

        }

        public void ShowSensorsOf(PosEnum sid, bool check)
        {
             
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (LineGraphDic == null) return;
                        if (check)
                        {
                            LineGraphDic[sid].Visibility = Visibility.Visible;
                            var sel = CacheData[sid];
                            LineGraph _LineGraph = LineGraphDic[sid];
                            _LineGraph.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
                        }
                        else
                        {
                            LineGraphDic[sid].Visibility = Visibility.Hidden;
                        }
                    }
                    catch (Exception ex)
                    {
                        ULogger.Info(ex.Message);
                        ULogger.Info(ex.StackTrace);
                    }
                }));

            
            
        }

        public void SetPageBoardItem(Model.Batch _batch, SensorGroupData data)
        {

        }

        TimeSpan MinInterval = TimeSpan.FromMilliseconds(500);
        DateTime SetDataRealTime_OnGetMQDataRefreshUI = DateTime.MinValue;
        Queue<SensorGroupData> Buffer = new Queue<SensorGroupData>();
        public void SetDataRealTime(Model.Batch _batch, SensorGroupData d) {
             
            Buffer.Enqueue(d);
            //if (SetDataRealTime_OnGetMQDataRefreshUI == DateTime.MinValue || (DateTime.Now - SetDataRealTime_OnGetMQDataRefreshUI) > MinInterval)
            {
                try
                {
                    SetDataRealTime_OnGetMQDataRefreshUI = DateTime.Now;//1 秒刷一次界面
                    List<SensorGroupData> datasList = new List<SensorGroupData>();
                    while (Buffer.Count>0)
                    {
                        SensorGroupData g = Buffer.Dequeue();
                        if (g.F_BatchId == _batch.F_BatchId) datasList.Add(g);//禁止上次任务的加入
                    }
                    SetDataRealTimeBatch(_batch, datasList);
                }
                catch (Exception ex)
                {
                    ULogger.Info(ex.Message);
                    ULogger.Info(ex.StackTrace);
                }
                
            }
        }

        DateTime SetDataRealTimeLastRender = DateTime.MinValue;//避免刷新太快了，卡界面
        /// <summary>
        ///  只展示选中的 最近 5 分钟的数据
        ///  性能很低，必须用线程优化
        /// </summary>
        /// <param name="_batch"></param>
        /// <param name="datas"></param>
        /// <param name="F_AddTime"></param>
        [LogTagClass(Title = "刷新Chart")]
        public void SetDataRealTimeBatch(Model.Batch _batch, List<SensorGroupData> datasList)
        {
            if (_batch == null) return; 
            Batch _Batch = _batch;
            if (_Batch.F_AgingStartTime != DateTime.MinValue && LineGraphDic != null)
            {
                TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;

                Dictionary<PosEnum, SensorDataItem> refreshes = new Dictionary<PosEnum, SensorDataItem>();
                foreach (SensorGroupData datas in datasList)
                {
                    foreach (MKSS.Model.SensorDataItem data in datas.Data().Values)
                    {
                        //检测中批次历史数据加上实时数据
                        if (!data.F_DataValueEmp && CacheData.ContainsKey(data.PosEnum))
                        {
                            if (!data.F_DataValueEmp) {
                                CacheData[data.PosEnum].Add(data);
                                if (!refreshes.ContainsKey(data.PosEnum)) {
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

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //int pixelX = (int)e.MouseDevice.GetPosition(plotter).X;
            //int pixelY = (int)e.MouseDevice.GetPosition(plotter).Y;

            //(double coordinateX, double coordinateY) = plotter.GetMouseCoordinates();

            //vLine.X = coordinateX;
            //hLine.Y = coordinateY;

            //plotter.Render();
        }


        private void LockX_Checked(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.LockX != null) plotter1.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            if (this.LockY != null) plotter1.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Chart plotter = sender as Chart;
            if (plotter == null) return;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin = plotter.PlotOriginY;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax = plotter.PlotOriginY + plotter.PlotHeight;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin = plotter.PlotOriginX;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMax = plotter.PlotOriginX + plotter.PlotWidth;

            ElectroChemicalConfgig.Save();

        }

        private void plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            plotter_MouseWheel(null, null);
        }



        private void TextPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                FrameworkElement ele = sender as FrameworkElement;
                if (ele == null) return;
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), false);

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
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);
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
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);

            }
            catch (Exception)
            {

            }

        }


        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            bool isA = it.Name.EndsWith("A");
            List<LagendObject> lss = isA ? LegendsA : LegendsB;
            foreach (var item in lss)
            {
                item.Checked = true; 
            }
        }

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            bool isA = it.Name.EndsWith("A");
            List<LagendObject> lss = isA ? LegendsA : LegendsB;
            foreach (var item in lss)
            {
                item.Checked = false;
            }
        }

        private void btnRevSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            bool isA = it.Name.EndsWith("A");
            List<LagendObject> lss = isA ? LegendsA : LegendsB;
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

    public class LagendObject : INotifyPropertyChanged
    {
        public LineGraph LineGraph;
        public LagendObject(LineGraph l) {
            LineGraph = l;
        }
        public bool Checked { get { return LineGraph.Visibility == Visibility.Visible; } set { LineGraph.Visibility = value ? Visibility.Visible : Visibility.Hidden; OnPropertyChanged("Checked"); } }
        public Brush Stroke { get { return LineGraph.Stroke; } }
        public string Description { get { return LineGraph.Description; } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }
    }
    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }



    //[StructLayout(LayoutKind.Sequential)]
    public class ChartDataCache
    {
        Dictionary<PosEnum, VoltagePointCollection> cache = new Dictionary<PosEnum, VoltagePointCollection>();
        public int Count { get { return cache.Count; } }
        public VoltagePointCollection this[PosEnum F_SensorId]
        {
            get
            {
                return cache[F_SensorId];
            }
            set
            {
                cache[F_SensorId] = value;
            }
        }
        public void ShowSize()
        {
            //int size = Marshal.SizeOf(this); //1个字节
            //ULogger.Info((string.Format("占用字节数：{0}", size)));
        }

        internal void Add(PosEnum f_SensorId, VoltagePointCollection dictionary)
        {
            cache.Add(f_SensorId, dictionary);
        }

        internal void Clear()
        {
            cache.Clear();
        }
        internal void ClearVoltagePointCollection()
        {
            foreach (var item in cache.Values)
            {
                item.Clear();
            }
        }

        internal bool ContainsKey(PosEnum f_SensorId)
        {
            return cache.ContainsKey(f_SensorId);
        }
    }
    public class VoltagePointCollection : RingArray<SensorDataItem>
    {
        private const int TOTAL_POINTS = 120 * 60 * 5;
        public VoltagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

    public class VoltagePoint
    {
        public TimeSpan F_AddTime { get; set; }

        public SensorDataItem SensorDataItem { get; set; }
        public double F_LoadDataValue { get; set; }

        public VoltagePoint(TimeSpan date, SensorDataItem voltage)
        {
            this.F_AddTime = date;
            this.SensorDataItem = voltage;
        }
    }
}
