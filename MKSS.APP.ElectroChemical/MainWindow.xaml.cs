
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
using MKSS.APP.ElectroChemical;
using MKSS.Service.ElectroChemical;

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在启动", "正在启动，请稍等......", this);
            InitializeComponent();
            if (!MKSS.Model.SensorGroupData.ShowV)
            {
                TextBlockName.Text = "智慧工厂生产数字化管理系统-电化学工业传感器筛选软件";
            }
            else
            {
                TextBlockName.Text = "智慧工厂生产数字化管理系统-电化学水性传感器筛选软件";
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                ElectroChemicalSaver.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
