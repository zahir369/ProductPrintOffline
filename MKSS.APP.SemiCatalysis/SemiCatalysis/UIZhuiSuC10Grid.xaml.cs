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

namespace MKSS.APP.SemiCatalysis
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

            

        }
        Batch Batch { get; set; } 
        public void SetData(Model.Batch _batch, Dictionary<string, MKSS.Model.SensorData> data, TimeSpan F_AddTime)
        {
            if (_batch == null || data == null) return;
             
            Batch = _batch;
            List<MKSS.Model.SensorData> tables = data.Values.Where(w=>w.Sensor!=null).OrderBy(w=>w.Sensor.VisibleXh).ToList();
            if (tables.Count == 0) return;

            int[] Seconds = UIZhuiSuC10Model.Instance.SettingModel.TxtTestTimePointsArr;
            Dictionary<int, Dictionary<string, MKSS.Model.SensorData>> dss = UIZhuiSuC10Model.Instance.ECService.GetDataRange(Seconds);
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    //Text="4,30,50,70,80,150,180,250,280,300"
                    BasdeData[i].PushBD(tables[i]);
                    BasdeData[i].SensorId = tables[i].F_SensorId;
                    BasdeData[i].PageSensorModel = tables[i];
                    BasdeData[i].XINHAO_AD = data.Count==0 ? "-" : (tables[i].F_DataValueEmp ? "-" : tables[i].F_ShowValue.ToString());

                    int[] secs = Seconds;
                    for (int x = 0; x < secs.Length; x++)
                    { 
                        PropertyInfo pro = BasdeData[i].GetType().GetProperty("BD_" + (x + 1));
                        if (pro != null) {
                            if (dss.ContainsKey(secs[x])) {
                                if (dss[secs[x]].ContainsKey(BasdeData[i].SensorId) && dss[secs[x]][BasdeData[i].SensorId]!=null)
                                {
                                    pro.SetValue(BasdeData[i], dss[secs[x]][BasdeData[i].SensorId].F_ShowValue.ToString());
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
            Batch = _Batch;
            BasdeData.Clear();
            datas = datas.OrderBy(w => w.VisibleXh).ToList();
            if (BasdeData.Count == 0)
            {
                foreach (Sensor sen in datas)
                {
                    BasdeData.Add(new C10GridRow() { WEIZHI = sen.PosString , SensorId = sen .F_SensorId});
                }
                this.dataGrid.DataContext = BasdeData;
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

                var plt = new ScottPlot.Plot();
                ScottPlot.WpfPlotViewer w = new ScottPlot.WpfPlotViewer(plt, 1600, 400, string.Format("{0} {1}", this.Batch.F_BatchName, _C10GridRow.SensorId));
                //w.Icon = null;
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //plt.SetViewLimits(0, (Batch.F_AgingEndTime - Batch.F_AgingStartTime).TotalSeconds, 0, 10000); ;
                plt.SetAxisLimits(new ScottPlot.AxisLimits(0, (Batch.F_AgingEndTime), 0, 1000));
                w.Show();
                List<double> ds = new List<double>();
                List<double> vs = new List<double>();

                DataTable dt = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
                    string.Format("select F_DataValue v,F_AddTime t from pd_sensordata_{0} where F_SensorId='{1}' and F_DataValue!=170 order by F_AddTime",
                    Batch.F_BatchId, _C10GridRow.SensorId))
                    .Result;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ds.Add(Convert.ToDouble(dt.Rows[i]["t"]));
                    vs.Add(Convert.ToDouble(dt.Rows[i]["v"]));
                }
                plt.AddScatter(ds.ToArray(), vs.ToArray());
                // Then tell the axis to display tick labels using a time format
                //plt.XAxis.DateTimeFormat(true);



            }
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
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensors(sendor_ids);
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

        /// <summary>
        ///  第四秒，后四秒差值
        /// </summary>
        public string BD_CZ
        {
            get;set;
        }
        //MKSS.Model.SensorData d1 = null;
        //MKSS.Model.SensorData d2 = null;
        //double CacheMaxTime { get; set; }
        List<MKSS.Model.SensorData> Cache = new List<SensorData>();
        public void PushBD(MKSS.Model.SensorData val)
        {

            //double time = val.F_AddTime;
            //if (time <= 4 && CacheMaxTime>4) {
            //    Cache.Clear();
            //    d1 = null;
            //    d2 = null;
            //    CacheMaxTime = 0;
            //}
            Cache.Add(val);
            SensorData SensorDataStart = UIZhuiSuC10SensorItem.SensorDataStartCache.ContainsKey(SensorId) 
                ? UIZhuiSuC10SensorItem.SensorDataStartCache[SensorId] : null;
            SensorData SensorDataEnd = UIZhuiSuC10SensorItem.SensorDataEndCache.ContainsKey(SensorId)
                ? UIZhuiSuC10SensorItem.SensorDataEndCache[SensorId] : null;
            BD_CZ = SensorData.Delta(SensorDataEnd, SensorDataStart).ToString("f1");

            //if (time <= 4) {
            //    if (d1 == null || d1.F_AddTime < time) d1 = val;
            //}

            //if (time >= 29)
            //{
            //    for (int i = 0; i <= Cache.Count - 1; i++)
            //    {
            //        if (Cache[i].F_AddTime <= 31) {
            //            d2 = Cache[i];
            //            BD_CZ = (d2.F_DataValue - d1.F_DataValue).ToString("f2");
            //            break;
            //        } 
            //    }
            //}
            //CacheMaxTime = time;

        }


        public string SensorId { get; internal set; }
        public MKSS.Model.SensorData PageSensorModel { get; internal set; }

        public static string DAR(MKSS.Model.SensorData s,TimeSpan t) {
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