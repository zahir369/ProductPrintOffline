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
using MKSS.APP.ECTester.UserCommon; 
using MKSS.Model;
using MKSS.Service.ECTester;
using MKSS.Util.Log;
using Newtonsoft.Json;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Painting.Effects;

namespace MKSS.APP.ECTester 
{

    /// <summary>
    /// UIZhuiSuC10WinChartV1.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10LiveChart : UserControl
    {

        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;

        List<Sensor> SensorList = null;
        /// <summary>
        ///  参考线
        /// </summary>
        Dictionary<string,LineSeries<ObservablePoint>> LineGraphStd { get; set; }
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, LineSeries<ObservablePoint>> LineGraphDic { get; set; }
        List<LineSeries<ObservablePoint>> Series = new List<LineSeries<ObservablePoint>>();
        LcChartDataCache CacheData = new LcChartDataCache();
        List<LcLagendObject> LegendsA = new List<LcLagendObject>();

           


        public UIZhuiSuC10LiveChart()
        {

            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            ShowModeDY.IsChecked = Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa;
            ShowModeAD.IsChecked = Model.SensorGroupData.ShowMode == ShowModeEnum.AD;
            ShowModeAD.Foreground = ECTesterService.IndustryMode ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);
            ShowModeDY.Foreground = !ECTesterService.IndustryMode ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);
            ShowModeAD.FontSize = ECTesterService.IndustryMode ? 19 : 12;
            ShowModeDY.FontSize = !ECTesterService.IndustryMode ? 12 : 19;

            //时间放到初始值之后，避免事件呗激发
            ShowModeDY.Checked += ShowMode_Checked;
            ShowModeAD.Checked += ShowMode_Checked;

            this.SetAxisLimits(-3, 25);
            LockX_Checked(null, null);

            if (LineGraphDic == null)
            {
                 
                SensorList = new List<Sensor>();
                LegendsA = new List<LcLagendObject>();//图例
                LineGraphStd = new Dictionary<string, LineSeries<ObservablePoint>>();//参考线
                LineGraphDic = new Dictionary<PosEnum, LineSeries<ObservablePoint>>();
                foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                {
                    LineGraphDic.Add(sid, AddLineGraph(sid));
                    CacheData.Add(sid, new LcVoltagePointCollection());
                    LegendsA.Add(new LcLagendObject(LineGraphDic[sid], sid, Series));
                }
                this.legengA.ItemsSource = LegendsA;
                this.plotter1.Series = Series;
                this.plotter1.XAxes = new Axis[]
                        {
                            new Axis
                            {
                                MinLimit = 0,
                                MaxLimit = 60,
                                ForceStepToMin = true,
                                MinStep = 3,
                                TextSize = 14,
                                LabelsPaint= new SolidColorPaint{ Color = SKColors.LightGray },
                                SeparatorsPaint = new SolidColorPaint
                                {
                                    Color = new SKColor(0X32, 0X3C, 0X6B, 255),
                                    StrokeThickness = 1,
                                    PathEffect = new DashEffect(new float[] { 3, 3 })
                                }
                            }
                        }; ;
                this.plotter1.YAxes = new Axis[]
                        {
                        new Axis
                        {
                            MinLimit = 0,
                            MaxLimit = 3,
                            ForceStepToMin = true,
                            MinStep = 1,
                            TextSize = 14,
                                LabelsPaint= new SolidColorPaint{ Color = SKColors.LightGray },
                            SeparatorsPaint = new SolidColorPaint
                            {
                                Color = new SKColor(0X32, 0X3C, 0X6B, 255),
                                StrokeThickness = 1,
                                PathEffect = new DashEffect(new float[] { 3, 3 })
                            }
                        }
                        };
                this.plotter1.DrawMarginFrame = new DrawMarginFrame
                {
                    Fill = new SolidColorPaint
                    {
                        Color = new SKColor(0, 0, 0, 30)
                    },
                    Stroke = new SolidColorPaint
                    {
                        Color = SKColors.LightGray,
                        StrokeThickness = 1
                    }
                };

            }

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
                    UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                    //this.SetAxisLimits();
                    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
                    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.RefreshMM();

