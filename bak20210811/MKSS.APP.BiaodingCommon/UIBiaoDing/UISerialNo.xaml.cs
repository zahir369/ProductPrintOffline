using DeviceDataMonitorWPF.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
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
using System.Windows.Shapes;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    /// <summary>
    /// UISerialNo.xaml 的交互逻辑
    /// </summary>
    public partial class UISerialNo : Window
    {
        public UISerialNo()
        {
            InitializeComponent();
        }

        SensorGroupDataModel GroupData;
        Address Address;
        CQTGridRow Row;
        public void ShowData(SensorGroupDataModel groupData, Address address, string fieldName, CQTGridRow row) { 
            
        }


        public static void ShowSerialNo(SensorGroupDataModel groupData, Address address, string fieldName, CQTGridRow row)
        {

            if (groupData == null || address == null || row == null) return;
            if ( address.V == 0 || row.WEIZHI==0) return;
            if (fieldName == "XULIEHAO") {
                UISerialNo ww = new UISerialNo();
                ww.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
                ww.ShowInTaskbar = false;
                ww.Title = string.Format("#{0}设备串号", address.V);
                ww.TxtOldValueDesc.Text = string.Format("位置{0}原串号", row.WEIZHI);
                ww.TxtNewValueDesc.Text = string.Format("位置{0}新串号", row.WEIZHI);
                ww.TabItemMulti.Visibility = Visibility.Hidden;
                ww.TabItemDateTime.Visibility = Visibility.Hidden;
                ww.TxtOldValue.Text = row.XULIEHAO;
                ww.TxtNewValue.Text = row.XULIEHAO;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.TabItemSingle.IsSelected = true;
                ww.GroupData = groupData;
                ww.Address = address;
                ww.Row = row;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.Show();
                ww.ShowData(groupData, address, fieldName, row);
            }

        }

        public static void SetSerialNoAll( )
        {

            ScrwEntity en = UISheBeiBiaoDingViewModel.Intance.ScrwEntity;
            if (en == null)
            {
                MessageBox.Show("请选择生产任务单");
                return;
            }
            
            UISerialNo ww = new UISerialNo();
            ww.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
            ww.ShowInTaskbar = false;
            ww.Title = string.Format("设备串号改写" );
            ww.TabItemSingle.Visibility = Visibility.Hidden;
            ww.TabItemDateTime.Visibility = Visibility.Hidden;
            ww.TxtOldValue1.Text = "自动";
            ww.TxtNewValue1.Text = "自增";
            ww.TabItemMulti.IsSelected = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show(); 

        }


        public static void SetSetTime()
        {

            UISerialNo ww = new UISerialNo();
            ww.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
            ww.ShowInTaskbar = false;
            ww.Title = string.Format("设备出厂日期、当前时间改写");
            ww.TabItemSingle.Visibility = Visibility.Hidden;
            ww.TabItemMulti.Visibility = Visibility.Hidden;
            ww.TabItemDateTime.IsSelected = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show();

        }

      
        private void BtnSetSingle_Click(object sender, RoutedEventArgs e)
        {
            ulong str = 0;
            try
            {
                str = ulong.Parse(TxtNewValue.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("请输入有效的设备串号，串号只支持数字");
                return;
            } 
            Task t = new Task(() => {
                Dispatcher.Invoke(() =>
                {
                    this.Cursor = Cursors.Wait;
                    this.Visibility = Visibility.Hidden;
                    WaitWindow.ShowWindow("正在写入", "正在写入串号......", this);
                });

                try
                {
                    UISheBeiBiaoDingViewModel.Intance.SerialModBus.SetSerialNo(Address, Row.WEIZHI, str,true);
                }
                catch (Exception ex)
                {
                    WaitWindow.CloseWindow(this);
                    MessageBox.Show(ex.Message);
                    return;
                }
               
                Dispatcher.Invoke(() =>
                {
                    WaitWindow.CloseWindow(this);
                    this.Cursor = Cursors.Arrow;
                    this.Close();
                });
            });
            t.Start();
        }

        private void BtnSetMulti_Click(object sender, RoutedEventArgs e)
        {


            ScrwEntity en = UISheBeiBiaoDingViewModel.Intance.ScrwEntity;
            if (en == null)
            {
                MessageBox.Show("请选择生产任务单");
                return;
            }


            bool overwrite = OverWriteCheckBox.IsChecked != null && OverWriteCheckBox.IsChecked.Value;
            Task t = new Task(() => {

                Dispatcher.Invoke(() =>
                {
                    this.Cursor = Cursors.Wait;
                    this.Visibility = Visibility.Hidden;
                    WaitWindow.ShowWindow("正在写入", "正在写入串号......", this);
                });

                var ProductModelGroupTable = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable;
                int rowno = 1;
                for (int i = 0; i < ProductModelGroupTable.Count; i++)
                {
                    SensorGroupDataModel mm = ProductModelGroupTable[i];
                    if (mm.Address.V == 0) continue;
                    //if (mm.Empty) continue;
                    for (int j = 0; j < mm.ProductTable.Count; j++)
                    {
                        rowno++;
                        string Serial_old = ProductModelGroupTable[i].ProductTable[j].Serial;
                        if (overwrite || string.IsNullOrEmpty(Serial_old) || Serial_old == "0")
                        {
                            ulong no = UISheBeiBiaoDingViewModel.Intance.ScrwdService.NextSerialNo(en.OrderNumber, 1).FirstOrDefault();
                            Dispatcher.Invoke(() =>
                            {
                                this.Cursor = Cursors.Wait;
                                this.Visibility = Visibility.Hidden;
                                WaitWindow.ShowWindow("正在写入", "正在写入串号【" + mm.Address + "-" + ProductModelGroupTable[i].ProductTable[j].Position + "," + no + "】......", this);
                            });
                            System.Threading.Thread.Sleep(100);
                            UISheBeiBiaoDingViewModel.Intance.SerialModBus.SetSerialNo(mm.Address, ProductModelGroupTable[i].ProductTable[j].Position, no, j==0);
                            System.Threading.Thread.Sleep(500);
                        }
                        else {
                            Dispatcher.Invoke(() =>
                            {
                                this.Cursor = Cursors.Wait;
                                this.Visibility = Visibility.Hidden;
                                WaitWindow.ShowWindow("串号已存在", "直接跳过位置【" + mm.Address + "-" + ProductModelGroupTable[i].ProductTable[j].Position + "," + ProductModelGroupTable[i].ProductTable[j].Serial + "】......", this);
                            });
                        }
                    }
                }

                WaitWindow.ShowWindow("正在刷新", "正在刷新串号......", this);

                UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNoContinue = true;
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNo().Wait();
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNoContinue = false;

                Dispatcher.Invoke(() =>
                {
                    WaitWindow.CloseWindow(this);
                    this.Cursor = Cursors.Arrow;
                    this.Close();
                });

            });
            t.Start();
        }

        

        private void BtnSetDateTimeMulti_Click(object sender, RoutedEventArgs e)
        {
            string str = TxtTqDateTime.Text;
            Task t = new Task(() => {

                Dispatcher.Invoke(() =>
                {
                    this.Cursor = Cursors.Wait;
                    this.Visibility = Visibility.Hidden;
                    WaitWindow.ShowWindow("正在校时", "正在校时、写入出厂时间......", this);
                });
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.SetDeviceDateTime(DateTime.Now, 1);
                System.Threading.Thread.Sleep(1000);
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.SetManufacturingDate(DateTime.Now.AddDays(1 * int.Parse(str)), 1);
                System.Threading.Thread.Sleep(1000);

                WaitWindow.ShowWindow("正在刷新", "正在刷新设备时间......", this);
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadDeviceDateTime().Wait();
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadManufacturingDate().Wait();

                Dispatcher.Invoke(() =>
                {
                    this.Cursor = Cursors.Arrow;
                    WaitWindow.CloseWindow(this);
                    this.Close();
                });

            });
            t.Start();
        }

        private void BtnReadSerial_Click(object sender, RoutedEventArgs e)
        {
            if (!UISheBeiBiaoDingViewModel.Intance.SerialModBus.IsOpen) return;
            this.Hide();
            WaitWindow.ShowWindow("正在刷新", "正在刷新串号......", this);
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNoContinue = true;
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNo().Wait();
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadSerialNoContinue = false;
            WaitWindow.CloseWindow(this);
            this.Close();
        }

        private void BtnReadTime_Click(object sender, RoutedEventArgs e)
        {
            if (!UISheBeiBiaoDingViewModel.Intance.SerialModBus.IsOpen) return;
            this.Hide();
            WaitWindow.ShowWindow("正在刷新", "正在刷新设备时间......", this);
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadDeviceDateTimeContinue = true;
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadDeviceDateTime().Wait();
            UISheBeiBiaoDingViewModel.Intance.SerialModBus.ReadDeviceDateTimeContinue = false;
            WaitWindow.CloseWindow(this);
            this.Close();
        }
    }
}
