
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
using MKSS.APP.BiaodingAlcohol.UIBiaoDing;

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {

        public bool Laading { get; set; } = true;
        public MainView()
        {
            Laading = true;
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
            MainOnline.Text = (BiaoDingConfgig.Instance.RunModeOnline ? "正式版" : "开发版");
            RunModeOnline.IsChecked = BiaoDingConfgig.Instance.RunModeOnline;

            this.Closing += MainView_Closing;
            this.Loaded += MainView_Loaded;
            UISheBeiBiaoDingViewModel.Intance.MainView = this;

            if (UISheBeiBiaoDingViewModel.Intance.IsLogin)
            {
                TxtUserLogout.Content = "退出登录";
            }
            else
            {
                TxtUserLogout.Content = "登录";

            }


        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            Laading = false;
            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUser.Text = BiaoDingConfgig.Instance.TxtUser;
            UISheBeiBiaoDingViewModel.Intance.MainView.TxtUserAdmin.Text = BiaoDingConfgig.Instance.TxtUserAdmin;

            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (UISheBeiBiaoDingViewModel.Intance.TaskStarted)
            {
                UISheBeiBiaoDingViewModel.Intance.BtnTaskEnd();
            }
            BiaoDingSaver.Dispose(); 
            UISheBeiBiaoDingViewModel.Intance.Closed();
            Application.Current.Shutdown();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //退出
        }

        private void RunModeOnline_Checked(object sender, RoutedEventArgs e)
        {
            if (Laading) return;
            BiaoDingConfgig.Instance.RunModeOnline = false;// RunModeOnline.IsChecked!=null && RunModeOnline.IsChecked.Value;
            if (BiaoDingConfgig.Instance.RunModeOnline) { 
                
            }
            BiaoDingConfgig.Save();
            MainOnline.Text = (BiaoDingConfgig.Instance.RunModeOnline ? "正式版" : "开发版");
            MessageBox.Show("已切换到"+ MainOnline.Text + "，请重开软件。");
            this.Close();
        }

        private void TxtUserLogout_Click(object sender, RoutedEventArgs e)
        {
            if (UISheBeiBiaoDingViewModel.Intance.IsLogin)
            {
                //退出登录状态
                MessageBoxResult result = MessageBox.Show("是否退出当前账户？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // do something
                    UISheBeiBiaoDingViewModel.Intance.IsLogin = false;
                    TxtUserLogout.Content = "登录";
                }
                else
                {
                    // do something
                }
            }
            else
            {
                UILoginWindow longinWindow = new UILoginWindow();
                longinWindow.Title = "登录";
                longinWindow.Owner = this;//设置父窗口，这样可以在父窗口中居中
                longinWindow.sendMessage = Recevie;
                longinWindow.ShowDialog();//模式，弹出！
            }
            
        }
        public void Recevie(string value)
        {
          
            if (UISheBeiBiaoDingViewModel.Intance.IsLogin)
            {
                TxtUserLogout.Content = "退出登录";
            }
            else
            {
                TxtUserLogout.Content = "登录";

            }
        }

    }
}
