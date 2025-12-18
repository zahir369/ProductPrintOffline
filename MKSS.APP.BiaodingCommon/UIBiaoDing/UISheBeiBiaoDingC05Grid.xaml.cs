using MKSS.APP.UIBiaoDing.Util;
using MKSS.APP.BiaodingAlcohol.UIBiaoDing;
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
    /// UISheBeiBiaoDingC05Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingC05Grid : UserControl
    { 
        public List<C05GridRow> BasdeData = new List<C05GridRow>();
        public UISheBeiBiaoDingC05Grid()
        {
            InitializeComponent();
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new C05GridRow() { WEIZHI = i + 1 });
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
                if (tables.Count > i) {
                    BasdeData[i].XINHAO_AD = data.Empty || tables[i].IsEmpValue(tables[i].V01) ? null : SensorDataX.VS(tables[i].V01);
                    BasdeData[i].XINHAO_ADHEGE = data.Empty || tables[i].IsEmpValue(tables[i].V01) ? null : tables[i].IsQualifiedV1;
                    BasdeData[i].CANKAO_AD = data.Empty || tables[i].IsEmpValue(tables[i].V02) ? null : SensorDataX.VS(tables[i].V02);
                    BasdeData[i].WENDU_AD = data.Empty || tables[i].IsEmpValue(tables[i].V04) ? null : SensorDataX.VS(tables[i].V04);
                    BasdeData[i].NONGDU = data.Empty || tables[i].IsEmpValue(tables[i].V07) ? null : SensorDataX.VS(tables[i].V07);
                    BasdeData[i].NONGDUHEGE = data.Empty || tables[i].IsEmpValue(tables[i].V07) ? null : tables[i].IsQualifiedV7;
                    BasdeData[i].WENDU1 = data.Empty || tables[i].IsEmpValue(tables[i].V08) ? null : SensorDataX.VS(tables[i].V08);
                    BasdeData[i].HEGE = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified[data.Address.APP, data.Address.V, tables[i].Position];
                    BasdeData[i].DATA = tables[i];
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
            if (e.Key == Key.Space) {
                C05GridRow row = dataGrid.SelectedItem as C05GridRow;
                SwithQualified(row.WEIZHI);
            }
            if (e.Key == Key.Down)
            {
                 
            }
        }

        void SwithQualified(int WEIZHI) {
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SensorGroupDataModel ss = this.DataContext as SensorGroupDataModel;
            if (ss == null) return;
            List<SensorDataX> list = new List<SensorDataX>();
            foreach (C05GridRow item in dataGrid.SelectedItems)
            {
                list.Add(item.DATA);
            }
            UISheBeiBiaoDingViewModel.Intance.ResetSelectedData(ss.Address.V, list,5);
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
                    if (cell.Column  is DataGridTextColumn) {
                        DataGridTextColumn c1 = cell.Column as DataGridTextColumn;
                        string path = ((Binding)c1.Binding).Path.Path;
                        UISheBeiBiaoChartWindows.ShowChart(  GroupData,GroupData.Address, path);
                    }
                    if (target is DataGridTemplateColumn)
                    {
                        DataGridTemplateColumn c1 = cell.Column as DataGridTemplateColumn;
                        string path = ((Binding)c1.ClipboardContentBinding).Path.Path;
                        UISheBeiBiaoChartWindows.ShowChart(  GroupData,GroupData.Address, path);
                    }
                    break;
                }
                target = VisualTreeHelper.GetParent(target);
            }
        }

    }

    public class C05GridRow : INotifyPropertyChanged
    {
        public C05GridRow() {
            HEGE = null;
        }
        public int WEIZHI { get; set; }
        public string XINHAO_AD { get; set; }
        public bool? XINHAO_ADHEGE { get; set; }
        public string CANKAO_AD { get; set; }
        public string WENDU_AD { get; set; }
        public string NONGDU { get; set; }
        public bool? NONGDUHEGE { get; set; }
        public string WENDU1 { get; set; }
        public bool? HEGE { get; set; }
        public string HEGE_STR { get { return HEGE == null || HEGE.Value ? "" : "X"; } }
        public event PropertyChangedEventHandler PropertyChanged;
        public SensorDataX DATA { get; set; }

    }


}
