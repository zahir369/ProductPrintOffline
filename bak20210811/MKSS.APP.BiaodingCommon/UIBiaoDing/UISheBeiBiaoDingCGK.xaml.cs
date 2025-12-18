using DeviceDataMonitorWPF.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.UIBiaoDing;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// UISheBeiBiaoDingCGK.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingCGK : UserControl
    {
        public UISheBeiBiaoDingCGK()
        {
            InitializeComponent();
            IniData();
        }

        public void IniData()
        {
            this.dataGrid.DataContext = UISheBeiBiaoDingViewModel.Intance.DbService.QueryAllBatch();
        }
          
        //第一个按钮点击事件
        private void BtnDelete_Click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("BtnDelete Click.");
            BatchExt _Batch = this.dataGrid.SelectedItem as BatchExt;
            if (_Batch != null)
            {

                if (UISheBeiBiaoDingViewModel.Intance.SerialComIsOpen)
                {
                    MessageBox.Show("请先关闭连接。");
                    return;
                }

                if (MessageBox.Show(
                    string.Format("确定要删除{0}吗？删除后数据不能恢复。", _Batch.F_BatchName),
                    "确认删除", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    try
                    {
                        this.Cursor = Cursors.AppStarting;
                        UISheBeiBiaoDingViewModel.Intance.DbService.DeleteBatch(_Batch);
                        this.Cursor = Cursors.Arrow;
                        IniData();
                        MessageBox.Show("删除成功");
                    }
                    catch (Exception ex)
                    {
                        IniData();
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            //MessageBox.Show(users[datagrid.SelectedIndex].Name);
        }


        private void OpenBatch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null)
            {
                if (UISheBeiBiaoDingViewModel.Intance.SerialComIsOpen)
                {
                    MessageBox.Show("请先关闭连接。");
                    return;
                }

                string file = BiaoDingSaver.DbNameof(_Batch);
                if (!File.Exists(file))
                {
                    MessageBox.Show("找不到数据文件："+ file + "。");
                    UISheBeiBiaoDingViewModel.Intance.DbService.DeleteBatch(_Batch);
                    this.dataGrid.DataContext = UISheBeiBiaoDingViewModel.Intance.DbService.QueryAllBatch();
                    return;
                }
                WaitWindow.ShowWindow("正在打开", "正在打开" + _Batch.F_BatchName + "......", this);

               


                UISheBeiBiaoDingViewModel.Intance.PageContext.TabItemVoltage.IsSelected = true;
                UISheBeiBiaoDingViewModel.Intance.DbService.OpenBatch(_Batch);
                List<Sensor> sens = UISheBeiBiaoDingViewModel.Intance.DbService.CurrentSensors;
                var addrs = sens.Select(w => w.F_BoardId).Distinct().ToList();
                string.Join(",", addrs);
                UISheBeiBiaoDingViewModel.Intance.TxtSensorGrougAddress = string.Join(",", addrs);
                List<Address> addrList = UISheBeiBiaoDingViewModel.Intance.Address;//必须调用一次，计算内存表
                var datas = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable;
                UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified.Clear();
                foreach (var addr in datas.Keys)
                {
                    var tab = datas[addr]; 
                    foreach (SensorDataX sen in tab.ProductTable)
                    {
                        sen.Reset();
                    }

                }
                if (sens.Count > 0) {
                    List<SensorData>  sensList = UISheBeiBiaoDingViewModel.Intance.DbService.LastSensorData;
                    Dictionary<string, SensorData> dataIdc = new Dictionary<string, SensorData>();
                    foreach (SensorData item in sensList)
                    {
                        if (!dataIdc.ContainsKey(item.F_SensorId)) {
                            dataIdc.Add(item.F_SensorId, item);
                        }
                    }
                    Dictionary<string, Sensor> senIdc = sens.ToDictionary(w => w.F_SensorId, x => x);
                    foreach (var addr in datas.Keys)
                    {
                        DateTime max = DateTime.MinValue;
                        var tab = datas[addr];
                        foreach (SensorDataX sen in tab.ProductTable)
                        {
                            string SensorID = Sensor.CreateCensorId(_Batch.F_BatchId, sen.Address.V, sen.Position);
                            if (dataIdc.ContainsKey(SensorID))
                            {
                                var d = dataIdc[SensorID];
                                if (d.F_AddTime > max) max = d.F_AddTime;
                                sen.V01 = d.V1;
                                sen.V02 = d.V2;
                                sen.V03 = d.V3;
                                sen.V04 = d.V4;
                                sen.V05 = d.V5;
                                sen.V06 = d.V6;
                                sen.V07 = d.V7;
                                sen.V08 = d.V8;
                                sen.V09 = d.V9;
                                sen.V10 = d.V10;
                            }
                        }
                        SensorGroupData dxz = new SensorGroupData(UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified, tab.ProductTable.ToArray(), false, tab.Address, max);
                        UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified.CurrentProductData.TryAdd(tab.Address, dxz);
                    }

                    foreach (var item in sens)
                    {
                        if ((item.V_HEGE + "").ToLower() == "false")
                        {
                            UISheBeiBiaoDingViewModel.Intance.SerialModBus.Qualified[(int)item.F_BoardId, item.F_SlotNO] = false;
                        }
                    }

                }
                
                UISheBeiBiaoDingViewModel.Intance.PageContext_Loaded(null, null);

                if (string.IsNullOrEmpty(_Batch.F_SCRWD_OrderNumber))
                {
                    UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwdTip.Badge = "!";
                    UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwd.ToolTip = "";
                    UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.ToolTip = "";
                    UISheBeiBiaoDingViewModel.Intance.LabelMessage = "已打开已有批次";
                    UISheBeiBiaoDingViewModel.Intance.ScrwEntity = null;
                }
                else
                {
                    ScrwEntity _ScrwEntity = new ScrwEntity()
                    {
                        OrderNumber = _Batch.F_SCRWD_OrderNumber,
                        ProductFullName = _Batch.F_SCRWD_ProductFullName,
                        Qty = _Batch.F_SCRWD_Qty,
                        Date = _Batch.F_SCRWD_Date.ToString(),
                        OrderID = int.Parse(_Batch.F_SCRWD_OrderID),
                        ProductCode = _Batch.F_SCRWD_ProductCode,
                        Department = _Batch.F_SCRWD_Department,
                    };
                    UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwdTip.Badge = "";
                    string title = string.Format(" 型号：{1}，单号：{0}，数量：{2}，日期：{3}", _ScrwEntity.OrderNumber, _ScrwEntity.ProductFullName, _ScrwEntity.Qty, _ScrwEntity.Date); ;
                    UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwd.ToolTip = title;
                    UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.ToolTip = title;
                    UISheBeiBiaoDingViewModel.Intance.LabelMessage = title;
                    UISheBeiBiaoDingViewModel.Intance.ScrwEntity = _ScrwEntity;
                }

                WaitWindow.CloseWindow(this);

            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("dataGrid_SelectionChanged." + this.dataGrid.SelectedItems.Count);
        }
    }
}