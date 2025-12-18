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
    /// UISheBeiBiaoDingC05Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingC05Grid : UserControl
    {
        List<C05GridRow> BasdeData = new List<C05GridRow>();
        public UISheBeiBiaoDingC05Grid()
        {
            InitializeComponent();
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new C05GridRow() { WEIZHI = i + 1 });
            } 
            this.dataGrid.DataContext = BasdeData;
        }
        public void SetData(SensorGroupDataModel data)
        {
            this.DataContext = data;
            List<SensormData> tables = data.ProductTable;
            for (int i = 0; i < BasdeData.Count; i++)
            {
                if (tables.Count > i) {
                    BasdeData[i].XINHAO_AD = data.Empty || tables[i].IsEmpSensor ? null : SensormData.VS(tables[i].V01);
                    BasdeData[i].XINHAO_ADHEGE = data.Empty || tables[i].IsEmpSensor ? null : tables[i].IsQualifiedV1;
                    BasdeData[i].CANKAO_AD = data.Empty || tables[i].IsEmpSensor ? null : SensormData.VS(tables[i].V02);
                    BasdeData[i].WENDU_AD = data.Empty || tables[i].IsEmpSensor ? null : SensormData.VS(tables[i].V04);
                    BasdeData[i].NONGDU = data.Empty || tables[i].IsEmpSensor ? null : SensormData.VS(tables[i].V07);
                    BasdeData[i].NONGDUHEGE = data.Empty || tables[i].IsEmpSensor ? null : tables[i].IsQualifiedV7;
                    BasdeData[i].WENDU1 = data.Empty || tables[i].IsEmpSensor ? null : SensormData.VS(tables[i].V08);
                    BasdeData[i].HEGE = tables[i].IsQualified ;
                }
            }
        }

        private void BtnSetQualified_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int WEIZHI = (int)btn.Tag;
            SwithQualified( WEIZHI);
        }

        private void dataGrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (dataGrid.SelectedItem == null) return;
            SensorGroupDataModel data = (SensorGroupDataModel)this.DataContext;
            if (e.Key == Key.Space) {
                C05GridRow row = dataGrid.SelectedItem as C05GridRow;
                SwithQualified( row.WEIZHI);
            }
            if (e.Key == Key.Down)
            {
                 
            }
        }

        void SwithQualified( int WEIZHI) {
            SensorGroupDataModel data = (SensorGroupDataModel)this.DataContext;
            if (WEIZHI > 0 && WEIZHI <= data.ProductTable.Count)
            {
                if (!data.ProductTable[WEIZHI - 1].IsEmpSensor)
                {
                    bool? Qualified = data.ProductTable[WEIZHI - 1].IsQualified;
                    if (Qualified == null) Qualified = true;
                    data.ProductTable[WEIZHI - 1].IsQualified = !Qualified;
                    BasdeData[WEIZHI - 1].HEGE = data.ProductTable[WEIZHI - 1].IsQualified;
                }
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
        public event PropertyChangedEventHandler PropertyChanged;
         
    }


}
