using MKSS.APP.SemiTester;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// UIZhuiSu.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSu : UserControl
    {
        UIZhuiSuModel _model;
        public UIZhuiSu()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            _model = this.DataContext as UIZhuiSuModel;
            _model.PageContext = this;

        }

        object PreSelect = null;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.TabItemCurrent.IsSelected) {
                if(TabItemCurrent!= PreSelect) UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.IniData(UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BatchCurrent, new System.Diagnostics.Stopwatch());
                PreSelect = this.TabItemCurrent;
            }
            if (this.TabItemAll.IsSelected)
            {
                //if (UIZhuiSuC10Model.Instance.SettingModel.Started== LaHuaTaskStatus.Running) {
                //    MessageBox.Show("请先结束检测项目。");
                //    this.TabItemAll.IsSelected = false;
                //    this.TabItemCurrent.IsSelected = true; 
                //    return;
                //}
                if (TabItemAll != PreSelect) UIZhuiSuCGK.IniData();
                PreSelect = this.TabItemAll;
            }
        } 

    }
}