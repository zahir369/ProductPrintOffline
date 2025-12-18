using MKSS.APP.ECTester;
using MKSS.APP.ECTester.UserCommon;
using MKSS.APP.UIBiaoDing.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.Win4In1Tester
{
   
    //“√”和“×”
    public partial class UIConnection : UserControl
    {

        PloyLineChart chart = null;
        public Model4In1Tester Saver = null;
        public ExcuteToExcel ETE = null;
        public int ETE_ROWNUM = 1;
        public bool Started { get; set; }
        List<Queue<DateTimePoint>> dataCahce = new List<Queue<DateTimePoint>>();
        List<SeriseDatas> LineGraphDic { get; set; }
        List<List<DateTimePoint>> CacheData { get; set; } 
        public System.Windows.Forms.ToolTip ToolTip { get { return this.toolTip1; } }
        public WinNBTester.UILabelInfo[] ICS = null;
        public string Port { get { return this.COM.SelectedItem==null?null: this.COM.SelectedItem + ""; } }
        public UIConnection()
        {
            InitializeComponent();
            if (DesignMode) return;
            RefreshBtn();


        }

        ~UIConnection() {
            if(ETE!=null) ETE.Dispose();
            Saver.Dispose();
        }

        public void RefreshBtn() {
            //this.BtnStart.Enabled = !string.IsNullOrEmpty(this.Port);
            this.BtnStart.Text = this.Started ? "停止读取": "开始读取";
            this.BtnExport.Enabled = !this.Started;
            this.COM.Enabled = !this.Started;
        }
        public void SelectCOM(int index)
        {
            if(this.COM.Items.Count> index) this.COM.SelectedIndex = index;
        }

        DateTime StartTime = DateTime.Now;
        private void UIConnection_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            foreach (var item in SerialPort.GetPortNames())
            {
                this.COM.Items.Add(item); 
            }

            Saver = new Model4In1Tester(this);

            string AssrtString = "";
            ICS = new WinNBTester.UILabelInfo[] {
                  IC2, IC5
            };

            int xe = 0;
            foreach (var IC in ICS)
            {
                xe++;
                IC.Text = "-";
                IC.ForeColor = Color.Gray;
                IC.LabelColor = xe % 2 == 0 ? Color.Red : Color.Blue;
                ToolTip.SetToolTip(IC, string.Format("{0}", "未知"));
            }

            StartTime = DateTime.Now;
            chart = new PloyLineChart(this.elementHost1);
            chart.ShowLinePoint = true;
            chart.Zooming.OnHitTestResult += Zooming_OnHitTestResult;
            dataCahce = new List<Queue<DateTimePoint>>();
            this.chart.XLabelMin = 0;
            this.chart.XLabelMax = 300;
            this.chart.YLabelMin = -10;
            this.chart.YLabelMax = 300;
            PloyLineChart.XLabelMinDefault = this.chart.XLabelMin;
            PloyLineChart.XLabelMaxDefault = this.chart.XLabelMax;
            PloyLineChart.YLabelMinDefault = this.chart.YLabelMin;
            PloyLineChart.YLabelMaxDefault = this.chart.YLabelMax;
            LineGraphDic = new List<SeriseDatas>();
            CacheData = new List<List<DateTimePoint>>();

            ETE = new ExcuteToExcel(new FileInfo(Application.StartupPath + "\\" + this.Name.Reverse().FirstOrDefault() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx"), "历史数据");
            int hr = 1;
            ETE_ROWNUM = 1;
            ETE.SetCellValue<string>(hr, 1, "时间");
            ETE.SetColumnWidth(1, 21);
            for (int i = 0; i < Saver.PAR_V1.Length; i++)
            {
                var def = Saver.PAR_V1[i];
                string name = String.Format("{0}（{1}）", def[1], def[2]);
                ETE.SetCellValue<string>(hr, 2 + i, name);
                ETE.SetColumnWidth(2 + i, 21);
                var dataLit = new List<DateTimePoint>();
                var lg = new SeriseDatas(String.Format("{0}", name), dataLit.Select(w => (float)w.Time).ToList(), dataLit.Select(w => (float)w.Value).ToList());
                lg.Line1Color = new System.Drawing.SolidBrush(ICS[i].LabelColor);
                chart.AllDatas.Add(lg);
                LineGraphDic.Add(lg);
                CacheData.Add(dataLit);
                dataCahce.Add(new Queue<DateTimePoint>());
            }
            chart.Init((int)this.elementHost1.Width, (int)this.elementHost1.Height);//不能用 img ，img 尺寸获取不到


            RefreshBtn();
        }

        private void Zooming_OnHitTestResult(HitTester.HitTestResult result)
        {
             
        }

        public void ResetIC()
        {

            Invoke((Action)delegate
            {
                foreach (var IC in ICS)
                {
                    IC.SenV = null; 
                    ToolTip.SetToolTip(IC, string.Format("{0}", "未知"));
                }
                Application.DoEvents();
                RefreshBtn();

            });

        }

        public void Start()
        {
            if (this.Port == null) return;
            try
            {
               
                this.chart.AppendAll();
                if (Saver != null) Saver.Connect();
            }
            catch (Exception ex)
            {
                UpdateTextBox(string.Format("发送命令时出错 {0}", ex.Message));
                UpdateTextBox(ex.Message);
            }
            finally
            {
                Started = true; 
                RefreshBtn();
            }
        }
        public void Stop()
        {
            if (this.Port == null) MessageBox.Show("请选择串口！");
            if (this.Port == null) return;
            try
            { 
                if(Saver!=null) Saver.DisConnect();
            }
            catch (Exception ex)
            {
                UpdateTextBox(string.Format("发送命令时出错 {0}", ex.Message));
                UpdateTextBox(ex.Message);
            }
            finally
            {
                Started = false;
                RefreshBtn();
            }
        }

        public void UpdateTextBox(string msg)
        {
             
        }

        public string UpdateText
        {
            set
            {
                UpdateTextBox(value);
            }
        }


        public void UpdateTextBoxCommand(string msg)
        {
            this.Invoke((Action)delegate
            {
                this.labelV.Text = Saver.Protocal.ToString();
                //this.TextLogger.AppendText(System.Environment.NewLine);
                //this.TextLogger.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg)); 
            });
        }

        private void IC6_Load(object sender, EventArgs e)
        {

        }

        public bool SaveToDb(List<SenV> vs)
        {
            if (vs == null || ETE==null) return false;
            ETE_ROWNUM++;
            DateTime dt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ETE.SetCellValue<DateTime>(ETE_ROWNUM, 1, dt , "HH:mm:ss");
            for (int i = 0; i < vs.Count; i++)
            {
                ETE.SetCellValue<int>(ETE_ROWNUM, 2+i, vs[i].Value);
                var d = new DateTimePoint((dt - StartTime).TotalSeconds, vs[i].Value);
                dataCahce[i].Enqueue(d);
                CacheData[i].Add(d);
            }
            if (ETE_ROWNUM % 5 == 0)
            {
                ETE.Package.Save();
                Application.DoEvents();
            }
            RefreshChart();
            return true;
        }


        public void RefreshChart() {
            if (this.dataCahce.Count == 0) return;


            //PloyLineChart.XLabelMinDefault = 0;
            //PloyLineChart.XLabelMaxDefault = 300;
            //PloyLineChart.YLabelMinDefault = 0;
            //PloyLineChart.YLabelMaxDefault = 300;
            double max = (DateTime.Now-StartTime).TotalSeconds;
            if (max >= this.chart.XLabelMax)
            {
                this.chart.XLabelMax = (float)max + 300;
                PloyLineChart.XLabelMaxDefault = this.chart.XLabelMax;
                this.RefreshPage();
            }
                

            while (true)
            {

                if (this.dataCahce[0].Count == 0) break;

                float time = 0;
                List<float> list = new List<float>();
                for (int i = 0; i < Saver.PAR_V1.Length; i++)
                {
                    DateTimePoint data = this.dataCahce[i].Dequeue();
                    time = (float)data.Time;
                    list.Add((float)data.Value);
                }
                try
                {
                    elementHost1.Invoke((Action)delegate
                    {
                        chart.AppendData(time, list);
                        chart.Img.InvalidateVisual();
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                

            }
            
            
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            string name = DateTime.Now.ToString("传感器历史数据yyyy年MM月dd日HH_mm_ss");
            var dlg = new SaveFileDialog()
            {
                Title = "标定成果-另存为",
                DefaultExt = "xlsx",
                Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                FileName = name,
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() ==  DialogResult.OK) {
                ETE.FilePath = dlg.FileName;
                ETE.SaveAsExcel();
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (Started)
            {
                Stop();
            }
            else {
                Start();
            }
            
        }




        public void RefreshPage()
        {

            this.BeginInvoke(new Action(() =>
            {
                if (LineGraphDic == null) return;
                for (int pose = 0; pose < Saver.PAR_V1.Length; pose++)
                {
                    SeriseDatas line = LineGraphDic[pose];
                    var sel = CacheData[pose];
                    line.XData.Clear();
                    line.RtData.Clear();
                    line.XData.AddRange(sel.Select(w => (float)w.Time));
                    line.RtData.AddRange(sel.Select(w => (float)w.Value));
                } 
                this.chart.AppendAll();
            }));

        }

        private void AddYMin_Click(object sender, EventArgs e)
        {
            this.chart.YLabelMin += 50;
            PloyLineChart.YLabelMinDefault = this.chart.YLabelMin; 
            this.RefreshPage();
        }


        private void MinusYMin_Click(object sender, EventArgs e)
        {
            this.chart.YLabelMin -= 50;
            PloyLineChart.YLabelMinDefault = this.chart.YLabelMin;
            this.RefreshPage();
        }


        private void AddYMax_Click(object sender, EventArgs e)
        {
            this.chart.YLabelMax += 50;
            PloyLineChart.YLabelMaxDefault = this.chart.YLabelMax;
            this.RefreshPage();
        }


        private void MinusYMax_Click(object sender, EventArgs e)
        {
            this.chart.YLabelMax -= 50;
            PloyLineChart.YLabelMaxDefault = this.chart.YLabelMax;
            this.RefreshPage();
        }

        private void panel3_SizeChanged(object sender, EventArgs e)
        {
            if (chart == null) return;
            chart.Init((int)this.elementHost1.Width, (int)this.elementHost1.Height);//不能用 img ，img 尺寸获取不到
            this.RefreshPage();
        }

    }

    public class DateTimePoint
    {
        public DateTimePoint(double d1, double d2)
        {
            Time = d1; Value = d2;
        }
        public double Time { get; set; }
        public double Value { get; set; }
    }
}