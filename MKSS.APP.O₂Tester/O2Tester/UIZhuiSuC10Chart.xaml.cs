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
using MKSS.APP.ECTester;
using MKSS.APP.ECTester.UserCommon;
using MKSS.APP.O2Tester.UserCommon;
using MKSS.Model;
using MKSS.Service.O2Tester;
using MKSS.Util.Log;
using Newtonsoft.Json;

namespace MKSS.APP.O2Tester
{

    /// <summary>
    /// UIZhuiSuC10ChartV1.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Chart : UserControl
    {

        float defalut_thickness = 1;
        float focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;

        PloyLineChart chart = null;
        List<Sensor> SensorList = null;
        /// <summary>
        ///  参考线
        /// </summary>
        Dictionary<string,SeriseDatas> LineGraphStd { get; set; }
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, SeriseDatas> LineGraphDic { get; set; }
        List<SeriseDatas> Series = new List<SeriseDatas>();
        MkssChartDataCache CacheData = new MkssChartDataCache();
        List<MkssLagendObject> LegendsA = new List<MkssLagendObject>();
        public UIZhuiSuC10Chart()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
                                                           //时间放到初始值之后，避免事件呗激发
            ShowModeDY.Checked += ShowMode_Checked;
            ShowModeAD.Checked += ShowMode_Checked;
            ShowModeNogDu.Checked += ShowMode_Checked;
            this.Loaded += UIZhuiSuC10Chart_Loaded; 
        }


