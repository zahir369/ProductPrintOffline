using MKSS.Model;
using MKSS.Service.SemiTester;
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
            //this.PosotionPanel.Foreground = new SolidColorBrush(Colors.Yellow);
            //string colorName = "FontColorStart";
            //if (mode == SensorItemEnum.Start) colorName = "FontColorStart";
            //if (mode == SensorItemEnum.Final) colorName = "FontColorEnd";
            //if (mode == SensorItemEnum.Delta) colorName = "FontColorDelta";
            //this.TextPanel.Foreground = (SolidColorBrush)this.FindResource(colorName);
        }

        SensorItemEnum mode = SensorItemEnum.Final;
        /// <summary>
        ///  展示模式
        /// </summary>
        public SensorItemEnum Mode { 
            get { 
                return mode;
            } 
            set { 
                mode = value;
                this.PosotionPanel.Visibility = Visibility.Hidden;
                switch (mode)
                {
                    case SensorItemEnum.Start:
                    case SensorItemEnum.Final:
                    case SensorItemEnum.Delta:
                        this.TextPanel.Visibility = Visibility.Visible;
                        this.ColorPanel.Visibility = Visibility.Visible;
                        this.ColorPanelTop.Visibility = Visibility.Hidden;
                        this.ColorPanelLeft.Visibility = Visibility.Hidden;
                        this.ColorPanelRight.Visibility = Visibility.Hidden;

                        break;
                    case SensorItemEnum.ColorDiagram:
                        this.TextPanel.Visibility = Visibility.Hidden;
                        this.ColorPanel.Visibility = Visibility.Hidden;
                        this.ColorPanelTop.Visibility = Visibility.Visible;
                        this.ColorPanelLeft.Visibility = Visibility.Visible;
                        this.ColorPanelRight.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            } 
        }

        public PosEnum PosEnum { get; set; } = PosEnum.A1;
        public Batch Batch { get; set; }
        public SensorDataItem Sensor { get; set; }
        public SensorGroupData SensorData { get; set; }
        public SensorGroupData SensorDataStart { get; set; }
        public double DataValueStart { get { return SensorDataStart == null ? 0 : SensorDataStart.F_LoadDataValue(PosEnum); } }
        public double DataValueDeleta { get { return SensorGroupData.Delta(SensorData, SensorDataStart, PosEnum); } }
        System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
        public void SetSensorData(Batch bat, SensorGroupData data, PosEnum posenum) {
            PosEnum = posenum; 
            Sensor = data.Data[posenum];
            if (Batch == null || bat.F_BatchId != Batch.F_BatchId)
            {
                Batch = bat;
                SensorDataStart = null;
            }

            if (SensorDataStart==null && data.F_AddTime >= UIZhuiSuC10Model.Instance.SettingModel.TxtZeroPoint) {
                SensorDataStart = data;
            }

            SensorData = data;
            Refresh();
        }


        public void Refresh(){

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (Sensor == null || SensorData == null) return;

            double value = 0;
            ProductGrade grade = null;
            bool find_color = false;
            this.PosotionPanel.Content = Sensor.PosString;
            SolidColorBrush defaultColor = (SolidColorBrush)this.FindResource("ColorPanelDefault");
            switch (Mode)
            {
                case SensorItemEnum.Start:
                    value = DataValueStart;
                    this.TextPanel.Content = value.ToString("f3");
                    this.TextPanel.ToolTip = Sensor.PosString +"初始值："+value.ToString("f3");
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.StartColor);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.Final:
                    value = SensorData.F_LoadDataValue(PosEnum);
                    this.TextPanel.Content = value.ToString("f3");
                    this.TextPanel.ToolTip = Sensor.PosString + "反应值：" + value.ToString("f3");
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.FinalColor);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.Delta:
                    value = DataValueDeleta;
                    this.TextPanel.Content = value.ToString("f3");
                    this.TextPanel.ToolTip = Sensor.PosString + "变化值（Δ）：" + value.ToString("f3");
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.DeltaColor);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.ColorDiagram:
                    double value_end = SensorData.F_LoadDataValue(PosEnum);
                    double value_start = this.DataValueStart;
                    double value_delta = this.DataValueDeleta;
                    bool find_color_top = false;
                    bool find_color_left = false;
                    bool find_color_right= false;
                    ProductGrade gradeDelta = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.Delta, value_delta);
                    ProductGrade gradeStart = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.Start, value_start);
                    ProductGrade gradeFinal = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.Final, value_end);


                    if (gradeDelta != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.Delta)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + "变化值（Δ）：" + value_delta.ToString("f3");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeDelta.DeltaColor); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.Delta)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + "变化值（Δ）：" + value_delta.ToString("f3");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeDelta.DeltaColor); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.Delta)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + "变化值（Δ）：" + value_delta.ToString("f3");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeDelta.DeltaColor); find_color_right = true;
                        }
                    }
                    if (gradeStart != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.Start)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + "初始值：" + value_start.ToString("f3");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeStart.StartColor); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.Start)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + "初始值：" + value_start.ToString("f3");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeStart.StartColor); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.Start)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + "初始值：" + value_start.ToString("f3");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeStart.StartColor); find_color_right = true;
                        }
                    }
                    if (gradeFinal != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.Final)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + "反应值：" + value_end.ToString("f3");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeFinal.FinalColor); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.Final)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + "反应值：" + value_end.ToString("f3");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeFinal.FinalColor); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.Final)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + "反应值：" + value_end.ToString("f3");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeFinal.FinalColor); find_color_right = true;
                        }
                    }

                    if (!find_color_top)
                    {
                        this.ColorPanelTop.Background = defaultColor;
                    }
                    if (!find_color_left)
                    {
                        this.ColorPanelLeft.Background = defaultColor;
                    }
                    if (!find_color_right)
                    {
                        this.ColorPanelRight.Background = defaultColor;
                    }

                    break;
                default:
                    break;
            }

        }

        public SensorItemEnum ColorDiagramTop
        {
            get
            {
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramTop;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramTop = value;
                SemiTesterConfgig.Save();
            }
        }
        public SensorItemEnum ColorDiagramLeft
        {
            get
            {
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramLeft;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramLeft = value;
                SemiTesterConfgig.Save();
            }
        }
        public SensorItemEnum ColorDiagramRight
        {
            get
            {
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight;
                SemiTesterConfgig.Save();
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight = value;
                SemiTesterConfgig.Save();
            }
        }

        public bool? IsChecked { get { return true; } }
        public string Text { get { return this.PosotionPanel.Content + ""; } set { this.PosotionPanel.Content = value; } }

        public void T1_Click(object sender, RoutedEventArgs e)
        { 
        }

        private void ColorPanelTop_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta , SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta };
            for (int i = 0; i < ps.Length; i++)
            {
                if (ColorDiagramTop == ps[i])
                {
                    ColorDiagramTop = ps[i + 1]; UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
                    return;
                }
            }
        }

        private void ColorPanelLeft_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta, SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta };
            for (int i = 0; i < ps.Length; i++)
            {
                if (ColorDiagramLeft == ps[i])
                {
                    ColorDiagramLeft = ps[i + 1]; UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
                    return;
                }
            }
        }

        private void ColorPanelRight_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta, SensorItemEnum.Start, SensorItemEnum.Final, SensorItemEnum.Delta };
            for (int i = 0; i < ps.Length; i++)
            {
                if (ColorDiagramRight == ps[i])
                {
                    ColorDiagramRight = ps[i + 1]; UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
                    return;
                }
            }
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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(PosEnum, false);
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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(PosEnum, true);
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
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.FocusSensor(PosEnum, true);
            }
            catch (Exception)
            {
                 
            }
        }
    }


    

}
