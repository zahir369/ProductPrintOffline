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
    /// UISheBeiBiaoDingC10.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingC10 : UserControl
    {
        public UISheBeiBiaoDingC10()
        {
            InitializeComponent();
        }

        public void SetData(List<SensorGroupDataModel> data)
        {

            int selectIndex = gridPager.SelectedIndex;
            int app = 4;
            gridPager.Items.Clear();
            List<UISheBeiBiaoDingPage> pagedatas = new List<UISheBeiBiaoDingPage>();
            for (int i = 0; i < data.Count; i = i + app)
            {
                int from = i;
                int to = data.Count - 1 >= i + app ? i + app : data.Count - 1;
                UISheBeiBiaoDingPage d = new UISheBeiBiaoDingPage() { };
                d.From = data[from];
                d.To = data[to];
                d.Data = data.GetRange(from, to - from).ToList();
                pagedatas.Add(d);
                gridPager.Items.Add(d);
            }
            if (selectIndex >= gridPager.Items.Count) selectIndex = 0;
            if (gridPager.Items.Count > 0) gridPager.SelectedItem = gridPager.Items[selectIndex];

        }


        public void gridPager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UISheBeiBiaoDingPage page = gridPager.SelectedItem as UISheBeiBiaoDingPage;
            if (page == null) return;
            int x = 0;
            foreach (UIElement item in grid.Children)
            {
                if (item is UISheBeiBiaoDingC10Grid)
                {
                    UISheBeiBiaoDingC10Grid g = (UISheBeiBiaoDingC10Grid)item;
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
