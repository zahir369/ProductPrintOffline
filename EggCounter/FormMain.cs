using MKSS.Service.UIBiaoDing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EggCounter
{
    public partial class FormMain : Form
    {

        public static FormMain Instance = null;
        EggSaver _EggSaver = new EggSaver();
        bool Connected { get { return this._serial.IsOpen; } }

        public FormMain()
        {
            InitializeComponent();
            Instance = this;
            this.dataGridView1.AutoGenerateColumns = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item == ConfigurationManager.AppSettings["DefaultCOM"])
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }
            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count > 0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.TxtSerialPorts.SelectedItem != null)
            {
                this._serial.PortName = this.TxtSerialPorts.SelectedItem + "";
                this._serial.BaudRate = 9600;
                this._serial.DataBits = 8;
                this._serial.Parity = Parity.None;
                this._serial.StopBits = StopBits.One;
                try
                {
                    this._serial.Open();
                    Log("" + this._serial.PortName + "已打开");
                    this.timer1.Enabled = true;

                    //01 03 00 00 00 08 44 0C
                    EggSaver.Start( );
                }
                catch (Exception ex)
                {
                    Log("打开" + this._serial.PortName + "失败：" + ex.Message);
                }
            }
            else {
                Log("请选择通信串口");
            }
        }

        DateTime lasttry = DateTime.Now;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Connected)
            {
                string command = "01 03 00 04 00 10 05 C7";
                byte[] bytes = HexStrToByteArray(command);
                Log("TX:"+ command);
                this._serial.Write(bytes, 0, bytes.Length);
            }
            else {
                if (DateTime.Now - lasttry > new TimeSpan(0, 0, 5)) {
                    
                }
            }
            this.Invoke((EventHandler)delegate
            {
                LabelCount1.Text = EggSaver.Floors[1].EggCountCurrent.ToString();
                LabelCount3.Text = EggSaver.Floors[3].EggCountCurrent.ToString();
                LabelCount5.Text = EggSaver.Floors[5].EggCountCurrent.ToString();
                //LabelCount4.Text = EggSaver.Floors[4].EggCountCurrent.ToString();
                //LabelCount5.Text = EggSaver.Floors[5].EggCountCurrent.ToString();
                //LabelCount6.Text = EggSaver.Floors[6].EggCountCurrent.ToString();
                //LabelCount7.Text = EggSaver.Floors[7].EggCountCurrent.ToString();
                //LabelCount8.Text = EggSaver.Floors[8].EggCountCurrent.ToString();
                BindingList<EggLayed> xx = new BindingList<EggLayed>();
                ;
                foreach (var floor in EggSaver.Floors.Values)
                {
                    foreach (var item in floor.Layers.Values)
                    {
                        xx.Insert(0, item);
                    }
                }

                Dictionary<int, List<EggLayed>> CaseLayed = new Dictionary<int, List<EggLayed>>();
                foreach (var floor in EggSaver.Floors.Values)
                {
                    foreach (var item in floor.Layers.Values)
                    {
                        if (!CaseLayed.ContainsKey(item.CaseNo)) {
                            CaseLayed.Add(item.CaseNo,new List<EggLayed>());
                        }
                        CaseLayed[item.CaseNo].Add(item);
                    }
                }


                BindingList<CaseLayed> xx2 = new BindingList<CaseLayed>();
                foreach (var key in CaseLayed.Keys)
                {
                    CaseLayed c = new CaseLayed()
                    {
                        Xh = key,
                        Count = CaseLayed[key].Count(),
                        TimeFrom = CaseLayed[key].Min(w => w.Dt),
                        TimeTo = CaseLayed[key].Max(w => w.Dt)
                    };
                    xx2.Add(c);
                }
                 

                dataGridView1.DataBindings.Clear();
                dataGridView1.DataSource = xx;
                dataGridView1.Refresh();
                dataGridView2.DataBindings.Clear();
                dataGridView2.DataSource = xx2;
                dataGridView2.Refresh();

            });

        }

        private void BtnDisConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this._serial.Close();
                Log("" + this._serial.PortName + "已关闭");
            }
            catch (Exception ex)
            {
                Log("关闭" + this._serial.PortName + "失败：" + ex.Message);
            }
            this.timer1.Enabled = false;
            EggSaver.Dispose( );
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int count = _serial.BytesToRead;
            if (count <= 0)  return;
            byte[] buffer = new byte[count];
            _serial.Read(buffer, 0, count);

            StringBuilder strBuider = new StringBuilder();
            for (int index = 0; index < count; index++)
            {
                strBuider.Append(((int)buffer[index]).ToString("X2"));
            } 
            Log("RX:" + strBuider);
            EggSaver.EnqueueTask(buffer.ToList());

        }

        public void Log(string msg) {
            this.Invoke((EventHandler)delegate
            {
                this.textBox1.AppendText(string.Format("{1}:{0}", msg,DateTime.Now.ToLongTimeString()));
                this.textBox1.AppendText(System.Environment.NewLine);
            });
        }

        private byte[] HexStrToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._serial.IsOpen) {
                MessageBox.Show("请先停止");
                e.Cancel = true;
            }
        }

    }
}
