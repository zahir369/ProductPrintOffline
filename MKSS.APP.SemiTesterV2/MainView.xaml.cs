
using MaterialDesignThemes.Wpf;
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
using MKSS.APP.UIBiaoDing;
using System.Configuration;
using MKSS.APP.UIBiaoDing.Config;

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {

        public MainView()
        {
            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在启动", "正在启动智慧工厂生产数字化管理系统-标定软件......", this);
            InitializeComponent();

            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (ConfigurationManager.AppSettings["SerialNoType"] != "SCRWD")
            {
                this.Title = ConfigurationManager.AppSettings["MainCompTile"];// Appsettings.App(new string[] { "MainCompTile" });
                this.MainSubTile.Text = ConfigurationManager.AppSettings["MainSubTile"];
                this.MainSubVersion.Text = ConfigurationManager.AppSettings["MainSubVersion"];
                this.MainLogo.Source = new BitmapImage(new Uri("pack://application:,,," + ConfigurationManager.AppSettings["MainLogo"])); ;
            }

            this.Closing += MainView_Closing;
            this.Loaded += MainView_Loaded;
            UISheBeiBiaoDingViewModel.Intance.MainView = this;

        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUser.Text = BiaoDingConfgig.Instance.TxtUser;
            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUserAdmin.Text = BiaoDingConfgig.Instance.TxtUserAdmin;

            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            BiaoDingSaver.Dispose();
            UISheBeiBiaoDingViewModel.Intance.Closed();
            Application.Current.Shutdown();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //退出
        }
         
    }
}
