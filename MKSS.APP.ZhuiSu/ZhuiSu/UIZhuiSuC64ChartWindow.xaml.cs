using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Win32;
using MKSS.APP.ZhuiSu.UserControls;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using ScottPlot;
using ScottPlot.Plottable;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC64ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC64ChartWindow : Window
    {

        VLine vLine;
        HLine hLine;
        public UIZhuiSuC64ChartWindow()
        {

            InitializeComponent(); 
            vLine = wpfPlot1.Plot.AddVerticalLine(0, color: System.Drawing.Color.Green, style: LineStyle.DashDot);
            hLine = wpfPlot1.Plot.AddHorizontalLine(0, color: System.Drawing.Color.Green, style: LineStyle.DashDot);
            wpfPlot1.Render(); 
            wpfPlot1.RightClicked -= wpfPlot1.DefaultRightClickEvent; 
            wpfPlot1.RightClicked += DeployCustomMenu;
        }

        private void DeployCustomMenu(object sender, EventArgs e)
        {
            MenuItem addSinMenuItem = new MenuItem() { Header = "设置环境值" };
            addSinMenuItem.Click += AddEnv;
            MenuItem clearPlotMenuItem = new MenuItem() { Header = "清除环境值" };
            clearPlotMenuItem.Click += ClearEnv;

            ContextMenu rightClickMenu = new ContextMenu();
            rightClickMenu.Items.Add(addSinMenuItem);
            rightClickMenu.Items.Add(clearPlotMenuItem);

            rightClickMenu.IsOpen = true;
        }

        Batch Batch { get; set; }
        C10GridRow C10GridRow { get; set; }
        List<Point> Points = new List<Point>();
        DataTable DtLabel = null;
        List<Env> listEnv = null;
        public void SetBatch(Batch _batch2, C10GridRow _C10GridRow2)
        {
             
            if (_batch2 == null|| _C10GridRow2==null) return;

            try
            {

                WaitWindow.ShowWindow("正在读取数据",  string.Format("正在获取 {0} {1} 数据......", _batch2.F_BatchName, _C10GridRow2.SensorId ) + "数据......", this);
                Batch = _batch2;
                C10GridRow = _C10GridRow2;
                Batch _batch = Batch;
                string F_SerialNO = "";
                Points = new List<Point>();
                DataTable tab1 = UIZhuiSuData.Instance.SensorServices.BaseDal.QueryTable("SELECT F_SerialNO,F_SensorId FROM pd_sensor WHERE F_BatchId=" + Batch.F_BatchId + " and F_SensorId='" + C10GridRow.SensorId + "'").Result;

                if (tab1.Rows.Count > 0)
                {
                    F_SerialNO = tab1.Rows[0]["F_SerialNO"] + "";
                }
                this.Title = string.Format("元器件特性曲线 {0} {1} {2}", _batch.F_BatchName, C10GridRow.SensorId, F_SerialNO);

                wpfPlot1.Plot.SetAxisLimits(new ScottPlot.AxisLimits(0, (_batch.F_AgingEndTime - _batch.F_AgingStartTime).TotalSeconds, 0, 5));
                //this.wpfPlotHumidity.Plot.SetAxisLimits(new ScottPlot.AxisLimits(0, (_batch.F_AgingEndTime - _batch.F_AgingStartTime).TotalSeconds, 0, 100));
                //this.wpfPlotTemperature.Plot.SetAxisLimits(new ScottPlot.AxisLimits(0, (_batch.F_AgingEndTime - _batch.F_AgingStartTime).TotalSeconds, 0, 70));

                DateTime dtTh = _batch.F_AgingStartTime;
                DateTime dtThEnd = _batch.F_AgingEndTime;
                listEnv = UIZhuiSuData.Instance.SensorDataServices.BaseDal.Db.Queryable<Env>().WhereIF(true, w => w.F_DateTime >= dtTh && w.F_DateTime <= dtThEnd && w.F_Humidity > 0).OrderBy("F_DateTime").ToListAsync().Result;
                if (listEnv.Count > 0) {
                    wpfPlot1.Plot.AddScatter(listEnv.Select(w => (w.F_DateTime - dtTh).TotalSeconds).ToArray(), listEnv.Select(w => (double)w.F_Humidity / 20).ToArray(), System.Drawing.Color.Green, 1, 0, MarkerShape.none, LineStyle.Dot, "湿度");
                    wpfPlot1.Plot.AddScatter(listEnv.Select(w => (w.F_DateTime - dtTh).TotalSeconds).ToArray(), listEnv.Select(w => (double)w.F_Temperature / 20).ToArray(), System.Drawing.Color.Red, 1, 0, MarkerShape.none, LineStyle.Dot, "温度");
                }

                DataTable dt = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
                    string.Format("select F_DataValue v,F_AddTime t from pd_sensordata_{0} where F_SensorId='{1}' and F_DataValue!=170 order by F_AddTime",
                    _batch.F_BatchId, C10GridRow.SensorId))
                    .Result;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Points.Add(new Point(Convert.ToDouble(dt.Rows[i]["t"]), Convert.ToDouble(dt.Rows[i]["v"])/10000 * 5));
                }
                if (Points.Count == 0) return;
                wpfPlot1.Plot.AddScatter(Points.Select(w => w.X).ToArray(), Points.Select(w => w.Y).ToArray(), System.Drawing.Color.Blue);

                Markers.Clear();
                MarkerTexts.Clear();
                CalcMarkers();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
                WaitWindow.CloseWindow(this);
            }
            

        }


        Dictionary<string, ScatterPlot> Markers = new Dictionary<string, ScatterPlot>();
        Dictionary<string, Text> MarkerTexts = new Dictionary<string, Text>();
        void CalcMarkers() {

            DtLabel = UIZhuiSuData.Instance.SensorDataServices.QueryTable(
                   string.Format("select * from pd_batch_condition where F_BatchId={0} order by F_StartTime",
                   Batch.F_BatchId))
                   .Result;
            Dictionary<string, string> tests = new Dictionary<string, string>();
            for (int i = 0; i < DtLabel.Rows.Count; i++)
            {

                string F_ConditionId = DtLabel.Rows[i]["F_ConditionId"] + ""; tests.Add(F_ConditionId, F_ConditionId);
                if (Markers.ContainsKey(F_ConditionId)) {
                    continue;
                }
                double m_pos = Convert.ToDouble(DtLabel.Rows[i]["F_StartTime"]);
                string str = string.Format("温度：{0}℃，湿度：{1}%", DtLabel.Rows[i]["F_Temperature"], DtLabel.Rows[i]["F_Humidity"]);
                List<Point> vq = Points.Where(w => w.X > m_pos).ToList();
                double v = 0;
                if (vq.Count > 0)
                {
                    v = vq[0].Y;
                }

                var c1 = wpfPlot1.Plot.AddPoint(m_pos, v, System.Drawing.Color.Magenta, size: 10, shape: MarkerShape.openDiamond);
                var t1 = wpfPlot1.Plot.AddText(str, m_pos, v, size: 16, color: System.Drawing.Color.Blue);
                Markers.Add(DtLabel.Rows[i]["F_ConditionId"] + "", c1);
                MarkerTexts.Add(DtLabel.Rows[i]["F_ConditionId"] + "", t1);
            }

            foreach (var item in Markers.Keys.ToArray())
            {
                if (!tests.ContainsKey(item))
                {
                    wpfPlot1.Plot.Remove(Markers[item]);
                    wpfPlot1.Plot.Remove(MarkerTexts[item]);
                    Markers.Remove(item);
                    MarkerTexts.Remove(item);
                }
            }

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int pixelX = (int)e.MouseDevice.GetPosition(wpfPlot1).X;
            int pixelY = (int)e.MouseDevice.GetPosition(wpfPlot1).Y;
            (double coordinateX, double coordinateY) = wpfPlot1.GetMouseCoordinates();
            double poix = wpfPlot1.Plot.GetCoordinateX(pixelX);
            double poiy = wpfPlot1.Plot.GetCoordinateY(pixelY);
            XCoordinateLabel.Content = $"{poix:0.0}s";

            vLine.X = coordinateX;
            hLine.Y = coordinateY;

            bool find = false;
            List<Point> vq = Points.Where(w => w.X > poix).ToList();
            if (vq.Count > 0)
            {
                double v = 0;
                v = vq[0].Y;
                YCoordinateLabel.Content = $"{v:0.000}v";
                //hLine.Y = v;

                for (int i = 0; i < DtLabel.Rows.Count; i++)
                {
                    double m_pos = Convert.ToDouble(DtLabel.Rows[i]["F_StartTime"]);
                    if (poix > m_pos)
                    {
                        find = true;
                        XPixelLabel.Content = $"{DtLabel.Rows[i]["F_Temperature"]}℃";
                        YPixelLabel.Content = $"{DtLabel.Rows[i]["F_Humidity"]}%";
                    }
                }

                if (this.listEnv != null) {
                    for (int i = 0; i < this.listEnv.Count; i++)
                    {
                        Env env = listEnv[i];
                        double t = (env.F_DateTime - Batch.F_AgingStartTime).TotalSeconds;
                        if (t >= poix) {
                            find = true;
                            XPixelLabel.Content = $"{env.F_Temperature.ToString("f1")}℃";
                            YPixelLabel.Content = $"{env.F_Humidity.ToString("f1")}%";
                            break;
                        }
                    }
                }
                
            }
            else {
                YCoordinateLabel.Content = $"{poiy:0.000}v";
               
            }

            if (!find) {
                XPixelLabel.Content = $"--℃";
                YPixelLabel.Content = $"--%";
            }
            try
            {
                wpfPlot1.Render();
            }
            catch (Exception)
            {
                 
            }
        }

        /// <summary>
        ///  双击添加环境
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEnv(object sender, RoutedEventArgs e)
        {
            Point pp = Mouse.GetPosition(wpfPlot1);//WPF方法
            int pixelX = (int)pp.X;
            int pixelY = (int)pp.Y;
            (double coordinateX, double coordinateY) = wpfPlot1.GetMouseCoordinates();
            double F_StartTime = wpfPlot1.Plot.GetCoordinateX(pixelX);
            if (Batch == null) return;
            UIZhuiSuAddCondition add = new UIZhuiSuAddCondition(Batch.F_BatchId, F_StartTime);
            var res = add.ShowDialog();
            if (res != null && res.Value) { 
                
            }
            CalcMarkers();
        }
        private void ClearEnv(object sender, RoutedEventArgs e)
        {
            if (Batch == null) return;
            try
            {
                int res = UIZhuiSuData.Instance.BatchServices.BaseDal.ExecuteCommand(
                        @"delete from pd_batch_condition where F_BatchId=" + Batch.F_BatchId + ""
                        ).Result;
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(string.Format("删除{0}", res>0 ? "成功" : "失败"));
                CalcMarkers();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message); 
            }
        }


        private void LockX_Checked(object sender, RoutedEventArgs e)
        {
            //if (this.LockX != null) wpfPlot1.Plot.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            //if (this.LockY != null) plotter1.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
            //if (this.LockX != null) plotter2.IsVerticalNavigationEnabled = !(this.LockX.IsChecked == null || !this.LockX.IsChecked.Value);
            //if (this.LockY != null) plotter2.IsHorizontalNavigationEnabled = !(this.LockY.IsChecked == null || !this.LockY.IsChecked.Value);
        }

    }

}