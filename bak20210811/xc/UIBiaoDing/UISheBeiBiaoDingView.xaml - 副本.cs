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

        

    }




}
