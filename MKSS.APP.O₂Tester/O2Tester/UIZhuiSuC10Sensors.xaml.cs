using MKSS.Model;
using MKSS.Service.O2Tester;
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

namespace MKSS.APP.O2Tester
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
         
        SensorItemEnum mode = SensorItemEnum.SrcData;
        public SensorItemEnum Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                CalcTitle();
                foreach (UIZhuiSuC10SensorItem item in ItemDic.Values)
                {
                    item.Mode = mode;
                }
            }
        }

        public void CalcTitle()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            switch (mode)
            {
                case SensorItemEnum.SrcData:
                    this.ShowName.Content = string.Format((SensorGroupData.ShowModeDesc)+"（%）");
                    break;
                case SensorItemEnum.T90:
                    this.ShowName.Content = string.Format("T90时间（秒）");
                    break;
                case SensorItemEnum.T10:
                    this.ShowName.Content = string.Format("T10时间（秒）");
                    break;
                case SensorItemEnum.ColorDiagram:
                    string[] ps_str = new string[] { "上", "，左", "，右" };
                    SensorItemEnum[] ps = new SensorItemEnum[] {
                            ColorDiagramTop,ColorDiagramLeft,ColorDiagramRight
                        };
                    StringBuilder sb = new StringBuilder();
                    sb.Append("");
                    for (int i = 0; i < ps.Length; i++)
                    {
                        sb.Append(ps_str[i]);
                        switch (ps[i])
                        {
                            case SensorItemEnum.SrcData:
                                sb.Append(SensorGroupData.ShowModeDesc);
                                break;
                            case SensorItemEnum.T90:
                                sb.Append("T90");
                                break;
                            case SensorItemEnum.T10:
                                sb.Append("T10");
                                break;
                            default:
                                break;
                        }
                    }
                    sb.Append("：右键切换");
                    this.ShowName.Content = sb.ToString();
                    break;
                default:
                    break;
            }
            Refreshtime.Visibility = mode == SensorItemEnum.ColorDiagram ? Visibility.Hidden : Visibility.Visible;
            if (GroupData == null||Batch==null)
            {
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
            }
            else {
                Refreshtime.Content = string.Format("刷新时间：{0}", (Batch.F_AgingStartTime.AddSeconds(GroupData.F_AddTime)).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        public SensorItemEnum ColorDiagramTop
        {
            get
            {
                if (UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return SensorItemEnum.SrcData;
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramTop;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramTop = value;
                O2TesterConfgig.Save();
            }
        }
        public SensorItemEnum ColorDiagramLeft
        {
            get
            {
                if (UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return SensorItemEnum.T90;
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramLeft;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramLeft = value;
                O2TesterConfgig.Save();
            }
        }
        public SensorItemEnum ColorDiagramRight
        {
            get
            {
                if (UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return SensorItemEnum.T10;
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight;
                O2TesterConfgig.Save();
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramRight = value;
                O2TesterConfgig.Save();
            }
        }

        Dictionary<string, UIZhuiSuC10SensorItem> ItemDic = new Dictionary<string, UIZhuiSuC10SensorItem>();
        public UIZhuiSuC10Sensors()
        {

            InitializeComponent();

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            string[] region = O2TesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= O2TesterService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                     
                    if (t1 != null)
                    {
                        ItemDic.Add(n,t1);
                    }
                }
            }

        }

        public List<string> SelectSensorPositions {
            get {
                List<string> ret = new List<string>();
                string[] region = O2TesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= O2TesterService.RegionSize; c++)
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
                string[] region = O2TesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= O2TesterService.RegionSize; c++)
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
            string[] region = O2TesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= O2TesterService.RegionSize; c++)
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
            GroupData = null;
            if (datas.Count > 0 && _Batch != null)
            {
                string[] region = O2TesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= O2TesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null)
                        {
                            //string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            //t1.Sensor = datas.FirstOrDefault(w => w.F_SensorId == F_SensorId);
                            //if (t1.Sensor!=null)
                            {
                                
                            }
                            t1.T1_Click(null, null);//刷新选中状态
                        }
                    }
                }
            }
            else
            {

            }
        }
        SensorGroupData GroupData { get; set; }
        public void SetData(Model.Batch _Batch, SensorGroupData datas)
        {
            Batch = _Batch;
            GroupData = datas;
            if (datas.Data.Count > 0 && _Batch != null)
            {
                string[] region = O2TesterService.Regions;
                foreach (var boardNo in region)
                {
                    for (int c = 1; c <= O2TesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null) {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            PosEnum posenum = Sensor.CreatePosEnum(Batch.F_BatchId, boardNo, c);
                            if (datas.Data.ContainsKey(posenum))
                            {
                                t1.SetSensorData(_Batch, datas, posenum);
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
            Refreshtime.Visibility = mode == SensorItemEnum.ColorDiagram ? Visibility.Hidden : Visibility.Visible;
            if (GroupData == null || Batch == null)
            {
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
            }
            else
            {
                Refreshtime.Content = string.Format("刷新时间：{0}", (Batch.F_AgingStartTime.AddSeconds(GroupData.F_AddTime)).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }


        public void Refresh()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            string[] region = O2TesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= O2TesterService.RegionSize; c++)
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
            CalcTitle();
        }

    }
}
