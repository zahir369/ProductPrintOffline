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
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using ScottPlot;
using ScottPlot.Plottable;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC10Chart.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Chart : UserControl
    {
        VLine vLine;
        HLine hLine; 
        public UIZhuiSuC10Chart()
        {
            InitializeComponent();

            wpfPlot1.Plot.AddSignal(ScottPlot.DataGen.RandomWalk(null, 100));

            vLine = wpfPlot1.Plot.AddVerticalLine(0, color: System.Drawing.Color.Red, style: LineStyle.Dash);
            hLine = wpfPlot1.Plot.AddHorizontalLine(0, color: System.Drawing.Color.Red, style: LineStyle.Dash);

            wpfPlot1.Render();

            if (Batch != null)
            {
                wpfPlot1.Plot.SetAxisLimits(new ScottPlot.AxisLimits(0, (Batch.F_AgingEndTime - Batch.F_AgingStartTime).TotalSeconds, 0, 10000));
            }
            else {
                wpfPlot1.Plot.SetAxisLimits(new ScottPlot.AxisLimits(0, 1000, 0, 10000));
            }

        }

        Batch Batch { get; set; }
        List<Sensor> tabs = null;
        Dictionary <Sensor, ScatterPlot> ScatterPlotDic { get; set; }
        Dictionary<string, List<double>> dic_y = new Dictionary<string, List<double>>();
        Dictionary<string, List<double>> dic_x = new Dictionary<string, List<double>>();
        public void SetBatch(Batch _batch)
        {

            Batch = _batch;
            if (_batch == null) return;


            //历史批次直接读数据库
            dic_y.Clear();
            dic_x.Clear();
            string sql_s = " F_BatchId='" + Batch.F_BatchId + "' order by F_SensorId ";
            tabs = UIZhuiSuData.Instance.SensorServices.Query(sql_s).Result;
            ScatterPlotDic = new Dictionary<Sensor, ScatterPlot>();
            for (int i = 0; i < tabs.Count; i++)
            {
                Sensor s = tabs[i];
                if (!dic_x.ContainsKey(s.F_SensorId))
                {
                    dic_x.Add(s.F_SensorId, new List<double>());
                    dic_y.Add(s.F_SensorId, new List<double>()); 
                }
            }

            string sql1 = "SELECT t.F_DataValue,t.F_AddTime,t.F_SensorId FROM `pd_sensordata_" + Batch.F_BatchId + "` t order by t.F_AddTime ";
            DataTable tab1 = UIZhuiSuData.Instance.BatchServices.QueryTable(sql1).Result;


            for (int i = 0; i < tab1.Rows.Count; i++)
            {
                DataRow dr = tab1.Rows[i];
                string F_SensorId = dr["F_SensorId"] + "";
                double F_AddTime = Convert.ToDouble(dr["F_AddTime"]);
                double F_DataValue = Convert.ToDouble(dr["F_DataValue"]);
                dic_x[F_SensorId].Add(F_AddTime);
                dic_y[F_SensorId].Add(F_DataValue);
            }


        }

        public void ShowSensors(List<string> sendor_ids) {
            if (ScatterPlotDic.Count > 0) {
                foreach (var item in ScatterPlotDic.Values)
                {
                     wpfPlot1.Plot.Remove(item);
                }
            }
            ScatterPlotDic.Clear();
            foreach (string sid in sendor_ids)
            {
                if (!dic_x.ContainsKey(sid)) continue;
                var s = tabs.FirstOrDefault(w => w.F_SensorId == sid);
                if (s != null) {
                    if (!ScatterPlotDic.ContainsKey(s)) ScatterPlotDic.Add(s, null);
                    ScatterPlotDic[s] = wpfPlot1.Plot.AddScatter(dic_x[sid].ToArray(), dic_y[sid].ToArray());
                    wpfPlot1.Render();
                } 
                
            }
        }

        PageBoardItem DataA { get; set; }
        PageBoardItem DataB { get; set; }
        public void SetPageBoardItem(PageBoardItem data, PageBoardItem dataB) {
            DataA = data; DataB = dataB;
        }
        public void SetDataRealTime(List<PageBoardItem> datass)
        {
            if (Batch == null || DataA == null || DataB == null || datass.Count==0) return;
            foreach (PageBoardItem data in datass)
            {
                if (data.Address > 0 )
                {
                    Batch _Batch = Batch;
                    {

                        //检测中批次历史数据加上实时数据
                        if (_Batch.F_AgingStartTime != DateTime.MinValue && _Batch.F_AgingEndTime != DateTime.MinValue && ScatterPlotDic != null)
                        {
                            TimeSpan total = _Batch.F_AgingEndTime - _Batch.F_AgingStartTime;
                            TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;
                            List<PageSensorModel> tables = data.ProductTable;
                            foreach (PageSensorModel sem in tables)
                            {
                                if (sem.Value != null && dic_x.ContainsKey(sem.SensorId))
                                {
                                    dic_x[sem.SensorId].Add((DateTime.Now - _Batch.F_AgingStartTime).TotalSeconds);
                                    dic_y[sem.SensorId].Add(sem.Value.Value);
                                    var s = ScatterPlotDic.FirstOrDefault(w => w.Key.F_SensorId == sem.SensorId);
                                    if (s.Key != null)
                                    {
                                        ScatterPlot _ScatterPlot = s.Value;
                                        _ScatterPlot.Update(dic_x[sem.SensorId].ToArray(), dic_y[sem.SensorId].ToArray());
                                    }
                                }
                            }
                        }
                    } 

                }
                else
                {

                }
            }

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int pixelX = (int)e.MouseDevice.GetPosition(wpfPlot1).X;
            int pixelY = (int)e.MouseDevice.GetPosition(wpfPlot1).Y;

            (double coordinateX, double coordinateY) = wpfPlot1.GetMouseCoordinates();

            XPixelLabel.Content = $"{pixelX:0.000}";
            YPixelLabel.Content = $"{pixelY:0.000}";
            XCoordinateLabel.Content = $"{wpfPlot1.Plot.GetCoordinateX(pixelX):0.00000000}";
            YCoordinateLabel.Content = $"{wpfPlot1.Plot.GetCoordinateY(pixelY):0.00000000}";

            vLine.X = coordinateX;
            hLine.Y = coordinateY;

            wpfPlot1.Render();
        }
    }
}
