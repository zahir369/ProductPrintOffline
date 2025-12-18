using MKSS.APP.UIBiaoDing.Util;
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

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDingCQT.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingCQT : UserControl
    {
        public UISheBeiBiaoDingCQT()
        {
            InitializeComponent();
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
            int app = 4;
            gridPager.Items.Clear();
            int total = UISheBeiBiaoDingViewModel.MaxBoardCount;
            List<UISheBeiBiaoDingPage> pagedatas = new List<UISheBeiBiaoDingPage>();
            for (int i = 0; i < total; i = i + app)
            {
                int from = i;
                int to = total - 1 >= i + app ? i + app : total - 1;
                UISheBeiBiaoDingPage d = new UISheBeiBiaoDingPage() { };
                d.From = from;
                d.To = to;
                pagedatas.Add(d);
                gridPager.Items.Add(new ListBoxItem() { Tag = d, DataContext = d, Content = d.ToString() });
            }
            int selectIndex = gridPager.SelectedIndex;
            if (selectIndex >= gridPager.Items.Count || selectIndex < 0) selectIndex = 0;
            if (gridPager.Items.Count > 0 && selectIndex >= 0) gridPager.SelectedItem = gridPager.Items[selectIndex];
        }


        public void RefreshData(Address addr)
        {
            UISheBeiBiaoDingPage page = (gridPager.SelectedItem as ListBoxItem).Tag as UISheBeiBiaoDingPage;
            if (page == null) return;
            if (page.Contains(addr))
            {
                int x = page.From;
                foreach (UIElement item in grid.Children)
                {
                    if (item is UISheBeiBiaoDingCQTGrid)
                    {
                        UISheBeiBiaoDingCQTGrid g = (UISheBeiBiaoDingCQTGrid)item;
                        if (g.Address == addr)
                        {
                            g.RefreshData();
                        }
                    }
                }
            }
        }

        public void RefreshData()
        {
            foreach (ListBoxItem item in gridPager.Items)
            {
                item.Content = ((UISheBeiBiaoDingPage)item.Tag).ToString();
            }
            UISheBeiBiaoDingPage page = (gridPager.SelectedItem as ListBoxItem).Tag as UISheBeiBiaoDingPage;
            if (page == null) return;
            int x = page.From;
            foreach (UIElement item in grid.Children)
            {
                if (item is UISheBeiBiaoDingCQTGrid)
                {
                    UISheBeiBiaoDingCQTGrid g = (UISheBeiBiaoDingCQTGrid)item;
                    g.GroupIndex = x; x++;
                    g.RefreshData();
                }
            }

        }

        public void gridPager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshData();
        }



    }
}
