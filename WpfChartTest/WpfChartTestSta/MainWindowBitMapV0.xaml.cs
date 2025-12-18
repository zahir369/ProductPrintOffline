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
using WpfChartTest.ChartV0;

namespace WpfChartTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowBitMapV0 : Window
    {

        PloyLineChart db = null;

        public MainWindowBitMapV0()
        {
            InitializeComponent();
            this.Loaded += MainWindowBitMap_Loaded;
            
        }

        private void MainWindowBitMap_Loaded(object sender, RoutedEventArgs e)
        {
     
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
                db = new PloyLineChart(this.img);
                db.Init(zooming, (int)this.ActualWidth, (int)this.ActualHeight);
            }
            db.StartDrawGraph(img);
        }


        /// <summary>
        ///  数据线
        /// </summary>
        Dictionary<PosEnum, SeriseDatas> LineGraphDic { get; set; }
        ChartDataCache CacheData = new ChartDataCache();
        private void btnCahrt_Click(object sender, RoutedEventArgs e)
        {

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
             

            db = null;
            if (db == null)
            {
                db = new PloyLineChart(this.img);
                db.Init(zooming, (int)grid1.ActualWidth, (int)grid1.ActualHeight);//不能用 img ，img 尺寸获取不到
            }


            LineGraphDic = new Dictionary<PosEnum, SeriseDatas>();
            foreach (PosEnum sid in Enum.GetValues<PosEnum>())
            {
                var sel = CacheData[sid];
                var lg = new SeriseDatas(String.Format("{0}", sid), sel.Select(w => (float)w.F_AddTime).ToList(), sel.Select(w => (float)w.F_LoadDataValue).ToList());
                lg.Line1Color = new System.Drawing.SolidBrush(ColorDrawingUtil.Of.RandomNoDarkAlpha(100));
                db.AllDatas.Add(lg);
                LineGraphDic.Add(sid, lg);
            }
            db.ReDrawAll();

        }

    }

}
