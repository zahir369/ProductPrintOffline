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

namespace MKSS.APP.SemiCatalysis
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
        Dictionary<string, UIZhuiSuC10SensorItem> ItemDic = new Dictionary<string, UIZhuiSuC10SensorItem>();
        public UIZhuiSuC10Sensors()
        {

            InitializeComponent();

            string[] region = new string[] { "A" };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);

                    if (t1 != null)
                    {
                        ItemDic.Add(n, t1);
                    }
                }
            }

        }

        public List<string> SelectSensorPositions {
            get {
                List<string> ret = new List<string>();
                string[] region = new string[] { "A"  };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null && t1.IsChecked!=null && t1.IsChecked.Value)
                        {
                            ret.Add(string.Format("{0}{1}", boardNo, c));
                        }
                    }
                }
                return ret;
            }
        }


        public List<string> SelectSensorIds
        {
            get
            {
                List<string> ret = new List<string>();
                string[] region = new string[] { "A"  };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null && t1.IsChecked != null && t1.IsChecked.Value && t1.Sensor != null) {
                            ret.Add(t1.Sensor.F_SensorId);
                        } 
                    }
                }
                return ret;
            }
        }

        public void SetFontColor()
        {
            string[] region = new string[] { "A" };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    if (!ItemDic.ContainsKey(n)) continue;
                    UIZhuiSuC10SensorItem t1 = ItemDic[n];
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
                string[] region = new string[] { "A" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n]; 
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

        public void SetData(Model.Batch _Batch, Dictionary<string, MKSS.Model.SensorData> datas)
        {
            Batch = _Batch;  
            if (datas.Count > 0 && _Batch != null)
            {
                string[] region = new string[] { "A" };
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= 16; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null) {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            if (datas.ContainsKey(F_SensorId))
                            {
                                t1.SetSensorData(_Batch,datas[F_SensorId].Sensor, datas[F_SensorId]);
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
            string[] region = new string[] { "A"  };
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= 16; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    if (!ItemDic.ContainsKey(n)) continue;
                    UIZhuiSuC10SensorItem t1 = ItemDic[n];
                    if (t1 != null)
                    {
                        t1.Refresh();
                    }
                }
            }
        }

    }
}
