
using DeviceDataMonitorWPF;
using DeviceDataMonitorWPF.UserCommon;
using InteractiveDataDisplay.WPF;
using Microsoft.Win32; 
using MKSS.Model;
using MKSS.Service.LaoHuaElectroChemical;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
using System.Windows.Shapes;

namespace MKSS.APP.SemiTester
{


    /// <summary>
    /// UISheBeiBiaoChartWindows.xaml 的交互逻辑
    /// </summary>
    public partial class UIChartWindows : Window
    {
        public static UIChartWindows Instance { get; set; }
        List<LagendObject> LegendsA = new List<LagendObject>();
        public UIChartWindows()
        {
            InitializeComponent();
            this.SetAxisLimits();
            LockX_Checked(null, null);

            LegendsA = new List<LagendObject>();//图例
            LineGraphDic = new Dictionary<PosEnum, LineGraph>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                LineGraphDic.Add(sid, AddLineGraph(sid)); 
                LegendsA.Add(new LagendObject(LineGraphDic[sid]));
                cache.Add(sid, new VoltagePointCollection());
            }
            this.legengA.ItemsSource = LegendsA;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Instance = null;
        }

        public static void ShowChart(Batch batch, int FloorNo) {

            string title = "";
            bool ShowND = UIZhuiSuC10Model.Instance.SettingModel.ValueMode == SensorItemValueEnum.ValueND;
            title = string.Format("板卡{0}" + (ShowND ? "氧气浓度（%）" : "氧气传感器端电压（毫伏）"), FloorNo);


            UIChartWindows ww = new UIChartWindows();
            ww.Owner = MainWindow.Instance; 
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show();
            ww.SetBatch(batch,FloorNo,  title);

        }


        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue, double yMin = double.MinValue, double yMax = double.MinValue)
        {
            if ( ElectroChemicalConfgig.Instance == null) return;
            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotOriginX = xMin;
            else plotter.PlotOriginX = ElectroChemicalConfgig.Instance.XMin;

            if (yMin != double.MinValue && yMax != double.MinValue) plotter.PlotOriginY = yMin;
            else plotter.PlotOriginY = ElectroChemicalConfgig.Instance.YMin;

            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotWidth = xMax - xMin;
            else plotter.PlotWidth = ElectroChemicalConfgig.Instance.XMax - ElectroChemicalConfgig.Instance.XMin;

            if (yMin != double.MinValue && yMax != double.MinValue) plotter.PlotHeight = yMax - yMin;
            else plotter.PlotHeight = ElectroChemicalConfgig.Instance.YMax - ElectroChemicalConfgig.Instance.YMin;

        }

        double xmax = 0;
        int FloorNo; 
        string FieldNameTitle;
        Batch BatchCurrent; 
        List<SensorGroupData> table = null;
        Dictionary<PosEnum, LineGraph> LineGraphDic { get; set; }
        ChartDataCache cache = new ChartDataCache();
        public void SetBatch(Batch batch,int floorNo, string title )
        {
            xmax = 300;
            BatchCurrent = batch; 
            FloorNo = floorNo;  FieldNameTitle = title; 
            this.Title = string.Format("{0}历史曲线", title);
            this.ChartTitle.Text = this.Title;

            if (floorNo <= 0) return;
             
            SetAxisLimits();
            //历史批次直接读数据库
            GC.Collect();

            table = ElectroChemicalService.QueryData(BatchCurrent.F_BatchId, -1,floorNo);
            if (table == null|| BatchCurrent == null|| floorNo<=0|| table.Count==0) return;

            if (table != null) { 
                for (int i = 0; i < table.Count; i++)
                {
                    SensorGroupData dr = table[i];
                    foreach (PosEnum F_SensorId in SensorGroupData.ValuePropertyInfoCache.Keys)
                    { 
                        SensorDataItem val = dr.F_DataValue(F_SensorId);
                        double F_AddTime = (val.F_AddTime);
                        double value = val.F_DataValue;
                        if (value <= 0) continue;

                        cache[F_SensorId].Add(new VoltagePoint(TimeSpan.FromSeconds(F_AddTime), value));
                        if (F_AddTime > xmax) xmax = F_AddTime;
                    }
                    
                }
            }
            foreach (PosEnum F_SensorId in LineGraphDic.Keys)
            {
                LineGraphDic[F_SensorId].Plot(cache[F_SensorId].Select(w=>w.Date.TotalSeconds), cache[F_SensorId].Select(w => w.Voltage));
            }

            if(xmax>0) this.SetAxisLimits(0, xmax*1.3);

        }

        public void AppendPoint(Batch batch, SensorGroupData dr)
        {
            if (batch.F_BatchId != this.BatchCurrent.F_BatchId || dr.F_FloorNo!= FloorNo) return;
            foreach (PosEnum F_SensorId in SensorGroupData.ValuePropertyInfoCache.Keys)
            {
                SensorDataItem val = dr.F_DataValue(F_SensorId);
                double F_AddTime = (val.F_AddTime);
                double value = val.F_DataValue;
                if (value <= 0) continue;
                if (!LineGraphDic.ContainsKey(F_SensorId))
                {
                    cache.Add(F_SensorId, new VoltagePointCollection());
                    var res = AddLineGraph(F_SensorId);//  lines.Children.AddScatter(dic_x[sid].ToArray(), dic_y[sid].ToArray());
                    if (res == null) continue;
                    if (!LineGraphDic.ContainsKey(F_SensorId)) LineGraphDic.Add(F_SensorId, res);
                }
                cache[F_SensorId].Add(new VoltagePoint(TimeSpan.FromSeconds(F_AddTime), value));
                if (F_AddTime > xmax) xmax = F_AddTime;
            }
            foreach (PosEnum F_SensorId in LineGraphDic.Keys)
            {
                LineGraphDic[F_SensorId].Plot(cache[F_SensorId].Select(w => w.Date.TotalSeconds), cache[F_SensorId].Select(w => w.Voltage));
            }
            if (xmax > 0) this.SetAxisLimits(0, xmax * 1.3);
        }

        LineGraph AddLineGraph(PosEnum pos)
        {
            PosEnum F_SensorId = pos;
            var lg = new LineGraph();
            lines.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(ColorUtil.Of.RandomNoDark);
            lg.Description = String.Format("{0}", pos);
            lg.StrokeThickness = 1;
            lg.Tag = pos; 
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

        int xmin_sec = -10;
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
            if (this.LockX != null) plotter.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            if (this.LockY != null) plotter.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

        private void plotter_MouseWheel(object sender, MouseWheelEventArgs e)
        { 
            ElectroChemicalConfgig.Instance.YMin = plotter.PlotOriginY;
            ElectroChemicalConfgig.Instance.YMax = plotter.PlotOriginY + plotter.PlotHeight;
            ElectroChemicalConfgig.Instance.XMin = plotter.PlotOriginX;
            ElectroChemicalConfgig.Instance.XMax = plotter.PlotOriginX + plotter.PlotWidth;
            ElectroChemicalConfgig.Save();
        }

        private void plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            plotter_MouseWheel(null, null);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

            string name_scrwd = null;
             

            var batch = BatchCurrent;
            string title = "";
            bool ShowND = UIZhuiSuC10Model.Instance.SettingModel.ValueMode == SensorItemValueEnum.ValueND;
            title = string.Format("板卡{0}" + (ShowND ? "氧气浓度（%）" : "氧气传感器端电压（毫伏）"), FloorNo);


            string name = DateTime.Now.ToString(title);
            var dlg = new SaveFileDialog()
            {
                Title = title + "-另存为",
                DefaultExt = "xlsx",
                Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                FileName = name
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    //this.SerialModBus.Qualified.SaveCheckedDataToExcel(dlg.FileName);

                    ExcuteToExcel ETE = new ExcuteToExcel("历史数据");
                    int hr = 1;
                    int hrUser = 13;
                    ETE.SetCellValue<string>(hr, 1, "时间");
                    ETE.SetCellValue<DateTime>(hr, 2, batch.F_AgingStartTime, "yyyy-MM-dd HH:mm");
                    ETE.SetCellValue<string>(hr, hrUser + 0, "操作人");
                    ETE.SetCellValue<string>(hr, hrUser + 1, "");
                    ETE.SetCellValue<string>(hr, hrUser + 2, "核验人");
                    ETE.SetCellValue<string>(hr, hrUser + 3, "");
                    ETE.SetCellValue<string>(hr + 1, 1, "时间（秒）");

                    
                    int x = 0;
                    Dictionary<PosEnum, int> sen_dic = new Dictionary<PosEnum, int>();
                    foreach (PosEnum pos in SensorGroupData.ValuePropertyInfoCache.Keys)
                    {
                        string F_SensorId = pos + ""; x++;
                        string info = "位置" + F_SensorId;
                        ETE.SetCellValue<string>(hr + 1, 1 + x, info);
                        sen_dic.Add(pos, 1 + x);
                    }
 
                    if (table == null) return;
                    int rowno = hr + 1;
                    double F_AddTimePre = double.MinValue;

                    for (int i = 0; i < table.Count; i++)
                    {
                        SensorGroupData dr = table[i];
                        foreach (PosEnum pos in SensorGroupData.ValuePropertyInfoCache.Keys)
                        {
                            string F_SensorId = pos + "";
                            SensorDataItem val = dr.F_DataValue(pos);
                            double F_AddTime = (val.F_AddTime);
                            double value = val.F_DataValue;

                            if (value <= 0) continue;
                            if (F_AddTime != F_AddTimePre)
                            {
                                rowno++;
                                F_AddTimePre = F_AddTime;
                                ETE.SetCellValue<double>(rowno, 1, F_AddTime);
                            }
                            ETE.SetCellValue<double>(rowno, sen_dic[pos], value);

                        }
  
                        
                    }

                    ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
                    ETE.SetRowStyleOfFontToBold(1);
                    ETE.FilePath = dlg.FileName;
                    ETE.SaveAsExcel();
                    ETE.Dispose();

                    MessageBox.Show("导出成功。");

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出出错");
                }


            }

        }
         
        private void CheckBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

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
            SetBatch(this.BatchCurrent, this.FloorNo, this.FieldNameTitle);
        }

    }


    public class LagendObject : INotifyPropertyChanged
    {
        public LineGraph LineGraph;
        public LagendObject(LineGraph l)
        {
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

        internal bool ContainsKey(PosEnum f_SensorId)
        {
            return cache.ContainsKey(f_SensorId);
        }
    }
    public class VoltagePointCollection : RingArray<VoltagePoint>
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
