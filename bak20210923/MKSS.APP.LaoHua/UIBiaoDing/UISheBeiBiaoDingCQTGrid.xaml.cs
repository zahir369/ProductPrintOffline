using MKSS.APP.UIBiaoDing.Util;
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
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new CQTGridRow() { WEIZHI = i + 1 });
            }
            this.dataGrid.DataContext = BasdeData;
        }
        public void SetData(SensorGroupDataModel data)
        {
            this.DataContext = data;
            List<SensormData> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i)
                {
                    BasdeData[i].XULIEHAO = string.IsNullOrEmpty((tables[i].Serial)) ? null : (tables[i].Serial);
                    BasdeData[i].LIANGCHENG = tables[i].LiangCheng==null ? null : SensormData.VS(tables[i].LiangCheng);
                    BasdeData[i].DIANYAFANWEI = string.IsNullOrEmpty((tables[i].DianYaRange)) ? null : (tables[i].DianYaRange);
                    BasdeData[i].ZIJIAOZHUN = tables[i].AutoAdjustState==null ? null : (tables[i].AutoAdjustState).ToString();
                    BasdeData[i].SHUCHUDIANYA = tables[i].OutPutVolage==null ? null : SensormData.VS(tables[i].OutPutVolage); 
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
