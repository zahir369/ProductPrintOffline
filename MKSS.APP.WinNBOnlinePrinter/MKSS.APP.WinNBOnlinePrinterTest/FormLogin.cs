using MKSS.APP.WinNBOnlinePrinterTest.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Windows.Forms;
 
namespace MKSS.APP.WinNBOnlinePrinterTest
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // 窗体加载时的初始化代码
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public class LoginModel
        {
            public int code { get; set; }
            public string data { get; set; }
            public string msg { get; set; }
        }

        public class LoginPostBody
        {
            public  string username { get; set; }
            public string password { get; set; }
            public string code { get; set; }
            public string uuid { get; set; }
        }
            private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;


            //post验证用户名密码
            var client = new RestClient("http://mes.nbiotcenter.com/prod-api/login");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            LoginPostBody postBody = new LoginPostBody();
            postBody.username = "admin";
            postBody.password = "admin";
            postBody.code = "";
            postBody.uuid = "";

            string body = JsonConvert.SerializeObject(postBody);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
            LoginModel loginInfo = JsonConvert.DeserializeObject<LoginModel>(response.Content);
            if (loginInfo.code == 0)
            {
                MessageBox.Show("登录成功！");
              
               


                AppConfig.Token = loginInfo.data;
                Console.WriteLine(AppConfig.Token);
                Close();
            }
            else { 
                MessageBox.Show("登录失败！" + loginInfo.msg);

            }






            // 简单的用户名和密码验证
            //if (username == "admin" && password == "123456")
            //{
            //    MessageBox.Show("登录成功！");
            //    Close();

            //}
            //else
            //{
            //    MessageBox.Show("用户名或密码错误，请重试。");
            //}
        }
    }
}
