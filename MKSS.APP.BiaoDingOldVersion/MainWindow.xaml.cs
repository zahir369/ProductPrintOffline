
using DeviceDataMonitorWPF.UIControls;
using DeviceDataMonitorWPF.UserCommon;
using DeviceDataMonitorWPF.UserControls;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using MKSS.IServices;
using MKSS.Model.ViewModel;
using MKSS.Services;
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
using System.Linq;

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitWork()
        {
            /////  ITheme theme = paletteHelper.GetTheme();
            ////   Application.Current.Resources.SetTheme(theme);
            //UserButton ubtn = null;

            //for (int i = 0; i <= 5; i++)
            //{
            //    ubtn = new UserButton();
            //    // DataContext = ubtn;
            //    ubtn.ButtonImagePath = "/Resources/Images/tongji.png";
            //    ubtn.ButtonText = "butong" + i;
            //    pnlLeftMenu.Children.Add(ubtn);
            //}

            //menuTreeView.ItemTemplate.DataType = typeof(MenuData);

            LoadMainMenu(pnlLeftMenu);
        }

        //private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{ 

        //    TabItem tabitem = (sender as TabControl).SelectedItem as TabItem;

        //    string tabItemName = tabitem.Name as string;

        //    switch (tabItemName)
        //    {
        //        case "tabiDataMonitor":
        //            {
        //                tabitem.Content = new DataMonitor();
        //            }
        //            break;

        //        case "tabiDeviceManage":
        //            {
        //                tabitem.Content = new DeviceMange();
        //            }
        //            break;

        //        default:
        //            return;
        //    }

        //}

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //退出
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWork();
        }

        public void LoadMainMenu(StackPanel menuPanel)
        {

            MenuData defaultMenu = null;
            //< Separator Opacity = "0" Height = "20" />
            UserButton ubtn = null;
            Separator sep = null;
            foreach (MenuData item in MenuService.SysMenus)
            {

                //增加按钮之间间隔
                sep = new Separator()
                {
                    Opacity = 0,
                    Height = 15
                };
                menuPanel.Children.Add(sep);

                //增加菜单按钮
                ubtn = new UserButton();
                ubtn.ButtonImagePath = $"/Resources/Images/{item.ImagePath}";
                ubtn.ButtonText = item.MenuName;

                ubtn.CommandParameter = new MenuCommandParameter()
                {
                    MenuData = item,
                    MenuTreeView = menuTreeView,
                    MenuParentName = menuTreeViewParentName,
                    MenuDataDrawer = dhDrawerHost
                };

                ubtn.OpenMenuCommand = new OpenMenuCommand();
                menuPanel.Children.Add(ubtn);

                foreach (MenuData data in item.MenuDataChilds)
                {
                    if (data.MenuName == limit)
                    {
                        defaultMenu = data;
                    }
                }
                
            }

            if (defaultMenu != null) {
                menuTreeView_SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, defaultMenu));
            }

        }
        string limit = "设备标定";


        private void menuTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var md = e.NewValue as MenuData;
            if (md == null) return;

            var i = ComUtil.FindItemIndex(tabMain, md.MenuName);
            if (i > -1)
            {
                //存在
                tabMain.SelectedIndex = i;
                dhDrawerHost.IsLeftDrawerOpen = false;
                return;
            }

            UserTabItem item = new UserTabItem();
            ContentControl cc = new ContentControl();

            if(md.MenuName != limit) return;
            var uc = ComUtil.LoadUserControl(md.BindControlName);

            item.Header = md.MenuName;
            //item.ToolTip = md.MenuName;
            item.Margin = new Thickness(2, 0, 2, 0);
            item.Height = 30;
            cc.Content = uc;
            item.Content = uc;// cc;// uc;

            tabMain.Items.Add(item);
            tabMain.SelectedItem = item;

            dhDrawerHost.IsLeftDrawerOpen = false;
        }

    }
}
