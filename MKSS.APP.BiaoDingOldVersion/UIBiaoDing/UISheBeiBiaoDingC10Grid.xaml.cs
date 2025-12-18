using DeviceDataMonitorWPF.UIBiaoDing.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDingC10Grid.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingC10Grid : UserControl
    {
        List<C10GridRow> BasdeData = new List<C10GridRow>();
        public UISheBeiBiaoDingC10Grid()
        {
            InitializeComponent();
            for (int i = 0; i < 15; i++)
            {
                BasdeData.Add(new C10GridRow() { WEIZHI = i + 1 });
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
                    BasdeData[i].XINHAO_AD = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V01);
                    BasdeData[i].XINHAO_ADHEGE = data.Empty || tables[i].IsEmpSensor ? null : tables[i].IsQualifiedV1;
                    BasdeData[i].CANKAO_AD = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V02);
                    BasdeData[i].CANKAO_YULIU1 = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V03);
                    BasdeData[i].WENDU_AD = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V04);
                    BasdeData[i].CANKAO_YULIU2 = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V05);
                    BasdeData[i].CANKAO_YULIU3 = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V06);
                    BasdeData[i].NONGDU = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V07);
                    BasdeData[i].NONGDUHEGE = data.Empty || tables[i].IsEmpSensor ? null : tables[i].IsQualifiedV7;
                    BasdeData[i].WENDU1 = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V08);
                    BasdeData[i].WENDU2 = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V09);
                    BasdeData[i].MONIDIANYA = data.Empty || tables[i].IsEmpSensor ? null : SensorData.VS(tables[i].V10);
                    BasdeData[i].HEGE = UISheBeiBiaoDingModel.Intance.SerialModBus.Qualified[data.Address, tables[i].Position];
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
                    bool Qualified = !UISheBeiBiaoDingModel.Intance.SerialModBus.Qualified.NotQualified(data.Address, WEIZHI);
                    UISheBeiBiaoDingModel.Intance.SerialModBus.Qualified[data.Address, WEIZHI] = !Qualified;
                    BasdeData[WEIZHI - 1].HEGE = UISheBeiBiaoDingModel.Intance.SerialModBus.Qualified[data.Address, WEIZHI];
                }
            }
        }

    }

    public class C10GridRow : C05GridRow
    {
        public C10GridRow()
        {
            HEGE = true;
        }
        public string CANKAO_YULIU1 { get; set; }
        public string CANKAO_YULIU2 { get; set; }
        public string CANKAO_YULIU3 { get; set; }
        public string WENDU2 { get; set; }
        public string MONIDIANYA { get; set; } 
    }

}