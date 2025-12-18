using MKSS.APP.ZhuiSu;
using MKSS.APP.ZhuiSu.Util;
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

namespace MKSS.APP.ZhuiSu
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
            _model = this.DataContext as UIZhuiSuModel;
            _model.PageContext = this;

        }

        object PreSelect = null;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TabItemCurrent.IsSelected) {
                if(TabItemCurrent!= PreSelect) UIZhuiSuC64.UIZhuiSuC64Setting1.IniData(null);
                PreSelect = this.TabItemCurrent;
            }
            if (this.TabItemAll.IsSelected)
            {
                if (TabItemAll != PreSelect) UIZhuiSuCGK.IniData();
                PreSelect = this.TabItemAll;
            }
        } 
    }
}