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

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// <Grid>
    ///    <WrapPanel>
    ///        <local:ButtonEx Content = "信息" Width="75" Height="25" Margin="10" ButtonType="Normal"/>
    ///        <local:ButtonEx Icon = "/Images/button1.png"  Margin="10" ButtonType="Icon"/>
    ///        <local:ButtonEx Content = "文字按钮" Margin="10" ButtonType="Text"/>
    ///        <local:ButtonEx Content = "图文按钮" Icon="/Images/adshut.png" Margin="10" ButtonType="IconText"/>
    ///    </WrapPanel>
    /// </Grid>
    /// </summary>
    public class ButtonEx : Button
    {
        static ButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
        }

        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonType ButtonType
        {
            get { return (ButtonType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ButtonType), typeof(ButtonEx), new PropertyMetadata(ButtonType.Normal));

        /// <summary>
        /// 图片
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ButtonEx), new PropertyMetadata(null));

        /// <summary>
        ///  SVG地址
        /// </summary>
        public Uri SVGUri
        {
            get { return (Uri)GetValue(SVGUriProperty); }
            set { SetValue(SVGUriProperty, value); }
        }
        public static readonly DependencyProperty SVGUriProperty =
            DependencyProperty.Register("SVGUri", typeof(Uri), typeof(ButtonEx), new PropertyMetadata(null));


        /// <summary>
        /// 圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonEx), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 鼠标悬停字体颜色
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标按下文字颜色
        /// </summary>
        public Brush MousePressedForeground
        {
            get { return (Brush)GetValue(MousePressedForegroundProperty); }
            set { SetValue(MousePressedForegroundProperty, value); }
        }

        public static readonly DependencyProperty MousePressedForegroundProperty =
            DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标悬停边框颜色
        /// </summary>
        public Brush MouseOverBorderbrush
        {
            get { return (Brush)GetValue(MouseOverBorderbrushProperty); }
            set { SetValue(MouseOverBorderbrushProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBorderbrushProperty =
            DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标悬停背景颜色
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// 鼠标按下背景颜色
        /// </summary>
        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata());

        /// <summary>
        /// SVG 
        /// </summary>
        public Geometry PathData
        {
            get
            {
                return (Geometry)GetValue(PathDataProperty);
            }
            set
            {
                SetValue(PathDataProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PathData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathDataProperty =
           DependencyProperty.Register("PathData", typeof(Geometry), typeof(ButtonEx), new PropertyMetadata(new PathGeometry()));



        public double PathWidth
        {
            get { return (double)GetValue(PathWidthProperty); }
            set { SetValue(PathWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathWidthProperty =
            DependencyProperty.Register("PathWidth", typeof(double), typeof(ButtonEx), new PropertyMetadata());



        public double PathHeight
        {
            get { return (double)GetValue(PathHeightProperty); }
            set { SetValue(PathHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathHeightProperty =
            DependencyProperty.Register("PathHeight", typeof(double), typeof(ButtonEx), new PropertyMetadata());



    }

    public enum ButtonType
    {
        Normal,
        Icon,
        Text,
        IconText,
        SVG,
        Path,
        Content,
    }

}
