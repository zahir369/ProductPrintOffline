using MKSS.APP.BiaodingAlcohol.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Print;
using MKSS.APP.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Service.UIBiaoDing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDingCQTGrid.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingCQTGrid : UserControl
    { 
        List<CQTGridRow> BasdeData = new List<CQTGridRow>();
        public UISheBeiBiaoDingCQTGrid()
        {
            InitializeComponent();
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new CQTGridRow() { WEIZHI = i + 1 });
            }
            this.dataGrid.DataContext = BasdeData;
        }

        /// <summary>
        ///  内存表索引
        /// </summary>
        public int GroupIndex { get; set; }
        /// <summary>
        ///  采集板地址
        /// </summary>
        public Address Address { get { return GroupData == null ? new Address(0,0) : GroupData.Address; } }
        /// <summary>
        ///  采集板地址
        /// </summary>
        public SensorGroupDataModel GroupData { get { return UISheBeiBiaoDingViewModel.Intance.GroupDataOf(GroupIndex); } }

        public void RefreshData()
        {
            SensorGroupDataModel data = GroupData;
            AddressString.ToolTip = string.Format("刷新时间：{0}", data.LastReceiveTime);
            this.DataContext = data;
            this.AddressString.Text = Address.ToString();
            List<SensorDataX> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].XULIEHAO = string.IsNullOrEmpty((tables[i].Serial)) ? null : (tables[i].Serial);
                    BasdeData[i].XULIEHAO_HEGE = !tables[i].SerialError;
                    BasdeData[i].LIANGCHENG = tables[i].LiangCheng==null ? null : SensorDataX.VS(tables[i].LiangCheng);
                    BasdeData[i].DIANYAFANWEI = string.IsNullOrEmpty((tables[i].DianYaRange)) ? null : (tables[i].DianYaRange);
                    BasdeData[i].ZIJIAOZHUN = tables[i].AutoAdjustState==null ? null : (tables[i].AutoAdjustState).ToString();
                    BasdeData[i].SHIJIAN = tables[i].DeviceDateTime == null ? null : (tables[i].DeviceDateTime).ToString();
                    BasdeData[i].SHUCHUDIANYA = tables[i].OutPutVolage==null ? null : SensorDataX.VS(tables[i].OutPutVolage);
                    BasdeData[i].HEGE = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified[data.Address.APP, data.Address.V, tables[i].Position];

                }
            }
        }


        private void BtnSetQualified_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int WEIZHI = (int)btn.Tag;
            SensorGroupDataModel data = (SensorGroupDataModel)this.DataContext;
            SwithQualified(WEIZHI);
        }

        private void dataGrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (dataGrid.SelectedItem == null) return;
            if (e.Key == Key.Space)
            {
                C05GridRow row = dataGrid.SelectedItem as C05GridRow;
                SwithQualified(row.WEIZHI);
            }
            if (e.Key == Key.Down)
            {

            }
        }

        void SwithQualified(int WEIZHI)
        {
            SensorGroupDataModel data = (SensorGroupDataModel)this.DataContext;
            if (WEIZHI > 0 && WEIZHI <= data.ProductTable.Count)
            {
                if (!data.ProductTable[WEIZHI - 1].IsEmpSensor)
                {
                    bool Qualified = !UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.NotQualified(data.Address.APP, data.Address.V, WEIZHI);
                    UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified[data.Address.APP, data.Address.V, WEIZHI] = !Qualified;
                    var qq = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified[data.Address.APP, data.Address.V, WEIZHI];
                    UISheBeiBiaoDingViewModel.Intance.DbService.SetHEGE(GroupData.Address, WEIZHI, qq.Value);
                    UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.SetHEGE(UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity, data.ProductTable[WEIZHI - 1], qq.Value);

                    BasdeData[WEIZHI - 1].HEGE = qq;
                    UISheBeiBiaoDingViewModel.Intance.PageContext_Loaded(null, null);
                }
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;

            while (target != null)
            {
                if (target is DataGridCell)
                {
                    DataGridCell cell = ((DataGridCell)target);
                    if (cell.Column is DataGridTextColumn)
                    {
                        DataGridTextColumn c1 = cell.Column as DataGridTextColumn;
                        string path = ((Binding)c1.Binding).Path.Path;
                        UISerialNo.ShowSerialNo(GroupData, GroupData.Address, path, cell.DataContext as CQTGridRow);
                    }
                    if (target is DataGridTemplateColumn)
                    {
                        DataGridTemplateColumn c1 = cell.Column as DataGridTemplateColumn;
                        string path = ((Binding)c1.ClipboardContentBinding).Path.Path;
                        UISerialNo.ShowSerialNo(GroupData, GroupData.Address, path, cell.DataContext as CQTGridRow);
                    }
                    break;
                }
                target = VisualTreeHelper.GetParent(target);
            }
        }

        private void btnWriteTime_Click(object sender, RoutedEventArgs e)
        {
            UISheBeiBiaoDingViewModel.Intance.BtnSetTimeSingleFloor(this.GroupData);
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e) {
            PrintPreviewInner(BarCodeMode.SerialCode,false);
        }
        private void btnPrintPreviewV_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewInner(BarCodeMode.SerialCodeWithValidCode, false);
        }

        private void btnPrintPreviewSelect_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewInner(BarCodeMode.SerialCode,true);
        }
        private void btnPrintPreviewSelectV_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewInner(BarCodeMode.SerialCodeWithValidCode, true);
        }
        private void PrintPreviewInner(BarCodeMode BarMode,bool selected)
        {


            WaitWindow.ShowWindow("正在打开", "正在收集串号数据......", this);
            DocumentMiniSNData data = new DocumentMiniSNData() { 
                SN_LIST = new List<SN_ITEM>()
            };


            if (selected)
            {

                foreach (var item in this.dataGrid.SelectedItems)
                {
                    CQTGridRow row = item as CQTGridRow;
                    if (string.IsNullOrEmpty(row.XULIEHAO)) continue;

                    data.SN_LIST.Add(new SN_ITEM()
                    {
                        SN = row.XULIEHAO,
                        ADDR = GroupData.Address.V,
                        APP = GroupData.Address.APP,
                        POS = row.WEIZHI
                    });
                }
                if (data.SN_LIST.Count == 0)
                {

                    for (int i = 0; i < this.dataGrid.SelectedItems.Count; i++)
                    {
                        data.SN_LIST.Add(new SN_ITEM()
                        {
                            SN = "00000000000" + i,
                            ADDR = 18,
                            APP = GroupData.Address.APP,
                            POS = 10 + i
                        });
                    }

                }
                else
                {

                }

            }
            else {

                for (int i = 0; i < UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Count; i++)
                {
                    SensorGroupDataModel mm = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable[i];
                    if (mm.Address.V == 0) continue;
                    for (int j = 0; j < mm.ProductTable.Count; j++)
                    {
                        var rowData = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable[i].ProductTable[j];
                        if (string.IsNullOrEmpty(rowData.Serial)) continue;
                        data.SN_LIST.Add(new SN_ITEM()
                        {
                            SN = rowData.Serial,
                            ADDR = GroupData.Address.V,
                            APP = GroupData.Address.APP,
                            POS = rowData.Position
                        });
                    }
                }

            }

            Dictionary<string,int> vs = data.SN_LIST.Select(w=>w.SN).Distinct().ToDictionary(x => x, x => 9999);
            UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.SerialValidCodeOfSerialNo(vs);
            foreach (var item in data.SN_LIST)
            {
                if (vs.ContainsKey(item.SN)) item.F_SerialValidCode = vs[item.SN];
            }

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

    }

    public class CQTGridRow : INotifyPropertyChanged
    {
        public CQTGridRow()
        {
            HEGE = true;
        }
        public int WEIZHI { get; set; }
        public string XULIEHAO { get; set; }
        public bool XULIEHAO_HEGE { get; set; }
        public string SHIJIAN { get; set; }
        public bool SHIJIAN_HEGE
        {
            get
            {
                if (string.IsNullOrEmpty(SHIJIAN)) return true;
                if (SHIJIAN.StartsWith("0000-")) return true;
                if (SHIJIAN.StartsWith("0-")) return true;
                if (SHIJIAN.StartsWith("170-")) return true;
                if (SHIJIAN.StartsWith("0170-")) return true; 
                DateTime dt = DateTime.MinValue;
                if (DateTime.TryParse(SHIJIAN, out dt))
                {
                    if (Math.Abs((dt - DateTime.Now).TotalMinutes) < 15)
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
                return false;
            }
        }
        public string LIANGCHENG { get; set; }
        public string DIANYAFANWEI { get; set; }
        public string ZIJIAOZHUN { get; set; }
        public string SHUCHUDIANYA { get; set; }
        public bool? HEGE { get; set; }
        public string HEGE_STR { get { return HEGE == null || HEGE.Value ? "" : "X"; } }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
