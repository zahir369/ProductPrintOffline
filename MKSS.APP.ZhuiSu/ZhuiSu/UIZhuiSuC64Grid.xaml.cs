using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using MKSS.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC10Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC64Grid : UserControl
    {
        public List<C10GridRow> BasdeData = new List<C10GridRow>();
        public UIZhuiSuC64Grid()
        {
            InitializeComponent();
            for (int i = 0; i < 32; i++)
            {
                BasdeData.Add(new C10GridRow() { WEIZHI = string.Format("A{0}",(i+1).ToString("00")) });
            }
            for (int i = 0; i < 32; i++)
            {
                BasdeData.Add(new C10GridRow() { WEIZHI = string.Format("B{0}", (i + 1).ToString("00")) });
            }
            this.dataGrid.DataContext = BasdeData;

            foreach (DataGridColumn item in dataGrid.Columns)
            {

            }
            
        }

        public void SetSerialNo(string SensorId, string SNO)
        {
            C10GridRow row = BasdeData.FirstOrDefault(w => w.SensorId == SensorId);
            if (row != null) {
                row.SerialNO = SNO;
            }
            if (row.PageSensorModel != null)
            {
                row.PageSensorModel.SerialNO = SNO;
            }
        }

        Batch Batch { get; set; }
        PageBoardItem DataSrc { get; set; }
        public void SetData(PageBoardItem data, PageBoardItem dataB,Batch _batch)
        {
            Batch = _batch;
            this.DataSrc = data;
            this.DataContext = data; 
            BoardCase _BoardCase = null;
            Board _Board = null;
            Batch _Batch = UIZhuiSuData.Instance.OfBatch(data, ref _BoardCase, ref _Board);
            _Batch = Batch;
            if (data.Address > 0 && _Batch!=null)
            {

                PageSensorModel line = data.ProductTable[0];
                //dataGrid.Columns[2].Header = "最新" + line.HTString();
                for (int i = 3; i < dataGrid.Columns.Count; i++)
                {
                    DataGridColumn item = dataGrid.Columns[i];
                    if (PageSensorBase.Seconds.Length > i - 3) {
                        item.Header = UIZhuiSuAdd.ToMyFormat(TimeSpan.FromSeconds(PageSensorBase.Seconds[i - 3]));// string.Format("第{0}秒", PageSensorBase.Seconds[i - 2]);
                    }
                    if (PageSensorBase.Seconds.Length > i - 2)
                    {
                        //item.Header = item.Header + line.HTString();
                    }
                }

                if (_Batch.EnumAgingStatus == EnumAgingStatus.InAging) {
                    if (_Batch.F_AgingStartTime != DateTime.MinValue && _Batch.F_AgingEndTime != DateTime.MinValue)
                    {
                        TimeSpan total = _Batch.F_AgingEndTime - _Batch.F_AgingStartTime;
                        TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;
                        if (els <= total)
                        {
                            string format = (total - els).Days >= 1 ? "d'.'hh':'mm':'ss" : "hh':'mm':'ss"; 
                        }
                        else
                        {
                            string format = (total - els).Days >= 1 ? "d'.'hh':'mm':'ss" : "hh':'mm':'ss"; 
                        }
                    }
                }
                else { 
                }
                
            }
            else {
                 
            }
            

            List<PageSensorModel> tables = new List<PageSensorModel>();
            tables.AddRange(data.ProductTable);
            tables.AddRange(dataB.ProductTable);
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    //Text="4,30,50,70,80,150,180,250,280,300"
                    BasdeData[i].SensorId = tables[i].SensorId;
                    BasdeData[i].SerialNO = tables[i].SerialNO;
                    BasdeData[i].PageSensorModel = tables[i];
                    BasdeData[i].XINHAO_AD = data.Empty ? "-" : (tables[i].IsEmpSensor ? "-" : tables[i].ValueFormate.ToString("f2"));

                    int[] secs = PageSensorBase.Seconds;
                    for (int x = 0; x < secs.Length; x++)
                    {
                        PropertyInfo pro = BasdeData[i].GetType().GetProperty("BD_" + (x + 1));
                        if (pro != null) {
                            pro.SetValue(BasdeData[i], C10GridRow.DAR(tables[i], TimeSpan.FromSeconds(secs[x])));
                        }
                        //DataGridColumn item = dataGrid.Columns[x+3];
                        //item.Header = UIZhuiSuAdd.ToMyFormat(TimeSpan.FromSeconds(PageSensorBase.Seconds[x])) + C10GridRow.DAR_TH(tables[i], TimeSpan.FromSeconds(secs[x]));// string.Format("第{0}秒", PageSensorBase.Seconds[i - 2]);
                    } 
                }
            }

        }

        private void BtnFinishBoard_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataSrc != null && this.DataSrc.CaseNo > 0 && this.DataSrc.Address > 0)
            {
                BoardCase _BoardCase = null; 
                Board _Board = null;
                Batch _Batch = UIZhuiSuData.Instance.OfBatch(DataSrc, ref _BoardCase, ref _Board);

                if (MessageBox.Show(
                    string.Format("确定要结束{0} 第{1}层托盘任务吗？", _Batch.F_BatchName, _Board.F_FloorNO),
                    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    UIZhuiSuData.Instance.BtnFinishBoardExe(DataSrc);
                     
                    UIZhuiSuC64Model.Instance.RefreshAddress(_Batch);
                    UIZhuiSuC64Model.Instance.View.UIZhuiSuC64Setting1.IniData(null);
                }

            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Batch == null) return;
            C10GridRow _C10GridRow = this.dataGrid.SelectedItem as C10GridRow;
            if (_C10GridRow != null)
            {

                //DataTable dt_count = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
                //    string.Format("select count(*) from pd_sensordata_{0} where F_SensorId='{1}' order by F_AddTime",
                //    Batch.F_BatchId, _C10GridRow.SensorId))
                //    .Result;
                //int count = dt_count.Rows.Count > 0 ? Convert.ToInt32
                //    (dt_count.Rows[0][0]) : 0;

                //DateTime[] ds_arr = new DateTime[count];
                //int[] vs_arr = new int[count];

                UIZhuiSuC64ChartWindow wx = new UIZhuiSuC64ChartWindow();
                wx.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                wx.Show();
                wx.SetBatch(this.Batch, _C10GridRow);

                //var plt = new ScottPlot.Plot();
                //ScottPlot.WpfPlotViewer w = new ScottPlot.WpfPlotViewer(plt, 1600, 400, string.Format("{0} {1}", this.Batch.F_BatchName, _C10GridRow.SensorId));
                ////w.Icon = null;
                //w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ////plt.SetViewLimits(0, (Batch.F_AgingEndTime - Batch.F_AgingStartTime).TotalSeconds, 0, 10000); ;
                //plt.SetAxisLimits(new ScottPlot.AxisLimits(0, (Batch.F_AgingEndTime - Batch.F_AgingStartTime).TotalSeconds, 0, 10000));
                //w.Show();
                //List<double> ds = new List<double>();
                //List<double> vs = new List<double>();

                //DataTable dt = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
                //    string.Format("select F_DataValue v,F_AddTime t from pd_sensordata_{0} where F_SensorId='{1}' and F_DataValue!=170 order by F_AddTime",
                //    Batch.F_BatchId, _C10GridRow.SensorId))
                //    .Result;
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    ds.Add(Convert.ToDouble(dt.Rows[i]["t"]));
                //    vs.Add(Convert.ToDouble(dt.Rows[i]["v"]));
                //}
                //if (ds.Count == 0) return;

                //plt.AddScatter(ds.ToArray(), vs.ToArray());

                // Then tell the axis to display tick labels using a time format
                //plt.XAxis.DateTimeFormat(true);



            }
        }

        public List<string> SelectSensorIds { get; set; }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectSensorIds = new List<string>();
            foreach (var item in dataGrid.SelectedItems)
            {
                C10GridRow t = item as C10GridRow;
                if (t != null)
                {
                    DataGridRow dr = (DataGridRow)(dataGrid.ItemContainerGenerator.ContainerFromItem(t));
                    SelectSensorIds.Add(t.SensorId);
                }
            }
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensors(sendor_ids);
        }
         

    }

    public class C10GridRow : INotifyPropertyChanged
    {
        public C10GridRow()
        {
          
        }
        //Text="4,30,50,70,80,150,180,250,280,300"
        public string WEIZHI { get; set; } 
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
        public string SerialNO { get; internal set; }
        public PageSensorModel PageSensorModel { get; internal set; }

        public static string DAR(PageSensorModel s,TimeSpan t) {
            if (s.IsEmpSensor) return "-";
            if (s.Ranges.ContainsKey(t)) return s.Ranges[t].ToBdString();
            return "-";
        }
        public static string DAR_TH(PageSensorModel s, TimeSpan t)
        { 
            if (s.Ranges.ContainsKey(t)) return s.Ranges[t].HTString();
            return "/--℃/--%";
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