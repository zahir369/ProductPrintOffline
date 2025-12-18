using DeviceDataMonitorWPF.UIBiaoDing.Config;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
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

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDing.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDing : UserControl
    { 


        public UISheBeiBiaoDing()
        {
            InitializeComponent();


            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item== UISheBeiBiaoDingModel.Intance.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }

            this.TxtProductList.Items.Clear();
            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == UISheBeiBiaoDingModel.Intance.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                }
            }

            this.TxtPortsModeSelect.SelectedIndex = UISheBeiBiaoDingModel.Intance.TxtPortsModeSelect;

            UISheBeiBiaoDingModel.Intance.InitPage(this);
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
            UISheBeiBiaoDingModel.Intance.Closed();
        }

        private void TxtPortsModeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TxtTCPIP == null || this.TxtSerialPorts == null) return;


            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item == UISheBeiBiaoDingModel.Intance.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }


            bool TCPIP_TYPE = (this.TxtPortsModeSelect.SelectedIndex)==(UISheBeiBiaoDingModel.TCPIP_TYPE_NETWORK);
            this.TxtTCPIP.Visibility = TCPIP_TYPE ? Visibility.Visible: Visibility.Hidden;
            this.TxtSerialPorts.Visibility = !TCPIP_TYPE? Visibility.Visible : Visibility.Hidden;
            UISheBeiBiaoDingModel.Intance.TxtPortsModeSelect = this.TxtPortsModeSelect.SelectedIndex;
        }

        private void TxtValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtValueBase.Text == "0" || TxtValueBase.Text == "")
            {
                //读取平均值
                List<SensorGroupDataModel> list = UISheBeiBiaoDingModel.Intance.ProductModelGroupTable.Values.ToList();
                List<SensorData> listSensor = new List<SensorData>();
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
        }

        private void TxtVoltageValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtVoltageValueBase.Text == "0" || TxtVoltageValueBase.Text == "")
            {
                List<SensorGroupDataModel> list = UISheBeiBiaoDingModel.Intance.ProductModelGroupTable.Values.ToList();
                List<SensorData> listSensor = new List<SensorData>();
                foreach (SensorGroupDataModel d in list)
                {
                    listSensor.AddRange(d.ProductTable.Where(w => !w.IsEmpData));
                }
                int min = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Min(w => w.V01.Value);
                int max = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Max(w => w.V01.Value);
                int avg = (int)listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Average(w => w.V01.Value);
                //读取平均值
                this.TxtVoltageValueBase.Text = avg.ToString();
                this.TxtVoltageValueAdd.Text = (max - avg).ToString();
                this.TxtVoltageValueMinus.Text = (avg - min).ToString();
                //UISheBeiBiaoDingModel.Intance.BtnJudge();
            }
        }
    }
}
