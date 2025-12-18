using Microsoft.Win32;
using MKSS.APP.LaoHua.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

namespace MKSS.APP.LaoHua
{
    /// <summary>
    ///  UILaoHuaGuanChaC10Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaC10Grid : UserControl
    {
        List<C10GridRow> BasdeData = new List<C10GridRow>();
        public UILaoHuaGuanChaC10Grid()
        {
            InitializeComponent();
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new C10GridRow() { WEIZHI = i + 1 });
            }
            this.dataGrid.DataContext = BasdeData;
            this.BtnExportBoard.IsEnabled = false;
            //this.BtnFinishBoard.IsEnabled = false;
        }
        Batch Batch { get; set; }
        PageBoardItem DataSrc { get; set; }
        public void SetData(PageBoardItem data,Batch _batch)
        {
            if (_batch == null) return;
            Batch = _batch;
            this.DataSrc = data;
            this.DataContext = data;

            //this.BtnFinishBoard.IsEnabled = Batch.EnumAgingStatus== EnumAgingStatus.InAging 
            //    ? UILaoHuaGuanChaData.Instance.BtnFinishBoardExeEnabled(DataSrc)
            //    : false;

            this.BtnExportBoard.IsEnabled = Batch.EnumAgingStatus== EnumAgingStatus.InAging 
                ? UILaoHuaGuanChaData.Instance.BtnFinishBoardExeEnabled(DataSrc)
                : false;

            BoardCase _BoardCase = null;
            Board _Board = null;
            Batch _Batch = UILaoHuaGuanChaData.Instance.OfBatch(data, ref _BoardCase, ref _Board);
            _Batch = Batch;
            List<Sensor> senlist = new List<Sensor>();
            if (data.Address > 0 && _Batch!=null)
            {

                senlist = UILaoHuaGuanChaData.Instance.GetSensor(_Batch);
                //this.TxtProjectTitle.Content = _Batch.F_BatchName;
                this.TxtPostion.Content = string.Format("{0}柜{1}层{2}侧",
                    _BoardCase == null ? "" : _BoardCase.F_BoardCaseAddress,
                    _Board == null ? "" : _Board.F_FloorNO,
                    data.Address % 2 == 0 ? "右" : "左"
                    );
                if (_Batch.EnumAgingStatus == EnumAgingStatus.InAging) {
                    if (_Batch.F_AgingStartTime != DateTime.MinValue && _Batch.F_AgingEndTime != DateTime.MinValue)
                    {
                        TimeSpan total = _Batch.F_AgingEndTime - _Batch.F_AgingStartTime;
                        TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;
                        if (els <= total)
                        {
                            string format = (total - els).Days >= 1 ? "d'.'hh':'mm':'ss" : "hh':'mm':'ss";
                            this.StaTimeRevElapsed.Text = (total - els).ToString(format);
                        }
                        else
                        {
                            string format = (total - els).Days >= 1 ? "d'.'hh':'mm':'ss" : "hh':'mm':'ss";
                            this.StaTimeRevElapsed.Text = "+" + (total - els).ToString(format);
                        }
                    }
                }
                else {
                    this.StaTimeRevElapsed.Text = "已完成";
                }
                
            }
            else {
                this.TxtPostion.Content = "";
                //this.TxtProjectTitle.Content = "";
                this.StaTimeRevElapsed.Text = "00:00:00";
            }
            
            List<PageSensorModel> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].RefreshTime = tables[i].Parent.RefreshTime.ToString("yyyy-MM-dd HH:mm:ss");
                    BasdeData[i].SensorId = tables[i].SensorId;
                    Sensor _Sensor = senlist.FirstOrDefault(w => w.F_SensorId == tables[i].SensorId);
                    if (_Sensor != null) {
                        BasdeData[i].XULIEHAO = _Sensor.F_SerialNO;
                        BasdeData[i].SHIJIAN = _Sensor.F_DeviceHistoryClock;
                        BasdeData[i].XULIEHAO_DT = _Sensor.F_SerialNoDetectTime;
                        BasdeData[i].SHIJIAN_DT = _Sensor.F_DeviceHistoryClockDetectTime;
                    }
                    BasdeData[i].XINHAO_AD = data.Empty ? "-" : (tables[i].IsEmpSensor ? "-" : tables[i].Value.ToString());
                    BasdeData[i].BD_10SEC = C10GridRow.DAR(tables[i], new TimeSpan(0, 0, 10));
                    BasdeData[i].BD_1MIN = C10GridRow.DAR(tables[i], new TimeSpan(0, 1, 0));
                    BasdeData[i].BD_5MIN = C10GridRow.DAR(tables[i], new TimeSpan(0, 5, 0));
                    BasdeData[i].BD_15MIN = C10GridRow.DAR(tables[i], new TimeSpan(0, 15, 0));
                    BasdeData[i].BD_30MIN = C10GridRow.DAR(tables[i], new TimeSpan(0, 30, 0));
                    BasdeData[i].BD_1HOUR = C10GridRow.DAR(tables[i], new TimeSpan(1, 0, 0));
                    BasdeData[i].BD_3HOUR = C10GridRow.DAR(tables[i], new TimeSpan(3, 0, 0));
                    BasdeData[i].BD_6HOUR = C10GridRow.DAR(tables[i], new TimeSpan(6, 0, 0));
                    BasdeData[i].BD_12HOUR = C10GridRow.DAR(tables[i], new TimeSpan(12, 0, 0));
                    BasdeData[i].BD_24HOUR = C10GridRow.DAR(tables[i], new TimeSpan(24, 0, 0));
                    BasdeData[i].BD_72HOUR = C10GridRow.DAR(tables[i], new TimeSpan(72, 0, 0)); 

                }
            }
        }

        private void BtnFinishBoard_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataSrc != null && this.DataSrc.CaseNo > 0 && this.DataSrc.Address > 0)
            {
                BoardCase _BoardCase = null; 
                Board _Board = null;
                Batch _Batch = UILaoHuaGuanChaData.Instance.OfBatch(DataSrc, ref _BoardCase, ref _Board);

                if (MessageBox.Show(
                    string.Format("确定要结束{0} 第{1}层托盘任务吗？", _Batch.F_BatchName, _Board.F_FloorNO),
                    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    UILaoHuaGuanChaData.Instance.BtnFinishBoardExe(DataSrc);
                    //this.BtnFinishBoard.IsEnabled = Batch.EnumAgingStatus == EnumAgingStatus.InAging
                    //    ? UILaoHuaGuanChaData.Instance.BtnFinishBoardExeEnabled(DataSrc)
                    //    : false;
                    //this.BtnExportBoard.IsEnabled = Batch.EnumAgingStatus == EnumAgingStatus.InAging
                    //    ? UILaoHuaGuanChaData.Instance.BtnFinishBoardExeEnabled(DataSrc)
                    //    : false;
                    
                    UILaoHuaGuanChaC10Model.Instance.RefreshAddress(_Batch);
                    UILaoHuaGuanChaC10Model.Instance.View.IniData(null);
                }

            }
        }


        private void BtnExportBoard_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataSrc != null && this.DataSrc.CaseNo > 0 && this.DataSrc.Address > 0)
            {
                BoardCase _BoardCase = null;
                Board _Board = null;
                Batch _Batch = UILaoHuaGuanChaData.Instance.OfBatch(DataSrc, ref _BoardCase, ref _Board);
                string pos = string.Format("{0}柜{1}层{2}侧",
                    _BoardCase == null ? "" : _BoardCase.F_BoardCaseAddress,
                    _Board == null ? "" : _Board.F_FloorNO,
                    DataSrc.Address % 2 == 0 ? "右" : "左"
                    );
                string name = _Batch.F_BatchName.Replace("#", "-") + pos;
                var dlg = new SaveFileDialog()
                {
                    Title = _Batch.F_BatchName + "-另存为",
                    DefaultExt = "txt",
                    Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                    FileName = name
                };
                if (dlg.ShowDialog() == true)
                {
                    try
                    {

                        Cursor = Cursors.Wait;
                        WaitWindow.ShowWindow("正在导出", "正在导出" + pos + "......", this);
                        UILaoHuaGuanChaC10Model.Instance.ExportBoardExcel(dlg.FileName, _Batch, _BoardCase, _Board);
                        Cursor = Cursors.Arrow;
                        WaitWindow.CloseWindow(this);
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }


            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {


            C10GridRow _C10GridRow = this.dataGrid.SelectedItem as C10GridRow;
            if (_C10GridRow != null)
            {

                //DataTable dt_count = UILaoHuaGuanChaData.Instance.SensorDataServices.QueryTable(
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
                w.Show();
                List<double> ds = new List<double>();
                List<double> vs = new List<double>();

                DataTable dt = UILaoHuaGuanChaData.Instance.SensorDataServices.QueryTable(
                    string.Format("select F_DataValue v,F_AddTime t from pd_sensordata_{0} where F_SensorId='{1}' and F_DataValue!=170 order by F_AddTime",
                    Batch.F_BatchId, _C10GridRow.SensorId))
                    .Result;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ds.Add(Convert.ToDateTime(dt.Rows[i]["t"]).ToOADate());
                    vs.Add(Convert.ToDouble(dt.Rows[i]["v"]));
                }
                if (ds.Count > 0) {
                    plt.AddScatter(ds.ToArray(), vs.ToArray());
                    // Then tell the axis to display tick labels using a time format
                    plt.XAxis.DateTimeFormat(true);
                }


            }


        }

    }

    public class C10GridRow : INotifyPropertyChanged
    {
        public C10GridRow()
        {
          
        }
        public int WEIZHI { get; set; }
        public string XINHAO_AD { get; set; }
        public string BD_10SEC { get; set; }
        public string BD_1MIN { get; set; }
        public string BD_5MIN { get; set; }
        public string BD_15MIN { get; set; }
        public string BD_30MIN { get; set; }
        public string BD_1HOUR { get; set; }
        public string BD_3HOUR { get; set; }
        public string BD_6HOUR { get; set; }
        public string BD_12HOUR { get; set; }
        public string BD_24HOUR { get; set; }
        public string BD_72HOUR { get; set; }
        public string SensorId { get; internal set; }
        public string XULIEHAO { get; set; }
        public string SHIJIAN { get; set; }
        public DateTime XULIEHAO_DT { get; set; }
        public DateTime SHIJIAN_DT { get; set; }
        public string RefreshTime { get; internal set; }

        public static string DAR(PageSensorModel s,TimeSpan t) {
            if (s.IsEmpSensor) return "-";
            if (s.Ranges.ContainsKey(t)) return s.Ranges[t].ToBdString();
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