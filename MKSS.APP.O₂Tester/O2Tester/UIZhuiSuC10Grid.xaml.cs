using MKSS.Model;
using MKSS.Service.O2Tester;
using MKSS.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

namespace MKSS.APP.O2Tester
{
    /// <summary>
    /// UIZhuiSuC10Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Grid : UserControl
    {
        public List<C10GridRow> BasdeData = new List<C10GridRow>();
        public UIZhuiSuC10Grid()
        {
            InitializeComponent();

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            string[] region = O2TesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= O2TesterService.RegionSize; c++)
                {
                    string n = string.Format("{0}{1}", boardNo, c);
                    BasdeData.Add(new C10GridRow() { WEIZHI = n });
                }
            } 
            this.dataGrid.DataContext = BasdeData;

            foreach (DataGridColumn item in dataGrid.Columns)
            {

            }

        }
        Batch Batch { get; set; } 
        public void SetData(Model.Batch _batch, SensorGroupData data, TimeSpan F_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (_batch == null || data == null) return;
            Batch = _batch;
            List<MKSS.Model.SensorDataItem> tables = data.Data.Values.ToList();
            int[] Seconds = UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr;
            Dictionary<int, MKSS.Model.SensorGroupData> dss = UIZhuiSuC10Model.Instance.ECService.GetDataRange(Seconds);
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    //Text="4,30,50,70,80,150,180,250,280,300" 
                    BasdeData[i].SensorId = tables[i].F_SensorId;
                    BasdeData[i].PageSensorModel = tables[i];
                    BasdeData[i].XINHAO_AD = tables[i].F_DataValueEmp ? "-" : (tables[i].F_DataValueEmp ? "-" : tables[i].F_LoadDataValue.ToString());

                    int[] secs = Seconds;
                    for (int x = 0; x < secs.Length; x++)
                    { 
                        PropertyInfo pro = BasdeData[i].GetType().GetProperty("BD_" + (x + 1));
                        if (pro != null) {
                            if (dss.ContainsKey(secs[x])) {
                                if (dss[secs[x]].Data.ContainsKey(BasdeData[i].PosEnum) && dss[secs[x]].Data[BasdeData[i].PosEnum] !=null)
                                {
                                    pro.SetValue(BasdeData[i], dss[secs[x]].Data[BasdeData[i].PosEnum].F_LoadDataValue.ToString());
                                }
                            }
                            //pro.SetValue(BasdeData[i], C10GridRow.DAR(tables[i], TimeSpan.FromSeconds(secs[x])));
                        } 
                    } 
                }
            }

        }

        public void RefreshData( )
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
             
            int[] Seconds = UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr;
            Dictionary<int, MKSS.Model.SensorGroupData> dss = UIZhuiSuC10Model.Instance.ECService.GetDataRange(Seconds);
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (BasdeData[i].PageSensorModel !=null)
                {
                    var dat = BasdeData[i].PageSensorModel;
                    //Text="4,30,50,70,80,150,180,250,280,300"
                    BasdeData[i].SensorId = dat.F_SensorId; 
                    BasdeData[i].XINHAO_AD = (dat.F_DataValueEmp ? "-" : dat.F_LoadDataValue.ToString());

                    int[] secs = Seconds;
                    for (int x = 0; x < secs.Length; x++)
                    {
                        PropertyInfo pro = BasdeData[i].GetType().GetProperty("BD_" + (x + 1));
                        if (pro != null)
                        {
                            if (dss.ContainsKey(secs[x]))
                            {
                                if (dss[secs[x]].Data.ContainsKey(BasdeData[i].PosEnum) && dss[secs[x]].Data[BasdeData[i].PosEnum] != null)
                                {
                                    pro.SetValue(BasdeData[i], dss[secs[x]].Data[BasdeData[i].PosEnum].F_LoadDataValue.ToString());
                                }
                            }
                            //pro.SetValue(BasdeData[i], C10GridRow.DAR(tables[i], TimeSpan.FromSeconds(secs[x])));
                        }
                    }
                }
            }

        }

        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Batch = _Batch;

            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (datas.Count > i)
                {
                    //Text="4,30,50,70,80,150,180,250,280,300"
                    BasdeData[i].SensorId = datas[i].F_SensorId;
                    BasdeData[i].Reset();
                }
            }
        }

        public void SetCaption(int[]  Seconds) {
            for (int i = 2; i < dataGrid.Columns.Count; i++)
            {
                DataGridColumn item = dataGrid.Columns[i];
                if (Seconds.Length > i - 2) item.Header = string.Format("第{0}秒", Seconds[i - 2]);
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.Batch == null) return;
            //C10GridRow _C10GridRow = this.dataGrid.SelectedItem as C10GridRow;
            //if (_C10GridRow != null)
            //{

            //    //DataTable dt_count = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
            //    //    string.Format("select count(*) from pd_sensordata_{0} where F_SensorId='{1}' order by F_AddTime",
            //    //    Batch.F_BatchId, _C10GridRow.SensorId))
            //    //    .Result;
            //    //int count = dt_count.Rows.Count > 0 ? Convert.ToInt32
            //    //    (dt_count.Rows[0][0]) : 0;

            //    //DateTime[] ds_arr = new DateTime[count];
            //    //int[] vs_arr = new int[count];

            //    var plt = new ScottPlot.Plot();
            //    ScottPlot.WpfPlotViewer w = new ScottPlot.WpfPlotViewer(plt, 1600, 400, string.Format("{0} {1}", this.Batch.F_BatchName, _C10GridRow.SensorId));
            //    //w.Icon = null;
            //    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //    //plt.SetViewLimits(0, (Batch.F_AgingEndTime - Batch.F_AgingStartTime).TotalSeconds, 0, 10000); ;
            //    plt.SetAxisLimits(new ScottPlot.AxisLimits(0, (Batch.F_AgingEndTime), 0, 1000));
            //    w.Show();
            //    List<double> ds = new List<double>();
            //    List<double> vs = new List<double>();

            //    DataTable dt = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
            //        string.Format("select F_DataValue v,F_AddTime t from pd_sensordata_{0} where F_SensorId='{1}' and F_DataValue!=170 order by F_AddTime",
            //        Batch.F_BatchId, _C10GridRow.SensorId))
            //        .Result;
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        ds.Add(Convert.ToDouble(dt.Rows[i]["t"]));
            //        double v = Convert.ToDouble(dt.Rows[i]["v"]);
            //        vs.Add(SensorData.CalcDataValueByLoad(v));
            //    }
            //    plt.AddScatter(ds.ToArray(), vs.ToArray());
            //    // Then tell the axis to display tick labels using a time format
            //    //plt.XAxis.DateTimeFormat(true);



            //}
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> sendor_ids = new List<string>();
            foreach (var item in dataGrid.SelectedItems)
            {
                C10GridRow t = item as C10GridRow;
                if (t != null)
                {
                    DataGridRow dr = (DataGridRow)(dataGrid.ItemContainerGenerator.ContainerFromItem(t));
                    sendor_ids.Add(t.SensorId);
                }
            }
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensors(sendor_ids);
        }
         
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, dataGrid);
            //int row = -1;
            //StringBuilder Copystr = new StringBuilder();
            //foreach (DataGridCellInfo info in dataGrid.SelectedCells)
            //{
            //    if(info.row)
            //    FrameworkElement element = info.Column.GetCellContent(info.Item);
            //    string str = ((TextBlock)info.Column.GetCellContent(info.Item)).Text;
            //    Copystr.Append("\t");
            //    Copystr.Append(str);

            //}

            //Clipboard.SetText(Copystr.ToString());

            //INPUT inDown = new INPUT();
            //inDown.type = KeyBoardExe.INPUT_KEYBOARD;
            //inDown.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.LeftCtrl);
            //inDown.ki.dwFlags = KeyBoardExe.KEYEVENTF_KEYUP;
            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));

            //inDown.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.C);
            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));

            //this.Dispatcher.BeginInvoke(
            //    new Action(() => { SendKeys.Send(element, text); }),
            //    DispatcherPriority.Input
            //);

            //INPUT inDown = new INPUT();
            //inDown.type = KeyBoardExe.INPUT_KEYBOARD;
            //inDown.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.LeftCtrl);
            //inDown.ki.dwFlags = 0;

            //// 按下Ctrl键
            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));

            //// 按下S键
            //inDown.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.C);

            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));


            //inDown.ki.dwFlags = KeyBoardExe.KEYEVENTF_KEYUP;

            //// 放开S键
            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));


            //inDown.ki.wVk = (short)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.LeftCtrl);

            //// 放开Ctrl键
            //KeyBoardExe.SendInput(1, ref inDown, Marshal.SizeOf(inDown));

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnExcelExport_Click(null, null);
        }

    }

    public class C10GridRow : INotifyPropertyChanged
    {
        public C10GridRow()
        {
          
        }
        //Text="4,30,50,70,80,150,180,250,280,300"
        public string WEIZHI { get; set; }
        public RegionEnum RegionEnum { get { return SensorGroupData.Region(WEIZHI); } }
        public PosEnum PosEnum { get { return SensorGroupData.PosEnum(WEIZHI); } }
        public string XINHAO_AD { get; set; }
        public string BD_1 { get; set; }
        public string BD_2 { get; set; }
        public string BD_3 { get; set; }
        public string BD_4 { get; set; }
        public string BD_5 { get; set; }
        public string BD_6 { get; set; }
        public string BD_7 { get; set; }
        public string BD_8 { get; set; }
        public string BD_9 { get; set; }
        public string BD_10 { get; set; }
        public string BD_11 { get; set; }
        public string BD_12 { get; set; }
        public string SensorId { get; internal set; }

        public void Reset() {
            XINHAO_AD = null;
            BD_1 = null;
            BD_2 = null;
            BD_3 = null;
            BD_4 = null;
            BD_5 = null;
            BD_6 = null;
            BD_7 = null;
            BD_8 = null;
            BD_9 = null;
            BD_10 = null;
            BD_11 = null;
            BD_12 = null;
        }
        public MKSS.Model.SensorDataItem PageSensorModel { get; internal set; }

        public static string DAR(MKSS.Model.SensorDataItem s,TimeSpan t) {
            if (s.F_DataValueEmp) return "-";
            //if (s.Ranges.ContainsKey(t)) return s.Ranges[t].ToBdString();
            return "-";
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }

    // 定义转换器
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class DataColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString() == "")
                return new SolidColorBrush(Colors.Transparent);
            bool HEGE = (bool)value;
            if (!HEGE)
            {
                try
                {
                    return new SolidColorBrush(Color.FromRgb(211, 63, 52));
                }
                catch
                { throw; }
            }
            return new SolidColorBrush(Colors.Transparent); //new SolidColorBrush(Color.FromRgb(23,152,111));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }

    }

    // 定义转换器
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class DataColorConverterForGround : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString() == "")
                return new SolidColorBrush(Colors.Black);
            bool HEGE = (bool)value;
            if (!HEGE)
            {
                try
                {
                    return new SolidColorBrush(Colors.White);
                }
                catch
                { throw; }
            }
            return new SolidColorBrush(Colors.Black); //new SolidColorBrush(Color.FromRgb(23,152,111));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }

    }

}