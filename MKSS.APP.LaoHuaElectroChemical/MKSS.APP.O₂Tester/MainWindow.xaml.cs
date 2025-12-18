
using MKSS.APP.O2Tester.UIControls;
using MKSS.APP.O2Tester.UserCommon;
using MKSS.APP.O2Tester.UserControls;
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
using MKSS.APP.O2Tester;
using MKSS.Service.O2Tester;
using MKSS.Util.Log;
using System.Windows.Threading;

namespace MKSS.APP.O2Tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在启动", "正在启动，请稍等......", this);
            Instance = this;
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 1);//设置的间隔为5s
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();
        }
        public bool TimerRefreshIng { get { return false; } }
        private void Timer_Tick(object sender, EventArgs e)
        {

            DateTime d = DateTime.Now;
            TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss") + (TimerRefreshIng ? "..." : "");

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                O2TesterSaver.Dispose();
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
                O2TesterSaver.Dispose();
            }
            catch (Exception)
            {

            }
        }

        void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
        }

        [LogTagClass(Title = "Window_KeyDown")]
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ULogger.Log(e.Key.ToString()); 
            if (e.Key == Key.Enter || e.Key == Key.S)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnStart_Click(null, null);
            }
            if (e.Key == Key.Space || e.Key == Key.P)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnPause_Click(null, null);
            }


            if ( e.Key == Key.Q)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }


            if ( e.Key == Key.E)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.BtnExcelExport_Click(null, null);
            }
            if (e.Key == Key.U)
            {
                e.Handled = true;
                BtnExit_Click(null, null);
            }

            if ( e.Key == Key.Left)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowMode_Pre();
            }
            if ( e.Key == Key.Right)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowMode_Next();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Environment.Exit(1);
        }

        private readonly MKSS.APP.O2Tester.PaletteHelper _paletteHelper = new MKSS.APP.O2Tester.PaletteHelper();
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
