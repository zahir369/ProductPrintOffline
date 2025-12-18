using Microsoft.Extensions.Options;
using MKSS.IServices;
using MKSS.Model;
using MKSS.Services;
using MKSS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DeviceDataMonitorWPF.UIControls
{
    /// <summary>
    /// DataMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class DataMonitor : UserControl
    {
        //ITestUserServices iTestUserSer = new TestUserServices();
        IDataItemServices iDataItemSer = new DataItemServices();

        public DataMonitor()
        {
            InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //var result = await iTestUserSer.GetUser("456");
            //if (result != null)
            //{
            //    MessageBox.Show(result.TestId);
            //}

            var result = await GetData();

        }

        private async Task<IList<DataItem>> GetData()
        {
            //var users = Enumerable.Range(0, 10000).Select(a => new TestUser
            //{
            //    TestId = Guid.NewGuid().ToString(),
            //    UserAge = new Random().Next(100),
            //    UserName = a + ""
            //});

            try
            {
                //for (var a = 0; a < 5; a++)
                //{                    
                return await iDataItemSer.Query();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }

        }
    }
}
