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
    public partial class MainWindow : Window
    {

        TestData _TestData = new TestData();
         

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        Dictionary<PosEnum, LineDatas> Datas = new Dictionary<PosEnum, LineDatas>();
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            InitializeComponent();
            _TestData.OnNewData += _TestData_OnNewData;


            foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
            {
                LineDatas data1 = new LineDatas()
                {
                    Title = item.ToString(),
                    Min = 0,
                    Max = 800
                }; 
                Datas.Add(item, data1);
            }
            for (double i = 0; i <= 120; i=i+0.5)
            {
                ushort[] ret = TestData.GetRandNums(0, 650, 64).ToArray();
                SensorGroupData datag = new SensorGroupData(ret, Math.Round(i, 2));
                foreach (var item in datag.Data.Values)
                {
                    Datas[item.PosEnum].XData.Add(item.F_AddTime);
                    Datas[item.PosEnum].RtData.Add(item.F_LoadDataValue);
                    Datas[item.PosEnum].ThData.Add(item.F_LoadDataValue);
                    Datas[item.PosEnum].EndIndex = Datas[item.PosEnum].RtData.Count - 1;
                }
            }

            DrawingCanvas _DrawingCanvas = linechartInner;
            _DrawingCanvas.Width = 500;
            _DrawingCanvas.Height = 500;
            foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
            {
                _DrawingCanvas.AllDatas.Add(Datas[item]);
            }
            linechart.IniDrawingCanvas(_DrawingCanvas);
            //DrawingLine _DrawingLine = new DrawingLine(_DrawingCanvas);
            //_DrawingLine.Width = 500;
            //_DrawingLine.Height = 500;
            //this.linechart.Children.Add(_DrawingLine);
            _DrawingCanvas.Polyline();

        }

        private void TestData_Click(object sender, RoutedEventArgs e)
        {
            _TestData.StartTask();
        }

        private void _TestData_OnNewData(SensorGroupData datas_history, double F_AddTime)
        {
            //foreach (var item in datas_history.Data.Values)
            //{
            //    Datas[item.PosEnum].XData.Add(item.F_AddTime);
            //    Datas[item.PosEnum].RtData.Add(item.F_LoadDataValue);
            //    Datas[item.PosEnum].ThData.Add(item.F_LoadDataValue);
            //    Datas[item.PosEnum].EndIndex = Datas[item.PosEnum].RtData.Count - 1;
            //}
        }
         
    }

}
