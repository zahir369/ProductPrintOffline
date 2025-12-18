using System;
using System.Collections.Generic;
using System.IO;
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

namespace DeviceDataMonitorWPF.UserControls
{
    /// <summary>
    /// UserNongDuPie.xaml 的交互逻辑
    /// </summary>
    public partial class UserNongDuPie : UserControl
    {
        public void SetNongDuData(double recData)
        {
            //webbro.InvokeScript("jsShowData", recData);

            //var jsons = new JavaScriptSerializer().Serialize(listdata);

            string js = $"jsShowData({recData});";
            webbro.GetBrowser().MainFrame.ExecuteJavaScriptAsync(js);

        }
        public UserNongDuPie()
        {
            InitializeComponent();
            LoadChart();
        }
        private void LoadChart()
        {
            //var path = new Uri(Directory.GetCurrentDirectory() + "/WebChart/wendu.html");
            //webbro.Navigate(path);

            var path = Directory.GetCurrentDirectory() + "/WebChart/wendu.html";
            webbro.Address = path;
        }

        private void webbro_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            //加载完成

        }
    }
}
