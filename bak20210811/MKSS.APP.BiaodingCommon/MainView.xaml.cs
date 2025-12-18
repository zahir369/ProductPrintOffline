
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
using DeviceDataMonitorWPF.UIBiaoDing;

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
            this.Closing += MainView_Closing;
            UISheBeiBiaoDingViewModel.Intance.MainView = this;
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
