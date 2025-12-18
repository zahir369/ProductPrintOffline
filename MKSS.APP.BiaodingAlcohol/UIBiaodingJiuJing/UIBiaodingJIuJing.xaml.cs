using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    /// <summary>
    /// UILaoHuaGuanCha.xaml 的交互逻辑
    /// <summary>
    /// 
    /// 
    /// 
    //[13:41:12.007]发→◇AA BB 06 01 00 00 00 00 94 □
    //[13:41:12.027] 收←◆AA BB 06 00 8B 0A 01 00 FF
    // [13:41:15.475] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:16.242] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:16.272] 收←◆AA BB 03 01 09 00 79 00 15 
    //[13:41:17.465] 发→◇AA BB 02 00 00 00 00 00 99 □
    //[13:41:17.495] 收←◆AA BB 02 00 8B 0A 02 00 02 
    //[13:41:23.697] 发→◇AA BB 06 02 01 00 00 00 92 □
    //[13:41:23.717] 收←◆AA BB 06 00 88 0A 00 00 03 
    //[13:41:28.109] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:28.986] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:29.819] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:29.839] 收←◆AA BB 03 01 D5 00 14 41 6D 
    //[13:41:40.278] 发→◇AA BB 06 01 00 00 00 00 94 □
    //[13:41:40.308] 收←◆AA BB 06 00 3F 08 01 00 4D 
    //[13:41:43.755] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:44.564] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:44.585] 收←◆AA BB 03 01 0B 00 C8 00 C4
    // [13:41:45.741] 发→◇AA BB 02 00 00 00 00 00 99 □
    //[13:41:45.771] 收←◆AA BB 02 00 3D 08 00 00 54 
    //[13:41:52.117] 发→◇AA BB 06 02 01 00 00 00 92 □
    //[13:41:52.137] 收←◆AA BB 06 00 3B 08 01 00 51 
    //[13:41:57.232] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:59.732] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:41:59.752] 收←◆AA BB 03 01 DC 01 EE 97 35

    //[3:43:39.610]发→◇AA BB 06 00 01 00 00 00 94 □
    //[13:43:39.630] 收←◆AA BB 06 00 A1 08 04 00 E8
    // [13:43:46.572] 发→◇AA BB 03 00 00 00 00 00 98 □
    //[13:43:46.592] 收←◆AA BB 03 32 DE 00 64 48 DC
    /// </summary>
    public partial class UIBiaodingJiuJing : UserControl
    {

        SerialPort serialPort1 = new SerialPort();
        string[][] PAR = new string[][] {
            new string[]{ "AA BB 06 01 00 00 00 00 94", "AA BB 03 00 00 00 00 00 98","AA BB 03 01" },
            new string[]{ "AA BB 06 02 01 00 00 00 92", "AA BB 03 00 00 00 00 00 98", "AA BB 03 01" },
            new string[]{ "AA BB 06 00 01 00 00 00 94", "AA BB 03 00 00 00 00 00 98", "" },
            new string[]{ "AA BB 01 00 00 00 00 00 9A", "AA BB 02 00 00 00 00 00 99", "" },
        };
        public enum CommandType { Zero = 0, Span = 1, Test = 2, Other = 3 }
        public enum CommandSubType { Sub1 = 0, Sub2 = 1 }
        CommandType Main = CommandType.Test;
        CommandSubType Sub = CommandSubType.Sub1;
        string AssrtString = "";

        public UIBiaodingJiuJing()
        {
            InitializeComponent();
            TxtSerialPorts_MouseDoubleClick(null, null);
            this.BtnConnect.Focus();
        }


        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                int len = serialPort1.BytesToRead;
                Byte[] buf = new byte[len];
                int length = serialPort1.Read(buf, 0, len);
                StringBuilder s = new StringBuilder();
                foreach (var i in buf)
                {
                    string str = i.ToString("X");
                    while (str.Length < 2) str = "0" + str;
                    s.Append(string.Format("{0} ", str));
                }
                string result = System.Text.Encoding.ASCII.GetString(buf);
                UpdateTextBox("收←◆" + s.ToString());
                switch (this.Main)
                {
                    case CommandType.Zero:
                        switch (this.Sub)
                        {
                            case CommandSubType.Sub1:
                                break;
                            case CommandSubType.Sub2:
                                string sub3 = this.PAR[(int)this.Main][2];
                                if (s.ToString().StartsWith(sub3))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnZeroInfo.Text = "标定零点成功!";
                                    });
                                }
                                else {
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnZeroInfo.Text = "命令已发送 !";
                                    });
                                }
                                break; 
                        }
                        break;
                    case CommandType.Span:
                        switch (this.Sub)
                        {
                            case CommandSubType.Sub1:
                                break;
                            case CommandSubType.Sub2:
                                string sub3 = this.PAR[(int)this.Main][2];
                                if (s.ToString().StartsWith(sub3))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnSpanInfo.Text = "标定span点成功!";
                                    });

                                }
                                else
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnSpanInfo.Text = "命令已发送 !";
                                    });
                                }
                                break;
                        }
                        break;
                    case CommandType.Test:
                        switch (this.Sub)
                        {
                            case CommandSubType.Sub1:
                                break;
                            case CommandSubType.Sub2:
                                if (buf.Length>4)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnTestInfo.Text = string.Format("浓度值：{0}", (int)buf[3]);
                                    });
                                }
                                break;
                        }
                        break;
                    case CommandType.Other:
                        switch (this.Sub)
                        {
                            case CommandSubType.Sub1:
                                //[13:17:05.645]收←◆AA BB 01 04 A3 07 01 00 EB 
                                // 应答帧的参数1指示当前模块状态：
                                //    0x01：就绪状态（上电默认，检测到吹气后，模块自动进入0x02）
                                //    0x02：正在吹气状态（连续吹气时间达标后，模块自动进入0x03）
                                //    0x03：正在计算状态（计算完成后，模块自动进入0x04）
                                //    0x04：显示测试结果状态
                                //    0x05：故障状态（吹气中断等异常状态）
                                //    应答帧的参数2为压力信号AD值低字节；
                                //    应答帧的参数3为压力信号AD值高字节；
                                //    应答帧的参数4为酒精信号AD值低字节；
                                //    应答帧的参数5为酒精信号AD值高字节
                                if (buf.Length > 8)
                                {

                                    int sta = (int)buf[3];
                                    string str_str = "";
                                    if (sta == 1) str_str = "就绪状态";
                                    if (sta == 2) str_str = "正在吹气状态";
                                    if (sta == 3) str_str = "正在计算状态";
                                    if (sta == 4) str_str = "显示测试结果状态";
                                    if (sta == 5) str_str = "故障状态";
                                    short valtage = BitConverter.ToInt16(new byte[] { buf[4], buf[5] }, 0);
                                    short jiujing = BitConverter.ToInt16(new byte[] { buf[6], buf[7] }, 0);
                                    string mssg = string.Format("模块状态：{0}，压力信号AD值：{1}，酒精信号AD值：{2}",
                                        str_str, valtage, jiujing);
                                    UpdateTextBox(mssg);
                                    Dispatcher.Invoke(() =>
                                    {
                                        this.BtnStatusReadInfo.Text = mssg;
                                    });
                                }
                                break;
                            case CommandSubType.Sub2:
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }

        }

        private void BtnConnectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (this.TxtSerialPorts.SelectedItem == null)
                {
                    UpdateTextBox("请选择串口");
                    return  ;
                }
                serialPort1.PortName = this.TxtSerialPorts.SelectedItem.ToString();
                serialPort1.DataReceived -= SerialPort1_DataReceived;
                serialPort1.DataReceived += SerialPort1_DataReceived;
                serialPort1.Open();
                UpdateTextBox("已连接" + serialPort1.PortName);
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void BtnDisConnectCommand(object sender, ExecutedRoutedEventArgs e)
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
         
        private void BtnZeroSetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Zero;
                this.Sub = CommandSubType.Sub1;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command); 
                this.serialPort1.Write(bs, 0, bs.Length); 
                this.BtnZeroInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }
         
        private void BtnZeroTestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Zero;
                this.Sub = CommandSubType.Sub2;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length); 
                //this.BtnZeroInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }
         
        private void BtnSpanSetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Span;
                this.Sub = CommandSubType.Sub1;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length); 
                this.BtnSpanInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }
         
        private void BtnSpanTestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Span;
                this.Sub = CommandSubType.Sub2;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length); 
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void BtnTestSetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Test;
                this.Sub = CommandSubType.Sub1;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length);
                this.BtnTestInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void BtnTestTestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Test;
                this.Sub = CommandSubType.Sub2;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length);
                this.BtnTestInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void BtnStatusReadCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {

                this.Main = CommandType.Other;
                this.Sub = CommandSubType.Sub1;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length);
                this.BtnStatusReadInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void BtnReadyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.serialPort1.IsOpen)
            {
                UpdateTextBox("请连接串口");
                return;
            }
            try
            {
                this.Main = CommandType.Other;
                this.Sub = CommandSubType.Sub2;
                string command = PAR[(int)this.Main][(int)this.Sub];
                byte[] bs = ToBytes(command);
                this.serialPort1.Write(bs, 0, bs.Length);
                this.BtnStatusReadInfo.Text = "命令已发送!";
                UpdateTextBox(string.Format("发→◇{0}", command));
            }
            catch (Exception ex)
            {
                UpdateTextBox(ex.Message);
            }
        }

        private void TxtSerialPorts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
            }
            if (this.TxtSerialPorts.Items.Count>0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
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

    }
}