        private void UIZhuiSuC10Chart_Loaded(object sender, RoutedEventArgs e)
        {

            if (chart == null) {

                chart = new PloyLineChart(this.grid1);
                chart.ShowLinePoint = false;
                chart.Zooming.OnHitTestResult += Zooming_OnHitTestResult;


                this.SetAxisLimits(-3, 120);
                LockX_Checked(null, null);
                ShowMode_Checked(null, null);

                if (LineGraphDic == null)
                {
                    SensorList = new List<Sensor>();
                    LegendsA = new List<MkssLagendObject>();//图例
                    foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                    {
                        CacheData.Add(sid, new MkssVoltagePointCollection());
                    }
                    LineGraphStd = new Dictionary<string, SeriseDatas>();//参考线
                    LineGraphDic = new Dictionary<PosEnum, SeriseDatas>();
                    foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                    {
                        var sel = CacheData[sid];
                        var lg = new SeriseDatas(String.Format("{0}", sid), sel.Select(w => (float)w.F_AddTime).ToList(), sel.Select(w => (float)w.F_LoadDataValue).ToList());
                        lg.Line1Color = new System.Drawing.SolidBrush(ColorDrawingUtil.Of.RandomNoDarkAlpha(200));
                        chart.AllDatas.Add(lg);
                        LineGraphDic.Add(sid, lg);
                        LegendsA.Add(new MkssLagendObject(this, LineGraphDic[sid], sid, Series));
                    }
                    this.legengA.ItemsSource = LegendsA;
                }

                chart.Init((int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到

            }


        }


        PosEnum Zooming_OnHitTestResultPosEnum = PosEnum.A1;
        private void Zooming_OnHitTestResult(HitTester.HitTestResult result)
        {
            PosEnum pos = Enum.Parse<PosEnum>(result.SeriseDatas.Title);
            FocusSensor(Zooming_OnHitTestResultPosEnum, false);
            FocusSensor(pos, true);
            Zooming_OnHitTestResultPosEnum = pos;
            SensorDataItem item = this.CacheData[pos].FirstOrDefault(w => w.F_AddTime.ToString("f2") == result.SeriseDatas.XData[result.Index].ToString("f2"));
            if (item != null)
            {
                result.Desc =  SensorGroupData.ShowDesc(item.SensorGroupData, pos);
            }
        }

        private void ShowMode_Checked(object sender, RoutedEventArgs e)
        {
       
            try
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
                        UIZhuiSuC10SensorsSET.RefreshMM();

                        con10.UIZhuiSuC10Chart.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                        con10.UIZhuiSuC10Chart.RefreshPage();
                        con10.UIZhuiSuC10SensorsPage.Refresh();
                        con10.UIZhuiSuC10Grid.RefreshData();
                        UIZhuiSuC10Model.Instance.SettingModel.RefreshPage();

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {
                PloyLineChart.AxisXLabeleFormate = Model.SensorGroupData.ShowMode == ShowModeEnum.AD ? "f1" : "f1";
                PloyLineChart.AxisYLabeleFormate = Model.SensorGroupData.ShowMode == ShowModeEnum.AD ? "f0" : "f2";
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
            for (int i = 5; i < list.Count; i++)
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
         
        /// <summary>
        ///  T90参考线
        /// </summary>
        Dictionary<string, SeriseDatas> LineGraphT90 { get; set; } = new Dictionary<string, SeriseDatas>();
        public void ShowStdLine(string key,double v)
        {

            try
            {

                if (v <= 0) {
                    if (LineGraphT90.ContainsKey(key)) {
                        LineGraphT90.Remove(key);
                    }
                }

                if (!LineGraphT90.ContainsKey(key))
                {
                    var lg = new SeriseDatas();
                    bool is_zeroline = key.EndsWith("T90") || key.EndsWith("T10");
                    this.chart.AddStdLineX(ColorDrawingUtil.Of.Alpha(!is_zeroline ? ColorDrawingUtil.Of.Yellow : ColorDrawingUtil.Of.OrangeRed, 255), v);
                    LineGraphT90.Add(key, lg);
                }
                else
                {
                    var lg = LineGraphT90[key];
                    bool is_zeroline = key.EndsWith("T90") || key.EndsWith("T10");
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
                            if (sel_old != null) {
                                sel_old.Thinkness = defalut_thickness;
                            }
                           
                        }

                        var sel_new = LineGraphDic[pos];
                        var data_new = CacheData[pos];
                        if (data_new.Count > 0)
                        {
                            sel_new.Thinkness = this.focus_thickness;
                            this.chart.AppendAll();
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
                        sel_old.Thinkness = defalut_thickness;
                        var data_old = CacheData[pos];
                        if (data_old.Count > 0)
                        {
                            this.chart.AppendAll();
                            CurrentFocusDoing = false;
                        }
                    }));

                }


             

        }

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue)
        {
            if (UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
            if (xMin != double.MinValue && xMax != double.MinValue) this.chart.XLabelMin = (float)xMin;
            else this.chart.XLabelMin = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMin;


            if (xMin != double.MinValue && xMax != double.MinValue) this.chart.XLabelMax = (float)xMax;
            else this.chart.XLabelMax = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.XMax;
            //xAxis.MinStep = (int)((this.chart.XLabelMax - this.chart.XLabelMin) / 15);

            switch (SensorGroupData.ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    this.chart.YLabelMin
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinDianYa;
                    this.chart.YLabelMax
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxDianYa;
                    break;
                case ShowModeEnum.NongDu:
                    this.chart.YLabelMin
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinNongDu;
                    this.chart.YLabelMax
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxNongDu;
                    break;
                case ShowModeEnum.AD:
                    this.chart.YLabelMin
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinAD;
                    this.chart.YLabelMax
                        = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxAD;
                    break;
                default:
                    break;
            }
            PloyLineChart.XLabelMinDefault = this.chart.XLabelMin;
            PloyLineChart.XLabelMaxDefault = this.chart.XLabelMax;
            PloyLineChart.YLabelMinDefault = this.chart.YLabelMin;
            PloyLineChart.YLabelMaxDefault = this.chart.YLabelMax;
            //yAxis.MinStep = ((this.chart.YLabelMax - this.chart.YLabelMin) / 5).Value;
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
                            item.clear();
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
                                this.chart.AddStdLineX(ColorDrawingUtil.Of.Alpha(ColorDrawingUtil.Of.OrangeRed, 255), p);
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

                        chart.Init((int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到
                        chart.SetDefaultAxis();
                        chart.AppendAll();

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
         
        public void SetDataRealTime(Model.Batch _batch, SensorGroupData d) {


            UIZhuiSuC10SensorsSET.SetData(_batch, d, TimeSpan.FromSeconds(d.F_AddTime));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in d.Data.Values)
                {
                    PosEnum sid = item.PosEnum;
                    var sel = CacheData[sid];
                    sel.Add(item);
                }
                List<float> list = d.Data.Select(w => (float)w.Value.F_LoadDataValue).ToList();
                chart.AppendData((float)d.F_AddTime, list);
                chart.Img.InvalidateVisual();
                int ttt = chart.Img.Dispatcher.Thread.ManagedThreadId;
            }));
             
        }
         

        public void RefreshPage()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (LineGraphDic == null) return;
                foreach (var pose in LineGraphDic.Keys)
                {
                    SeriseDatas line = LineGraphDic[pose];
                    var sel = CacheData[pose];
                    line.XData.Clear();
                    line.RtData.Clear();
                    line.XData.AddRange(sel.Select(w => (float)w.F_AddTime));
                    line.RtData.AddRange(sel.Select(w => (float)w.F_LoadDataValue));
                }
                this.chart.AppendAll();
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
            //O2TesterConfgig.Save();


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


        public bool BatchSelectIng { get; set; }
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchSelectIng = true;
                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                List<MkssLagendObject> lss = LegendsA;
                foreach (var item in lss)
                {
                    item.Checked = true;
                }
                this.RefreshPage();
            }
            catch (Exception ex)
            {
                throw ex;
            }finally {
                BatchSelectIng = false;
            }

        }

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchSelectIng = true; 
                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                List<MkssLagendObject> lss = LegendsA;
                foreach (var item in lss)
                {
                    item.Checked = false;
                }
                this.RefreshPage();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                BatchSelectIng = false;
            }

            
        }

        private void btnRevSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchSelectIng = true;

                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                List<MkssLagendObject> lss = LegendsA;
                foreach (var item in lss)
                {
                    item.Checked = !item.Checked;
                }
                this.RefreshPage();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                BatchSelectIng = false;
            }

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshPage(); 
        }

        private void AddYMin_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.AD)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinAD += 100;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinDianYa += 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.NongDu)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinNongDu += 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }

        }


        private void MinusYMin_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.AD)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinAD -= 100;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinDianYa -= 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.NongDu)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMinNongDu -= 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
        }


        private void AddYMax_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.AD)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxAD += 100;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxDianYa += 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.NongDu)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxNongDu += 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }

        }


        private void MinusYMax_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.AD)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxAD -= 100;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.DuanDianYa)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxDianYa -= 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
            if (Model.SensorGroupData.ShowMode == ShowModeEnum.NongDu)
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ShowModeConfig.YMaxNongDu -= 0.1;
                this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
                O2TesterConfgig.Save();
                this.RefreshPage();
            }
        }

    }

    public class MkssLagendObject : INotifyPropertyChanged
    {
        public SeriseDatas LineGraph;
        public List<SeriseDatas> Lines;
        UIZhuiSuC10Chart Page;
        public MkssLagendObject(UIZhuiSuC10Chart page, SeriseDatas l, PosEnum p, List<SeriseDatas> lines)
        {
            Page = page;
            LineGraph = l;
            PosEnum = p;
            Lines = lines;
            string sh = O2TesterConfgig.Instance.ShowChart + "";
            if (string.IsNullOrEmpty(sh))
            {
                List<string> arr = new List<string>();
                for (int i = 0; i < 64; i++) arr.Add("1");
                O2TesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(arr);
                O2TesterConfgig.Save();
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
                string sh = O2TesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(O2TesterConfgig.Instance.ShowChart);
                return json[PosIndex] == "1";
            }
            set
            {
                string sh = O2TesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(O2TesterConfgig.Instance.ShowChart);
                json[PosIndex] = value ? "1" : "0";
                O2TesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(json);
                O2TesterConfgig.Save();
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
                if (Page.IsLoaded && !Page.BatchSelectIng)
                {
                    Page.RefreshPage();
                }
            }
        }
        public Brush Stroke { get { var c = (LineGraph.Line1Color as System.Drawing.SolidBrush).Color; return new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)); } }
        public string Description { get { return LineGraph.Title; } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }
    }
    public class MkssVisibilityToCheckedConverter : IValueConverter
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
    public class MkssChartDataCache
    {
        Dictionary<PosEnum, MkssVoltagePointCollection> cache = new Dictionary<PosEnum, MkssVoltagePointCollection>();
        public int Count { get { return cache.Count; } }
        public MkssVoltagePointCollection this[PosEnum F_SensorId]
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

        internal void Add(PosEnum f_SensorId, MkssVoltagePointCollection dictionary)
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


    public class MkssVoltagePointCollection : RingArray<SensorDataItem>
    {
        private const int TOTAL_POINTS = 120 * 60 * 5;
        public MkssVoltagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

    public class MkssVoltagePoint
    {
        public TimeSpan F_AddTime { get; set; }

        public SensorDataItem SensorDataItem { get; set; }
        public double F_LoadDataValue { get; set; }

        public MkssVoltagePoint(TimeSpan date, SensorDataItem voltage)
        {
            this.F_AddTime = date;
            this.SensorDataItem = voltage;
        }
    }


}
