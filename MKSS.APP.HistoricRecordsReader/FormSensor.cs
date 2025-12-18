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

namespace MKSS.APP.HistoricRecordsReader
{
    public partial class FormSensor : Form
    {
        int delay = 5000;
        Dictionary<CommandType, int> Records = new Dictionary<CommandType, int>();
        public CommandExtCache Commands { get; set; }
        public FormSensor()
        {
            InitializeComponent();
            this.treeView1.ExpandAll();
            Commands = new CommandExtCache();
            CbxSerialPorts_Click(null,null);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void CbxSerialPorts_Click(object sender, EventArgs e)
        {
            if (CbxSerialPorts.Enabled) {
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
            if (Connected)
            {
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
                this.serialPort.BaudRate = 4800;
                this.serialPort.Parity = Parity.Even;
                this.serialPort.Open();
            }

            this.toolStripProgressBar1.Minimum = 0;
            this.toolStripProgressBar1.Maximum = 100;
            this.toolStripProgressBar1.Value = 0;

            if (Connected)
            {
                Commands.Clear();
                Commands = new CommandExtCache();
                //this.toolStripProgressBar1.Visible = true;
                CommandExecuter.Start(this, int.Parse(this.textBoxInterval.Text.TrimEnd("ms".ToCharArray())));
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.Overdue, Par1 = 0X01 });
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.DateTimeNow, Par1 = CommandExt.Par1Default });
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.NetData, Par1 = CommandExt.Par1Default });
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.SerialNo, Par1 = CommandExt.Par1Default });
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.ZeroSpanNongDu, Par1 = CommandExt.Par1Default });
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.RecordCount, Par1 = CommandExt.Par1Default });
                //CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.NetData, Par1 = 0XFF });
                //CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.CurrentRecordCount, Par1 = 0XFF }); 
                this.dataGridView1.DataSource = this.Commands.OfList( CommandType.None);
            }
            else
            {

                CommandExecuter.Dispose( );
            }

            RefreshBtnStatus();
        }

        public void RefreshBtnStatus()
        {
            if (Connected)
            {
                this.BtnPause.Enabled = true;
                BtnConnect.Text = "断开连接(&D)";
            }
            else
            {
                this.BtnPause.Enabled = false;
                BtnConnect.Text = "连接设备(&C)";
            }
            if (Reading)
            {
                this.BtnPause.Image = global::MKSS.APP.HistoricRecordsReader.Properties.Resources.pause;
                this.BtnPause.ToolTipText = "点击后暂停读取";
            }
            else {
                this.BtnPause.Image = global::MKSS.APP.HistoricRecordsReader.Properties.Resources.forward;
                this.BtnPause.ToolTipText = "点击后继续读取";
            }
        }

        private void BtnPause_ButtonClick(object sender, EventArgs e)
        {
            if (Reading)
            {
                CommandExecuter.Tasks.Clear();
            }
            else
            {
                foreach (CommandExt item in this.Commands.Commands.Values)
                {
                    if (!item.Reponsed)
                    {
                        CommandExecuter.EnqueueTask(item);
                    }
                } 
            }
            RefreshBtnStatus();
        }



        void ReadData()
        {
            Task<bool> sendEmailMessage = Task.Run(() => {
                try
                {
                    ExeuteCommand(CommandType.RecordCount, -1);
                    Task.Delay(delay);
                    ExeuteCommand(CommandType.DateTimeNow, -1);
                    Task.Delay(delay);
                    ExeuteCommand(CommandType.Overdue, -1);
                    Task.Delay(delay);
                    return true;
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    return false;
                }

            });
        }


        void ReadData(CommandType comm,int count)
        {
            Task<bool> sendEmailMessage = Task.Run(() => {
                try
                {
                    ExeuteCommand(comm, count);
                    Task.Delay(delay);
                    return true;
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    return false;
                }
            });
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


        void ZeroSpanNongDuText(string v1, string v2, string v3)
        {
            this.BeginInvoke((EventHandler)(delegate {
                labelZero.Text = v1;
                labelSpan.Text = v2;
                labelNongdu.Text = v3;
            }));
        }

        string StatusLabelGGDescText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    StatusLabelGGDesc.Text = value;
                    labelStatusLabelGGDesc.Text = value;
                }));
            }
        }

        bool StatusLabelGGDescError
        {
            set
            {
                this.BeginInvoke((EventHandler)(delegate
                {
                    StatusLabelGGDesc.ForeColor = !value ? Color.Black : Color.Red;
                    labelStatusLabelGGDesc.ForeColor = !value ? Color.Black : Color.Red;
                }));
            }
        }

        string StatusLabelSerialNoText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    StatusLabelSerialNo.Text = value;
                    labelStatusLabelSerialNo.Text = value;
                }));
            }
        }

        

        string RecordNumberShowBoxText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    RecordNumberShowBox.Text = value;
                }));
            }
        }



        public bool ProgressBarVisible
        {
            set
            { 
                this.BeginInvoke((EventHandler)(delegate {
                    this.toolStripProgressBar1.Visible = value;
                }));
            }
        }

        string ConfigStateLableText
        {
            set
            {
                Log(value);
                this.BeginInvoke((EventHandler)(delegate {
                    ConfigStateLable.Text = value;
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
                    labelStatusLabelTimeDesc.Text = value;
                    labelStatusLabelTimeDesc.ForeColor = value == "传感器未失效" ? Color.Green : Color.Red;
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
                    labelStatusLabelTime.Text = value;
                }));
            }
        }
        DateTime DeviceTimeValue { get; set; }
        DateTime DeviceOverdueTimeValue { get; set; }



        byte[] tempdata = new byte[1024];
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int readd = 0;
            try
            {
                readd = serialPort.Read(tempdata, 0, serialPort.BytesToRead);
            }
            catch
            {

            }
            ParseBytes(readd);
        }


        void ParseBytes(int readd)
        {
            // byte data=0;
            byte MonthData, DayData, HourData, MinuteData;
            short YearData;
            CommandExt response = null;
            short Concentration;
            if (tempdata[0] == 0xAA)
            {
                switch (tempdata[2])
                {
                    case 0X07:   // 失效记录  AA 00 07 07 01 07 EA 0A 07  10 2A  F5 55
                        {
                            response = this.Commands.Of(CommandType.Overdue, 1);
                            if ((tempdata[12] == 0x55) && (tempdata[3] == 0x07) && (tempdata[4] == 0x00) && (tempdata[5] == 0x00)) //  传感器未失效
                            {
                                // MessageBox.Show("传感器未失效");
                                StatusLabelTimeDescText = "传感器未失效";
                                response.Result = CommandExt.ResultEmpString;
                            }
                            else
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                DeviceOverdueTimeValue = ToDateTimeSrc(YearData, MonthData, DayData, HourData, MinuteData, 0);
                                // MessageBox.Show("传感器失效");
                                StatusLabelTimeDescText = "传感器失效" + Environment.NewLine +
                                                         "失效时间：" + YearData.ToString() + "年"
                                                         + MonthData.ToString() + "月"
                                                         + DayData.ToString() + "日";
                                response.Result = DeviceOverdueTimeValue.ToString();
                            }


                        }
                        break;
                    case 0X08:   // 报警器当前时间
                        {
                            response = this.Commands.Of(CommandType.DateTimeNow, CommandExt.Par1Default);
                            if ((tempdata[3] == 0x06) && (tempdata[11] == 0x55))
                            {
                                YearData = (short)((tempdata[4] << 8) + tempdata[5]);
                                MonthData = tempdata[6];
                                DayData = tempdata[7];
                                HourData = tempdata[8];
                                MinuteData = tempdata[9];
                                DeviceTimeValue = ToDateTimeSrc(YearData, MonthData, DayData, HourData, MinuteData, 0);
                                StatusLabelTimeText = DeviceTimeValue.ToString("yyyy年MM月dd日 HH:mm");
                                response.Result = DeviceTimeValue.ToString();
                            }
                            else
                            {
                                // MessageBox.Show("请重新读取");
                                RecordTimeShowBoxText = "请重新读取";
                                response.Result = CommandExt.ResultEmpString;
                            }

                        }
                        break;
                    case 0X00:   // 记录总数
                        {

                            response = this.Commands.Of(CommandType.RecordCount, CommandExt.Par1Default);
                            if ((tempdata[12] == 0x55) && (tempdata[3] == 0x07))
                            {
                                RecordTimeShowBoxText = "报警记录数：    " + tempdata[4].ToString() + "条，"   +
                                                         "报警恢复记录数：" + tempdata[5].ToString() + "条，"   +
                                                        "故障记录数：    " + tempdata[6].ToString() + "条，"  +
                                                        "故障恢复记录数：" + tempdata[7].ToString() + "条，"  +
                                                        "掉电记录数：    " + tempdata[8].ToString() + "条，"  +
                                                        "上电记录数：    " + tempdata[9].ToString() + "条，"  +
                                                        "失效记录数：    " + tempdata[10].ToString() + "条；";
                                this.BeginInvoke((EventHandler)(delegate {
                                    Application.DoEvents();
                                }));
                                Records.Add(CommandType.Overdue, tempdata[10]);
                                Records.Add(CommandType.Warn, tempdata[4]);
                                Records.Add(CommandType.WarnRecover, tempdata[5]);
                                Records.Add(CommandType.Error, tempdata[6]);
                                Records.Add(CommandType.ErrorRecover, tempdata[7]);
                                Records.Add(CommandType.PowerOff, tempdata[8]);
                                Records.Add(CommandType.PowerOffRecover, tempdata[9]);

                                this.BeginInvoke((EventHandler)(delegate {
                                    this.toolStripProgressBar1.Minimum = 0;
                                    this.toolStripProgressBar1.Maximum = Records.Sum(w=>w.Value)+5;
                                    this.toolStripProgressBar1.Value = 0;
                                }));

                                for (int i = 0; i < 200; i++)
                                {
                                    foreach (var item in Records.Keys)
                                    {
                                        if (Records[item] > i) CommandExecuter.EnqueueTask(new CommandExt() { CommandType = item, Par1 = i + 1 });
                                    }
                                }
                                

                            }
                            response.Result = CommandExt.ResultEmpString;

                        }
                        break;
                    case 0X01:   // 报警器0-录
                        {
                            response = this.Commands.Of(CommandType.Warn, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();

                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();

                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;
                            }
                            else
                            {
                                //MessageBox.Show("读取数超过实际存储数");
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;
                    case 0X02:   // 报警器报警恢复记录
                        {
                            response = this.Commands.Of(CommandType.WarnRecover, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();

                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();

                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;

                            }
                            else
                            {
                                // MessageBox.Show("请重新读取");
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;
                    case 0X03:   // 报警器故障记录
                        {
                            response = this.Commands.Of(CommandType.Error, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();

                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();

                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;

                            }
                            else
                            {
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;
                    case 0X04:   // 报警器故障恢复记录
                        {
                            response = this.Commands.Of(CommandType.ErrorRecover, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();
                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();
                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;

                            }
                            else
                            {
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;
                    case 0X05:   // 报警器掉电记录
                        {
                            response = this.Commands.Of(CommandType.PowerOff, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();

                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();

                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;

                            }
                            else
                            {
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;
                    case 0X06:   // 报警器上电记录
                        {
                            response = this.Commands.Of(CommandType.PowerOffRecover, tempdata[4]);
                            if ((tempdata[3] == 0x07) && (tempdata[12] == 0x55) && (tempdata[5] != 0x00) && (tempdata[7] != 0x00))
                            {
                                YearData = (short)((tempdata[5] << 8) + tempdata[6]);
                                MonthData = tempdata[7];
                                DayData = tempdata[8];
                                HourData = tempdata[9];
                                MinuteData = tempdata[10];
                                if (MinuteData >= 10)
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + MinuteData.ToString();

                                }
                                else
                                {
                                    RecordTimeShowBoxText = YearData.ToString() + "年" + MonthData.ToString() + "月" + DayData.ToString() + "日" + HourData.ToString() + ":" + "0" + MinuteData.ToString();

                                }
                                response.Result = (ToDateTime(YearData, MonthData, DayData, HourData, MinuteData, 0)) ;

                            }
                            else
                            {
                                //  RecordTimeShowBoxText = "读取数超过实际存储数";
                                RecordTimeShowBoxText = "无此记录";
                                response.Result = CommandExt.ResultEmpString;
                            } 
                        }
                        break;

                    default:
                        break;
                }
            }
            switch (tempdata[0])
            {
                case 0xAB:
                    {
                        if ((tempdata[1] == 0x01) && (tempdata[2] != 0x00))  // 设置成功
                        {
                            //this.ConfigStateLable.BackColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                            this.BeginInvoke((EventHandler)(delegate {
                                this.ConfigStateLable.ForeColor = Color.Green;
                                ConfigStateLableText = "成功";
                            }));

                        }
                        else
                        {
                            // this.ConfigStateLable.BackColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                    


                            this.BeginInvoke((EventHandler)(delegate {
                                this.ConfigStateLable.ForeColor = Color.Red;
                                ConfigStateLableText = "失败";
                            }));


                        }


                    }
                    break;
                case 0xAC:
                    {
                        if (tempdata[1] == 0x01)  // 设置成功
                        {
                            //this.ConfigStateLable.BackColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                            //this.ConfigStateLable.BackColor = Color.Green;
       
                            this.BeginInvoke((EventHandler)(delegate {
                                this.ConfigStateLable.ForeColor = Color.Green;
                                ConfigStateLableText = "成功";
                            }));

                        }
                        else
                        {
                            // this.ConfigStateLable.BackColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                            // this.ConfigStateLable.BackColor = Color.Red;
         
                            this.BeginInvoke((EventHandler)(delegate {
                                this.ConfigStateLable.ForeColor = Color.Red;
                                ConfigStateLableText = "失败";
                            }));
                        }
                    }
                    break;
                case 0xAD:
                    {
                        response = this.Commands.Of(CommandType.SerialNo, CommandExt.Par1Default);
                        byte[] bs = new byte[8];
                        Array.Copy(tempdata,2, bs, 2, 6);
                        StringBuilder strBuider = new StringBuilder();
                        for (int index = 0; index < bs.Length; index++)
                        {
                            strBuider.Append(((int)bs[index]).ToString("X2")); 
                        }
                        ulong sno = UInt64.Parse(strBuider.ToString(), System.Globalization.NumberStyles.HexNumber);
                        this.SerialNo = sno;
                        response.Result = this.SerialNo.ToString();
                        this.BeginInvoke((EventHandler)(delegate {
                            StatusLabelSerialNoText = this.SerialNo.ToString();
                        }));
                    }
                    break;
                case 0xAE:
                    {
                        if (tempdata[3] == 0x04)
                        {
                            response = this.Commands.Of(CommandType.NetData, CommandExt.Par1Default);
                            bool error = false;
                            string desc = "";
                            Concentration = (short)((tempdata[6] << 8) + tempdata[7]);
                            if ((tempdata[5] & 0x04) == 0x04)   //寿命到期报警
                            {
                                desc = "报警器状态：寿命失效";
                                error = true;
                            }
                            else if ((tempdata[5] & 0x02) == 0x02)//故障报警
                            {
                                desc = "报警器状态：故障";
                                error = true;

                            }
                            else if ((tempdata[5] & 0x08) == 0x08)//累积浓度报警
                            {
                                desc = "报警器状态：累积浓度报警，浓度值：" + Concentration.ToString();
                                error = true;

                            }
                            else if ((tempdata[5] & 0x01) == 0x01)//浓度报警
                            {
                                desc = "报警器状态：浓度报警，浓度值：" + Concentration.ToString() + "%LEL";
                                error = true;


                            }
                            else
                            {
                                desc = "报警器状态：正常，浓度值：" + Concentration.ToString() + "%LEL";

                            }
                            if (readd >= 13) {
                                //双气
                                desc = desc + "，";
                                Concentration = (short)((tempdata[10] << 8) + tempdata[11]);
                                if ((tempdata[9] & 0x04) == 0x04)   //寿命到期报警
                                {
                                    desc = desc + "第二气状态：寿命失效";
                                    error = true;

                                }
                                else if ((tempdata[9] & 0x02) == 0x02)//故障报警
                                {
                                    desc = desc + "第二气状态：故障";
                                    error = true;

                                }
                                else if ((tempdata[9] & 0x08) == 0x08)//累积浓度报警
                                {
                                    desc = desc + "第二气状态：累积浓度报警 浓度：" + Concentration.ToString();
                                    error = true;
                                }
                                else if ((tempdata[9] & 0x01) == 0x01)//浓度报警
                                {
                                    desc = desc + "第二气状态：浓度报警 浓度：" + Concentration.ToString() + "%LEL";
                                    error = true;
                                }
                                else
                                {
                                    desc = desc + "第二气状态：正常，浓度值：" + Concentration.ToString() + "%LEL";
                                }
                            }
                            response.Result = desc;
                            StatusLabelGGDescText = desc;
                            StatusLabelGGDescError = error;
                        }
                    }
                    break;




                case 0xAF: //AF 01 00 07 00 00 00 00 01 03 00 BB 55 
                    {
                        response = this.Commands.Of(CommandType.RecordCount, CommandExt.Par1Default);
                        if ((tempdata[1] == 0x01) && (tempdata[2] == 0X00) && (tempdata[14] == 0x55))  // 设置成功
                        {
                          
                            string str = string.Format(
                                "报警/恢复记录{0}/{1}条，故障/恢复记录{2}/{3}条，掉电/上电记录{4}/{5}条，温度{6}℃。",
                                tempdata[4].ToString(),
                                tempdata[5].ToString(),
                                tempdata[6].ToString(),
                                tempdata[7].ToString(),
                                tempdata[8].ToString(),
                                tempdata[9].ToString(),
                                tempdata[12].ToString()
                                ); ;
                            RecordNumberShowBoxText = str;
                            RecordTimeShowBoxText = "当前记录数读取成功";
                            response.Result = str;

                        }
                        else
                        {
                            RecordNumberShowBoxText = "请重新读取";
                            RecordTimeShowBoxText = "当前记录数读取失败";
                            response.Result = "请重新读取";

                        }

                    }
                    break;
                case 0xFF: //AF 01 00 07 00 00 00 00 01 03 00 BB 55 
                    {
                        //0x89 - 读取零点AD、标定点AD、标定点浓度点命令
                        //发送命令
                        //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                        //起始字节    预留 命令  - - -校验值
                        //0xFF    0x01    0x89    0x00    0x00    0x00    0x00    0x00    校验和
                        //返回值
                        //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                        //起始字节    命令 - - - - -校验值
                        //0xFF    0x89    零点AD高位 零点AD低位  标定点AD高位 标定点AD低位 标定浓度点高位 标定浓度点低位 校验和
                        response = this.Commands.Of(CommandType.RecordCount, CommandExt.Par1Default);
                        if (tempdata.Length>=10 && (tempdata[1] == 0x89) && (tempdata[9] == 0x55)) 
                        {

                            ushort zero = BitConverter.ToUInt16(new byte[] { tempdata[3] , tempdata[2] },0);
                            ushort span = BitConverter.ToUInt16(new byte[] { tempdata[5], tempdata[4] }, 0);
                            ushort nogdu = BitConverter.ToUInt16(new byte[] { tempdata[7], tempdata[6] }, 0);
                            ZeroSpanNongDuText(zero + "", span + "", nogdu + "");
                            string str = string.Format(
                                "零点AD{0}，标定点AD{1}，标定浓度{2}。",
                                zero, span, nogdu );
                            Log("读取零点AD、标定点AD、标定点浓度点读取成功");
                            StatusLabelGGDescError = false;
                            response.Result = str;

                        }
                        else
                        {
                            

                        }

                    }
                    break;
                default:
                    break;
            }


            this.BeginInvoke((EventHandler)(delegate {
                if(this.Commands.ReponsedCount<= this.toolStripProgressBar1.Maximum) this.toolStripProgressBar1.Value = this.Commands.ReponsedCount;
                if(this.toolStripProgressBar1.Value%10==0) this.dataGridView1.Refresh();
                RedarNode(this.treeView1.Nodes[0]);
                foreach (var item in this.treeView1.Nodes[0].Nodes)
                {
                    RedarNode(item as TreeNode);
                }
            }));
            this.BeginInvoke((EventHandler)(delegate { }));

        }

        DateTime ToDateTimeSrc(int YearData, int MonthData, int DayData, int HourData, int MinuteData, int sec)
        {
            try
            {
                DateTime dt = new DateTime(YearData, MonthData, DayData, HourData, MinuteData, sec);
                return dt ;
            }
            catch (Exception ex)
            {
                Log(string.Format("时间格式错误：{0}-{1}-{2} {3}:{4}:{5}", YearData, MonthData, DayData, HourData, MinuteData, 0));
                return DateTime.MinValue;
            }

        }
        string ToDateTime(int YearData, int MonthData, int DayData, int HourData, int MinuteData,int sec) {
            try
            {
                DateTime dt = new DateTime(YearData, MonthData, DayData, HourData, MinuteData, sec);
                return dt.ToString();
            }
            catch (Exception ex)
            {
                Log(string.Format("时间格式错误：{0}-{1}-{2} {3}:{4}:{5}", YearData, MonthData, DayData, HourData, MinuteData, 0));
                return string.Format("{0}-{1}-{2} {3}:{4}:{5}", YearData, MonthData, DayData, HourData, MinuteData, 0);
            }
           
        }

        void RedarNode(TreeNode node) {
            string tag = node.Tag + "";
            CommandType t = CommandType.None;
            Enum.TryParse<CommandType>(tag, out t);
            string cnName = CommandExt.CommandTypeString(t);
            if (t == CommandType.None) cnName = "全部";
            List < CommandExt >  list = this.Commands.OfList(t);
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
                        textBoxLogger.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + ":" + msg);
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

        public void ExeuteCommand(CommandType Command, int recordNo)
        {
            byte[] buffer = new byte[8];
            RecordTimeShowBoxText = " ";
            byte tem1;
            // Int32 tem1;
            // tem1= (byte)Convert.ToInt32(RecordNumbersBox.Text, 16); 
            tem1 = (byte)Convert.ToByte(recordNo);
            if (tem1 == 0)
            {
                MessageBox.Show("记录条数不能为零", "提示");
            }
            else
            {
                try
                {
                    switch (Command)
                    {

                        case CommandType.Warn:// "报警记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;

                                //  buffer[1] = (byte)bcd2hex(tem1);
                                buffer[1] = tem1;
                                buffer[2] = 0x01;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.WarnRecover://"报警恢复记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = tem1;
                                buffer[2] = 0x02;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.Error://"故障记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = tem1;
                                buffer[2] = 0x03;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.ErrorRecover://"故障恢复记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = tem1;
                                buffer[2] = 0x04;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.PowerOff://"掉电记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = tem1;
                                buffer[2] = 0x05;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.PowerOffRecover://"上电记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = tem1;
                                buffer[2] = 0x06;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }

                            break;
                        case CommandType.Overdue://"传感器失效记录":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = 0x00;
                                buffer[2] = 0x07;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.NetData://"联网数据":   //AA 01 01 07 B3 55
                            {
                                buffer[0] = 0xAE;
                                buffer[1] = 0X00;
                                buffer[2] = 0x00;
                                buffer[3] = 0x00;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.SerialNo://"序列号":   //AD 01 55
                            {
                                buffer[0] = 0xAD;
                                buffer[1] = 0X01;
                                buffer[2] = Get_SumCheck(buffer, 4);
                                buffer[3] = 0x55;
                            }
                            break;
                        case CommandType.SerialNoWrite://"写入序列号":   //AD 00 11 11 11 11 11 11 55
                            {
                                byte[] bn = BitConverter.GetBytes(SerialNo);
                                buffer[0] = bn.Length > 7 ? bn[7] : (byte)0x00;
                                buffer[1] = bn.Length > 6 ? bn[6] : (byte)0x00;
                                buffer[2] = bn.Length > 5 ? bn[5] : (byte)0x00;
                                buffer[3] = bn.Length > 4 ? bn[4] : (byte)0x00;
                                buffer[4] = bn.Length > 3 ? bn[3] : (byte)0x00;
                                buffer[5] = bn.Length > 2 ? bn[2] : (byte)0x00;
                                buffer[6] = bn.Length > 1 ? bn[1] : (byte)0x00;
                                buffer[7] = bn.Length > 0 ? bn[0] : (byte)0x00;
                                buffer[8] = Get_SumCheck(buffer, 4);
                                buffer[9] = 0x55;
                            }
                            break;
                        case CommandType.RecordCount://"记录总数":   //AA 00 00 07 B1 55
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = 0X00;
                                buffer[2] = 0x00;
                                buffer[3] = 0x07;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.CurrentRecordCount://"当前记录数":   //AF 01 00 00 B0 55
                            {
                                buffer[0] = 0xAF;
                                buffer[1] = 0X01;
                                buffer[2] = 0x00;
                                buffer[3] = 0x00;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.DateTimeNow://"报警器当前时间":   //AA 00 08 06 B8 55 
                            {
                                buffer[0] = 0xAA;
                                buffer[1] = 0X00;
                                buffer[2] = 0x08;
                                buffer[3] = 0x06;
                                buffer[4] = Get_SumCheck(buffer, 4);
                                buffer[5] = 0x55;
                            }
                            break;
                        case CommandType.ZeroSpanNongDu://读取零点AD、标定点AD、标定点浓度点命令
                            { 
                                buffer = new byte[10];
                                //0x89 - 读取零点AD、标定点AD、标定点浓度点命令
                                //发送命令
                                //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                                //起始字节    预留 命令  - - -校验值
                                //0xFF    0x01    0x89    0x00    0x00    0x00    0x00    0x00    校验和
                                //返回值
                                //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                                //起始字节    命令 - - - - -校验值
                                //0xFF    0x89    零点AD高位 零点AD低位  标定点AD高位 标定点AD低位 标定浓度点高位 标定浓度点低位 校验和
                                {
                                    buffer[0] = (byte)0xFF;
                                    buffer[1] = (byte)0x01;
                                    buffer[2] = (byte)0x89;
                                    buffer[3] = (byte)0x00;
                                    buffer[4] = (byte)0x00;
                                    buffer[5] = (byte)0x00;
                                    buffer[6] = (byte)0x00;
                                    buffer[7] = (byte)0x00;
                                    buffer[8] = Get_SumCheck(buffer, 8);
                                    buffer[9] = 0x55;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace); 
                }
                
            }
            try
            {
                serialPort.Write(buffer, 0, 6);
                // serialPort1.Write(buffer,0,6);

            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);
            }

        }

        /// <summary>
        ///  串号
        ///  举例：串号，990036990666 =》HEX E6 82 CD 9A CA
        ///  99 表示年份，00369表示任务单流水号，90666表示单内流水号
        /// </summary>
        public ulong SerialNo
        {
            get;
            set;
        } = 111111111111;

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
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string tag = e.Node.Tag+"";
            CommandType t = CommandType.None;
            Enum.TryParse<CommandType>(tag,out t);
            if (t != CommandType.None)
            {
                this.dataGridView1.DataSource = this.Commands.OfList(t);
                this.Commands.FirstDealWith(t);
                CommandExecuter.Tasks.Clear();
                foreach (CommandExt item in Commands.Commands.Values)
                {
                    if (!item.Reponsed)
                    {
                        CommandExecuter.EnqueueTask(item);
                    }
                }
            }
            else {
                this.dataGridView1.DataSource = this.Commands.OfList(t);
            }
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
                saveFileDialog.FileName = name+".csv";
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
                        //this.toolStripProgressBar1.Visible = true;
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
                            this.toolStripProgressBar1.Value = 100 * (j + 1) / this.dataGridView1.Rows.Count;
                        }
                        streamWriter.Close();
                        stream.Close();
                        MessageBox.Show("文件成功保存到：" + saveFileDialog.FileName.ToString(), "数据导出成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        this.toolStripProgressBar1.Value = 0;
                        this.toolStripProgressBar1.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "数据导出错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }
         



        public bool SetSerialNo(ulong text2)
        {
            byte[] buffer = new byte[10];
            byte[] bn = BitConverter.GetBytes(text2);
            //"写入序列号":   //AD 00 11 11 11 11 11 11 55
            { 
                buffer[0] = (byte)0xAD;
                buffer[1] = bn.Length > 6 ? bn[6] : (byte)0x00;
                buffer[2] = bn.Length > 5 ? bn[5] : (byte)0x00;
                buffer[3] = bn.Length > 4 ? bn[4] : (byte)0x00;
                buffer[4] = bn.Length > 3 ? bn[3] : (byte)0x00;
                buffer[5] = bn.Length > 2 ? bn[2] : (byte)0x00;
                buffer[6] = bn.Length > 1 ? bn[1] : (byte)0x00;
                buffer[7] = bn.Length > 0 ? bn[0] : (byte)0x00;
                buffer[8] = Get_SumCheck(buffer, 8);
                buffer[9] = 0x55;
            } 
            try
            {
                this.serialPort.Write(buffer, 0, 10);
                return true;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);
                return false;
            }
        }

        public bool SetDeviceTime(DateTime now, string text2)
        {
            byte[] array = new byte[9]; 
            string text = now.Year.ToString();
            string value = text.Substring(0, 2);
            this.ConfigStateLable.Text = " "; 
            string a = text2;
            if (a == "报警器寿命起始时间")
            {
                array[0] = 172;
            }
            else
            {
                array[0] = 171;
            }
            array[1] = (byte)Convert.ToInt32(value, 16);
            value = text.Substring(2, 2);
            array[2] = (byte)Convert.ToInt32(value, 16);
            text = now.Month.ToString();
            array[3] = (byte)Convert.ToInt32(text, 16);
            text = now.Day.ToString();
            array[4] = (byte)Convert.ToInt32(text, 16);
            text = now.Hour.ToString();
            array[5] = (byte)Convert.ToInt32(text, 16);
            text = now.Minute.ToString();
            array[6] = (byte)Convert.ToInt32(text, 16);
            array[7] = this.Get_SumCheck(array, 7);
            array[8] = 85;
            try
            {
                this.serialPort.Write(array, 0, 9);
                return true;
            }
            catch(Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);
                return false;
            }
        }

        private void StatusLabelTime_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen) {
                Log("请连接设备");
                return;
            }
            FormModifyTime.Show(this);
        }
 
        private void StatusLabelSerialNo_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
            FormSerialNO.Show(this, this.SerialNo);
        }



        public bool SetZero()
        {
            byte[] buffer = new byte[9]; 
            {
                buffer = new byte[10];
                //0x87-零点校准命令
                //发送命令
                //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                //起始字节    预留 命令	-	-	-	-	-	校验值
                //0xFF	0x01	0x87	0x00	0x00	0x00	0x00	0x00	校验和
                //返回值
                //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                //起始字节    命令	-	状态		-	-	-	校验值
                //0xFF	0x87	0x00	0x01/0x00	0x00	0x00	0x00	0x00	校验和
                {
                    buffer[0] = (byte)0xFF;
                    buffer[1] = (byte)0x01;
                    buffer[2] = (byte)0x87;
                    buffer[3] = (byte)0x00;
                    buffer[4] = (byte)0x00;
                    buffer[5] = (byte)0x00;
                    buffer[6] = (byte)0x00;
                    buffer[7] = (byte)0x00;
                    buffer[8] = Get_SumCheck(buffer, 8);
                    buffer[9] = 0x55;
                }
            }
            try
            {
                this.serialPort.Write(buffer, 0, 10);
                return true;
            }
            catch (Exception ex)
            {
                Log("校准零点出错");
                Log(ex.Message);
                Log(ex.StackTrace);
                return false;
            }
        }



        public bool SetSpan()
        {
            byte[] buffer = new byte[9];
            {
                buffer = new byte[10];
                //0x88 - 校准SPAN点命令
                //发送命令
                //Byte0   Byte1 Byte2   Byte3 Byte4   Byte5 Byte6   Byte7 Byte8
                //起始字节 预留  命令 - SPAN高8位 SPAN低8位 - -校验值
                //0xFF    0x01    0x88    0x00    HIGH LOW 0x00    0x00    校验和
                //返回值
                //Byte0 Byte1   Byte2 Byte3   Byte4 Byte5   Byte6 Byte7   Byte8
                //起始字节    命令 - 状态 - - - -校验值
                //0xFF    0x88    0x00    0x01 / 0x00   0x00    0x00    0x00    0x00    校验和
                {
                    buffer[0] = (byte)0xFF;
                    buffer[1] = (byte)0x01;
                    buffer[2] = (byte)0x88;
                    buffer[3] = (byte)0x00;
                    buffer[4] = (byte)0x00;
                    buffer[5] = (byte)0x00;
                    buffer[6] = (byte)0x00;
                    buffer[7] = (byte)0x00;
                    buffer[8] = Get_SumCheck(buffer, 8);
                    buffer[9] = 0x55;
                }
            }
            try
            {
                this.serialPort.Write(buffer, 0, 10);
                return true;
            }
            catch (Exception ex)
            {
                Log("校准SPAN点出错");
                Log(ex.Message);
                Log(ex.StackTrace);
                return false;
            }
        }

        private void labelZero_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }

            if (MessageBox.Show(
                  string.Format("确定要重置零点吗？"),
                  "确认重置", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (this.SetZero())
                {
                    CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.ZeroSpanNongDu, Par1 = CommandExt.Par1Default });
                }
            }
        }

        private void labelSpan_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
            if (MessageBox.Show(
                 string.Format("确定要重置SPAN点吗？"),
                 "确认重置", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (this.SetSpan())
                {
                    CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.ZeroSpanNongDu, Par1 = CommandExt.Par1Default });
                }
            }
        }

        private void button标定零点_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }

            if (MessageBox.Show(
                  string.Format("确定要重置零点吗？"),
                  "确认重置", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (this.SetZero()) {
                    CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.ZeroSpanNongDu, Par1 = CommandExt.Par1Default });
                }
            }
        }

        private void button标Span点_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
            if (MessageBox.Show(
                 string.Format("确定要重置SPAN点吗？"),
                 "确认重置", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                if (this.SetSpan())
                {
                    CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.ZeroSpanNongDu, Par1 = CommandExt.Par1Default });
                }
            }
        }

        private void button写串号_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
            FormSerialNO.Show(this, this.SerialNo);
        }

        private void button写时间_Click(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                Log("请连接设备");
                return;
            }
            FormModifyTime.Show(this);
        }


    }

}