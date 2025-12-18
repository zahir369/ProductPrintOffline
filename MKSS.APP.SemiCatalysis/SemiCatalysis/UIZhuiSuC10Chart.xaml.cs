using System;
using System.Collections.Generic;
using System.Data;
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
using DeviceDataMonitorWPF.UserCommon;
using InteractiveDataDisplay.WPF;
using MKSS.Model;
using MKSS.Service.SemiCatalysis;
using MKSS.Util.Log;

namespace MKSS.APP.SemiCatalysis
{

    /// <summary>
    /// UIZhuiSuC10ChartV1.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Chart : UserControl
    {

        bool PageLoaded = false;
        public UIZhuiSuC10Chart()
        {
            InitializeComponent();
            PageLoaded = true;
            this.SetAxisLimits();
            this.Loaded += UIZhuiSuC10Chart_Loaded;
        }

        private void UIZhuiSuC10Chart_Loaded(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
            this.ShowDt.IsChecked = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ShowDelta;
            this.LockX.IsChecked = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.LockX;
            this.LockY.IsChecked = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.LockY;
            LockX_Checked(null, null);

            if (!UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ShowDelta)
            {
                plotter.PlotOriginX = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMin;
                plotter.PlotOriginY = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMin;
                plotter.PlotWidth = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMax - UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMin;
                plotter.PlotHeight = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMax - UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMin;
            }
            else
            {
                plotter.PlotOriginX = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMinDelta;
                plotter.PlotOriginY = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMinDelta;
                plotter.PlotWidth = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMaxDelta - UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMinDelta;
                plotter.PlotHeight = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMaxDelta - UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMinDelta;
            }

        }

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue )
        {
            if (UIZhuiSuC10Model.Instance.SettingModel==null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;

            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotOriginX = xMin;
            else plotter.PlotOriginX = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMin;

            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotWidth = xMax - xMin;
            else plotter.PlotWidth = UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMax - UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMin;


        }

        Batch Batch { get; set; }
        List<Sensor> tabs = null;
        Dictionary<string, LineGraph> LineGraphDic { get; set; }
        ChartDataCache dic_data = new ChartDataCache();
        public void SetBatch(Batch _batch)
        {

            if (_batch == null) return;
            if (_batch == Batch) return;//批次未变化
            Batch = _batch;

            if (Batch != null)
            {
                //lines.Children.SetAxisLimitsX(0, Batch.F_AgingEndTime);
            }
            else
            { 
                SetAxisLimits( );
            }

            //历史批次直接读数据库
            dic_data.Clear();
            GC.Collect();

            if (LineGraphDic != null)
            {
                if (LineGraphDic.Count > 0)
                {
                    foreach (var item in LineGraphDic.Values)
                    {
                        lines.Children.Remove(item); 
                    }
                }
                LineGraphDic.Clear();
            }


            tabs = new List<Sensor>();
            LineGraphDic = new Dictionary<string, LineGraph>();
            foreach (Sensor s in UIZhuiSuC10Model.Instance.ECService.BatchSensorDictionary.Values.OrderBy(w=>w.VisibleXh).ToArray())
            {
                if (!dic_data.ContainsKey(s.F_SensorId))
                {
                    dic_data.Add(s.F_SensorId, new VoltagePointCollection());
                    tabs.Add(s);
                }
            }

            foreach (TimeSpan ts in UIZhuiSuC10Model.Instance.ECService.BatchSensorDataDictionary.Keys.ToArray())
            {
                foreach (var F_SensorId in UIZhuiSuC10Model.Instance.ECService.BatchSensorDataDictionary[ts].Keys.ToArray())
                {
                    var data = UIZhuiSuC10Model.Instance.ECService.BatchSensorDataDictionary[ts][F_SensorId];
                    dic_data[F_SensorId].Add(data);
                }
            }
            //plotter.Render();

        }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;

        }
        public void ShowSensors(List<string> sendor_ids)
        {
            if (LineGraphDic == null) return;
            if (LineGraphDic.Count > 0)
            {
                foreach (var item in LineGraphDic.Values)
                {
                    lines.Children.Remove(item);
                }
            }
            LineGraphDic.Clear();
            foreach (string sid in sendor_ids)
            {
                if (!dic_data.ContainsKey(sid)) continue;
                var s = tabs.FirstOrDefault(w => w.F_SensorId == sid);
                if (s != null)
                {
                    if (!LineGraphDic.ContainsKey(sid)) LineGraphDic.Add(sid, null);
                    if (dic_data[sid].Count == 0) continue;
                    var res = AddLineGraph(s);//  lines.Children.AddScatter(dic_x[sid].ToArray(), dic_y[sid].ToArray());
                    if (res == null) continue;
                    LineGraphDic[sid] = res;
                }

            }
            //plotter.Render();
        }


