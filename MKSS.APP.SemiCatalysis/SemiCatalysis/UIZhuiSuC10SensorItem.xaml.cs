using MKSS.Model;
using MKSS.Service.SemiCatalysis;
using System;
using System.Collections.Generic;
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

namespace MKSS.APP.SemiCatalysis
{
    /// <summary>
    /// UIZhuiSuC10SensorItem.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10SensorItem : UserControl
    {
        public UIZhuiSuC10SensorItem()
        {
            InitializeComponent();
            this.CheckBox.Click += T1_Click;
            this.CheckBox.Checked += T1_Click;
            if (SensorDataStartCache == null) SensorDataStartCache = new Dictionary<string, SensorData>();
            if (SensorDataEndCache == null) SensorDataEndCache = new Dictionary<string, SensorData>();
        }

        public void SetFontColor() {
            this.Button.Foreground = new SolidColorBrush( Colors.White);
            this.ButtonDt.Foreground = new SolidColorBrush(Colors.White);
            this.Label.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        public Batch Batch { get; set; }
        public Sensor Sensor { get; set; }
        public SensorData SensorData { get; set; }
        public SensorData SensorDataStart { get; set; }
        public SensorData SensorDataEnd { get; set; }
        public static Dictionary<string, SensorData> SensorDataStartCache { get; set; }
        public static Dictionary<string, SensorData> SensorDataEndCache { get; set; }
        public double DataValueDeleta { get { return SensorData.Delta(SensorDataEnd, SensorDataStart); } }
        System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
        public void SetSensorData(Batch bat,Sensor sen, SensorData data) {

            if (Batch == null || bat.F_BatchId != Batch.F_BatchId)
            {
                SensorDataStart = new SensorData();
                SensorDataEnd = new SensorData();
                Batch = bat;
                SensorDataStartCache.Clear();
                SensorDataEndCache.Clear();
            }

            if (data.F_AddTime <= UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ZeroTime + 1)
            {
                SensorDataStart = data;
            } 
            if (!SensorDataStartCache.ContainsKey(sen.F_SensorId)) SensorDataStartCache.Add(sen.F_SensorId, SensorDataStart);
            else SensorDataStartCache[sen.F_SensorId] = SensorDataStart;

            SensorDataEnd = data;
            if (!SensorDataEndCache.ContainsKey(sen.F_SensorId)) SensorDataEndCache.Add(sen.F_SensorId, data);
            else SensorDataEndCache[sen.F_SensorId] = data;

            Sensor = sen;
            SensorData = data;
            Refresh();

        }


        public void Refresh(){
            if (Sensor != null && SensorData != null && SensorDataStart!=null)
            {
                this.Label.Content = this.Text;
                this.Button.Content =  string.Format("{0}", ( (SensorDataStart.F_ShowValue)).ToString("f1"));
                this.ButtonDt.Content =  string.Format("{0}", (DataValueDeleta).ToString("f1"));
                this.LabelNagetive.Content = string.Format("{0}", SensorData.F_ShowValue > 0 ? "+" : "-");
                this.LabelNagetive.Foreground = SensorData.F_ShowValue > 0 ? (Brush)converter.ConvertFromString("#0000FF") : (Brush)converter.ConvertFromString("#DC143C");
                bool find_color = false; bool find_color_dt = false;
                foreach (ProductGrade grade in UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.Grades)
                {
                    if (grade.From <= (SensorDataStart.F_ShowValue) && grade.To > (SensorDataStart.F_ShowValue))
                    {
                        this.Button.Background = (Brush)converter.ConvertFromString(grade.Color);
                        find_color = true;
                    }
                    if (grade.DtFrom <= (DataValueDeleta) && grade.DtTo > (DataValueDeleta))
                    {
                        this.ButtonDt.Background = (Brush)converter.ConvertFromString(grade.Color);
                        find_color_dt = true;
                    }
                }
                if (!find_color)
                {
                    this.Button.Background = (Brush)converter.ConvertFromString("#d0d0d0");
                }
                if (!find_color_dt)
                {
                    this.ButtonDt.Background = (Brush)converter.ConvertFromString("#d0d0d0");
                }
            }
        }

        public bool? IsChecked { get { return this.CheckBox.IsChecked; } set{ this.CheckBox.IsChecked = value; } }
        public string Text { get { return this.Label.Content + ""; } set { this.Label.Content = value; } }

        public void T1_Click(object sender, RoutedEventArgs e)
        {
            MKSS.Model.SensorData ses = SensorData;
            if (ses != null)
            {
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensorsOf(ses.F_SensorId, this.CheckBox.IsChecked!=null && this.CheckBox.IsChecked.Value);
            }
            if (Sensor != null) Sensor.IsChecked = this.CheckBox.IsChecked;
        }

    }
}
