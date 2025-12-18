using DeviceDataMonitorWPF;
using DeviceDataMonitorWPF.UIBiaoDing;
using DeviceDataMonitorWPF.UIBiaoDing.Config;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using InteractiveDataDisplay.WPF;
using Microsoft.Win32;
using MKSS.Model;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
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

namespace MKSS.APP.BiaodingAlcohol.UIBiaoDing
{


    /// <summary>
    /// UISheBeiBiaoChartWindows.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoChartWindows : Window
    {

        public UISheBeiBiaoChartWindows()
        {
            InitializeComponent();
            this.SetAxisLimits();
            LockX_Checked(null, null);
        }

        public static void ShowChart(SensorGroupDataModel groupData, Address address, string fieldName) {

            string title = "";
            if (fieldName == "XINHAO_AD") title = "实时AD";
            if (fieldName == "CANKAO_AD") title = "零点AD";
            if (fieldName == "WENDU_AD") title = "SPAN点AD";
            if (UISheBeiBiaoDingViewModel.Intance.StaZeroSpanTitleExchange)
            {
                if (fieldName == "CANKAO_AD") title = "SPAN点AD";
                if (fieldName == "WENDU_AD") title = "零点AD";
            }

            if (fieldName == "CANKAO_YULIU1") title = "标定点浓度";
            if (fieldName == "CANKAO_YULIU2") title = "预留2";
            if (fieldName == "CANKAO_YULIU3") title = "预留3";
            if (fieldName == "NONGDU") title = "实时浓度";
            if (fieldName == "WENDU1") title = "温度1";
            if (fieldName == "WENDU2") title = "温度2";
            if (fieldName == "MONIDIANYA") title = "模拟电压";

            if (fieldName == "XINHAO_AD") fieldName = "V1";
            if (fieldName == "CANKAO_AD") fieldName = "V2";
            if (fieldName == "CANKAO_YULIU1") fieldName = "V3";
            if (fieldName == "WENDU_AD") fieldName = "V4";
            if (fieldName == "CANKAO_YULIU2") fieldName = "V5";
            if (fieldName == "CANKAO_YULIU3") fieldName = "V6";
            if (fieldName == "NONGDU") fieldName = "V7";
            if (fieldName == "WENDU1") fieldName = "V8";
            if (fieldName == "WENDU2") fieldName = "V9";
            if (fieldName == "MONIDIANYA") fieldName = "V10";

           


            UISheBeiBiaoChartWindows ww = new UISheBeiBiaoChartWindows();
            ww.Topmost = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show();
            ww.SetBatch(groupData,address, fieldName, title);

        }


        public void SetAxisLimits(double xMin = double.MinValue, double xMax = double.MinValue, double yMin = double.MinValue, double yMax = double.MinValue)
        {
            if ( BiaoDingConfgig.Instance == null) return;
            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotOriginX = xMin;
            else plotter.PlotOriginX = BiaoDingConfgig.Instance.XMin;

            if (yMin != double.MinValue && yMax != double.MinValue) plotter.PlotOriginY = yMin;
            else plotter.PlotOriginY = BiaoDingConfgig.Instance.YMin;

            if (xMin != double.MinValue && xMax != double.MinValue) plotter.PlotWidth = xMax - xMin;
            else plotter.PlotWidth = BiaoDingConfgig.Instance.XMax - BiaoDingConfgig.Instance.XMin;

            if (yMin != double.MinValue && yMax != double.MinValue) plotter.PlotHeight = yMax - yMin;
            else plotter.PlotHeight = BiaoDingConfgig.Instance.YMax - BiaoDingConfgig.Instance.YMin;

        }

        Address Address;
        string FieldName;
        string FieldNameTitle;
        SensorGroupDataModel GroupData;
        List<Sensor> tabs = null;
        Dictionary<string, LineGraph> LineGraphDic { get; set; }
        public void SetBatch(SensorGroupDataModel groupData,Address address,string fieldName,string title )
        {
            GroupData = groupData;
            Address = address; FieldName = fieldName; FieldNameTitle = title;
            var batch = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentBatch;
            this.Title = string.Format("{2}#{1}地址{0}历史曲线", title, address.V, batch==null? "": batch.F_BatchName + batch.F_BatchId + " " );
            this.ChartTitle.Text = this.Title;

            if (address.V <= 0) return;
            var tables = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable;
            if (!tables.ContainsKey(address.V)) return;
             
            SetAxisLimits();
            //历史批次直接读数据库
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

            LineGraphDic = new Dictionary<string, LineGraph>();

            int xmax = 0;
            tabs = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentSensors;
            Batch _Batch = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentBatch;
            DataTable table = UISheBeiBiaoDingViewModel.Intance.DbService.QueryDataOf(address.V, fieldName);
            if (tabs == null|| _Batch==null|| table==null) return;

            ChartDataCache cache = new ChartDataCache();
            if (table != null) {
                tabs = tabs.OrderBy(w => w.F_SlotNO).ToList();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow dr = table.Rows[i];
                    string F_SensorId = dr["S"] + "";
                    DateTime F_AddTime = Convert.ToDateTime(dr["D"]);
                    int value = Convert.ToInt32(dr["V"]);

                    if (!LineGraphDic.ContainsKey(F_SensorId))
                    {
                        cache.Add(F_SensorId,new VoltagePointCollection());
                        Sensor s = tabs.FirstOrDefault(w => w.F_SensorId == F_SensorId);
                        var res = AddLineGraph(s);//  lines.Children.AddScatter(dic_x[sid].ToArray(), dic_y[sid].ToArray());
                        if (res == null) continue;
                        if (!LineGraphDic.ContainsKey(s.F_SensorId)) LineGraphDic.Add(s.F_SensorId, res);
                    }
                    cache[F_SensorId].Add(new VoltagePoint((F_AddTime - _Batch.F_AgingStartTime), value));
                    if ((F_AddTime - _Batch.F_AgingStartTime).TotalSeconds > xmax) xmax = (int)(F_AddTime - _Batch.F_AgingStartTime).TotalSeconds;
                }
            }
            foreach (var F_SensorId in LineGraphDic.Keys)
            {
                LineGraphDic[F_SensorId].Plot(cache[F_SensorId].Select(w=>w.Date.TotalSeconds), cache[F_SensorId].Select(w => w.Voltage));
            }


            if(xmax>0) this.SetAxisLimits(0, xmax + 10);

        }


