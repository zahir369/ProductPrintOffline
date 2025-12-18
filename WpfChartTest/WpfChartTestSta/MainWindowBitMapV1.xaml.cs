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
using MKSS.APP.ECTester;

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowBitMapV1 : Window
    {

        PloyLineChart db = null;

        public MainWindowBitMapV1()
        {
            InitializeComponent();
            this.Loaded += MainWindowBitMap_Loaded;
            
        }

        private void MainWindowBitMap_Loaded(object sender, RoutedEventArgs e)
        {

            CacheData.Clear();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                CacheData.Add(sid, new List<SensorDataItem>());
            }
            //for (double i = 0; i <= 120; i = i + 1)
            //{
            //    ushort[] ret = TestData.GetRandNums(0, 650, 64).ToArray();
            //    SensorGroupData datag = new SensorGroupData(ret, Math.Round(i, 2));
            //    foreach (var item in datag.Data.Values)
            //    {
            //        CacheData[item.PosEnum].Add(item);
            //    }
            //}

            PloyLineChart.XLabelMaxDefault = 12;
            PloyLineChart.YLabelMinDefault = 30;
            PloyLineChart.YLabelMaxDefault = 60;
            for (double i = 0; i <= 10; i = i + 1)
            {
                SensorGroupData datag = new SensorGroupData((int)(i* 6), i);
                foreach (var item in datag.Data.Values)
                {
                    CacheData[item.PosEnum].Add(item);
                }
            }
          


            if (db == null)
            {
                db = new PloyLineChart(this.grid1);
            }
            db.Init((int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到


            LineGraphDic = new Dictionary<PosEnum, SeriseDatas>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                var lg = new SeriseDatas(String.Format("{0}", sid), sel.Select(w => (float)w.F_AddTime).ToList(), sel.Select(w => (float)w.F_LoadDataValue).ToList());
                lg.Line1Color = new System.Drawing.SolidBrush(ColorDrawingUtil.Of.RandomNoDarkAlpha(100));
                db.AllDatas.Add(lg);
                LineGraphDic.Add(sid, lg);
            }

            _TestData.OnNewData -= _TestData_OnNewData;
            _TestData.OnNewData += _TestData_OnNewData;

            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                sel.Clear();
                LineGraphDic[sid].clear();
            }

            db.AppendAxises();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

           
            Button btn = (Button)sender;
            if (!db.Continued)
            {
                db.Continued = true;
                btn.Content = "开始";
            }
            else
            {
                db.Continued = false;
                btn.Content = "停止";
            }
        }

        private void BtnBengin_Click(object sender, RoutedEventArgs e)
        {
            db = null;
            if (db == null)
            {
                db = new PloyLineChart(this.grid1);
                db.Init((int)this.ActualWidth, (int)this.ActualHeight);
            }
            db.StartDrawGraph( );
        }


        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, SeriseDatas> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        private void btnCahrt_Click(object sender, RoutedEventArgs e)
        {
            CacheData.Clear();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                CacheData.Add(sid, new List<SensorDataItem>());
            }

            PloyLineChart.XLabelMaxDefault = 210;
            PloyLineChart.YLabelMinDefault = 300;
            PloyLineChart.YLabelMaxDefault = 700; 

            for (double i = 0; i <= 210; i = i + 1)
            {
                ushort[] ret = TestData.GetRandNums(0, 650, 64).ToArray();
                SensorGroupData datag = new SensorGroupData(ret, Math.Round(i, 2));
                foreach (var item in datag.Data.Values)
                {
                    CacheData[item.PosEnum].Add(item);
                }
            }
            //for (double i = 0; i <= 20; i = i + 1)
            //{
            //    SensorGroupData datag = new SensorGroupData(60F-(float)(i * 6.6), i);
            //    foreach (var item in datag.Data.Values)
            //    {
            //        CacheData[item.PosEnum].Add(item);
            //    }
            //}

            if (db == null)
            {
                db = new PloyLineChart(this.grid1);
            }
            db.SetDefaultAxis();
            db.AllDatas.Clear();
            db.Init((int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到

            LineGraphDic = new Dictionary<PosEnum, SeriseDatas>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                var lg = new SeriseDatas(String.Format("{0}", sid), sel.Select(w => (float)w.F_AddTime).ToList(), sel.Select(w => (float)w.F_LoadDataValue).ToList());
                lg.Line1Color = new System.Drawing.SolidBrush(ColorDrawingUtil.Of.Yellow);
                db.AllDatas.Add(lg);
                LineGraphDic.Add(sid, lg);
            }
            db.AppendAll();

        }


        TestData _TestData = new TestData();
        private void btnDyCahrt_Click(object sender, RoutedEventArgs e)
        {

            _TestData.StartTask();

        }


        private void _TestData_OnNewData(SensorGroupData datas_history, double F_AddTime)
        {

 

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var item in datas_history.Data.Values)
                    {
                        PosEnum sid = item.PosEnum;
                        var sel = CacheData[sid];
                        sel.Add(item);
                    }
                    List<float> list = datas_history.Data.Select(w => (float)w.Value.F_LoadDataValue).ToList();
                    db.AppendData((float)datas_history.F_AddTime, list);
                    db.Img.InvalidateVisual();
                    int ttt = db.Img.Dispatcher.Thread.ManagedThreadId;
                }));
            }));

        }

    }

}
