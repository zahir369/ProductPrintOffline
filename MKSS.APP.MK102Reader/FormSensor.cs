using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.MK102Reader
{
    public partial class FormSensor : Form
    {
        int delay = 5000;
        Dictionary<SensorType, int> Records = new Dictionary<SensorType, int>();
        public CommandExtCache Commands { get; set; }
        public FormSensor()
        {
            InitializeComponent();
            this.treeView1.ExpandAll();
            Commands = new CommandExtCache();

            CbxSerialPorts_Click(null, null);

            if (CommandAbs.Sensors.Count == 0)
            {
                for (int i = 1; i <= 8; i++)
                {
                    CommandAbs.Sensors.Add(new SensorEntity() { Enum = (SensorType)i, Status = SensorStatus.未知, NongDu = "/" });
                }
            }
            this.dataGridView1.DataSource = CommandAbs.Sensors;

            CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.Status, 8));
            CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.ControllerStatus, 8));
            CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.NongDu, 8));

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void CbxSerialPorts_Click(object sender, EventArgs e)
        {
            if (CbxSerialPorts.Enabled)
            {
                this.CbxSerialPorts.Items.Clear();
                string[] portNames = SerialPort.GetPortNames();
                for (int i = 0; i < portNames.Length; i++)
                {
                    string item = portNames[i];
                    this.CbxSerialPorts.Items.Add(item);
                }
                this.CbxSerialPorts.SelectedIndex = this.CbxSerialPorts.Items.Count - 1;
            }
        }

        public bool Connected { get { return this.serialPort.IsOpen; } }
        public bool Reading { get { return CommandExecuter.Tasks.Count > 0; } }
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (Connected)
                {
                    this.timer1.Enabled = false;
                    this.serialPort.Close();
                }
                else
                {
                    if (this.CbxSerialPorts.SelectedItem == null)
                    {
                        Log("请选择串口");
                        return;
                    }
                    Records.Clear();
                    serialPort.PortName = this.CbxSerialPorts.SelectedItem + "";
                    this.serialPort.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
                    this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
                    this.serialPort.BaudRate = 9600;
                    this.serialPort.Parity = Parity.None;
                    this.serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
             

            if (Connected)
            {
                CommandExecuter.Start(this);
                CommandParser.Start(this);
                this.dataGridView1.DataSource = CommandAbs.Sensors;
                this.timer1.Interval = int.Parse(this.textBoxInterval.Text.Trim("ms".ToCharArray()));
                if (this.timer1.Interval < 1000) {
                    this.timer1.Interval = 1000;
                    this.textBoxInterval.Text = "1000ms";
                }
                this.timer1.Enabled = true;
            }
            else
            {

                CommandExecuter.Dispose();
            }

            RefreshBtnStatus();
        }


        int tempdataLen = 0;
        byte[] BufferData = new byte[1024];
        StringBuilder strBuider = new StringBuilder();
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                tempdataLen = serialPort.BytesToRead;
                serialPort.Read(BufferData, 0, tempdataLen);
                CommandParser.EnqueueTask(BufferData.Take(tempdataLen).ToList());
            }
            catch
            {

            }
             
        }


        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            if (this.serialPort.IsOpen)
            {
                StatusLabelTimeText = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.Status, 8));
                CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.ControllerStatus, 8));
                CommandExecuter.EnqueueTask(CommandAbs.Create(SensorType.Sensor1, CommandType.NongDu, 8));
            }
            else
            {
                this.Log("请打开串口");
                this.timer1.Enabled = false;
            }

        }

        public void RefreshBtnStatus()
        {
            if (Connected)
            { 
                BtnConnect.Text = "断开连接(&D)";
            }
            else
            { 
                BtnConnect.Text = "连接设备(&C)";
            }

            try
            {
                List<ControllerStatus> ss = CommandAbs.Of(CommandType.ControllerStatus).ToControllerStatus();
                StringBuilder strBuiderxx = new StringBuilder();
                for (int index = 0; index < ss.Count; index++)
                {
                    strBuiderxx.Append(string.Format("{0}", ss[index]));
                }
                StatusLabelTimeDescText = strBuiderxx.ToString();
            }
            catch (Exception)
            {
                 
            }
           
            this.dataGridView1.DataSource = CommandAbs.Sensors;
            this.dataGridView1.Invalidate();

        }

        private void BtnPause_ButtonClick(object sender, EventArgs e)
        {
            if (Reading)
            {
                CommandExecuter.Tasks.Clear();
            }
            else
            {
                foreach (CommandAbs item in this.Commands.Commands.Values)
                {
                    if (!item.Reponsed)
                    {
                        CommandExecuter.EnqueueTask(item);
                    }
                }
            }
            RefreshBtnStatus();
        }

        string RecordTimeShowBoxText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    RecordTimeShowBox.Text = value;
                }));
            }
        }
         

        string StatusLabelTimeDescText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    StatusLabelTimeDesc.Text = value;
                    StatusLabelTimeDesc.ForeColor = value == "传感器未失效" ? Color.Green : Color.Red;
                }));
            }
        }

        string StatusLabelTimeText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    StatusLabelTime.Text = value;
                }));
            }
        }
        DateTime DeviceTimeValue { get; set; }
        DateTime DeviceOverdueTimeValue { get; set; }

        void RedarNode(TreeNode node)
        {
            string tag = node.Tag + "";
            SensorType t = SensorType.None;
            Enum.TryParse<SensorType>(tag, out t);
            string cnName = CommandAbs.SensorTypeString(t);
            if (t == SensorType.None) cnName = "全部";
            List<CommandAbs> list = this.Commands.OfList(t);
            node.Text = string.Format("{0}【{1}】", cnName, list.Count(w => w.Reponsed));
        }

        public void Log(string msg)
        {
            try
            {
                this.BeginInvoke((EventHandler)(delegate {
                    try
                    {
                        textBoxLogger.AppendText(System.Environment.NewLine);
                        textBoxLogger.AppendText(System.DateTime.Now.ToString("HH:mm:ss:fff") + ":" + msg);
                    }
                    catch (Exception)
                    {

                    }
                }));
            }
            catch (Exception)
            {

            }


        }

        public void ExeuteCommand(CommandAbs data)
        {

            try
            {

                if (data.Command == CommandType.Status)
                {
                    /**
                        1.读取1号控制器的1号探测器状态：
                        发→◇01 03 00 01 00 08 15 CC 
                        收←◆01 03 10 00 0A 00 01 00 01 00 01 00 01 00 01 00 01 00 01 D8 B3
                     ***/
                    string command = "01 03 00 01 00 08 15 CC";
                    byte[] bytes = HexStrToByteArray(command);
                    Log("TX:" + command);
                    this.serialPort.Write(bytes, 0, bytes.Length);
                    return;
                }

                if (data.Command == CommandType.NongDu)
                {
                    /**
                        2.读取1号控制器的1号探测器数值：
                        发→◇01 04 00 01 00 08 A0 0C 
                        收←◆01 04 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 2C
                        注：bit15表示指数的符号位；bit14~13表示指数位数；bit12~0表示数值。
                        如：0xA019，表示25 * 10^（-1）= 2.5
                     ***/
                    string command = "01 04 00 01 00 08 A0 0C";
                    byte[] bytes = HexStrToByteArray(command);
                    Log("TX:" + command);
                    this.serialPort.Write(bytes, 0, bytes.Length);
                    return;
                }


                if (data.Command == CommandType.ControllerStatus)
                {

                    /**
                        3.读取1号控制器的状态
                        发→◇01 02 00 01 00 04 28 09 □
                        收←◆01 02 01 00 A1 88
                        bit0就是代表备电，bit1代表主电
                     ***/
                    string command = "01 02 00 01 00 04 28 09";
                    byte[] bytes = HexStrToByteArray(command);
                    Log("TX:" + command);
                    this.serialPort.Write(bytes, 0, bytes.Length);
                    return;

                }

            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);
            }

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


        private byte Get_SumCheck(byte[] Pdata, byte longth)
        {
            byte i = 0;
            byte sum = 0;
            for (i = 0; i < longth; i++)
                sum += Pdata[i];
            return sum;
        }

        private void FormSensor_FormClosing(object sender, FormClosingEventArgs e)
        {
            CommandExecuter.EnqueueTask(null);
            CommandExecuter.EnqueueTask(null);
            CommandExecuter.Dispose();
            CommandParser.EnqueueTask(null);
            CommandParser.EnqueueTask(null);
            CommandParser.Dispose();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell = null;
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("没有数据无法保存", "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {

                string name = this.treeView1.SelectedNode.Text;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.CreatePrompt = true;
                saveFileDialog.FileName = name + ".csv";
                saveFileDialog.Title = "导出CSV文件";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stream stream = saveFileDialog.OpenFile();
                    StreamWriter streamWriter = new StreamWriter(stream, Encoding.GetEncoding(0));
                    string text = "";
                    try
                    {
                        for (int i = 0; i < this.dataGridView1.ColumnCount; i++)
                        {
                            if (i > 0)
                            {
                                text += ",";
                            }
                            text += this.dataGridView1.Columns[i].HeaderText;
                        }
                        text.Remove(text.Length - 1);
                        streamWriter.WriteLine(text); 
                        for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                        {
                            text = "";
                            for (int k = 0; k < this.dataGridView1.Columns.Count; k++)
                            {
                                if (k > 0)
                                {
                                    text += ",";
                                }
                                if (this.dataGridView1.Rows[j].Cells[k].Value == null)
                                {
                                    text = (text ?? "");
                                }
                                else
                                {
                                    string text2 = this.dataGridView1.Rows[j].Cells[k].Value.ToString().Trim();
                                    text += text2.Replace(",", "，");
                                }
                            }
                            text.Remove(text.Length - 1);
                            streamWriter.WriteLine(text); 
                        }
                        streamWriter.Close();
                        stream.Close();
                        MessageBox.Show("文件成功保存到：" + saveFileDialog.FileName.ToString(), "数据导出成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                         
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "数据导出错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }

        private void StatusLabelTime_DoubleClick(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
        }

        private void FormSensor_Load(object sender, EventArgs e)
        {

        }


    }

}