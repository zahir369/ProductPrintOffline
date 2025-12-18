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
using MKSS.APP.ECTester;
using MKSS.APP.ECTester.UserCommon;
using MKSS.Model;
using MKSS.Service.SemiTester;
using MKSS.Util.Log;
using Newtonsoft.Json;

namespace MKSS.APP.SemiTester
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

        PloyLineChart chart1 = null;
        PloyLineChart chart2 = null;
        List<Sensor> SensorList = null;
        /// <summary>
        ///  参考线
        /// </summary>
        Dictionary<string,SeriseDatas> SeriseDatasStd { get; set; }
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, SeriseDatas> LineGraphDic { get; set; }
        List<SeriseDatas> Series = new List<SeriseDatas>();
        MkssChartDataCache CacheData = new MkssChartDataCache();
        List<MkssLagendObject> LegendsA = new List<MkssLagendObject>();
        List<MkssLagendObject> LegendsB = new List<MkssLagendObject>();
        public UIZhuiSuC10Chart()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
             
            this.Loaded += UIZhuiSuC10Chart_Loaded;
             
        }

        bool PageLOaded { get; set; } = false;
        private void UIZhuiSuC10Chart_Loaded(object sender, RoutedEventArgs e)
        {
            PloyLineChart.XRightMargin = 10;
            chart1 = new PloyLineChart(this.grid1);
            chart2 = new PloyLineChart(this.grid2);
            chart1.Zooming.OnHitTestResult += Zooming_OnHitTestResult;
            chart2.Zooming.OnHitTestResult += Zooming_OnHitTestResult;
            chart1.ShowLinePoint = false; 
            chart2.ShowLinePoint = false;
            PloyLineChart.AxisXLabeleFormate = "f0";
            PloyLineChart.AxisYLabeleFormate = "f2";

            this.SetAxisLimits(-3, 60);
            LockX_Checked(null, null);

            if (LineGraphDic == null)
            {
                SensorList = new List<Sensor>();
                LegendsA = new List<MkssLagendObject>();//图例
                foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                {
                    CacheData.Add(sid, new MkssVoltagePointCollection());
                }
                SeriseDatasStd = new Dictionary<string, SeriseDatas>();//参考线
                LineGraphDic = new Dictionary<PosEnum, SeriseDatas>();
                foreach (PosEnum sid in Enum.GetValues<PosEnum>())
                {
                    var sel = CacheData[sid];
                    var lg = new SeriseDatas(String.Format("{0}", sid), sel.Select(w => (float)w.F_AddTime).ToList(), sel.Select(w => (float)w.F_LoadDataValue).ToList());
                    lg.Line1Color = new System.Drawing.SolidBrush(ColorDrawingUtil.Of.RandomNoDarkAlpha(200));
                    
                    if (sid.ToString().StartsWith("A")) chart1.AllDatas.Add(lg);
                    if (sid.ToString().StartsWith("B")) chart2.AllDatas.Add(lg);

                    LineGraphDic.Add(sid, lg);
                    if (sid.ToString().StartsWith("A")) LegendsA.Add(new MkssLagendObject(this, LineGraphDic[sid], sid, Series));
                    if (sid.ToString().StartsWith("B")) LegendsB.Add(new MkssLagendObject(this, LineGraphDic[sid], sid, Series));

                }
                this.legengA.ItemsSource = LegendsA;
                this.legengB.ItemsSource = LegendsB;
            }

            chart1.Init((int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到
            chart2.Init((int)grid2.ActualWidth, (int)grid2.ActualHeight);//不能用 img ，img 尺寸获取不到

            PageLOaded = true;
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
                result.Desc = " " + pos + "：" + SensorGroupData.ShowDesc(item.SensorGroupData, pos);
            }
        }

        private void Resistance_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            if (r == null) return;
            if (!PageLOaded) return;
            string con = r.Content + "";
            foreach (ResistanceEnum item in Enum.GetValues(typeof(ResistanceEnum)))
            {
                string str = SensorGroupData.ResistanceString(item);
                if (str == con)
                {
                    SensorGroupData.Resistance = item;
                    UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                    //this.SetAxisLimits();
                    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
                    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshMM();
                    con10.UIZhuiSuC10Chart.RefreshPage();
                    con10.UIZhuiSuC10SensorsPage.Refresh();
                    con10.UIZhuiSuC10Grid.RefreshData();
                    UIZhuiSuC10Model.Instance.SettingModel.RefreshPage();
                }
            }
        }

        public void Resistance_PreSet()
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
            string str = SensorGroupData.ResistanceString(SensorGroupData.Resistance);
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

            if (pre != null)
            {
                PAnelControl.Children.Remove(pre);
                PAnelControl.Children.Insert(PAnelControl.Children.IndexOf(cur)+1, pre);
            }

        }
        public void Resistance_NextSet()
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
            string str = SensorGroupData.ResistanceString(SensorGroupData.Resistance);
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

            if (nex != null) {
                PAnelControl.Children.Remove(nex);
                PAnelControl.Children.Insert(PAnelControl.Children.IndexOf(cur), nex);
            }
             
        }

        public void Resistance_Next()
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
            string str = SensorGroupData.ResistanceString(SensorGroupData.Resistance);
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


        public void Resistance_Pre()
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
            string str = SensorGroupData.ResistanceString(SensorGroupData.Resistance);
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
                            this.chart1.AppendAll();
                            if (pos.ToString().StartsWith("A")) this.chart1.AppendAll();
                            if (pos.ToString().StartsWith("B")) this.chart2.AppendAll();
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
                            if (pos.ToString().StartsWith("A")) this.chart1.AppendAll();
                            if (pos.ToString().StartsWith("B")) this.chart2.AppendAll();
                            CurrentFocusDoing = false;
                        }
                    }));

                }


             

        }

        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue, double yMin = double.MinValue, double yMax = double.MinValue)
        {
            if (chart1 == null || chart2 == null) return;
            PloyLineChart[] cs = new PloyLineChart[] { chart1, chart2 };
            foreach (var chart in cs)
            {
                if (UIZhuiSuC10Model.Instance.SettingModel == null || UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig == null || UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return;
                if (xMin != double.MinValue && xMax != double.MinValue) chart.XLabelMin = (float)xMin;
                else chart.XLabelMin = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin;
                 
                if (xMin != double.MinValue && xMax != double.MinValue) chart.XLabelMax = (float)xMax;
                else chart.XLabelMax = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMax;

                if (yMin != double.MinValue && yMax != double.MinValue) chart.YLabelMin = (float)yMin;
                else chart.YLabelMin = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;

                if (yMin != double.MinValue && yMax != double.MinValue) chart.YLabelMax = (float)yMax;
                else chart.YLabelMax = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax;

                //chart.YLabelMin = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin;
                //chart.YLabelMax = (float)UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax;

            }
            PloyLineChart.XLabelMinDefault = this.chart1.XLabelMin;
            PloyLineChart.XLabelMaxDefault = this.chart1.XLabelMax;
            PloyLineChart.YLabelMinDefault = this.chart1.YLabelMin;
            PloyLineChart.YLabelMaxDefault = this.chart1.YLabelMax;
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
                        PloyLineChart[] plots = new PloyLineChart[] { this.chart1, this.chart2 };
                        List<string> StdList = new List<string>();//记录添加项目
                        foreach (var p in UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr)
                        {
                            if (p == 0) continue;
                            for (int i = 0; i < plots.Length; i++)
                            {
                                string key = string.Format("{0}_{1}", i, p);
                                StdList.Add(key);
                                if (!SeriseDatasStd.ContainsKey(key))
                                {
                                    var lg = new SeriseDatas();
                                    bool is_zeroline = (p == UIZhuiSuC10Model.Instance.SettingModel.TxtZeroPoint)
                                        || (p == UIZhuiSuC10Model.Instance.SettingModel.TxtSpanPoint);
                                    plots[i].AddStdLineX(ColorDrawingUtil.Of.Alpha(!is_zeroline ? ColorDrawingUtil.Of.LightBlue : ColorDrawingUtil.Of.Red, 255), p);
                                    SeriseDatasStd.Add(key, lg);

                                }
                            }


                        }

                        //清除绘制参考线
                        foreach (var item in SeriseDatasStd.Keys.ToArray())
                        {
                            if (!StdList.Contains(item))
                            {
                                if (chart1.AllDatas.Contains(SeriseDatasStd[item])) chart1.AllDatas.Add(SeriseDatasStd[item]);
                                if (chart2.AllDatas.Contains(SeriseDatasStd[item])) chart2.AllDatas.Add(SeriseDatasStd[item]);
                                SeriseDatasStd.Remove(item);
                            }
                        }

                        this.RefreshPage();
                         
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

        TimeSpan MinInterval = TimeSpan.FromMilliseconds(1500);
        DateTime SetDataRealTime_OnGetMQDataRefreshUI = DateTime.MinValue;
        Queue<SensorGroupData> Buffer = new Queue<SensorGroupData>();
        DateTime SetDataRealTimeLastRender = DateTime.MinValue;//避免刷新太快了，卡界面
        public void SetDataRealTime(Model.Batch _batch, SensorGroupData d) {
             
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in d.Data.Values)
                {
                    PosEnum sid = item.PosEnum;
                    var sel = CacheData[sid];
                    sel.Add(item);
                }
                List<float> listA = d.Data.Where(w => w.Key.ToString().StartsWith("A")).Select(w => (float)w.Value.F_LoadDataValue).ToList();
                List<float> listB = d.Data.Where(w => w.Key.ToString().StartsWith("B")).Select(w => (float)w.Value.F_LoadDataValue).ToList();

                chart1.AppendData((float)d.F_AddTime, listA);
                chart2.AppendData((float)d.F_AddTime, listB);
                chart1.Img.InvalidateVisual();
                chart2.Img.InvalidateVisual();
                //int ttt = chart.Img.Dispatcher.Thread.ManagedThreadId;
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
                this.chart1.AppendAll();
                this.chart2.AppendAll();
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
            //if (this.LockX != null) plotter2.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            //if (this.LockY != null) plotter2.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            //if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            //Chart plotter = sender as Chart;
            //if (plotter == null) return;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin = plotter.PlotOriginY;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax = plotter.PlotOriginY + plotter.PlotHeight;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMin = plotter.PlotOriginX;
            //UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.XMax = plotter.PlotOriginX + plotter.PlotWidth;

            //SemiTesterConfgig.Save();

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

            try
            {

                BatchSelectIng = true;

                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                bool isA = it.Name.EndsWith("A");
                List<MkssLagendObject> lss = isA ? LegendsA : LegendsB;
                foreach (var item in lss)
                {
                    item.Checked = true;
                }
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

        private void btnUnSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                BatchSelectIng = true;
                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                bool isA = it.Name.EndsWith("A");
                List<MkssLagendObject> lss = isA ? LegendsA : LegendsB;
                foreach (var item in lss)
                {
                    item.Checked = false;
                }

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

        public bool BatchSelectIng { get; set; }
        private void btnRevSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                BatchSelectIng = true;
                ItemsControl it = sender as ItemsControl;
                if (it == null) return;
                bool isA = it.Name.EndsWith("A");
                List<MkssLagendObject> lss = isA ? LegendsA : LegendsB;
                foreach (var item in lss)
                {
                    item.Checked = !item.Checked;
                }
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
            UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin += 0.1;
            this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
            SemiTesterConfgig.Save();
            this.RefreshPage();
        }


        private void MinusYMin_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMin -= 0.1;
            this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
            SemiTesterConfgig.Save();
            this.RefreshPage();
        }


        private void AddYMax_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax += 0.1;
            this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
            SemiTesterConfgig.Save();
            this.RefreshPage();

        }


        private void MinusYMax_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.ResistanceConfig.YMax -= 0.1;
            this.SetAxisLimits(0, (UIZhuiSuC10Model.Instance.SettingModel.TxtTotalSpan) * 1.3);
            SemiTesterConfgig.Save();
            this.RefreshPage();
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
            string sh = SemiTesterConfgig.Instance.ShowChart + "";
            if (string.IsNullOrEmpty(sh))
            {
                List<string> arr = new List<string>();
                for (int i = 0; i < 64; i++) arr.Add("1");
                SemiTesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(arr);
                SemiTesterConfgig.Save();
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
                string sh = SemiTesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(SemiTesterConfgig.Instance.ShowChart);
                return json[PosIndex] == "1";
            }
            set
            {
                string sh = SemiTesterConfgig.Instance.ShowChart + "";
                List<string> json = JsonConvert.DeserializeObject<List<string>>(SemiTesterConfgig.Instance.ShowChart);
                json[PosIndex] = value ? "1" : "0";
                SemiTesterConfgig.Instance.ShowChart = JsonConvert.SerializeObject(json);
                SemiTesterConfgig.Save();
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
                if (Page.IsLoaded)
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
