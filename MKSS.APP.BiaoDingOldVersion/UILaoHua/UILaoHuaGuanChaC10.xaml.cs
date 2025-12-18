using DeviceDataMonitorWPF.UIBiaoDing;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using DeviceDataMonitorWPF.UIBiaodingJiuJing.Util;
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

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    /// <summary>
    /// UILaoHuaGuanChaC10.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaC10 : UserControl
    {
        public UILaoHuaGuanChaC10()
        {
            InitializeComponent();
        }

        public void SetData(List<DataAgingSensorGroupDataModel> data)
        {

            int selectIndex = gridPager.SelectedIndex;
            if (selectIndex < 0) selectIndex = 0;
            int app = 4;
            gridPager.Items.Clear();
            List<UILaoHuaGuanChaPage> pagedatas = new List<UILaoHuaGuanChaPage>();
            for (int i = 0; i < data.Count; i = i + app)
            {
                int from = i;
                int to = data.Count - 1 >= i + app ? i + app : data.Count - 1;
                UILaoHuaGuanChaPage d = new UILaoHuaGuanChaPage() { };
                d.From = data[from];
                d.To = data[to];
                d.Data = data.GetRange(from, to - from).ToList();
                pagedatas.Add(d);
                gridPager.Items.Add(d);
            }
            if (selectIndex >= gridPager.Items.Count) selectIndex = 0;
            if (gridPager.Items.Count > 0) gridPager.SelectedItem = gridPager.Items[selectIndex];

        }


        private void gridPager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UILaoHuaGuanChaPage page = gridPager.SelectedItem as UILaoHuaGuanChaPage;
            if (page == null) return;
            int x = 0;
            foreach (UIElement item in grid.Children)
            {
                if (item is UILaoHuaGuanChaC10Grid)
                {
                    UILaoHuaGuanChaC10Grid g = (UILaoHuaGuanChaC10Grid)item;
                    if (page.Data.Count > x)
                    {
                        g.SetData(page.Data[x]);
                        x++;
                    }
                }
            }
        }

    }
}
