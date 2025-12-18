
using MKSS.APP.LaoHua.UIControls;
using MKSS.APP.LaoHua.UserCommon;
using MKSS.APP.LaoHua.UserControls;
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
using MKSS.Service.UIBiaoDing;
using MKSS.Util;
using MKSS.APP.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Config;

namespace MKSS.APP.LaoHua
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在刷新", "正在读取老化任务......", this);
            InitializeComponent();
            if (Appsettings.App(new string[] { "SerialNoType" }) != "SCRWD") {
                this.Title = Appsettings.App(new string[] { "MainCompTile" });
                this.MainSubTile.Text = Appsettings.App(new string[] { "MainSubTile" });
                this.MainSubVersion.Text = Appsettings.App(new string[] { "MainSubVersion" });
                this.MainLogo.Source = new BitmapImage(new Uri("pack://application:,,," + Appsettings.App(new string[] { "MainLogo" }))); ;
            }
            UISheBeiBiaoDingViewModel.Intance.MainView = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUser.Text = BiaoDingConfgig.Instance.TxtUser;
            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUserAdmin.Text = BiaoDingConfgig.Instance.TxtUserAdmin;

        }
    }
}
