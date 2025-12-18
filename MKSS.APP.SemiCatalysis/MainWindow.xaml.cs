
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
using MKSS.APP.SemiCatalysis;
using MKSS.Service.SemiCatalysis;
using MKSS.Util.Log;

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


        [LogTagClass(Title = "Window_KeyDown")]
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ULogger.Log(e.Key.ToString());
            if (e.Key == Key.Enter || e.Key == Key.S)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Setting.BtnStart_Click(null, null);
            }
            if (e.Key == Key.Space || e.Key == Key.P)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Setting.BtnPause_Click(null, null);
            }


            if (e.Key == Key.Q)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Setting.BtnFinish_Click(null, null);
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Setting.BtnFinish_Click(null, null);
            }


            if (e.Key == Key.E)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Setting.BtnExcelExport_Click(null, null);
            }
            if (e.Key == Key.U)
            {
                e.Handled = true;
                BtnExit_Click(null, null);
            }

            
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Environment.Exit(1);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                SemiCatalysisSaver.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                SemiCatalysisSaver.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
