using DeviceDataMonitorWPF.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.LaoHua;
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
                    UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetSerialNo(Address, Row.WEIZHI, str,true);
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


                List<Task> allTask = new List<Task>();
                var ProductModelGroupTable = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable;
                int rowno = 1;
                for (int i = 0; i < ProductModelGroupTable.Count; i++)
                {

                    SensorGroupDataModel mm = ProductModelGroupTable[i];
                    if (mm.Address.V == 0) continue;
                    //if (mm.Empty) continue;

                    List<SerialNoByConnectionEntity> TaskList = new List<SerialNoByConnectionEntity>();
                    for (int j = 0; j < mm.ProductTable.Count; j++)
                    {
                        rowno++;
                        string Serial_old = ProductModelGroupTable[i].ProductTable[j].Serial;
                        if (overwrite || string.IsNullOrEmpty(Serial_old) || Serial_old == "0")
                        {
                            SerialNoByConnectionEntity enNo = new SerialNoByConnectionEntity() { Address= mm.Address, Position= ProductModelGroupTable[i].ProductTable[j].Position, SerialNo=0 };
                            TaskList.Add(enNo);
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

                    WaitWindow.ShowWindow("正在写入", "正在获取串号【" + en.OrderNumber + "-" + TaskList.Count + "个】......", DeviceDataMonitorWPF.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                    List<ulong> SerialNoList = UISheBeiBiaoDingViewModel.Intance.ScrwdService.NextSerialNo(en.OrderNumber, TaskList.Count);
                    
                    for (int n = 0; n < SerialNoList.Count; n++)
                    {
                        TaskList[n].SerialNo = SerialNoList[n];
                    }
                    
                    Task t_writer = new Task(() => {
                        WaitWindow.ShowWindow("正在写入", "正在写入串号【" + en.OrderNumber + "-" + TaskList.Count + "个】......", this);
                        UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetSerialNoByConnection(TaskList);
                    });
                    t_writer.Start();
                    allTask.Add(t_writer);

                }

                Task.WaitAll(allTask.ToArray());//等待所有任务完成

                WaitWindow.ShowWindow("正在读取", "正在读取串号......", this);

                foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadSerialNoContinue = true;
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadSerialNo().Wait();
                foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadSerialNoContinue = false;

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
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetDeviceDateTime(DateTime.Now, 1);
                System.Threading.Thread.Sleep(1000);
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetManufacturingDate(DateTime.Now.AddDays(1 * int.Parse(str)), 1);
                System.Threading.Thread.Sleep(1000);

                WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this);
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadDeviceDateTime().Wait();
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadManufacturingDate().Wait();

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
            if (!UISheBeiBiaoDingViewModel.Intance.ConnectionPool.IsOpen) return;
            this.Hide();
            WaitWindow.ShowWindow("正在读取", "正在读取串号......", this);
            foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadSerialNoContinue = true;
            UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadSerialNo().Wait();
            foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadSerialNoContinue = false;
            WaitWindow.CloseWindow(this);
            this.Close();
        }

        private void BtnReadTime_Click(object sender, RoutedEventArgs e)
        {
            if (!UISheBeiBiaoDingViewModel.Intance.ConnectionPool.IsOpen) return;
            this.Hide();
            WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this);
            foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = true;
            UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadDeviceDateTime().Wait();
            foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = false;
            WaitWindow.CloseWindow(this);
            this.Close();
        }
    }
}
