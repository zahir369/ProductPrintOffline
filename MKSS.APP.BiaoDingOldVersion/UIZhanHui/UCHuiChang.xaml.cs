using GodSharp.SerialPort;
using MKSS.SerialPortUtil;
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

namespace DeviceDataMonitorWPF.UIZhanHui
{
    /// <summary>
    /// UCHuiChang.xaml 的交互逻辑
    /// </summary>
    public partial class UCHuiChang : UserControl
    {
        GodSerialPort gsPort = null;
        public UCHuiChang()
        {
            InitializeComponent();
        }

        public bool InitDataPort()
        {
            if (gsPort != null && gsPort.IsOpen == true) return true;


            var sp = SerialPortHelper.InitPort();
            if (sp == null) return false;

            gsPort = new GodSerialPort(
                portName: sp.PortName,
                baudRate: sp.BaudRate,
                dataBits: sp.DataBits,
                stopBits: sp.StopBits
            );

            return true;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!InitDataPort())
            {
                MessageBox.Show("设备串口初始化失败，请检查配置是否正确！");
                return;
            }

            gsPort.UseDataReceived(true, GetPortData);

            if (gsPort.IsOpen) return;

            var flag = gsPort.Open();
            if (!flag)
            {
                MessageBox.Show("设备串口打开失败，请检查配置是否正确！");
                return;
            }
        }

        private void GetPortData(GodSerialPort gsPort, byte[] recDatas)
        {

            var data = recDatas;

            if (data == null) return;

            if (data.Length != 14) return;
            if (
                data[0] != 0x16
                || data[1] != 0x0B
                || data[2] != 0x01
                ) return;

            byte[] bsTemp;

            //co2
            bsTemp = new byte[] { data[3], data[4] };
            Array.Reverse(bsTemp);
            int co2 = BitConverter.ToInt16(bsTemp);

            //voc
            bsTemp = new byte[] { data[5], data[6] };
            Array.Reverse(bsTemp);
            int voc = BitConverter.ToInt16(bsTemp);

            //湿度
            bsTemp = new byte[] { data[7], data[8] };
            Array.Reverse(bsTemp);
            double humidity = BitConverter.ToUInt16(bsTemp) / 10.0;

            //温度
            bsTemp = new byte[] { data[9], data[10] };
            Array.Reverse(bsTemp);
            double temperature = (BitConverter.ToUInt16(
               bsTemp
                ) - 500) / 10.0;

            //pm2.5
            bsTemp = new byte[] { data[11], data[12] };
            Array.Reverse(bsTemp);
            int pm25 = BitConverter.ToInt16(bsTemp);


            string str = "co2" + co2 + "--voc" + voc + "--湿度" + humidity + "--温度" + temperature + "--pm2.5" + pm25;

            temperature = new Random().Next(100)/10.0;

            ucNongDuPie.Dispatcher.Invoke(new Action(
                delegate
                {
                    ucNongDuPie.SetNongDuData(temperature);
                })
             );
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

            if (gsPort != null)
            {
                if (gsPort.IsOpen) gsPort.Close();
                gsPort = null;
            }
        }
    }
}
