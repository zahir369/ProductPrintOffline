using DeviceDataMonitorWPF.UIBiaoDing.Util;
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

namespace DeviceDataMonitorWPF.UIBiaoDing
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
        public Address Address { get { return GroupData == null ? new Address(0) : GroupData.Address; } }
        /// <summary>
        ///  采集板地址
        /// </summary>
        public SensorGroupDataModel GroupData { get { return UISheBeiBiaoDingViewModel.Intance.GroupDataOf(GroupIndex); } }

        public void RefreshData()
        {
            SensorGroupDataModel data = GroupData;
            this.DataContext = data;
            this.AddressString.Text = Address.V == 0 ? "" : Address.V.ToString("00");
            List<SensorDataX> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].XULIEHAO = string.IsNullOrEmpty((tables[i].Serial)) ? null : (tables[i].Serial);
                    BasdeData[i].LIANGCHENG = tables[i].LiangCheng==null ? null : SensorDataX.VS(tables[i].LiangCheng);
                    BasdeData[i].DIANYAFANWEI = string.IsNullOrEmpty((tables[i].DianYaRange)) ? null : (tables[i].DianYaRange);
                    BasdeData[i].ZIJIAOZHUN = tables[i].AutoAdjustState==null ? null : (tables[i].AutoAdjustState).ToString();
                    BasdeData[i].SHIJIAN = tables[i].DeviceDateTime == null ? null : (tables[i].DeviceDateTime).ToString();
                    BasdeData[i].SHUCHUDIANYA = tables[i].OutPutVolage==null ? null : SensorDataX.VS(tables[i].OutPutVolage); 
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

    }

    public class CQTGridRow : INotifyPropertyChanged
    {
        public CQTGridRow()
        { 
        }
        public int WEIZHI { get; set; }
        public string XULIEHAO { get; set; }
        public string SHIJIAN { get; set; }
        public string LIANGCHENG { get; set; }
        public string DIANYAFANWEI { get; set; }
        public string ZIJIAOZHUN { get; set; }
        public string SHUCHUDIANYA { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
