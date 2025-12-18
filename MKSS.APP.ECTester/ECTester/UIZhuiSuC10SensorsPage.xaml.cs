using MKSS.Model;
using MKSS.Service.ECTester;
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

namespace MKSS.APP.ECTester
{
    /// <summary>
    /// UIZhuiSuC10SensorsPage.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10SensorsPage : UserControl
    {
        Batch Batch { get; set; }
        public UIZhuiSuC10SensorsPage()
        {
            InitializeComponent();

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            this.UIZhuiSuC10SensorsNongDu.Mode = SensorItemEnum.SrcData;
             
        }


        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan f_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            UIZhuiSuC10SensorsNongDu.SetData(_Batch, datas,-1,false);
        }
         
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            UIZhuiSuC10SensorsNongDu.IniTask(_Batch, datas);
        }

        public void SetFontColor()
        {
            UIZhuiSuC10SensorsNongDu.SetFontColor();
        }


        public void Refresh()
        { 
            UIZhuiSuC10SensorsNongDu.Refresh();
        }

        public void RefreshMM() {
             
        }

    }
    
}
