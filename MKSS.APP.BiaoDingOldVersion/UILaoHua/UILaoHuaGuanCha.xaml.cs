using DeviceDataMonitorWPF.UIBiaoDing;
using DeviceDataMonitorWPF.UIBiaoDing.Config;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using MKSS.Model;
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
    /// </summary>
    public partial class UILaoHuaGuanCha : UserControl
    {

        public UILaoHuaGuanCha()
        {
            InitializeComponent();


            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item == UILaoHuaGuanChaModel.Intance.TxtSerialPorts)
                {
                   this.TxtSerialPorts.SelectedItem = item;
                }
            }

            this.TxtProductList.Items.Clear();
            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == UILaoHuaGuanChaModel.Intance.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                }
            }

            this.TxtPortsModeSelect.SelectedIndex = UILaoHuaGuanChaModel.Intance.TxtPortsModeSelect;

            UILaoHuaGuanChaModel.Intance.InitPage(this);

            this.TxtBoardCaseList01.Items.Clear();
            this.TxtBoardCaseList02.Items.Clear(); 
            List<BoardCase>  list = UILaoHuaGuanChaModel.Intance.DBListBoardCase;
            for (int i = 0; i < list.Count; i++)
            {
                BoardCase item = list[i];
                if (i < 10)
                {
                    this.TxtBoardCaseList01.Items.Add(new ListBoxItem() { Tag = item, Content= item, IsEnabled = item.EnumUseInFree == EnumUseInFree.Free });
                }
                else if (i < 20 && i>=10)
                {
                    this.TxtBoardCaseList02.Items.Add(new ListBoxItem() { Tag = item, Content = item, IsEnabled = item.EnumUseInFree == EnumUseInFree.Free });
                }
            }
            if(list.Count>0) this.TxtBoardCaseList01.SelectedItem = list[0];
            this.TxtBatchList.Items.Clear();
            this.TxtBatchList.ItemsSource = UILaoHuaGuanChaModel.Intance.DBListBatch;

        }

        private void TxtSensorGrougAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InputAddress(e);
            UILaoHuaGuanChaModel.Intance.CalcAddress();
        }

        private void Txt_InputNumber(object sender, KeyEventArgs e)
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
            UILaoHuaGuanChaModel.Intance.TxtProductListObject = c;
            if (c == null) return;
            this.TxtVoltageValueAdd.Text = c.VoltageValueAdd.ToString();
            this.TxtVoltageValueBase.Text = c.VoltageValueBase.ToString();
            this.TxtVoltageValueMinus.Text = c.VoltageValueMinus.ToString();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UILaoHuaGuanChaModel.Intance.Closed();
        }

        private void TxtPortsModeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TxtTCPIP == null || this.TxtSerialPorts == null) return;
            bool TCPIP_TYPE = (this.TxtPortsModeSelect.SelectedIndex) == (UILaoHuaGuanChaModel.TCPIP_TYPE_NETWORK);
            bool PACKAGE_TYPE = (this.TxtPortsModeSelect.SelectedIndex) == (UILaoHuaGuanChaModel.BATCH_TYPE_NETWORK);
            bool SERIPORTS_TYPE = (this.TxtPortsModeSelect.SelectedIndex) == (UILaoHuaGuanChaModel.SERIPORTS_TYPE_NETWORK);
            this.TxtTCPIP.Visibility = TCPIP_TYPE ? Visibility.Visible : Visibility.Hidden;
            this.TxtSerialPorts.Visibility = SERIPORTS_TYPE ? Visibility.Visible : Visibility.Hidden;
            this.TxtBoardCase.Visibility = PACKAGE_TYPE ? Visibility.Visible : Visibility.Hidden;
            UILaoHuaGuanChaModel.Intance.TxtPortsModeSelect = this.TxtPortsModeSelect.SelectedIndex;
            UILaoHuaGuanChaModel.Intance.CalcAddress();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);
            TxtSensorGrougAddress.Visibility = SERIPORTS_TYPE ? Visibility.Visible : Visibility.Hidden;
            TxtBatchCaseSelect.Visibility = PACKAGE_TYPE ? Visibility.Visible : Visibility.Hidden;
            TxtBatchCaseSelectBox.Visibility = PACKAGE_TYPE ? Visibility.Visible : Visibility.Hidden;
        }


        private void TxtBatchCaseSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoardCase c = this.TxtBatchCaseSelect.SelectedItem as BoardCase;
            if (c == null) return;
            this.TxtBatchCaseSelectFloor.Items.Clear();
            foreach (Board item in UILaoHuaGuanChaModel.Intance.DBBoardCaseDictionary[c])
            {
                bool sel = UILaoHuaGuanChaModel.Intance.SelectBatchBoard.ContainsKey(item)
                    ? UILaoHuaGuanChaModel.Intance.SelectBatchBoard[item] : false;
                this.TxtBatchCaseSelectFloor.Items.Add(new ListBoxItem() { 
                    Tag = item, 
                    Content = item, 
                    IsSelected = sel,
                    IsEnabled = item.EnumUseInFree == EnumUseInFree.Free });
            }
            UILaoHuaGuanChaModel.Intance.CalcAddress();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);
        }

        private void TxtBatchCaseSelectFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Board _Sample = null;
            List<Board> sel = new List<Board>();
            foreach (ListBoxItem item in this.TxtBatchCaseSelectFloor.Items)
            {
                if (item.Content is Board)
                {
                    _Sample = item.Content as Board;
                    if (item.IsSelected)
                        sel.Add(item.Content as Board);
                }
            }
            if (_Sample == null) return;
            BoardCase _BoardCase = UILaoHuaGuanChaModel.Intance.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseId == _Sample.F_BoarCaseId);
            UILaoHuaGuanChaModel.Intance.SelectBatchEventByAddBoard(_BoardCase, sel);
            UILaoHuaGuanChaModel.Intance.CalcAddress();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);
        }


        private void TxtBatchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Batch batch = this.TxtBatchList.SelectedItem as Batch;
            UILaoHuaGuanChaModel.Intance.SelectBatchEventByBatches(batch);
            UILaoHuaGuanChaModel.Intance.CalcAddress();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);
        }
        
        private void TxtBoardCaseSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);
            UILaoHuaGuanChaModel.Intance.TxtBoardCaseSelectObject = new List<BoardCase>();
            List<BoardCase> sel = UILaoHuaGuanChaModel.Intance.TxtBoardCaseSelectObject;
            if (TxtBoardCaseList01 != null) {
                foreach (ListBoxItem item in this.TxtBoardCaseList01.Items)
                {
                    if (item.Content is BoardCase) {
                        if (item.IsSelected)
                            sel.Add(item.Content as BoardCase);
                    }
                }
            }

            if (TxtBoardCaseList02 != null) {
                foreach (ListBoxItem item in this.TxtBoardCaseList02.SelectedItems)
                {
                    if (item.Content is BoardCase)
                    {
                        if (item.IsSelected)
                            sel.Add(item.Content as BoardCase);
                    }
                }
            }

            StringBuilder s = new StringBuilder();
            foreach (BoardCase item in sel)
            {
                s.Append(string.Format("{0},",item.F_BoardCaseAddress.ToString("00")));
            }
            if (this.TxtPortsModeSelect.SelectedIndex == UILaoHuaGuanChaModel.BATCH_TYPE_NETWORK) {
                UILaoHuaGuanChaModel.Intance.TxtSensorGrougAddress = s.ToString(); 
            }
            UILaoHuaGuanChaModel.Intance.SelectBatchEventByAddBoardCase(sel);
            UILaoHuaGuanChaModel.Intance.CalcAddress();
            UILaoHuaGuanChaModel.Intance.EnsureMode(this.TxtPortsModeSelect.SelectedIndex);

            if (TxtBatchCaseSelect != null) {
                this.TxtBatchCaseSelect.Items.Clear();
                foreach (BoardCase item in sel)
                {
                    this.TxtBatchCaseSelect.Items.Add(item);
                }
                if (sel.Count > 0) this.TxtBatchCaseSelect.SelectedItem = sel[0];
            }
  


        }

    }
}