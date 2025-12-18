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

namespace MKSS.APP.ElectroChemical
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
        List<UIZhuiSuC10SensorItem> list = new List<UIZhuiSuC10SensorItem>();
        public UIZhuiSuC10Sensors()
        {

            InitializeComponent();

            string[] region = new string[] { "A", "B", "C", "D" };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                     
                    if (t1 != null)
                    {
                        list.Add(t1);
                    }
                }
            }

            this.CellA1.IsChecked = true;
            this.CellB1.IsChecked = true;
            this.CellC1.IsChecked = true;
            this.CellD1.IsChecked = true;

        }

        public List<string> SelectSensorPositions {
            get {
                List<string> ret = new List<string>();
                string[] region = new string[] { "A", "B", "C", "D" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);

                        if (t1 != null && t1.IsChecked!=null && t1.IsChecked.Value)
                        {
                            ret.Add(string.Format("{0}{1}", boardNo, c));
                        }
                    }
                }
                return ret;
            }
        }


        public List<PosEnum> SelectSensorIds
        {
            get
            {
                List<PosEnum> ret = new List<PosEnum>();
                string[] region = new string[] { "A", "B", "C", "D" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                        if (t1 != null && t1.IsChecked != null && t1.IsChecked.Value && t1.Sensor != null) {
                            ret.Add(t1.Sensor.PosEnum);
                        } 
                    }
                }
                return ret;
            }
        }

        public void SetFontColor()
        {
            string[] region = new string[] { "A", "B", "C", "D" };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);

                    if (t1 != null)
                    {
                        t1.SetFontColor();
                    }
                }
            } 
        }


        Batch Batch { get; set; }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;
            if (datas.Count > 0 && _Batch != null)
            {
                string[] region = new string[] { "A", "B", "C", "D" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                        if (t1 != null)
                        {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            t1.Sensor = datas.FirstOrDefault(w => w.F_SensorId == F_SensorId);
                            if (t1.Sensor!=null)
                            {
                                t1.T1_Click(null, null);//刷新选中状态
                            }
                        }
                    }
                }
            }
            else
            {

            }
        }

        public void SetData(Model.Batch _Batch, SensorGroupData datas)
        {
            Batch = _Batch;
            if (datas.Data.Count > 0 && _Batch != null)
            {
                string[] region = new string[] { "A", "B", "C", "D" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                        if (t1 != null) {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            PosEnum posenum = Sensor.CreatePosEnum(Batch.F_BatchId, boardNo, c);
                            if (datas.Data.ContainsKey(posenum))
                            {
                                t1.SetSensorData(_Batch, datas, t1.Sensor, posenum);
                                if (t1.IsChecked!=null && t1.IsChecked.Value) {
                                    //t1.T1_Click(null, null);
                                }
                            }
                        } 
                    }
                }
            }
            else
            {

            }
        }


        public void Refresh()
        {
            string[] region = new string[] { "A", "B", "C", "D" };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                    if (t1 != null)
                    {
                        t1.Refresh();
                    }
                }
            }
        }

    }
}