        LineGraph AddLineGraph(Sensor _Sensor)
        {
            string F_SensorId = _Sensor.F_SensorId;
            var lg = new LineGraph();
            lines.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(ColorUtil.Of.Populor15[_Sensor.F_SlotNO-1]);
            lg.Description = String.Format("位置{0}", _Sensor.F_SlotNO);
            lg.StrokeThickness = 3;
            lg.Tag = _Sensor;
            lg.DataContext = new SolidColorBrush(QualifiedColor(_Sensor.F_SlotNO));
            return lg;
        }

        private void BtnSetQualified_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            Button _Button = sender as Button;
            if (_Button == null) return;
            Sensor _Sensor = _Button.Tag as Sensor;
            if (_Sensor == null) return;
            SwithQualified(_Sensor.F_SlotNO);
            _Button.Background = new SolidColorBrush(QualifiedColor(_Sensor.F_SlotNO));
        }
        Color QualifiedColor(int pos) {
            bool? b = UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified[Address.V, pos];
            if (b == null || b.Value) return Colors.Transparent;
            return Colors.Red;
        }

        void SwithQualified(int WEIZHI)
        {
            SensorGroupDataModel data = this.GroupData;
            if (WEIZHI > 0 && WEIZHI <= data.ProductTable.Count)
            {
                if (!data.ProductTable[WEIZHI - 1].IsEmpSensor)
                {
                    bool Qualified = !UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified.NotQualified(data.Address.V, WEIZHI);
                    UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified[data.Address.V, WEIZHI] = !Qualified;

                    var qq = UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified[data.Address.V, WEIZHI];
                    UISheBeiBiaoDingViewModel.Intance.DbService.SetHEGE(GroupData.Address, WEIZHI, qq.Value);
                    UISheBeiBiaoDingViewModel.Intance.PageContext_Loaded(null,null);
                }
            }
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
            BiaoDingConfgig.Instance.YMin = plotter.PlotOriginY;
            BiaoDingConfgig.Instance.YMax = plotter.PlotOriginY + plotter.PlotHeight;
            BiaoDingConfgig.Instance.XMin = plotter.PlotOriginX;
            BiaoDingConfgig.Instance.XMax = plotter.PlotOriginX + plotter.PlotWidth;
            BiaoDingConfgig.Save();
        }

        private void plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            plotter_MouseWheel(null, null);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

            string name_scrwd = null;
            if (UISheBeiBiaoDingViewModel.Intance.ScrwEntity != null)
            {
                name_scrwd = string.Format("{0}[{1}]{2}", UISheBeiBiaoDingViewModel.Intance.ScrwEntity.ProductFullName, UISheBeiBiaoDingViewModel.Intance.ScrwEntity.OrderNumber, name_scrwd);
            }

            var batch = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentBatch; 
            string title = string.Format("{2}#{1}{0}历史数据", FieldNameTitle, Address.V,
                name_scrwd==null ? (batch == null ? "" : batch.F_BatchName + batch.F_BatchId + " ") : name_scrwd);

            
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
                    ETE.SetCellValue<string>(1, 1, "时间（秒）");
                    for (int i = 1; i <= 15; i++)
                    {
                        string info = "位置" + i;
                        if (this.GroupData.ProductTable != null ) {
                            string Serial = this.GroupData.ProductTable[i-1].Serial;
                            if (!string.IsNullOrEmpty(Serial))
                            {

                                info = info + "["+ Serial + "]";
                            }
                        }
                        ETE.SetCellValue<string>(1, 1 + i, info);
                    }
                     

                    tabs = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentSensors;
                    Batch _Batch = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentBatch;
                    DataTable table = UISheBeiBiaoDingViewModel.Intance.DbService.QueryDataOfTime(Address.V, FieldName);
                    var sen_dic = tabs.ToDictionary(w => w.F_SensorId, w => w);
                    if (table == null) return;

                    int rowno = 1;
                    DateTime F_AddTimePre = DateTime.MinValue;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dr = table.Rows[i];
                        string F_SensorId = dr["S"] + "";
                        DateTime F_AddTime = Convert.ToDateTime(dr["D"]);
                        int value = Convert.ToInt32(dr["V"]);
                        if (sen_dic.ContainsKey(F_SensorId))
                        {

                            if (F_AddTime != F_AddTimePre)
                            {
                                rowno++;
                                F_AddTimePre = F_AddTime;
                                ETE.SetCellValue<double>(rowno, 1, (F_AddTime- _Batch.F_AgingStartTime).TotalSeconds);
                            }
                            ETE.SetCellValue<int>(rowno, sen_dic[F_SensorId].F_SlotNO + 1, value);
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
