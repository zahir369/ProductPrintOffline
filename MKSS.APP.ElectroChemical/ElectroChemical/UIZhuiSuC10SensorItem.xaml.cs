using MKSS.Model;
using MKSS.Service.ElectroChemical;
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

namespace MKSS.APP.ElectroChemical
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
        }

        public void SetFontColor() {
            this.Button.Foreground = new SolidColorBrush( Colors.White);
            this.Label.Foreground = new SolidColorBrush(Colors.Yellow);
        }
        public PosEnum PosEnum { get; set; } = PosEnum.A1;
        public Batch Batch { get; set; }
        public Sensor Sensor { get; set; }
        public SensorDataItem SensorDataItem { get; set; }
        public SensorGroupData SensorData { get; set; }
        public double DataValue { get { return SensorData == null ? 0 : SensorData.F_LoadDataValue(PosEnum); } }
        public string DataValueString { get { return SensorData == null ? "0" : SensorData.F_LoadDataValueString(PosEnum); } }
        System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
        public void SetSensorData(Batch bat, SensorGroupData data, Sensor  s, PosEnum posenum)
        {
            PosEnum = posenum;
            Sensor = s;
            SensorDataItem = data.Data[posenum];
            SensorData = data;
            Refresh();
        }


        public void Refresh(){
            if (Sensor != null && SensorData != null)
            {
                this.Label.Content = Sensor.PosString;
                this.Button.Content = DataValueString;
                bool find_color = false;
                foreach (ProductGrade grade in UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.Grades)
                {
                    if (grade.From < DataValue && grade.To >= DataValue)
                    {
                        this.Button.Background = (Brush)converter.ConvertFromString(grade.Color);
                        find_color = true;
                    }
                }
                if (!find_color)
                {
                    this.Button.Background = (Brush)converter.ConvertFromString("#d0d0d0");
                }
            }
        }

        public bool? IsChecked { get { return this.CheckBox.IsChecked; } set{ this.CheckBox.IsChecked = value; } }
        public string Text { get { return this.Label.Content + ""; } set { this.Label.Content = value; } }


        public void T1_Click(object sender, RoutedEventArgs e)
        {
            SensorDataItem ses = SensorDataItem;
            if (ses != null)
            {
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensorsOf(ses.PosEnum, this.CheckBox.IsChecked!=null && this.CheckBox.IsChecked.Value);
            }
            if (Sensor != null) Sensor.IsChecked = this.CheckBox.IsChecked;
        }

    }
}
