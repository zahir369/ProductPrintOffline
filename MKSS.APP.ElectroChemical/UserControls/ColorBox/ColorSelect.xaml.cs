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

namespace ColorBoxTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ColorSelect : Window
    {
        public ColorSelect(Brush inti)
        {
            InitializeComponent();
            this.Loaded += ColorSelect_Loaded;
            SelectColor = inti;
            this.ColorBox1.BrushType = ColorBox.BrushTypes.Solid;
            this.ColorBox1.Color = ((SolidColorBrush)inti).Color;
        }
        public ColorSelect( )
        {
            InitializeComponent(); 
        }
        private void ColorSelect_Loaded(object sender, RoutedEventArgs e)
        {
            //this.ColorBox1.Brush = new SolidColorBrush(SelectColor);
        }

        public Brush SelectColor { get; private set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CB.IsPopupOpen = true;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SelectColor  = (this.ColorButton1.Background  );
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
