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
    /// UIZhuiSuC10SensorsPage.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10SensorsPage : UserControl
    {
        public Batch Batch { get { return UIZhuiSuC10Model.Instance.BatchCurrent; } }
        Dictionary<int, UIZhuiSuC10Sensors> ItemDic = new Dictionary<int, UIZhuiSuC10Sensors>();
        public UIZhuiSuC10SensorsPage()
        {
            InitializeComponent();

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            for (int floor = 1; floor <= ElectroChemicalService.FloorSize; floor++)
            {
                string n = string.Format("UIZhuiSuC10SensorsFloor{0}", floor);
                UIZhuiSuC10Sensors t1 = UIZhuiSuC10Sensors.GetChildObject<UIZhuiSuC10Sensors>(AllText, n);

                if (t1 != null)
                {
                    t1.FloorNo = floor;
                    t1.FloorName.Content = string.Format("板卡{0}", floor);
                    t1.Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
                    ItemDic.Add(floor, t1);
                }
            }

        }
         
        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan f_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (ItemDic.ContainsKey(datas.F_FloorNo)) {
                ItemDic[datas.F_FloorNo].SetData(_Batch, datas);
            }
            this.UIZhuiSuC10SensorsSET.RefreshProgress.Value = datas.F_FloorNo * 100 / 15;
            this.UIZhuiSuC10SensorsSET.RefreshProgressText.Content = datas.F_FloorNo.ToString("00");
        }

        public UIZhuiSuC10Sensors Of(int floor) {
            return ItemDic.ContainsKey(floor) ? ItemDic[floor] : null;
        }

        public void SetFontColor()
        {
            foreach (var item in ItemDic.Values)
            {
                item.SetFontColor();
            } 
        }
        public void Refresh()
        {
            foreach (var item in ItemDic.Values)
            { 
                item.Refresh();
            } 
        }
        public void RefreshCk(Dictionary<int, bool> vs)
        {
            if (vs != null) {
                foreach (var item in ItemDic.Values)
                {
                    item.SetChecked(vs[item.FloorNo]);
                }
            }
        }

        public void RefreshMM() {
             
        }

    }
    
}
