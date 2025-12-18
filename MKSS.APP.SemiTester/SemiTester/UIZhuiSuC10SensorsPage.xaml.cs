using MKSS.Model;
using MKSS.Service.SemiTester;
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
        Batch Batch { get; set; }
        public UIZhuiSuC10SensorsPage()
        {
            InitializeComponent();
            this.UIZhuiSuC10SensorsLT.PartA = true;
            this.UIZhuiSuC10SensorsLM.PartA = true;
            this.UIZhuiSuC10SensorsLB.PartA = true;

            this.UIZhuiSuC10SensorsRT.PartA = false;
            this.UIZhuiSuC10SensorsRM.PartA = false;
            this.UIZhuiSuC10SensorsRB.PartA = false;


            this.UIZhuiSuC10SensorsMT.PartA = true;
            this.UIZhuiSuC10SensorsMB.PartA = false;


            this.UIZhuiSuC10SensorsLT.Mode = SensorItemEnum.Final;
            this.UIZhuiSuC10SensorsLM.Mode = SensorItemEnum.Delta;
            this.UIZhuiSuC10SensorsLB.Mode = SensorItemEnum.Start;

            this.UIZhuiSuC10SensorsRT.Mode = SensorItemEnum.Final;
            this.UIZhuiSuC10SensorsRM.Mode = SensorItemEnum.Delta;
            this.UIZhuiSuC10SensorsRB.Mode = SensorItemEnum.Start;


            this.UIZhuiSuC10SensorsMT.Mode = SensorItemEnum.ColorDiagram;
            this.UIZhuiSuC10SensorsMB.Mode = SensorItemEnum.ColorDiagram;

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
        }


        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan f_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            UIZhuiSuC10SensorsLT.SetData(_Batch,  datas);
            UIZhuiSuC10SensorsLM.SetData(_Batch, datas);
            UIZhuiSuC10SensorsLB.SetData(_Batch, datas);
            UIZhuiSuC10SensorsRT.SetData(_Batch, datas);
            UIZhuiSuC10SensorsRM.SetData(_Batch, datas);
            UIZhuiSuC10SensorsRB.SetData(_Batch, datas);
            UIZhuiSuC10SensorsMT.SetData(_Batch, datas);
            UIZhuiSuC10SensorsMB.SetData(_Batch, datas);
            UIZhuiSuC10SensorsSET.SetData(_Batch, datas, f_AddTime);
        }
         
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            UIZhuiSuC10SensorsLT.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsLM.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsLB.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsRT.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsRM.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsRB.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsMT.IniTask(_Batch, datas);
            UIZhuiSuC10SensorsMB.IniTask(_Batch, datas);
        }
        public void SetFontColor()
        {
            UIZhuiSuC10SensorsLT.SetFontColor();
            UIZhuiSuC10SensorsLM.SetFontColor();
            UIZhuiSuC10SensorsLB.SetFontColor();
            UIZhuiSuC10SensorsRT.SetFontColor();
            UIZhuiSuC10SensorsRM.SetFontColor();
            UIZhuiSuC10SensorsRB.SetFontColor();
            UIZhuiSuC10SensorsMT.SetFontColor();
            UIZhuiSuC10SensorsMB.SetFontColor();
        }
        public void Refresh()
        { 
            UIZhuiSuC10SensorsLT.Refresh();
            UIZhuiSuC10SensorsLM.Refresh();
            UIZhuiSuC10SensorsLB.Refresh();
            UIZhuiSuC10SensorsRT.Refresh();
            UIZhuiSuC10SensorsRM.Refresh();
            UIZhuiSuC10SensorsRB.Refresh();
            UIZhuiSuC10SensorsMT.Refresh();
            UIZhuiSuC10SensorsMB.Refresh();
        }

        public void RefreshMM() {
            List<ProductGradeExtEdit> gList = UIZhuiSuC10Model.Instance.SettingModel.GradeList;
            UIZhuiSuC10MM.ItemsSource = gList;
        }

    }
    
}
