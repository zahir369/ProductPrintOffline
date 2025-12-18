using MaterialDesignThemes.Wpf;
using MKSS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKSS.APP.ECTester.UserCommon
{
    public class MenuTreeViewItem
    {
        public static ObservableCollection<MenuData> MenuTreeViewItemSource { get; set; }
    }

    public class MenuCommandParameter
    {
        public MenuData MenuData { get; set; }
        public TreeView MenuTreeView { get; set; }
        public DrawerHost MenuDataDrawer { get; set; }
        public TextBlock MenuParentName { get; set; }
    }


    public class OpenMenuCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null) return false;

            var cmdParam = parameter as MenuCommandParameter;

            var menuTree = cmdParam.MenuTreeView;

            //if (cmdParam.MenuData.MenuDataChilds.Count() == 0) return false;

            //if (cmdParam.MenuDataDrawer.IsLeftDrawerOpen) return false;  




            return true;
        }

        public void Execute(object parameter)
        {
            var cmdParam = parameter as MenuCommandParameter;

            var drawer = cmdParam.MenuDataDrawer;

            var menuTree = cmdParam.MenuTreeView;

            var menuTreeParentName = cmdParam.MenuParentName;

            if (null == drawer) return;

            //Canvas.SetZIndex(drawer, 10000);

            menuTree.ItemsSource = null;

            MenuTreeViewItem.MenuTreeViewItemSource = new ObservableCollection<MenuData>(cmdParam.MenuData.MenuDataChilds);

            if (menuTree.ItemTemplate.DataType == null)
                menuTree.ItemTemplate.DataType = typeof(MenuData);
            
            menuTree.ItemsSource = MenuTreeViewItem.MenuTreeViewItemSource;
            menuTreeParentName.Text = cmdParam.MenuData.MenuName;

            drawer.IsLeftDrawerOpen = true;
        }
    }
}
