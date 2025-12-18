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
            this.UIZhuiSuC10SensorsT10.Mode = SensorItemEnum.T10;
            this.UIZhuiSuC10SensorsT90.Mode = SensorItemEnum.T90;

            this.UIZhuiSuC10SensorsMulti.Mode = SensorItemEnum.ColorDiagram; 
             
        }


        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan f_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            UIZhuiSuC10SensorsNongDu.SetData(_Batch,  datas);
            UIZhuiSuC10SensorsT10.SetData(_Batch, datas);
            UIZhuiSuC10SensorsT90.SetData(_Batch, datas); 
            UIZhuiSuC10SensorsMulti.SetData(_Batch, datas); 
        }
         
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            UIZhuiSuC10SensorsNongDu.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsT10.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsT90.IniTask(_Batch, datas); 
            UIZhuiSuC10SensorsMulti.IniTask(_Batch, datas); 
        }
        public void SetFontColor()
        {
            UIZhuiSuC10SensorsNongDu.SetFontColor();
            UIZhuiSuC10SensorsT10.SetFontColor();
            UIZhuiSuC10SensorsT90.SetFontColor(); 
            UIZhuiSuC10SensorsMulti.SetFontColor(); 
        }
        public void Refresh()
        { 
            UIZhuiSuC10SensorsNongDu.Refresh();
            UIZhuiSuC10SensorsT10.Refresh();
            UIZhuiSuC10SensorsT90.Refresh(); 
            UIZhuiSuC10SensorsMulti.Refresh(); 
        }

        public void RefreshMM() {
             
        }

    }
    
}
