using MKSS.APP.ZhuiSu.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
        List<Button> list = new List<Button>();
        public UIZhuiSuC10Sensors()
        {
            InitializeComponent();
            for (int r = 1; r <= 8; r++)
            {
                for (int c = 1; c <= 8; c++)
                {
                    string n = string.Format("Cell{0}X{1}", r, c);
                    Button t1 = UIZhuiSuC64SensorsAdd.GetChildObject<Button>(AllText, n);
                    t1.Click -= T1_Click;
                    t1.Click += T1_Click;
                    if (t1 != null)
                    {
                        list.Add(t1);
                    }
                }
            } 
        }

        private void T1_Click(object sender, RoutedEventArgs e)
        {
            Button t1 = sender as Button;
            if (t1 == null) return;
            PageSensorModel ses = t1.Tag as PageSensorModel;
            if (ses != null)
            {
                List<string> sendor_ids = new List<string>();
                sendor_ids.Add(ses.SensorId);
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowSensors(sendor_ids);
            }
        }

        Batch Batch { get; set; }
        PageBoardItem DataSrc { get; set; }
        public void SetData(PageBoardItem data, PageBoardItem dataB, Batch _batch)
        {
            Batch = _batch;
            this.DataSrc = data;
            this.DataContext = data;
            BoardCase _BoardCase = null;
            Board _Board = null;
            Batch _Batch = UIZhuiSuData.Instance.OfBatch(data, ref _BoardCase, ref _Board);
            _Batch = Batch;
            if (data.Address > 0 && _Batch != null)
            {


                if (_Batch.EnumAgingStatus == EnumAgingStatus.InAging)
                {
                    if (_Batch.F_AgingStartTime != DateTime.MinValue && _Batch.F_AgingEndTime != DateTime.MinValue)
                    {
                        TimeSpan total = _Batch.F_AgingEndTime - _Batch.F_AgingStartTime;
                        TimeSpan els = DateTime.Now - _Batch.F_AgingStartTime;
                         
                    }
                }
                else
                {
                }

            }
            else
            {

            }

            List<PageSensorModel> tables = new List<PageSensorModel>();
            tables.AddRange(data.ProductTable);
            tables.AddRange(dataB.ProductTable);
            for (int r = 1; r <= 8; r++)
            {
                for (int c = 1; c <= 8; c++)
                {
                    string n = string.Format("Cell{0}X{1}", r, c);
                    Button t1 = UIZhuiSuC64SensorsAdd.GetChildObject<Button>(AllText, n);
                    int index = (r - 1) * 8 + c - 1;
                    if(index< tables.Count) t1.Tag = tables[index];
                }
            }
             

        }
    
    }
}
