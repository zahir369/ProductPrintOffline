using MKSS.APP.BiaodingAlcohol.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Print;
using MKSS.APP.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Model.Laohua;
using MKSS.Service.LaoHua;
using MKSS.Service.UIBiaoDing;
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

namespace MKSS.APP.UIBiaoDing
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
            if (address.V == 0 || row.WEIZHI == 0) return;
            if (fieldName == "XULIEHAO")
            {
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

            SerialNoRuleEntity en = UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity;
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


        public static void SetSetTime(SensorGroupDataModel groupData = null)
        {

            UISerialNo ww = new UISerialNo();
            ww.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
            ww.ShowInTaskbar = false;
            ww.Title = string.Format("设备出厂日期、当前时间改写");
            if (groupData != null)
            {
                ww.GroupData = groupData;
                ww.Address = groupData.Address;
                ww.Title = string.Format("设备出厂日期、当前时间改写-"+ groupData.Address.ToString());
            }
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


            SerialNoRuleEntity en = UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity;
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

                    List<int> slotNo = new List<int>();
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

                    

                    var addr = mm.Address;
                    long F_BoardCaseId = 0;
                    long F_BoardId = 0;
                    if (UISheBeiBiaoDingViewModel.Intance.BoardCaseSelectEntity.DBListBoardCase != null)
                    {
                        BoardCase _BoardCase = UISheBeiBiaoDingViewModel.Intance.BoardCaseSelectEntity.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == addr.APP);
                        F_BoardCaseId = _BoardCase.F_BoardCaseId;
                        Board _Board = UISheBeiBiaoDingViewModel.Intance.BoardCaseSelectEntity.DBBoardCaseDictionary[_BoardCase].FirstOrDefault(w => w.AddrList.Contains(addr.Vb));
                        if (_Board != null)
                        {
                            F_BoardId = _Board.F_BoardId;
                        }
                    }

                    WaitWindow.ShowWindow("正在写入", "正在获取串号【" + en.OrderNumber + "-" + TaskList.Count + "个】......", MKSS.APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.PageContext);
                    List<ulong> SerialNoList = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.NextSerialNo(en,en.OrderNumber, TaskList.Count, F_BoardCaseId, F_BoardId, TaskList.Select(w=>w.F_SlotNO).ToList());
                    
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
                UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadSerialNo();
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

                if (this.GroupData == null)
                {
                    UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetReadDeviceDateTime(DateTime.Now, DateTime.Now.AddDays(1 * int.Parse(str)), 1);
                }
                else {
                    UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetReadDeviceDateTime(this.GroupData, DateTime.Now, DateTime.Now.AddDays(1 * int.Parse(str)), 1);
                }

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
            UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadSerialNo();
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
            UISheBeiBiaoDingViewModel.Intance.ConnectionPool.ReadDeviceDateTime();
            foreach (var conn in UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = false;
            WaitWindow.CloseWindow(this);
            this.Close();
        }

        private void BtnTmpPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtNewValue.Text)|| (TxtNewValue.Text.Length!=12&& TxtNewValue.Text.Length != 14)) {
                MessageBox.Show("请输入12位或14位串号");
                return;
            }
            PrintPreviewInner(BarCodeMode.SerialCode);
            this.Close();
        }

        private void PrintPreviewInner(BarCodeMode BarMode)
        {
            DocumentMiniSNData data = new DocumentMiniSNData()
            {
                SN_LIST = new List<SN_ITEM>()
            };
            data.SN_LIST.Add(new SN_ITEM()
            {
                SN = TxtNewValue.Text,
                F_SerialValidCode = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.SerialValidCodeOfSerialNo(TxtNewValue.Text),
                ADDR = 18,
                APP = 0,
                POS = 10  
            });

            if (previewWndPub != null)
            {
                previewWndPub.Close();
                previewWndPub = null;
            }
            PrintPreviewWindow previewWnd = new PrintPreviewWindow("通用单联标签.btw", data, BarMode);
            previewWnd.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
            previewWnd.ShowInTaskbar = false;
            previewWndPub = previewWnd;
            previewWnd.ShowDialog();

        }
        PrintPreviewWindow previewWndPub = null;

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.TxtNewValue.Text.Length >= 13)
            {
                this.TxtNewValue.Text = MKSS.Service.UIBiaoDing.ISO7064.CalculateHybridSystemCheckDigit(this.TxtNewValue.Text.Substring(0,13), ISO7064.NumericCharSet);
            }
        }

    }
}