        public void ShowSensorsOf(string sid, bool check)
        {
            if (LineGraphDic == null) return;
            if (check)
            {
                var s = tabs.FirstOrDefault(w => w.F_SensorId == sid);
                if (s != null)
                {
                    if (!LineGraphDic.ContainsKey(sid))
                    {
                        if (!LineGraphDic.ContainsKey(sid)) LineGraphDic.Add(sid, null);
                        var res = AddLineGraph(s);
                        if (res == null) return;
                        LineGraphDic[sid] = res;// lines.Children.AddScatter(dic_x[sid].ToArray(), dic_y[sid].ToArray());
                    }
                }
                //plotter.Render();

            }
            else
            {
                var s = tabs.FirstOrDefault(w => w.F_SensorId == sid);
                if (s != null)
                {
                    if (LineGraphDic.ContainsKey(sid))
                    {
                        lines.Children.Remove(LineGraphDic[sid]);
                        LineGraphDic.Remove(sid);
                    }
                }
                //plotter.Render();
            }

        }

        public void SetPageBoardItem(Model.Batch _batch, Dictionary<string, MKSS.Model.SensorData> data)
        {

        }
         

        /// <summary>
        ///  只展示选中的 最近 5 分钟的数据
        /// </summary>
        /// <param name="_batch"></param>
        /// <param name="datas"></param>
        /// <param name="F_AddTime"></param>
        public void SetDataRealTime(Model.Batch _batch, Dictionary<string, MKSS.Model.SensorData> datas, TimeSpan f_AddTime)
        {
            if (Batch == null) return;

            bool emp = true;
            Batch _Batch = Batch; 
            if (_Batch.F_AgingStartTime != DateTime.MinValue && LineGraphDic != null)
            {
                TimeSpan total = TimeSpan.FromSeconds(_Batch.F_AgingEndTime);
                TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;
                foreach (MKSS.Model.SensorData data in datas.Values)
                {
                    UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpan = (int)data.F_AddTime;//默认按照最新时间分级 
                    UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanLatest = (int)data.F_AddTime;//默认按照最新时间分级 
                    //检测中批次历史数据加上实时数据
                    if (!data.F_DataValueEmp && dic_data.ContainsKey(data.F_SensorId))
                    {
                        dic_data[data.F_SensorId].Add(data);
                    }
                }

                if (DateTime.Now - SetDataRealTimeLastRender < new TimeSpan(0, 0, 1)) return;
                SetDataRealTimeLastRender = DateTime.Now;

                foreach (MKSS.Model.SensorData data in datas.Values.OrderBy(w=>w.Sensor.VisibleXh))
                {

                    //检测中批次历史数据加上实时数据
                    if (!data.F_DataValueEmp && dic_data.ContainsKey(data.F_SensorId))
                    {



                        var s = LineGraphDic.FirstOrDefault(w => w.Key == data.F_SensorId);
                        if (s.Key != null)
                        {

                            LineGraph _LineGraph = s.Value;
                            if (_LineGraph == null)
                            {
                                var sens = tabs.FirstOrDefault(w => w.F_SensorId == data.F_SensorId);
                                LineGraphDic[data.F_SensorId] = AddLineGraph(sens);// lines.Children.AddScatter(dic_data[data.F_SensorId].Keys.ToArray(), dic_data[data.F_SensorId].Values.ToArray());
                                if (LineGraphDic[data.F_SensorId] != null)
                                {

                                }
                            }
                            else
                            {
                                //更新之前，先截取短数据
                                //_LineGraph.Update(dic_x[data.F_SensorId].ToArray(), dic_y[data.F_SensorId].ToArray());
                                UpdateLineGraph(_LineGraph, data.F_SensorId);
                            }

                            emp = false;

                            //vLine.X = data.F_AddTime;
                            //hLine.Y = data.F_DataValue;



                        }
                    }
                }

                
            }


        }
        DateTime SetDataRealTimeLastRender = DateTime.MinValue;//避免刷新太快了，卡界面

