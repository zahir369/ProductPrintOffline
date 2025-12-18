using MKSS.APP.BiaodingAlcohol.UIBiaoDing.Util;
using MKSS.APP.BiaodingAlcohol.UIBiaoDing.Util.Beans;
using MKSS.APP.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Util;
using MKSS.Core.Common.HttpRestSharp;
using MKSS.Service.UIBiaoDing;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MKSS.APP.BiaodingAlcohol.UIBiaoDing
{
    /// <summary>
    /// UILoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UILoginWindow : Window
    {
        public UILoginWindow()
        {
            //WindowStartupLocation = WindowStartupLocation.CenterScreen;//屏幕居中
            WindowStartupLocation = WindowStartupLocation.CenterOwner;//在父窗口中居中

            InitializeComponent();
            //设置默认输入焦点
            //FocusManager.SetFocusedElement(this, tbContent);
        }

        public delegate void SendMessage(string value);
        public SendMessage sendMessage;

        private void TxtUserLogout_Click(object sender, RoutedEventArgs e)
        {


            EnInfoLoginToken token =  IotEnInfoController.LoginByPassword(TxtUser.Text, TxtPsw.Text);
            if (token.Success)
            {
                //存储当前账号密码
                UISheBeiBiaoDingViewModel.Intance.Account = TxtUser.Text;
                UISheBeiBiaoDingViewModel.Intance.Password = TxtPsw.Text;
                UISheBeiBiaoDingViewModel.Intance.IsLogin = true;
                MessageBox.Show("登录成功");
            }
            sendMessage("ok");
            this.Close();
        }
    }
}
