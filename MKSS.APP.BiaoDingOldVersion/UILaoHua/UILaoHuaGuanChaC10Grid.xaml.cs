using DeviceDataMonitorWPF.UIBiaoDing.Util;
using DeviceDataMonitorWPF.UIBiaodingJiuJing.Util;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    /// <summary>
    /// UILaoHuaGuanChaC10Grid.xaml 的交互逻辑
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
            this.BtnFinishBoard.IsEnabled = false;
        }
        DataAgingSensorGroupDataModel DataSrc { get; set; }
        public void SetData(DataAgingSensorGroupDataModel data)
        {

            this.DataSrc = data;
            this.DataContext = data;
            this.BtnFinishBoard.IsEnabled = UILaoHuaGuanChaModel.Intance.BtnFinishBoardExeEnabled(data);

            Batch _Batch = UILaoHuaGuanChaModel.Intance.OfBatch(data);
            if (data.Address > 0 && _Batch!=null)
            {

                this.TxtProjectTitle.Content = _Batch.F_BatchName;
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
                this.TxtProjectTitle.Content = "";
                this.StaTimeRevElapsed.Text = "00:00:00";
            }
            
            List<DataAgingSensorModel> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].XINHAO_AD = data.Empty || tables[i].IsEmpSensor ? null : tables[i].Value;
                    BasdeData[i].BD_10SEC = C10GridRow.DAR(tables[i].Parent, new TimeSpan(0, 0, 10));
                    BasdeData[i].BD_1MIN = C10GridRow.DAR(tables[i].Parent, new TimeSpan(0, 1, 0));
                    BasdeData[i].BD_5MIN = C10GridRow.DAR(tables[i].Parent, new TimeSpan(0, 5, 0));
                    BasdeData[i].BD_15MIN = C10GridRow.DAR(tables[i].Parent, new TimeSpan(0, 15, 0));
                    BasdeData[i].BD_30MIN = C10GridRow.DAR(tables[i].Parent, new TimeSpan(0, 30, 0));
                    BasdeData[i].BD_1HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(1, 0, 0));
                    BasdeData[i].BD_3HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(3, 0, 0));
                    BasdeData[i].BD_6HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(6, 0, 0));
                    BasdeData[i].BD_12HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(12, 0, 0));
                    BasdeData[i].BD_24HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(24, 0, 0));
                    BasdeData[i].BD_72HOUR = C10GridRow.DAR(tables[i].Parent, new TimeSpan(72, 0, 0));
                }
            }
        }
          

        private void BtnFinishBoard_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataSrc != null && this.DataSrc.CaseNo > 0 && this.DataSrc.Address > 0) {
                UILaoHuaGuanChaModel.Intance.BtnFinishBoardExe(DataSrc);
                this.BtnFinishBoard.IsEnabled = UILaoHuaGuanChaModel.Intance.BtnFinishBoardExeEnabled(DataSrc);
                UILaoHuaGuanChaModel.Intance.CalcAddress();
                UILaoHuaGuanChaModel.Intance.EnsureMode(UILaoHuaGuanChaModel.Intance.TxtPortsModeSelect);
            }
        }

    }

    public class C10GridRow : INotifyPropertyChanged
    {
        public C10GridRow()
        {
          
        }
        public int WEIZHI { get; set; }
        public int? XINHAO_AD { get; set; }
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
        public static string DAR(DataAgingSensor s,TimeSpan t) {
            if (s.IsEmpSensor) return null;
            if (s.Ranges.ContainsKey(t)) return s.Ranges[t].ToBdString();
            return null;
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