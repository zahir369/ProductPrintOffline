using MKSS.Model;
using MKSS.Service.ECTester;
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

namespace MKSS.APP.ECTester
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
                    case SensorItemEnum.SrcData:
                        if (
                            ECTesterService.ECSensotMode== ECSensotMode.IndustryMode 
                            || 
                            ECTesterService.ECSensotMode == ECSensotMode.AcidityMode)
                        {
                            //四行布局，左右排布节省空间
                            this.TextPanel.SetValue(Grid.ColumnProperty, 1);
                            this.TextPanel.SetValue(Grid.RowProperty, 0);
                            this.TextPanel.SetValue(Grid.ColumnSpanProperty, 3);
                            this.TextPanel.SetValue(Grid.RowSpanProperty, 2);
                            this.ColorPanel.SetValue(Grid.ColumnProperty, 0);
                            this.ColorPanel.SetValue(Grid.RowProperty, 0);
                            this.ColorPanel.SetValue(Grid.ColumnSpanProperty, 1);
                            this.ColorPanel.SetValue(Grid.RowSpanProperty, 2);
                        }
                        if (ECTesterService.ECSensotMode == ECSensotMode.AqueousMode)
                        {
                            //两行布局，上下排布节省空间
                            this.TextPanel.SetValue(Grid.ColumnProperty, 0);
                            this.TextPanel.SetValue(Grid.RowProperty, 1);
                            this.TextPanel.SetValue(Grid.ColumnSpanProperty, 4);
                            this.TextPanel.SetValue(Grid.RowSpanProperty, 1);
                            this.ColorPanel.SetValue(Grid.ColumnProperty, 0);
                            this.ColorPanel.SetValue(Grid.RowProperty, 0);
                            this.ColorPanel.SetValue(Grid.ColumnSpanProperty, 4);
                            this.ColorPanel.SetValue(Grid.RowSpanProperty, 1);
                        }
                        this.TextPanel.Visibility = Visibility.Visible;
                        this.ColorPanel.Visibility = Visibility.Visible;
                        this.ColorPanelFull.Visibility = Visibility.Hidden;
                        break;
                    case SensorItemEnum.ColorDiagram:
                        this.TextPanel.Visibility = Visibility.Hidden;
                        this.ColorPanel.Visibility = Visibility.Hidden;
                        this.ColorPanelFull.Visibility = Visibility.Visible;
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


        public bool PauseMode { get { return UIZhuiSuC10Model.Instance.ECService.Pause && ECTesterConfgig.Instance.PauseEnabled; } }
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
                    if (SensorGroupData.ShowMode == ShowModeEnum.AD) f = "f0";
                    this.TextPanel.Content = value.ToString(f);
                    this.TextPanel.ToolTip = SensorGroupData.ShowDesc(SensorData, PosEnum);
                    //暂停时，特殊着色模式
                    if (PauseMode)
                    {
                        bool except = (value < ECTesterConfgig.Instance.PauseValueExtend[0] || value > ECTesterConfgig.Instance.PauseValueExtend[1]);
                        this.ColorPanel.Background = except
                            ? (Brush)converter.ConvertFromString(ECTesterConfgig.Instance.PauseExcepColor)
                            : (Brush)converter.ConvertFromString(ECTesterConfgig.Instance.PauseColor);
                    }
                    else {
                        grade = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(Mode, value);
                        if (grade != null)
                        {
                            this.ColorPanel.Background = (Brush)converter.ConvertFromString(grade.Color());
                            find_color = true;
                        }
                        if (!find_color)
                        {
                            this.ColorPanel.Background = defaultColor;
                        }
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
                    ProductGrade gradeFinal = UIZhuiSuC10Model.Instance.SettingModel.ProductModelConfig.FetchGrade(SensorItemEnum.SrcData, value_end);

                     
                    if (gradeFinal != null)
                    {
                        if (ColorDiagramFull == SensorItemEnum.SrcData)
                        {
                            ColorPanelFull.ToolTip = Sensor.PosString + SensorGroupData.ShowModeDesc +  "：" + value_end.ToString("f2");
                            ColorPanelFull.Background = (Brush)converter.ConvertFromString(gradeFinal.Color()); find_color_top = true;
                        } 
                    }

                    if (!find_color_top)
                    {
                        this.ColorPanelFull.Background = defaultColor;
                    }
                    this.ColorPanelFull.Content = SensorData.F_Sensibility(PosEnum).ToString("f1");

                    break;
                default:
                    break;
            }

        }

        public SensorItemEnum ColorDiagramFull
        {
            get
            {
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramFull;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramFull = value;
                ECTesterConfgig.Save();
            }
        } 

        public bool? IsChecked { get { return true; } }
        public string Text { get { return this.PosotionPanel.Content + ""; } set { this.PosotionPanel.Content = value; } }

        public void T1_Click(object sender, RoutedEventArgs e)
        {
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensorsOf(this.PosEnum, true);
        }

        private void ColorPanelFull_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SensorItemEnum[] ps = new SensorItemEnum[] {  SensorItemEnum.SrcData,   SensorItemEnum.SrcData, };
            for (int i = 0; i < ps.Length; i++)
            {
                if (ColorDiagramFull == ps[i])
                {
                    ColorDiagramFull = ps[i + 1]; UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
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
