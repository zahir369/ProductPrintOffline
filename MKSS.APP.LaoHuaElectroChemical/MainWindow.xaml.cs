
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
using MKSS.APP.SemiTester;
using MKSS.Service;
using MKSS.Util.Log;
using System.Windows.Threading;
using MKSS.Service.LaoHuaElectroChemical;
using MKSS.APP.LaoHuaService.MQTT;
using MKSS.Model;

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        DispatcherTimer timer = new DispatcherTimer();
        public Batch TaskFirst { get; set; }
        public MainWindow()
        {
            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在启动", "正在启动，请稍等......", this);
            Instance = this;
            InitializeComponent();

            switch (LaoHuaDataProvider.UIMode)
            {
                case LaoHuaDataProviderUIMode.SingleBoard:
                    MainTitle.Text = "智慧工厂生产数字化管理系统-氧气传感器老化观察软件单层版";
                    break;
                case LaoHuaDataProviderUIMode.SerialPort:
                    MainTitle.Text = "智慧工厂生产数字化管理系统-氧气传感器老化观察软件本地版";
                    break;
                case LaoHuaDataProviderUIMode.NetWork:
                    MainTitle.Text = "智慧工厂生产数字化管理系统-氧气传感器老化观察软件网络版";
                    break;
                default:
                    break;
            }
            this.Title = MainTitle.Text;

            timer.Interval = new TimeSpan(0, 0, 1);//设置的间隔为5s
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();
        }
        public bool TimerRefreshIng { get { return false; } }
        private void Timer_Tick(object sender, EventArgs e)
        {

            if (
                UIZhuiSuC10Model.Instance!=null && UIZhuiSuC10Model.Instance.SettingModel!=null && 
                UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan.TotalSeconds > 0)
            {
                TimeSpan d = UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan;
                TimeNow.Content = string.Format("{0}天{1}时{2}分{3}秒", d.Days,d.Hours,d.Minutes, d.Seconds);
            }
            else {
                DateTime d = DateTime.Now;
                TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss") + (TimerRefreshIng ? "..." : "");
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

                System.Environment.Exit(1);
            }
            catch (Exception)
            {

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception)
            {

            }
        }


        [LogTagClass(Title = "Window_KeyDown")]
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ULogger.Log(e.Key.ToString()); 
            if (e.Key == Key.Enter || e.Key == Key.S)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnStart_Click(null, null);
            }
            if (e.Key == Key.Space || e.Key == Key.P)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnPause_Click(null, null);
            }


            if ( e.Key == Key.Q)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }


            if ( e.Key == Key.E)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnExcelExport_Click(null, null);
            }
            if (e.Key == Key.U)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnExit_Click(null, null);
            }

            if ( e.Key == Key.Left)
            {
                e.Handled = true;
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.Resistance_Pre();
            }
            if ( e.Key == Key.Right)
            {
                e.Handled = true;
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.Resistance_Next();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
            //加载完成后加载上次任务，以及状态
            if (TaskFirst != null) {
                UIZhuiSuTaskList.OpenBatch(TaskFirst, this);
            }
        }

        private readonly MKSS.APP.SemiTester.PaletteHelper _paletteHelper = new MKSS.APP.SemiTester.PaletteHelper();
        private void ApplyTheme(bool isDark)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var b = ToggleDarkStyle.IsChecked;
            ApplyTheme(b!=null && b.Value);
        }
    }
}
