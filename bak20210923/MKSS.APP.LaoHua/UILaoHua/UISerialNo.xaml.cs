using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.Util;
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

        C10GridRow Row;
        PageBoardItem GroupData; 
        public static void ShowSerialNo(PageBoardItem groupData,   string fieldName, C10GridRow row)
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }

            if (groupData == null || row == null) return;
            if (groupData.Address == 0 || row.WEIZHI==0) return;
            if (fieldName == "XULIEHAO") {
                UISerialNo ww = new UISerialNo();
                ww.Owner = UILaoHuaGuanChaModel.Intance.MainView;
                ww.ShowInTaskbar = false;
                ww.Title = string.Format("#{0}设备串号", groupData.Address);
                ww.TxtOldValueDesc.Text = string.Format("位置{0}原串号", row.WEIZHI);
                ww.TxtNewValueDesc.Text = string.Format("位置{0}新串号", row.WEIZHI);
                ww.TabItemMulti.Visibility = Visibility.Hidden;
                ww.TabItemDateTime.Visibility = Visibility.Hidden;
                ww.TxtOldValue.Text = row.XULIEHAO;
                ww.TxtNewValue.Text = row.XULIEHAO;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.TabItemSingle.IsSelected = true;
                ww.GroupData = groupData;
                ww.Row = row;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.Show();
                //ww.ShowData(groupData,  fieldName, row);
            }

        }

        public static void SetSerialNoAll( )
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }
            
            UISerialNo ww = new UISerialNo();
            ww.Owner = UILaoHuaGuanChaModel.Intance.MainView;
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

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }

            UISerialNo ww = new UISerialNo();
            ww.Owner = UILaoHuaGuanChaModel.Intance.MainView;
            ww.ShowInTaskbar = false;
            ww.Title = string.Format("设备出厂日期、当前时间改写");
            ww.TabItemSingle.Visibility = Visibility.Hidden;
            ww.TabItemMulti.Visibility = Visibility.Hidden;
            ww.TabItemDateTime.IsSelected = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show();

        }

      
        //private void BtnSetSingle_Click(object sender, RoutedEventArgs e)
        //{

            //Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            //if (en == null)
            //{
            //    MessageBox.Show("请选择批次");
            //    return;
            //}

            //ulong str = 0;
            //try
            //{
            //    str = ulong.Parse(TxtNewValue.Text);
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("请输入有效的设备串号，串号只支持数字");
            //    return;
            //}

            //ScrwdNoEntity scrw = new ScrwdNoEntity(en.F_SCRWD_OrderNumber);
            //if ((long)str < scrw.GetMin() || (long)str > scrw.GetMax()) {
            //    MessageBox.Show("请输入有效的设备串号，串号范围：" + scrw.GetMin() + "-" + scrw.GetMax());
            //    return;
            //}

            //ulong strOld = 0;
            //try
            //{
            //    strOld = ulong.Parse(TxtOldValue.Text);
            //}
            //catch (Exception)
            //{
            //}

            //if (UILaoHuaGuanChaData.Instance.Exists(en.F_SCRWD_OrderNumber, (long)str, (long)strOld)) {
            //    MessageBox.Show("串号已存在，：" + str);
            //}

            //Task t = new Task(() =>
            //{

            //    Dispatcher.Invoke(() =>
            //    {
            //        this.Cursor = Cursors.Wait;
            //        this.Visibility = Visibility.Hidden;
            //        WaitWindow.ShowWindow("正在写入", "正在写入串号......", this);
            //    });

            //    try
            //    {
            //        UILaoHuaGuanChaData.Instance.NewSerialNo(en.F_SCRWD_OrderNumber, (long)str);
            //        UILaoHuaGuanChaModel.Intance.SerialModBus.SetSerialNo(Address, Row.WEIZHI, str, true);
            //    }
            //    catch (Exception ex)
            //    {
            //        WaitWindow.CloseWindow(this);
            //        MessageBox.Show(ex.Message);
            //        return;
            //    }

            //    Dispatcher.Invoke(() =>
            //    {
            //        WaitWindow.CloseWindow(this);
            //        this.Cursor = Cursors.Arrow;
            //        this.Close();
            //    });
            //});
            //t.Start();

        //}

        private void BtnSetMulti_Click(object sender, RoutedEventArgs e)
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }


            bool overwrite = OverWriteCheckBox.IsChecked != null && OverWriteCheckBox.IsChecked.Value;
            EnumSerialCoded sql = EnumSerialCoded.CodeAll;
            string msgSucess = "";
            if (overwrite) {
                sql = EnumSerialCoded.CodeAll;
                msgSucess = "已开始生成批次"+ en.F_BatchName + "的所有串号，串号将在10分钟后生成完成，请十分钟后刷新结果。";
            }
            else {
                sql = EnumSerialCoded.CodeInvalid;
                msgSucess = "将开始生成批次" + en.F_BatchName + "的所有无效串号，串号将在10分钟后生成完成，请十分钟后刷新结果。";
            }
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_SerialCoded=" + (int)sql + " where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else {

                MessageBox.Show("生成串号启动失败");
                return;
            }
            
        }



        private void BtnSetDateTimeMulti_Click(object sender, RoutedEventArgs e)
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }



            bool overwrite = OverWriteCheckBox.IsChecked != null && OverWriteCheckBox.IsChecked.Value;
            EnumDeviceTimed sql = EnumDeviceTimed.CodeAll;
            string msgSucess = "";
            if (overwrite)
            {
                sql = EnumDeviceTimed.CodeAll;
                msgSucess = "已开始生成批次" + en.F_BatchName + "的所有设备时间，设备时间将在10分钟后生成完成，请十分钟后刷新结果。";
            }
            else
            {
                sql = EnumDeviceTimed.CodeInvalid;
                msgSucess = "将开始生成批次" + en.F_BatchName + "的所有无效设备时间，设备时间将在10分钟后生成完成，请十分钟后刷新结果。";
            }
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimed=" + (int)sql + " where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show(msgSucess);
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }


        }

        private void BtnReadSerial_Click(object sender, RoutedEventArgs e)
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }

            DateTime test = new DateTime(1000, 1, 1);
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimedUpdateTime=" + test.ToString() + " where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show("已开始读取批次" + en.F_BatchName + "的所有设备时间，设备时间将在10分钟后读取完成，请十分钟后刷新结果。");
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }

        }

        private void BtnReadTime_Click(object sender, RoutedEventArgs e)
        {

            Batch en = UILaoHuaGuanChaC10Model.Instance.View.CurrentBatch;
            if (en == null)
            {
                MessageBox.Show("请选择批次");
                return;
            }


            DateTime test = new DateTime(1000, 1, 1);
            if (UILaoHuaGuanChaData.Instance.ExecuteCommandAsync(
                string.Format("update pd_batch set F_DeviceTimedUpdateTime=" + test.ToString() + " where F_BatchId=" + en.F_BatchId + ";")
                ) > 0)
            {
                MessageBox.Show("已开始读取批次" + en.F_BatchName + "的所有设备时间，设备时间将在10分钟后读取完成，请十分钟后刷新结果。");
                return;
            }
            else
            {

                MessageBox.Show("生成设备时间启动失败");
                return;
            }


        }

    }
}
