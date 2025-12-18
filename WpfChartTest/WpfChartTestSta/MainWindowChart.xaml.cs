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

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowChart : Window
    {
         
        double defalut_thickness = 1;
        double focus_thickness = 4;

        bool CurrentFocusDoing = false;
        PosEnum CurrentFocus = PosEnum.A1;
         
        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, LineDatas> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        
        public MainWindowChart()
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
            for (double i = 0; i <= 120; i = i + 1)
            {
                ushort[] ret = TestData.GetRandNums(0, 650, 64).ToArray();
                SensorGroupData datag = new SensorGroupData(ret, Math.Round(i, 2));
                foreach (var item in datag.Data.Values)
                {
                    CacheData[item.PosEnum].Add(item); 
                }
            }
             
            this.plotter1.IniDrawingCanvas(this.DrawingCanvas1);
            LineGraphDic = new Dictionary<PosEnum, LineDatas>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                var lg = new LineDatas(String.Format("{0}", sid), sel.Select(w => w.F_AddTime), sel.Select(w => w.F_LoadDataValue));
                lg.Line1Color = new SolidColorBrush(ColorUtil.Of.RandomNoDarkAlpha(100));
                this.DrawingCanvas1.AllDatas.Add(lg);
                LineGraphDic.Add(sid, lg);
            }
            this.DrawingCanvas1.Polyline();

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            XXXXX.Content = DateTime.Now.ToString("HH:mm:ss:fff");
        }

        TestData _TestData = new TestData();


        private void Chart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SSSS_Click(object sender, RoutedEventArgs e)
        {
            _TestData.OnNewData -= _TestData_OnNewData;
            _TestData.OnNewData += _TestData_OnNewData;

            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                sel.Clear();
                LineGraphDic[sid].clear();
            }

            _TestData.StartTask();
        }


        private void _TestData_OnNewData(SensorGroupData datas_history, double F_AddTime)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in datas_history.Data.Values)
                {
                    PosEnum sid = item.PosEnum;
                    var sel = CacheData[sid];
                    sel.Add(item);
                    LineGraphDic[sid].XData.Add(F_AddTime);
                    LineGraphDic[sid].RtData.Add(item.F_LoadDataValue);
                }
                this.plotter1.IniDrawingCanvas(this.DrawingCanvas1);
                this.DrawingCanvas1.Polyline();
            }));

            
        }

    }

}