        void UpdateLineGraph(LineGraph _LineGraph, string F_SensorId)
        {
            bool ShowDtData = this.ShowDt.IsChecked != null && this.ShowDt.IsChecked.Value;
            SensorData _StartData = UIZhuiSuC10SensorItem.SensorDataStartCache.ContainsKey(F_SensorId) ? UIZhuiSuC10SensorItem.SensorDataStartCache[F_SensorId] : new SensorData();
            new Thread(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //更新之前，先截取短数据
                    var sel = dic_data[F_SensorId];
                    _LineGraph.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => ShowDtData? SensorData.Delta(w, _StartData) : w.F_ShowValue));
                }));
            }).Start();
        }

        LineGraph AddLineGraph(Sensor _Sensor)
        {
            bool ShowDtData = this.ShowDt.IsChecked != null && this.ShowDt.IsChecked.Value;
            string F_SensorId = _Sensor.F_SensorId;
            SensorData _StartData = UIZhuiSuC10SensorItem.SensorDataStartCache.ContainsKey(F_SensorId) ? UIZhuiSuC10SensorItem.SensorDataStartCache[F_SensorId] : new SensorData();
            //更新之前，先截取短数据
            var sel = dic_data[F_SensorId];
            if (sel.Count == 0) return null;

            var lg = new LineGraph(); 
            lines.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(ColorUtil.Of.RandomDark);
            lg.Description = String.Format("{0}", _Sensor.PosString);
            lg.StrokeThickness = 2;
            lg.Plot(sel.Select(w=>w.F_AddTime), sel.Select(w => ShowDtData ? SensorData.Delta(w, _StartData) : w.F_ShowValue));

            //var ds = new EnumerableDataSource<VoltagePoint>(voltagePointCollection);
            //ds.SetXMapping(x => dateAxis.ConvertToDouble(x.Date));
            //ds.SetYMapping(y => y.Voltage);
            //plotter.AddLineGraph(ds, Colors.Green, 2, "Volts"); // to use this method you need "using Microsoft.Research.DynamicDataDisplay;"

            return lg;
        }


        void Refresh()
        {
            new Thread(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    double[] x = new double[200];
                    for (int i = 0; i < x.Length; i++)
                        x[i] = 3.1415 * i / (x.Length - 1);

                    for (int i = 0; i < 25; i++)
                    {
                        var lg = new LineGraph();
                        lines.Children.Add(lg);
                        lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(i * 10), 0));
                        lg.Description = String.Format("Data series {0}", i + 1);
                        lg.StrokeThickness = 2;
                        lg.Plot(x, x.Select(v => Math.Sin(v + i / 10.0)).ToArray());
                    }
                }));
            }).Start();
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

        int xmin_sec = - 10;
        int xmax_sec = 10 * 80 + 10;
        public static int XMinValue { get; set; } = -1;
        public static int XMaxValue { get; set; } = -1;
        public DateTime SetSpanLast = DateTime.MinValue;
        public void SetSpan(int value, int min, int max)
        {

            if (SetSpanLast != DateTime.MinValue && (DateTime.Now - SetSpanLast) < new TimeSpan(0, 5, 0))
            {
                return;
            }
            SetSpanLast = DateTime.Now;

            if (XMinValue < 0)
            {
                XMinValue = min - 10;
            }
            if (XMaxValue < 0)
            {
                XMaxValue = max > xmax_sec ? xmax_sec : max;
            }
            //vLine.X = value;
            if (value + (int)(xmax_sec * 0.3) > XMaxValue)
            {
                XMaxValue = value + (int)(xmax_sec * 0.3);
                XMinValue = value + (int)(xmax_sec * 0.3) - xmax_sec;
            }
            if (value < XMinValue)
            {
                XMinValue = value - 10;
                XMaxValue = (value + xmax_sec) > max ? max : (value + xmax_sec);
            }


            this.SetAxisLimits(XMinValue, XMaxValue);
            //lines.Children.SetAxisLimitsX(XMinValue, XMaxValue);
            //plotter.Render();

        }

        private void LockX_Checked(object sender, RoutedEventArgs e)
        {
            if (!PageLoaded || UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;

            if (this.LockX != null) plotter.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            if (this.LockY != null) plotter.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
            UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.LockX = this.LockX.IsChecked != null && this.LockX.IsChecked.Value;
            UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.LockY = this.LockY.IsChecked != null && this.LockY.IsChecked.Value;
            SemiCatalysisConfgig.Save();
        }

        private void ShowDt_Checked(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
            UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ShowDelta = this.ShowDt.IsChecked!=null && this.ShowDt.IsChecked.Value;
            SemiCatalysisConfgig.Save();
            ShowSensors(this.dic_data.IDS);
        }
        

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ShowDelta)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMin = plotter.PlotOriginY;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMax = plotter.PlotOriginY + plotter.PlotHeight;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMin = plotter.PlotOriginX;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMax = plotter.PlotOriginX + plotter.PlotWidth;
                SemiCatalysisConfgig.Save();
            }
            else
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMinDelta = plotter.PlotOriginY;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.YMaxDelta = plotter.PlotOriginY + plotter.PlotHeight;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMinDelta = plotter.PlotOriginX;
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.XMaxDelta = plotter.PlotOriginX + plotter.PlotWidth;
                SemiCatalysisConfgig.Save();
            }
           
        }

        private void plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            plotter_MouseWheel(null,null);
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
        Dictionary<string, VoltagePointCollection> cache = new Dictionary<string, VoltagePointCollection>();

        public List<string> IDS
        {
            get
            {
                return this.cache.Keys.ToList();
            }
        }
        public VoltagePointCollection this[string F_SensorId]
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

        internal void Add(string f_SensorId, VoltagePointCollection dictionary)
        {
            cache.Add(f_SensorId, dictionary);
        }

        internal void Clear()
        {
            cache.Clear();
        }

        internal bool ContainsKey(string f_SensorId)
        {
            return cache.ContainsKey(f_SensorId);
        }
    }
    public class VoltagePointCollection : RingArray<MKSS.Model.SensorData>
    {
        private const int TOTAL_POINTS = 120 * 60 * 5;
        public VoltagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

    public class VoltagePoint
    {
        public TimeSpan Date { get; set; }

        public double Voltage { get; set; }

        public VoltagePoint(TimeSpan date, double voltage)
        {
            this.Date = date;
            this.Voltage = voltage;
        }
    }
}
