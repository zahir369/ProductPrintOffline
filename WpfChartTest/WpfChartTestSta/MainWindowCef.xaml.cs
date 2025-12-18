using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;
using WpfChartTest.Model;
using MKSS.APP.ECTester.UserCommon;
using System.IO;

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowCef : Window
    {
         
        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;
          
        ChartDataCache CacheData = new ChartDataCache();
        
        public MainWindowCef()
        {
            InitializeComponent();

            var path = Directory.GetCurrentDirectory() + "/EChart/index.html";
            webbro.Address = path;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            InitializeComponent();

            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                CacheData.Add(sid, new List<SensorDataItem>());
            }
            for (double i = 0; i <= 120; i = i + 0.5)
            {
                ushort[] ret = TestData.GetRandNums(0, 650, 64).ToArray();
                SensorGroupData datag = new SensorGroupData(ret, Math.Round(i, 2));
                foreach (var item in datag.Data.Values)
                {
                    CacheData[item.PosEnum].Add(item); 
                }
            }
 
        }

          

        public void RefreshPage()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                
            }));

        }
         
          
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshPage();
        }

    }

}
