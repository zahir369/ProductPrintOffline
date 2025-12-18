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
using InteractiveDataDisplay.WPF;
using System.Windows.Controls;
using WpfChartTest.Model;
using MKSS.APP.ECTester.UserCommon;

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         
        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;
         
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, LineGraph> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        
        public MainWindow()
        {
            InitializeComponent();
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

            LineGraphDic = new Dictionary<PosEnum, LineGraph>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            { 
                LineGraphDic.Add(sid, AddLineGraph(sid));
            }

        }

         
        LineGraph AddLineGraph(PosEnum PosEnum)
        {

            var lg = new LineGraph();
            lines1.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(ColorUtil.Of.RandomNoDark);
            lg.Description = String.Format("{0}", PosEnum);
            lg.StrokeThickness = defalut_thickness;
            if (!CacheData.ContainsKey(PosEnum)) return lg;
            var sel = CacheData[PosEnum];
            if (sel.Count == 0) return lg;
            lg.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
            return lg;

        }


        public void RefreshPage()
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (LineGraphDic == null) return;
                foreach (var pose in LineGraphDic.Keys)
                {
                    LineGraph _LineGraph = LineGraphDic[pose];
                    var sel = CacheData[pose];
                    _LineGraph.Plot(sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
                }
            }));

        }
         
          
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshPage();
        }

    }

}