                    con10.UIZhuiSuC10Chart.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                    con10.UIZhuiSuC10Chart.RefreshPage();
                    con10.UIZhuiSuC10SensorsPage.Refresh();
                    con10.UIZhuiSuC10Grid.RefreshData();
                    UIZhuiSuC10Model.Instance.SettingModel.RefreshPage();

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

        void AddPoint(LineSeries<ObservablePoint> line, double[] a, double[] b)
        {
            ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)line.Values;
            po.Add(new ObservablePoint(a[0], a[1]));
            po.Add(new ObservablePoint(b[0], b[1]));
        }

        void AddPoint(LineSeries<ObservablePoint> line, double x, double y)
        {
            ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)line.Values;
            po.Add(new ObservablePoint(x, y));
        }

        void AddPoint(PosEnum sid, double x, double y)
        {
            ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)LineGraphDic[sid].Values;
            po.Add(new ObservablePoint(x, y));
        }


        LineSeries<ObservablePoint> LineGraphT90Focus = null;
        /// <summary>
        ///  T90参考线
        /// </summary>
        Dictionary<string, LineSeries<ObservablePoint>> LineGraphT90 { get; set; } = new Dictionary<string, LineSeries<ObservablePoint>>();
        public void ShowStdLine(string key,double v)
        {

            try
            {

                if (v <= 0) {
                    if (LineGraphT90.ContainsKey(key)) {
                        Series.Remove(LineGraphT90[key]);
                        LineGraphT90.Remove(key);
                    }
                }

                if (!LineGraphT90.ContainsKey(key))
                {
                    bool is_zeroline = key.EndsWith("T90") || key.EndsWith("T10");
                    var lg = new LineSeries<ObservablePoint>() {
                        Stroke =  new SolidColorPaint(SKColorUtil.Of.Yellow, 1.6F * 2F) {
                            PathEffect = new DashEffect(new float[] { 3, 3 })
                        },
                        Fill = null,
                        GeometryStroke = null,
                        GeometryFill = null,
                        GeometrySize = 0,
                        
                        Values = new ObservableCollection<ObservablePoint>()
                    };
                    Series.Add(lg);
                    //lg.StrokeThickness = 1.6 * 2;
                    //lg.StrokeDashArray = new DoubleCollection() { 6, 9 };

                    lg.Name = String.Format("{0}秒", v);
                    //lg.ShowInLegand = false;
                    AddPoint(lg,new double[] { v, 0 }, new double[] { v, 5000 });
                    LineGraphT90.Add(key, lg);
                    //if (LineGraphT90Focus != null && LineGraphT90Focus != lg)
                    //{
                    //    LineGraphT90Focus.Stroke = LineGraphT90Focus.StrokeThickness / 2;
                    //}
                    LineGraphT90Focus = lg;
                }
                else
                {
                    var lg = LineGraphT90[key]; 
                    lg.Stroke = new SolidColorPaint(SKColorUtil.Of.Yellow)
                    {
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    };
                    //lg.StrokeThickness = 1.6 * 2;
                    //lg.StrokeDashArray = new DoubleCollection() { 6, 9 };

                    lg.Name = String.Format("{0}秒", v);
                    //lg.ShowInLegand = false;
                    AddPoint(lg, new double[] { v, 0 }, new double[] { v, 5000 });

                    //if (LineGraphT90Focus != null && LineGraphT90Focus != lg)
                    //{
                    //    LineGraphT90Focus.StrokeThickness = LineGraphT90Focus.StrokeThickness / 2;
                    //}
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

            //if (LineGraphDic == null || !LineGraphDic.ContainsKey(pos) || LineGraphDic[pos] == null) return;

            //if (pos == CurrentFocus && CurrentFocusDoing)
            //{
            //    return;
            //}
             
            //    if (foucus)
            //    {

            //        Dispatcher.BeginInvoke(new Action(() =>
            //        {

            //            if (CurrentFocus != pos)
            //            {
            //                var sel_old = LineGraphDic[CurrentFocus];
            //                if (sel_old != null) {
            //                    sel_old.StrokeThickness = defalut_thickness;
            //                    var data_old = CacheData[CurrentFocus];
            //                    if (data_old.Count > 0)
            //                    {
            //                        sel_old.Plot(data_old.Select(w => w.F_AddTime), data_old.Select(w => w.F_LoadDataValue));
            //                    }
            //                }
                           
            //            }

            //            var sel_new = LineGraphDic[pos];
            //            var data_new = CacheData[pos];
            //            if (data_new.Count > 0)
            //            {
            //                sel_new.StrokeThickness = this.focus_thickness;
            //                sel_new.Plot(data_new.Select(w => w.F_AddTime), data_new.Select(w => w.F_LoadDataValue));
            //                CurrentFocusDoing = true;
            //                CurrentFocus = pos;
            //            }


            //        }));

            //    }
            //    else
            //    {

            //        Dispatcher.BeginInvoke(new Action(() =>
            //        {
            //            var sel_old = LineGraphDic[pos];
            //            sel_old.StrokeThickness = defalut_thickness;
            //            var data_old = CacheData[pos];
            //            if (data_old.Count > 0)
            //            {
            //                sel_old.Plot(data_old.Select(w => w.F_AddTime), data_old.Select(w => w.F_LoadDataValue));
            //                CurrentFocusDoing = false;
            //            }
            //        }));

            //    }


             

        }

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue)
        {
            Axis xAxis = this.plotter1.XAxes.FirstOrDefault() as Axis;
            Axis yAxis = this.plotter1.YAxes.FirstOrDefault() as Axis;
            if (UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
            if (xMin != double.MinValue && xMax != double.MinValue) xAxis.MinLimit = xMin;
            else xAxis.MinLimit = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMin;


            if (xMin != double.MinValue && xMax != double.MinValue) xAxis.MaxLimit = xMax ;
            else xAxis.MaxLimit = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMax;
            xAxis.MinStep = (int)((xAxis.MaxLimit - xAxis.MinLimit) / 15);

            switch (SensorGroupData.ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    yAxis.MinLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinDianYa;
                    yAxis.MaxLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxDianYa ;
                    break;
                case ShowModeEnum.YuLiu:
                    yAxis.MinLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinYuLiu;
                    yAxis.MaxLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxYuLiu ;
                    break;
                case ShowModeEnum.AD:
                    yAxis.MinLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinAD;
                    yAxis.MaxLimit
                        = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxAD ;
                    break;
                default:
                    break;
            }
            yAxis.MinStep = ((yAxis.MaxLimit - yAxis.MinLimit) / 5).Value;
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
                            (item.Values as ObservableCollection<ObservablePoint>).Clear();
                        }
                        foreach (PosEnum pos in Enum.GetValues<PosEnum>())
                        {
                            CacheData[pos].Clear();
                        }

                        SensorList.Clear(); 

                        ULogger.Info("Chart SetBatch 重绘参考线:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        //缓存重绘参考线  
                        List<string> StdList = new List<string>();//记录添加项目
                        foreach (var p in UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr)
                        {
                            if (p == 0) continue;
                            {
                                string key = string.Format("{0}_{1}", 1, p);
                                StdList.Add(key);
                                if (!LineGraphStd.ContainsKey(key))
                                {
                                    var lg = new LineSeries<ObservablePoint>()
                                    {
                                        Stroke = new SolidColorPaint(SKColorUtil.Of.Red, (float)defalut_thickness)
                                        {
                                            PathEffect = new DashEffect(new float[] { 3, 3 })
                                        },
                                        Fill = null,
                                        GeometryStroke = null,
                                        GeometryFill = null,
                                        GeometrySize = 0,
                                        Values = new ObservableCollection<ObservablePoint>()
                                    };
                                    Series.Add(lg);

                                    lg.Name = String.Format("{0}秒", p);
                                    //lg.ShowInLegand = false;
                                    AddPoint(lg, new double[] { p, 0 }, new double[] { p, 5000 });
                                    LineGraphStd.Add(key, lg);
                                }
                            }


                        }

                        //清除绘制参考线
                        foreach (var item in LineGraphStd.Keys.ToArray())
                        {
                            if (!StdList.Contains(item))
                            {
                                if (Series.Contains(LineGraphStd[item])) Series.Remove(LineGraphStd[item]);
                                LineGraphStd.Remove(item);
                            }
                        }

                        ULogger.Info("Chart SetBatch 加载传感器:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        //加载传感器
                        foreach (Sensor s in UIZhuiSuC10Model.Instance.ECService.BatchSensorDictionary.Values)
                        {
                            SensorList.Add(s);
                        }
                         
                        ULogger.Info("Chart SetBatch Finish:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

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
       // DateTime DataIntervalLast = DateTime.MinValue;
        Queue<SensorGroupData> Buffer = new Queue<SensorGroupData>();
        public void SetDataRealTime(Model.Batch _batch, SensorGroupData d) {

            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.SetData(_batch, d, TimeSpan.FromSeconds(d.F_AddTime));
            Buffer.Enqueue(d);

            //bool data_valid = true;//数据取舍，120秒内 不舍弃数据，120秒后 时间间隔逐渐变大，减少绘制负担
            //if (d.F_AddTime > 120)
            //{
            //    TimeSpan DataInterval = TimeSpan.FromSeconds(d.F_AddTime / 50);
            //    if (DataIntervalLast == DateTime.MinValue || (DateTime.Now - DataIntervalLast) > DataInterval)
            //    {
            //        data_valid = true; DataIntervalLast = DateTime.Now;
            //    }
            //    else
            //    {
            //        data_valid = false;
            //    }
            //}
            //else {
            //    DataIntervalLast = DateTime.Now;
            //}


            //if (data_valid)
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
                TimeSpan total = TimeSpan.FromSeconds(_Batch.F_AgingEndTime);
                TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;

                Dictionary<PosEnum, List<SensorDataItem>> refreshes = new Dictionary<PosEnum, List<SensorDataItem>>();
                foreach (SensorGroupData datas in datasList)
                {
                    foreach (MKSS.Model.SensorDataItem data in datas.Data.Values)
                    {
                        //检测中批次历史数据加上实时数据
                        if (!data.F_DataValueEmp && CacheData.ContainsKey(data.PosEnum))
                        {
                            if (!data.F_DataValueEmp) {
                                CacheData[data.PosEnum].Add(data);
                                if (!refreshes.ContainsKey(data.PosEnum))
                                {
                                    refreshes.Add(data.PosEnum, new List<SensorDataItem>());
                                }
                                refreshes[data.PosEnum].Add(data);
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
                            LineSeries<ObservablePoint> _LineGraph = LineGraphDic[pose];
                            foreach (var item in refreshes[pose])
                            {
                                AddPoint(_LineGraph, item);
                            }
                        }
                        this.plotter1.Series = Series;
                    }
                    catch (Exception ex)
                    {
                        ULogger.Info(ex.Message);
                        ULogger.Info(ex.StackTrace);
                    }
                }));

            }


        }

        void AddPoint(LineSeries<ObservablePoint> line, MKSS.Model.SensorDataItem w)
        {
            ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)line.Values;
            po.Add(new ObservablePoint(w.F_AddTime, w.F_LoadDataValue));
        }


        LineSeries<ObservablePoint> AddLineGraph(PosEnum PosEnum)
        {  

            var lg = new LineSeries<ObservablePoint>() {
                Stroke = new SolidColorPaint(SKColorUtil.Of.RandomNoDark, (float)defalut_thickness),
                Fill = null,
                GeometryStroke = null,
                GeometryFill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Values = new ObservableCollection<ObservablePoint>() 
            }; 
            lg.Name = String.Format("{0}", PosEnum);
            Series.Add(lg);

            //ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)lg.Values;
            //for (int i = 0; i < 100; i++)
            //{
            //    po.Add(new ObservablePoint(i, i));
            //}
            //lg.StrokeThickness = defalut_thickness;
            //if (!CacheData.ContainsKey(PosEnum)) return lg;
            //var sel = CacheData[PosEnum];
            //if (sel.Count == 0) return lg;
            //lg.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
            return lg;

        }


        public void RefreshPage()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (LineGraphDic == null) return;
                foreach (var pose in LineGraphDic.Keys)
                {
                    LineSeries<ObservablePoint> line = LineGraphDic[pose];
                    var sel = CacheData[pose];
                    ObservableCollection<ObservablePoint> po = (ObservableCollection<ObservablePoint>)line.Values;
                    po.Clear();
                    po.AddRange(sel.Select(w => new ObservablePoint(w.F_AddTime, w.F_LoadDataValue)));
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
            //if (this.LockX != null) plotter1.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            //if (this.LockY != null) plotter1.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            //if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            //Chart plotter = sender as Chart;
            //if (plotter == null) return; 
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMin = plotter.PlotOriginX;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMax = plotter.PlotOriginX + plotter.PlotWidth;

            //switch (SensorGroupData.ShowMode)
            //{
            //    case ShowModeEnum.DuanDianYa:
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinDianYa = plotter.PlotOriginY;
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxDianYa = plotter.PlotOriginY + plotter.PlotHeight;
            //        break;
            //    case ShowModeEnum.YuLiu:
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinYuLiu = plotter.PlotOriginY;
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxYuLiu = plotter.PlotOriginY + plotter.PlotHeight;
            //        break;
            //    case ShowModeEnum.AD:
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinAD = plotter.PlotOriginY;
            //        UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxAD = plotter.PlotOriginY + plotter.PlotHeight;
            //        break;
            //    default:
            //        break;
            //}
            //ECTesterConfgig.Save();

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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), false);

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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);
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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(SensorGroupData.PosEnum(ele.Tag + ""), true);

            }
            catch (Exception)
            {

            }

        }


        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LcLagendObject> lss = LegendsA;
            foreach (var item in lss)
            {
                item.Checked = true; 
            }
        }

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LcLagendObject> lss = LegendsA;
            foreach (var item in lss)
            {
                item.Checked = false;
            }
        }

        private void btnRevSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl it = sender as ItemsControl;
            if (it == null) return;
            List<LcLagendObject> lss = LegendsA;
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


    public class LcLagendObject : INotifyPropertyChanged
    {
        public LineSeries<ObservablePoint> LineGraph;
        public List<LineSeries<ObservablePoint>> Lines;
        public LcLagendObject(LineSeries<ObservablePoint> l, PosEnum p,List<LineSeries<ObservablePoint>> lines)
        {
            LineGraph = l;
            PosEnum = p;
            Lines = lines;
            string sh = ECTesterConfgig.Instance.ShowChart + "";
            if (string.IsNullOrEmpty(sh))
            {
                List<string> arr = new List<string>();
                for (int i = 0; i < 64; i++) arr.Add("1");
                ECTesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(arr);
                ECTesterConfgig.Save();
            }
            
            if (ConfigChecked && !Lines.Contains(LineGraph))
            {
                LineGraph.IsVisible = true;
            }
            if (!ConfigChecked && Lines.Contains(LineGraph))
            {
                LineGraph.IsVisible = false; 
            }
        }
        public PosEnum PosEnum { get; set; }
        int PosIndex { get { PosEnum p = PosEnum; return (((int)p) / 100 - 1) * 16 + ((int)p) % 100 - 1; } }
        bool ConfigChecked
        {
            get
            {
                string sh = ECTesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(ECTesterConfgig.Instance.ShowChart);
                return json[PosIndex] == "1";
            }
            set
            {
                string sh = ECTesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(ECTesterConfgig.Instance.ShowChart);
                json[PosIndex] = value ? "1" : "0";
                ECTesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(json);
                ECTesterConfgig.Save();
            }
        }

        public bool Checked
        {
            get
            {
                return LineGraph.IsVisible;
            }
            set
            {

                LineGraph.IsVisible = value;
                OnPropertyChanged("Checked");
                ConfigChecked = value;
            }
        }
        public Brush Stroke { get { var c = (LineGraph.Stroke as SolidColorPaint).Color; return new SolidColorBrush(Color.FromArgb(c.Alpha, c.Red,c.Green,c.Blue)); } }
        public string Description { get { return LineGraph.Name; } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }
    }
    public class LcVisibilityToCheckedConverter : IValueConverter
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
    public class LcChartDataCache
    {
        Dictionary<PosEnum, LcVoltagePointCollection> cache = new Dictionary<PosEnum, LcVoltagePointCollection>();
        public int Count { get { return cache.Count; } }
        public LcVoltagePointCollection this[PosEnum F_SensorId]
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

        internal void Add(PosEnum f_SensorId, LcVoltagePointCollection dictionary)
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


    public class LcVoltagePointCollection : RingArray<SensorDataItem>
    {
        private const int TOTAL_POINTS = 120 * 60 * 5;
        public LcVoltagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

    public class LcVoltagePoint
    {
        public TimeSpan F_AddTime { get; set; }

        public SensorDataItem SensorDataItem { get; set; }
        public double F_LoadDataValue { get; set; }

        public LcVoltagePoint(TimeSpan date, SensorDataItem voltage)
        {
            this.F_AddTime = date;
            this.SensorDataItem = voltage;
        }
    }


}
