using MKSS.Model;
using MKSS.Service.LaoHuaElectroChemical;
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

namespace MKSS.APP.SemiTester
{

    /// <summary>
    /// UIZhuiSuC10SensorItem.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10SensorItem : UserControl
    {

        public UIZhuiSuC10SensorItem()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
        }

        public void SetFontColor() { 
           
        } 
        public PosEnum PosEnum { get; set; } = PosEnum.A1;
        public Batch Batch { get; set; }
        public SensorDataItem Sensor { get; set; }
        public SensorGroupData SensorData { get; set; }
        public SensorGroupData SensorDataStart { get; set; }
        public double DataValueStart { get { return SensorDataStart == null ? 0 : SensorDataStart.F_LoadDataValue(PosEnum); } }
        System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
        public void SetSensorData(Batch bat, SensorGroupData data, PosEnum posenum) {
            PosEnum = posenum; 
            Sensor = data.Data()[posenum];
            if (Batch == null || bat.F_BatchId != Batch.F_BatchId)
            {
                Batch = bat;
                SensorDataStart = null;
            }

            if (SensorDataStart==null ) {
                SensorDataStart = data;
            }

            SensorData = data;
            Refresh();
        }

        public SensorItemValueEnum Mode { get { return UIZhuiSuC10Model.Instance.SettingModel.ValueMode; } } 
         
        public void Refresh(){

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (Sensor == null || SensorData == null) return;
            if (UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig == null) return;

            double value = 0;
            ProductGrade grade = null;
            bool find_color = false; 
            SolidColorBrush defaultColor = (SolidColorBrush)this.FindResource("ColorPanelDefault");

            double v_src = SensorData.F_LoadDataValueSrc(PosEnum); 
            switch (Mode)
            {
                case SensorItemValueEnum.ValueDY:
                    value = SensorData.F_LoadDataValue(PosEnum);
                    this.TextPanel.Content = value > 100
                        ? value.ToString("f0")
                        : value == 0 ? "0.00" : value.ToString("f2");
                    this.TextPanel.ToolTip = Sensor.PosString + "传感器端电压：" + value.ToString("f2") + "毫伏，AD值："+ v_src;
                    break;
                case SensorItemValueEnum.ValueND:
                    value = SensorData.F_LoadDataValue(PosEnum);
                    this.TextPanel.Content = value > 100
                        ? value.ToString("f0")
                        : value == 0 ? "0.00" : value.ToString("f2");
                    this.TextPanel.ToolTip = Sensor.PosString + "传感器浓度值：" + value.ToString("f2") + "%，AD值：" + v_src;
                    break;
                default:
                    break;
            }
            
            grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
            if (grade != null)
            {
                this.TextPanel.Background = (Brush)converter.ConvertFromString(Mode== SensorItemValueEnum.ValueDY? grade.ADColor : grade.NDColor);
                find_color = true;
            }
            if (!find_color)
            {
                this.TextPanel.Background = defaultColor;
            } 

        }
         

        public bool? IsChecked { get { return true; } }
        public string Text { get { return this.TextPanel.Content + ""; } set { this.TextPanel.Content = value; } }

        public void T1_Click(object sender, RoutedEventArgs e)
        {
             
        }
         
         
        public void FocusSensor(bool foucus)
        {
              
            if (foucus)
            {
                TextPanel.FontWeight = FontWeights.Bold;
            }
            else
            {
                TextPanel.FontWeight = FontWeights.Normal;
            }

        }

        private void TextPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                FocusSensor(false);
            }
            catch (Exception)
            {

            }
        }

        private void TextPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                FocusSensor(true);
            }
            catch (Exception)
            {

            }
        }

        private void TextPanel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                FocusSensor(true);
            }
            catch (Exception)
            {
                 
            }
        }

        private void TextPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Batch == null || SensorData == null) return;
            UIChartWindows.ShowChart(Batch, SensorData.F_FloorNo);
        }

    }
    

}
