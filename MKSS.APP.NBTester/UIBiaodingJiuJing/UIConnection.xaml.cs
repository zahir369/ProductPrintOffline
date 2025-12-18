using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MKSS.APP.UIBiaodingJiuJing
{
     
    /// </summary>
    public partial class UIConnection : UserControl
    {

        MaterialDesignThemes.Wpf.PackIcon[] ics = null;
        public string Port { get; set; } = null;
        SerialPort serialPort1 = new SerialPort();
        string[][] PAR = new string[][] {

            new string[]{ "AT+QSCLK=0", "OK", "","" },//设置睡眠模式
            new string[]{ "AT+CGATT?", "OK", "","" },//查询附着网络
            new string[]{ "AT+QSCLK=0", "OK", "","" },//禁用睡眠模式
            
            new string[]{ "AT+NNMI=1", "OK", "","" },//设置直吐模式
            new string[]{ "AT+CGPADDR?", "OK", "","" },//查询模块是否成功注网
            new string[]{ "AT+NMGS=7,010548454c4c4f", "OK", "","" },//通讯测试
            new string[]{ "AT+NCFG=0,86400", "OK", "","" },//设置端口
            new string[]{ "AT+NCDPOPEN=\"221.229.214.202\"", "OK", "","" },//设置电信IP
            new string[]{ "AT+CGSN=1", "OK", "","" },//读取设备IMEI
            new string[]{ "AT+NRB", "OK", "","" },//复位
            new string[]{ "AT+CSQ", "OK", "","" },//查信号
            new string[]{ "AT+QCCID", "OK", "","" },//读取IMSI卡号 
            
            new string[]{ "RDY", "CFUN: 1", "已上电","上电失败" },//上电
            new string[]{ "+CPIN", "READY", "初始化成功","初始化失败" },//初始化

        };

        
        public enum CommandType
        {
            设置睡眠模式 = 0,
            查询附着网络 = 1, 
            禁用睡眠模式 = 2,
            设置直吐模式 = 3,
            查询模块是否成功注网 = 4,
            通讯测试 = 5,
            设置端口 = 6,
            设置电信IP = 7,
            读取设备IMEI = 8,
            复位 = 9,
            查信号 = 10,
            读取IMSI卡号 = 11,

            上电 = 12,
            初始化 = 13,

        }
        public enum CommandResult
        {
            成功 = 1,
            等待 = 3,
            失败 = 2 
        }

        int CommandAllCurrent = -1;
        CommandType[] CommandAll = new CommandType[]{

                    CommandType.上电,
                    CommandType.初始化,
                    CommandType.禁用睡眠模式,
                    CommandType.查信号,
                    CommandType.查询附着网络,
                    CommandType.设置直吐模式,
                    CommandType.设置直吐模式,
                    CommandType.查询模块是否成功注网,
                    CommandType.设置端口,
                    CommandType.设置电信IP,
                    CommandType.读取设备IMEI,
                    CommandType.读取IMSI卡号

                };


        string[][] StatusTypePAR = new string[][] {

            new string[]{ "已上电", "RDY\n\n+CFUN: 1", "" }, 
            new string[]{ "已初始化", "+CPIN: READY", "" },  

        }; 
         
        string AssrtString = "";

        public UIConnection()
        {
            InitializeComponent();
            ics = new MaterialDesignThemes.Wpf.PackIcon[] {
                    IC1,IC2,IC3,IC4,IC5,IC6,IC7,IC8,IC9,IC10,IC11,IC12
                };
        }

        public void Start()
        {
            try
            {
                this.COM.Content = this.Port;
                BtnConnectCommand(); 
            }
            catch (Exception ex)
            {
                UpdateTextBox(string.Format("发送命令时出错 {0}", ex.Message));
                UpdateTextBox(ex.Message);
            }
            finally
            {
                BtnDisConnectCommand();
            }
        }

        public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                UpdateTextBox(e.Error.ToString());
                return;
            }
            if (!e.Cancelled)
                UpdateTextBox("处理完毕!");
            else
                UpdateTextBox("处理终止!");

        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            CommandType Main = CommandType.上电;
            CommandResult MainResult = CommandResult.等待;
            try
            {
                int len = serialPort1.BytesToRead;
                Byte[] buf = new byte[len];
                int length = serialPort1.Read(buf, 0, len);
                StringBuilder sx = new StringBuilder();
                foreach (var i in buf)
                {
                    string str = i.ToString("X");
                    while (str.Length < 2) str = "0" + str;
                    sx.Append(string.Format("{0} ", str));
                }
                string result = System.Text.Encoding.ASCII.GetString(buf);
                UpdateTextBox("收←◆" + result);


                bool find = false;
                //判断命令
                for (int i = 0; i < PAR.Length; i++)
                {
                    var ling = PAR[i];
                    if (result.StartsWith(ling[0])) {
                        Main = (CommandType)i;
                        find = true;
                    }
                }

                if (!find) {
                    UpdateText = string.Format("非预期结果：" + result);
                }

                string comm_str = this.PAR[(int)Main][0];

                if (!result.StartsWith(comm_str)) {
                    UpdateText = string.Format("【"+ Main + "】非预期结果：" + result);
                    return;
                }

                string sucess_str = this.PAR[(int)Main][1];
                string sucess_desc = this.PAR[(int)Main][2];
                string fali_desc = this.PAR[(int)Main][3];
                switch (Main)
                {
                    case CommandType.上电:
                        if (result.ToString().EndsWith(sucess_str))
                        {
                            MainResult = CommandResult.成功;
                            UpdateText = sucess_desc;
                            this.IC1.Kind = MaterialDesignThemes.Wpf.PackIconKind.PowerPlug;
                            this.IC1.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else {
                            MainResult = CommandResult.失败;
                            this.IC1.Kind = MaterialDesignThemes.Wpf.PackIconKind.PowerPlugOff;
                            this.IC1.Foreground = new SolidColorBrush(Colors.Gray);
                            UpdateText = fali_desc;
                        }
                        break;
                    case CommandType.初始化:
                        if (result.ToString().EndsWith(sucess_str))
                        {
                            MainResult = CommandResult.成功;
                            UpdateText = sucess_desc;
                            this.IC2.Kind = MaterialDesignThemes.Wpf.PackIconKind.AlphaICircle;
                            this.IC2.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            MainResult = CommandResult.失败;
                            this.IC2.Kind = MaterialDesignThemes.Wpf.PackIconKind.AlphaICircleOutline;
                            this.IC2.Foreground = new SolidColorBrush(Colors.Gray);
                            UpdateText = fali_desc;
                        }
                        break;
                    case CommandType.禁用睡眠模式:
                        if (result.ToString().EndsWith(sucess_str))
                        {
                            MainResult = CommandResult.成功;
                            UpdateText = sucess_desc;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.SleepOff;
                            this.IC3.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            MainResult = CommandResult.失败;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.Sleep;
                            this.IC3.Foreground = new SolidColorBrush(Colors.Gray);
                            UpdateText = fali_desc;
                        }
                        break;
                    case CommandType.设置直吐模式:
                        if (result.ToString().EndsWith(sucess_str))
                        {
                            MainResult = CommandResult.成功;
                            UpdateText = sucess_desc;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckNetwork;
                            this.IC3.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            MainResult = CommandResult.失败;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseNetworkOutline;
                            this.IC3.Foreground = new SolidColorBrush(Colors.Gray);
                            UpdateText = fali_desc;
                        }
                        break;
                    default:
                        if (result.ToString().EndsWith(sucess_str))
                        {
                            MainResult = CommandResult.成功;
                            UpdateText = sucess_desc;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckNetwork;
                            this.IC3.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            MainResult = CommandResult.失败;
                            UpdateText = fali_desc;
                            this.IC3.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseNetworkOutline;
                            this.IC3.Foreground = new SolidColorBrush(Colors.Gray);
                        }
                        break; 
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }

            if (MainResult == CommandResult.成功)
            {
                SendCommandNext();
            }
            else {
                SendCommandAgain();
            }

        }

        private void BtnConnectCommand( )
        {
            try
            {
                if (this.Port == null)
                {
                    UpdateTextBox("请选择串口");
                    return;
                }

                this.serialPort1.PortName = this.Port;
                this.serialPort1.BaudRate = 9600;
                this.serialPort1.DataBits = 8;
                this.serialPort1.Parity = Parity.None;
                this.serialPort1.StopBits = StopBits.One;
                serialPort1.WriteTimeout = 3000;
                serialPort1.ReadTimeout = 3000;
                serialPort1.ReceivedBytesThreshold = 1;//有数据过来时数据长度大于1
                serialPort1.NewLine = "\r\n";// 用于解释通过ReadLine()与WriteLine()的值
                serialPort1.RtsEnable = false;//获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号
                serialPort1.Encoding = Encoding.ASCII;
                serialPort1.DiscardNull = false;

                //serialPort1.DataReceived -= SerialPort1_DataReceived;
                serialPort1.DataReceived += SerialPort1_DataReceived;
                serialPort1.ErrorReceived += SerialPort1_ErrorReceived;
                serialPort1.Open();

                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
                UpdateTextBox("已连接" + serialPort1.PortName);
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void SerialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        private void BtnDisConnectCommand( )
        {
            try
            {
                serialPort1.DataReceived -= SerialPort1_DataReceived;
                serialPort1.Close();
                UpdateTextBox("已断开连接" + serialPort1.PortName);
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }


        private void SendCommandNext()
        {
            if (CommandAllCurrent >= CommandAll.Length) return;
            if (CommandAllCurrent <= 0) CommandAllCurrent = 0;
            else CommandAllCurrent++;
            SendCommand(CommandAll[CommandAllCurrent]); 
        }

        private void SendCommandAgain()
        {
            if (CommandAllCurrent >= CommandAll.Length) return;
            if (CommandAllCurrent <= 0) CommandAllCurrent = 0;
            else CommandAllCurrent++;
            SendCommand(CommandAll[CommandAllCurrent]);
        }


        private void SendCommand(CommandType sender)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                string command = PAR[(int)sender][0];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length);
                this.UpdateText = sender+"已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }
          

        byte[] ToBytes(string str_in)
        {
            List<byte> ret = new List<byte>();
            string[] arr = str_in.Split(' ');
            foreach (var item in arr)
            {
                string s = item;
                if (string.IsNullOrEmpty(s)) continue;
                s = s.Trim();
                if (string.IsNullOrEmpty(s)) continue;
                int ii = System.Int32.Parse(s, System.Globalization.NumberStyles.HexNumber);
                ret.Add((byte)ii);
            }
            return ret.ToArray();
        }

        void UpdateTextBox(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                this.TextLogger.AppendText(System.Environment.NewLine);
                this.TextLogger.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
                this.TextLogger.ScrollToEnd();
            }); 
        }

        string UpdateText  
        {
                set{
                    UpdateTextBox(value);
                }
        }

        private void TextLogger_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Start();
        }




        //private BackgroundWorker bgWorker = null;
        //public void StartXX() {

        //    if (bgWorker != null)
        //    {
        //        try
        //        {
        //            bgWorker.CancelAsync();
        //            bgWorker.Dispose();
        //            bgWorker = null;
        //        }
        //        catch (Exception)
        //        {

        //        }
        //        Thread.Sleep(200);
        //    }

        //    bgWorker = new BackgroundWorker();
        //    bgWorker.WorkerReportsProgress = true;
        //    bgWorker.WorkerSupportsCancellation = true;
        //    bgWorker.DoWork -= new DoWorkEventHandler(bgWorker_DoWork);
        //    bgWorker.ProgressChanged -= new ProgressChangedEventHandler(bgWorker_ProgessChanged);
        //    bgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
        //    bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
        //    bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
        //    bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);

        //    CommandAllCurrent = 0; 
        //    foreach (var item in ics)
        //    {
        //        item.Foreground = new SolidColorBrush(Colors.Gray);
        //    }

        //    bgWorker.RunWorkerAsync();

        //}


        //public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        BtnConnectCommand();


        //        SendCommandNext();
        //    }
        //    catch (Exception ex)
        //    {
        //        UpdateTextBox(string.Format("发送命令时出错 {0}", ex.Message));
        //        UpdateTextBox(ex.Message);
        //    }
        //    finally
        //    {
        //        BtnDisConnectCommand();
        //    }

        //}

    }
}