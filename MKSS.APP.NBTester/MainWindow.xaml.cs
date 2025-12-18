
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
using System.Linq;
using System.IO.Ports;

namespace MKSS.APP.NBTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            SerialPort serialPort1 = new SerialPort();
            serialPort1.BaudRate = 9600;//波特率
            serialPort1.PortName = "COM13";
            serialPort1.Parity = Parity.None;//校验法：无
            serialPort1.DataBits = 8;//数据位：8
            serialPort1.StopBits = StopBits.One;//停止位：1
            try
            {
                serialPort1.DataReceived += Port_DataReceived;
                serialPort1.DtrEnable = true;//设置DTR为高电平
                serialPort1.RtsEnable = false;

                serialPort1.Encoding = Encoding.ASCII;
                serialPort1.DiscardNull = false;
                serialPort1.ReceivedBytesThreshold = 1;
                serialPort1.ReadTimeout = 0x7fff_ffff;
                serialPort1.ReadBufferSize = 0x1000;
                serialPort1.WriteBufferSize = 0x800;
                serialPort1.WriteTimeout = 0x7fff_ffff;

                serialPort1.Open();//打开串口


                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                string str = (string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12} ",
                        serialPort1.PortName, serialPort1.BaudRate,
                        serialPort1.DataBits, serialPort1.Parity,
                        serialPort1.StopBits, serialPort1.RtsEnable,
                        serialPort1.Encoding, serialPort1.DiscardNull,
                        serialPort1.ReceivedBytesThreshold, serialPort1.ReadTimeout,
                        serialPort1.ReadBufferSize, serialPort1.WriteBufferSize,
                        serialPort1.WriteTimeout));
            }
            catch (Exception ex)
            {
                //打开串口出错，显示错误信息
                MessageBox.Show(ex.Message);
            }

        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //退出
            Application.Current.Shutdown();
        }
         
    }
}
