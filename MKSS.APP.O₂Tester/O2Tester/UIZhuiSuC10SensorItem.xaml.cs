using MKSS.Model;
using MKSS.Service.O2Tester;
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

namespace MKSS.APP.O2Tester
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

        SensorItemEnum mode = SensorItemEnum.SrcData;
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
                    case SensorItemEnum.T90:
                        this.TextPanel.Visibility = Visibility.Visible;
                        this.ColorPanel.Visibility = Visibility.Visible;
                        this.ColorPanelTop.Visibility = Visibility.Hidden;
                        this.ColorPanelLeft.Visibility = Visibility.Hidden;
                        this.ColorPanelRight.Visibility = Visibility.Hidden;
                        this.TextPanel.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#323C6B"));
                        break;
                    case SensorItemEnum.SrcData:
                        this.TextPanel.Visibility = Visibility.Visible;
                        this.ColorPanel.Visibility = Visibility.Visible;
                        this.ColorPanelTop.Visibility = Visibility.Hidden;
                        this.ColorPanelLeft.Visibility = Visibility.Hidden;
                        this.ColorPanelRight.Visibility = Visibility.Hidden;
                        this.TextPanel.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#41564f"));
                        break;
                    case SensorItemEnum.T10:
                        this.TextPanel.Visibility = Visibility.Visible;
                        this.ColorPanel.Visibility = Visibility.Visible;
                        this.ColorPanelTop.Visibility = Visibility.Hidden;
                        this.ColorPanelLeft.Visibility = Visibility.Hidden;
                        this.ColorPanelRight.Visibility = Visibility.Hidden;
                        this.TextPanel.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#324d6b"));
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

        //public double DataValueT90 { get { return SensorGroupData.T90(SensorData, PosEnum); } }
        //public double DataValueT10 { get { return SensorGroupData.T10(SensorData, PosEnum); } }
        System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
        public void SetSensorData(Batch bat, SensorGroupData data, PosEnum posenum) {
            PosEnum = posenum; 
            Sensor = data.Data[posenum];
            if (Batch == null || bat.F_BatchId != Batch.F_BatchId)
            {
                Batch = bat;
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
                case SensorItemEnum.SrcData:
                    value = SensorData.F_LoadDataValue(PosEnum);
                    string f = "f2";
                    if(SensorGroupData.ShowMode== ShowModeEnum.AD) f = "f0";
                    this.TextPanel.Content = value.ToString(f);
                    this.TextPanel.ToolTip = SensorGroupData.ShowDesc(SensorData, PosEnum);
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.NongDuColor);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.T90:
                    SensorStaData sta1 =SensorStaDataCache.Instance[PosEnum];
                    value = sta1.T90;
                    this.TextPanel.Content = value.ToString("f2");
                    this.TextPanel.ToolTip = SensorGroupData.ShowDesc(SensorData, PosEnum);  
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.T90Color);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.T10:
                    SensorStaData sta2 = SensorStaDataCache.Instance[PosEnum];
                    value = sta2.T10;
                    this.TextPanel.Content = value.ToString("f2");
                    this.TextPanel.ToolTip = SensorGroupData.ShowDesc(SensorData, PosEnum);
                    grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                    if (grade != null)
                    {
                        this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.T10Color);
                        find_color = true;
                    }
                    if (!find_color)
                    {
                        this.ColorPanel.Background = defaultColor;
                    }
                    break;
                case SensorItemEnum.ColorDiagram:
                    double value_end = SensorData.F_LoadDataValue(PosEnum);
                    SensorStaData sta3 = SensorStaDataCache.Instance[PosEnum];
                    double value_T90 = sta3.T90;
                    double value_T10 = sta3.T10;
                    bool find_color_top = false;
                    bool find_color_left = false;
                    bool find_color_right= false;
                    ProductGrade gradeDelta = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.T10, value_T10);
                    ProductGrade gradeStart = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.T90, value_T90);
                    ProductGrade gradeFinal = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.SrcData, value_end);


                    if (gradeDelta != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.T10)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + "T10时间：" + value_T10.ToString("f2");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeDelta.T10Color); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.T10)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + "T10时间：" + value_T10.ToString("f2");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeDelta.T10Color); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.T10)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + "T10时间：" + value_T10.ToString("f2");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeDelta.T10Color); find_color_right = true;
                        }
                    }
                    if (gradeStart != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.T90)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + "T90时间：" + value_T90.ToString("f2");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeStart.T90Color); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.T90)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + "T90时间：" + value_T90.ToString("f2");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeStart.T90Color); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.T90)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + "T90时间：" + value_T90.ToString("f2");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeStart.T90Color); find_color_right = true;
                        }
                    }
                    if (gradeFinal != null)
                    {
                        if (ColorDiagramTop == SensorItemEnum.SrcData)
                        {
                            ColorPanelTop.ToolTip = Sensor.PosString + SensorGroupData.ShowModeDesc +  "：" + value_end.ToString("f2");
                            ColorPanelTop.Background = (Brush)converter.ConvertFromString(gradeFinal.NongDuColor); find_color_top = true;
                        }
                        if (ColorDiagramLeft == SensorItemEnum.SrcData)
                        {
                            ColorPanelLeft.ToolTip = Sensor.PosString + SensorGroupData.ShowModeDesc + "：" + value_end.ToString("f2");
                            ColorPanelLeft.Background = (Brush)converter.ConvertFromString(gradeFinal.NongDuColor); find_color_left = true;
                        }
                        if (ColorDiagramRight == SensorItemEnum.SrcData)
                        {
                            ColorPanelRight.ToolTip = Sensor.PosString + SensorGroupData.ShowModeDesc + "：" + value_end.ToString("f2");
                            ColorPanelRight.Background = (Brush)converter.ConvertFromString(gradeFinal.NongDuColor); find_color_right = true;
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
                O2TesterConfgig.Save();
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
                O2TesterConfgig.Save();
            }
        }
        public SensorItemEnum ColorDiagramRight
        {
            get
            {
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight;
                O2TesterConfgig.Save();
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight = value;
                O2TesterConfgig.Save();
            }
        }

        public bool? IsChecked { get { return true; } }
        public string Text { get { return this.PosotionPanel.Content + ""; } set { this.PosotionPanel.Content = value; } }

        public void T1_Click(object sender, RoutedEventArgs e)
        {
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensorsOf(this.PosEnum, true);
        }

        private void ColorPanelTop_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10 , SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10 };
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
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10, SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10 };
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
            SensorItemEnum[] ps = new SensorItemEnum[] { SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10, SensorItemEnum.T90, SensorItemEnum.SrcData, SensorItemEnum.T10 };
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
