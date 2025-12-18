using DeviceDataMonitorWPF.UIBiaoDing.Config;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
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

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDing.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingView : UserControl
    { 


        public UISheBeiBiaoDingView()
        {
            InitializeComponent();

            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item.Name== UISheBeiBiaoDingViewModel.Intance.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }
            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count>0) {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }

            bool selP = false;
            this.TxtProductList.Items.Clear();
            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == UISheBeiBiaoDingViewModel.Intance.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                    selP = true;
                }
            } 
            if (!selP && BiaoDingConfgig.Instance.Product.Count > 0) this.TxtProductList.SelectedItem = BiaoDingConfgig.Instance.Product[0];
            this.TxtPortsModeSelect.SelectedIndex = UISheBeiBiaoDingViewModel.Intance.TxtPortsModeSelect;
            UISheBeiBiaoDingViewModel.Intance.InitPage(this);


        }


        private void TxtSerialPorts_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            List<HardWareInfo>  list = SerialPortNames();
            foreach (var item in list)
            {
                if (!this.TxtSerialPorts.Items.Contains(item)) {
                    this.TxtSerialPorts.Items.Add(item);
                }
            }
            foreach (var item in this.TxtSerialPorts.Items)
            {
                HardWareInfo h = item as HardWareInfo;
                if (list.Count(w=>w.Name==h.Name)==0)
                {
                    this.TxtSerialPorts.Items.Remove(item);
                    break;
                }
            }
        }


        private void TxtSensorGrougAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InputAddress(e);
        }

        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
        }

        private void TxtVoltageValueBase_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
            
        }


        private void TxtValueBase_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
            
        }

        public static void InputAddress(KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else if ((e.Key == Key.OemComma || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else
            {

                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字，“,”或“-”");
                return;
            }
            List<Address> addrList = UISheBeiBiaoDingViewModel.Intance.Address;//必须调用一次，计算内存表
        }

        public static void InputNumber(KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
            } 
            else
            {

                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字");
                return;
            }
        }

        private void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;

            this.TxtZreo.Text = c.Zero.ToString();
            this.TxtSpan.Text = c.Span.ToString();
            //this.TxtTimeInterval.Text = c.TimeInterval.ToString();
            this.TxtValueAdd.Text = c.ValueAdd.ToString();
            this.TxtValueBase.Text = c.ValueBase.ToString();
            this.TxtValueMinus.Text = c.ValueMinus.ToString();

            this.TxtVoltageValueAdd.Text = c.VoltageValueAdd.ToString();
            this.TxtVoltageValueBase.Text = c.VoltageValueBase.ToString();
            this.TxtVoltageValueMinus.Text = c.VoltageValueMinus.ToString();
             
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void TxtPortsModeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TxtTCPIP == null || this.TxtSerialPorts == null) return;


            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item == UISheBeiBiaoDingViewModel.Intance.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }


            bool TCPIP_TYPE = (this.TxtPortsModeSelect.SelectedIndex)==(UISheBeiBiaoDingViewModel.TCPIP_TYPE_NETWORK);
            this.TxtTCPIP.Visibility = TCPIP_TYPE ? Visibility.Visible: Visibility.Hidden;
            this.TxtSerialPorts.Visibility = !TCPIP_TYPE? Visibility.Visible : Visibility.Hidden;
            UISheBeiBiaoDingViewModel.Intance.TxtPortsModeSelect = this.TxtPortsModeSelect.SelectedIndex;

            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count > 0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }

        }

        private void TxtValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtValueBase.Text == "0" || TxtValueBase.Text == "")
            {
                try
                {
                    //读取平均值
                    List<SensorGroupDataModel> list = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
                    List<SensorDataX> listSensor = new List<SensorDataX>();
                    foreach (SensorGroupDataModel d in list)
                    {
                        listSensor.AddRange(d.ProductTable.Where(w => !w.IsEmpData));
                    }
                    int min = listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Min(w => w.V07.Value);
                    int max = listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Max(w => w.V07.Value);
                    int avg = (int)listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Average(w => w.V07.Value);
                    //读取平均值
                    this.TxtValueBase.Text = avg.ToString();
                    this.TxtValueAdd.Text = (max - avg).ToString();
                    this.TxtValueMinus.Text = (avg - min).ToString();
                    //UISheBeiBiaoDingModel.Intance.BtnJudge();
                }
                catch (Exception)
                {
                     
                }
                
            }
        }

        private void TxtVoltageValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtVoltageValueBase.Text == "0" || TxtVoltageValueBase.Text == "")
            {
                try
                {
                    List<SensorGroupDataModel> list = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
                    List<SensorDataX> listSensor = new List<SensorDataX>();
                    foreach (SensorGroupDataModel d in list)
                    {
                        listSensor.AddRange(d.ProductTable.Where(w => !w.IsEmpData));
                    }
                    if (listSensor.Count == 0) return;
                    int min = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Min(w => w.V01.Value);
                    int max = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Max(w => w.V01.Value);
                    int avg = (int)listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Average(w => w.V01.Value);
                    //读取平均值
                    this.TxtVoltageValueBase.Text = avg.ToString();
                    this.TxtVoltageValueAdd.Text = (max - avg).ToString();
                    this.TxtVoltageValueMinus.Text = (avg - min).ToString();
                    //UISheBeiBiaoDingModel.Intance.BtnJudge();
                }
                catch (Exception)
                {
                     
                }
                
            }
        }

        private void StaZeroSpanTitleExchange_Checked(object sender, RoutedEventArgs e)
        {
            this.UISheBeiBiaoDingC10.SetStaZeroSpanTitleExchange(StaZeroSpanTitleExchange.IsChecked != null && StaZeroSpanTitleExchange.IsChecked.Value);
            this.UISheBeiBiaoDingC05.SetStaZeroSpanTitleExchange(StaZeroSpanTitleExchange.IsChecked != null && StaZeroSpanTitleExchange.IsChecked.Value);
            UISheBeiBiaoDingViewModel.Intance.SetStaZeroSpanTitleExchange(StaZeroSpanTitleExchange.IsChecked != null && StaZeroSpanTitleExchange.IsChecked.Value);
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlMain.SelectedItem == this.TabItemBatchList) {
                UISheBeiBiaoDingCGK.IniData();
            }
        }

        public static List<HardWareInfo> SerialPortNames() {
            List<HardWareInfo> ret = new List<HardWareInfo>();
            foreach (var item in SerialPort.GetPortNames())
            {
                ret.Add(MulGetHardwareInfo( HardwareEnum.Win32_SerialPort, item)); 
            }
            return ret;
        }

        /// <summary>
        /// WMI取硬件信息
        /// </summary>
        /// <param name="hardType"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        public static HardWareInfo MulGetHardwareInfo(HardwareEnum hardType, string keyValue)
        {
            HardWareInfo ret = HardWareInfo.Create(  keyValue ,  keyValue );
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties["Name"].Value.ToString().Contains(keyValue))
                        {
                            ret.DescName = hardInfo.Properties["Name"].Value.ToString();
                        }

                    }
                    searcher.Dispose();
                }
            }
            catch
            {

            }
            finally
            {
            }
            return ret;
        }

    }

    public class HardWareInfo {
        static Dictionary<string, HardWareInfo> Cache = new Dictionary<string, HardWareInfo>();
        public static HardWareInfo Create(string name, string keyValue) {
            if (!Cache.ContainsKey(name)) {
                Cache.Add(name,new HardWareInfo() { Name=name, DescName= keyValue });
            }
            Cache[name].DescName = keyValue;
            return Cache[name];
        }
        public string Name { get; set; }
        public string DescName { get; set; }
        HardWareInfo() { }
        public override string ToString()
        {
            return DescName;
        }
    }


    /// <summary>
    /// 枚举win32 api
    /// </summary>
    public enum HardwareEnum
    {
        // 硬件
        Win32_Processor, // CPU 处理器
        Win32_PhysicalMemory, // 物理内存条
        Win32_Keyboard, // 键盘
        Win32_PointingDevice, // 点输入设备，包括鼠标。
        Win32_FloppyDrive, // 软盘驱动器
        Win32_DiskDrive, // 硬盘驱动器
        Win32_CDROMDrive, // 光盘驱动器
        Win32_BaseBoard, // 主板
        Win32_BIOS, // BIOS 芯片
        Win32_ParallelPort, // 并口
        Win32_SerialPort, // 串口
        Win32_SerialPortConfiguration, // 串口配置
        Win32_SoundDevice, // 多媒体设置，一般指声卡。
        Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
        Win32_USBController, // USB 控制器
        Win32_NetworkAdapter, // 网络适配器
        Win32_NetworkAdapterConfiguration, // 网络适配器设置
        Win32_Printer, // 打印机
        Win32_PrinterConfiguration, // 打印机设置
        Win32_PrintJob, // 打印机任务
        Win32_TCPIPPrinterPort, // 打印机端口
        Win32_POTSModem, // MODEM
        Win32_POTSModemToSerialPort, // MODEM 端口
        Win32_DesktopMonitor, // 显示器
        Win32_DisplayConfiguration, // 显卡
        Win32_DisplayControllerConfiguration, // 显卡设置
        Win32_VideoController, // 显卡细节。
        Win32_VideoSettings, // 显卡支持的显示模式。

        // 操作系统
        Win32_TimeZone, // 时区
        Win32_SystemDriver, // 驱动程序
        Win32_DiskPartition, // 磁盘分区
        Win32_LogicalDisk, // 逻辑磁盘
        Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
        Win32_LogicalMemoryConfiguration, // 逻辑内存配置
        Win32_PageFile, // 系统页文件信息
        Win32_PageFileSetting, // 页文件设置
        Win32_BootConfiguration, // 系统启动配置
        Win32_ComputerSystem, // 计算机信息简要
        Win32_OperatingSystem, // 操作系统信息
        Win32_StartupCommand, // 系统自动启动程序
        Win32_Service, // 系统安装的服务
        Win32_Group, // 系统管理组
        Win32_GroupUser, // 系统组帐号
        Win32_UserAccount, // 用户帐号
        Win32_Process, // 系统进程
        Win32_Thread, // 系统线程
        Win32_Share, // 共享
        Win32_NetworkClient, // 已安装的网络客户端
        Win32_NetworkProtocol, // 已安装的网络协议
        Win32_PnPEntity,//all device
    }
    



}
