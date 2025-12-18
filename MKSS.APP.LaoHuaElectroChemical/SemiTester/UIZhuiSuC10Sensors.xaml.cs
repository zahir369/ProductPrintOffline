using MKSS.Model;
using MKSS.Service;
using MKSS.Service.LaoHuaElectroChemical;
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

namespace MKSS.APP.SemiTester
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
        bool _PartA = true;
        public bool PartA { 
            get {
                return _PartA;
            }
            set { 
                _PartA = value;
                if (_PartA)
                {
                    this.Background = (SolidColorBrush)this.FindResource("BgColorABoard");
                }
                else {
                    this.Background = (SolidColorBrush)this.FindResource("BgColorBBoard");
                }
            } 
        }
        public int FloorNo { get; set; }
        SensorItemValueEnum mode = SensorItemValueEnum.ValueND;
        public SensorItemValueEnum Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value; 
            }
        } 
        Dictionary<string, UIZhuiSuC10SensorItem> ItemDic = new Dictionary<string, UIZhuiSuC10SensorItem>();
        public UIZhuiSuC10Sensors()
        {

            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            string[] region = ElectroChemicalService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                    t1.Text = "/";
                    if (t1 != null)
                    {
                        ItemDic.Add(n,t1);
                    }
                }
            }

        }


        /// <summary>
        /// 获取子控件
        /// </summary>
        /// <typeparam name="T">子控件类型</typeparam>
        /// <param name="obj">父控件</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);


                if (child is T && (((T)child).Name == name && !string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        public List<string> SelectSensorPositions {
            get {
                List<string> ret = new List<string>();
                string[] region = ElectroChemicalService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
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
                string[] region = ElectroChemicalService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
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
            string[] region = ElectroChemicalService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
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


        public Batch Batch { get { return UIZhuiSuC10Model.Instance.BatchCurrent; } }
        public void SetData(Model.Batch _Batch, SensorGroupData datas)
        { 
            if (FloorNo != datas.F_FloorNo) return;
            if (datas.Data().Count > 0 && _Batch != null)
            {
                {
                    bool ShowND = UIZhuiSuC10Model.Instance.SettingModel.ValueMode == SensorItemValueEnum.ValueND;

                    FloorName.Content = string.Format("板卡{0}" + (ShowND ? "浓度（%）" : "端电压（毫伏）"), FloorNo); 
                    Refreshtime.Content = string.Format("刷新时间：{0}", (_Batch.F_AgingStartTime.AddSeconds(datas.F_AddTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                    int floor = datas.F_FloorNo;
                    string[] region = ElectroChemicalService.Regions;
                    for (int r = 0; r < region.Length; r++)
                    {
                        string boardNo =  region[r];
                        for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
                        {
                            string n = string.Format("Cell{0}{1}", boardNo, c);
                            if (!ItemDic.ContainsKey(n)) continue;
                            UIZhuiSuC10SensorItem t1 = ItemDic[n];
                            if (t1 != null)
                            {
                                string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId,floor, boardNo, c);
                                PosEnum posenum = Sensor.CreatePosEnum(Batch.F_BatchId, boardNo, c);
                                if (datas.Data().ContainsKey(posenum))
                                {
                                    t1.SetSensorData(_Batch, datas, posenum);
                                    if (t1.IsChecked != null && t1.IsChecked.Value)
                                    {
                                        //t1.T1_Click(null, null);
                                    }
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
            bool ShowND = UIZhuiSuC10Model.Instance.SettingModel.ValueMode == SensorItemValueEnum.ValueND;
            FloorName.Content = string.Format("板卡{0}" + (ShowND ? "浓度（%）" : "端电压（毫伏）"), FloorNo);
            string[] region = ElectroChemicalService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ElectroChemicalService.RegionSize; c++)
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

        public void RefreshErrorInfo(ErrorEntity status)
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ErrorInfo.Visibility = Visibility.Visible;
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
                this.ErrorInfo.ToolTip = status.Message;
            }));
        }

        public void ClearErrorInfo()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ErrorInfo.Visibility = Visibility.Hidden;
            }));
        }


        public void SetChecked(bool ckd) {
            FloorVisible.IsChecked = ckd;
        }

        private void Refreshtime_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            bool ck = FloorVisible.IsChecked != null && FloorVisible.IsChecked.Value;
            if (ck)
            {
                UIZhuiSuC10Model.Instance.DataProvider.Query(Batch, MqttTaskCommand.SetHidden, this.FloorNo.ToString());
                this.ErrorInfo.Visibility = Visibility.Hidden;
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
            }
            else
            {
                UIZhuiSuC10Model.Instance.DataProvider.Query(Batch, MqttTaskCommand.SetVisible, this.FloorNo.ToString());
            }
        }

    }
}
