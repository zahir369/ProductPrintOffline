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
        public void SetData(SensorGroupDataModel data)
        {
            this.DataContext = data;
            List<SensorData> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].XULIEHAO = data.Empty ? null : (tables[i].Serial);
                    BasdeData[i].LIANGCHENG = data.Empty ? null : SensorData.VS(tables[i].LiangCheng);
                    BasdeData[i].DIANYAFANWEI = data.Empty ? null : (tables[i].DianYaRange);
                    BasdeData[i].ZIJIAOZHUN = data.Empty ? null : (tables[i].AutoAdjustState).ToString();
                    BasdeData[i].SHUCHUDIANYA = data.Empty ? null : SensorData.VS(tables[i].OutPutVolage); 
                }
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
        public string LIANGCHENG { get; set; }
        public string DIANYAFANWEI { get; set; }
        public string ZIJIAOZHUN { get; set; }
        public string SHUCHUDIANYA { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
