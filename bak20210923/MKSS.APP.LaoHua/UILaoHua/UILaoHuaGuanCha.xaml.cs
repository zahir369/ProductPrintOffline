using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.Util;
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

namespace MKSS.APP.LaoHua 
{
    /// <summary>
    /// UILaoHuaGuanCha.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanCha : UserControl
    {
        UILaoHuaGuanChaModel _model;
        public UILaoHuaGuanCha()
        {
            InitializeComponent();
            _model = this.DataContext as UILaoHuaGuanChaModel;
            _model.PageContext = this;

        }

        object PreSelect = null;
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TabItemCurrent.IsSelected) {
                if(TabItemCurrent!= PreSelect) UILaoHuaGuanChaC10.IniData(null);
                PreSelect = this.TabItemCurrent;
            }
            if (this.TabItemAll.IsSelected)
            {
                if (TabItemAll != PreSelect) UILaoHuaGuanChaCGK.IniData();
                PreSelect = this.TabItemAll;
            }
        } 
    }
}