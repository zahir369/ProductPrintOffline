using MKSS.Util.Log.License;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MKSS.APP.LicenseTools
{
    public partial class FormMain : Form
    {
        public static FormMain Instance;
        static bool AuthroizChecked { get; set; }
        public static bool Authroized { get; private set; }
        public FormMain()
        {
            Instance = this;
            InitializeComponent();
        }

        public void Log(string msg) {
            this.textBox1.AppendText(System.Environment.NewLine);
            this.textBox1.AppendText(msg);
        }


        string StartupPath
        {
            get {
                return this.textBox2.Text;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            string str = "";
            #region  xxx

            Log("正在检查环境......");
            ComputerInfo.Instance.EncryptTo(new SHA512CryptoServiceProvider(), StartupPath + "\\MKSS.Util.License.lic");
            if (!AuthroizChecked)
            {
                try
                {

                    try
                    {
                        Authroized = false;
                        AuthroizChecked = true;
                        string pathSrc = StartupPath + "\\MKSS.Util.License.src";
                        ComputerInfo.Instance.SrcTo(pathSrc);
                        string msg = "";
                        //临时检查未通过，正式有检查
                        Authroized = ComputerInfo.Instance.Match(new SHA512CryptoServiceProvider(), StartupPath + "\\MKSS.Util.License.lic",ref msg);
                        Log(Authroized ? "满足要求 ！" : "环境缺少必备SDK ！"+ msg);
                    }
                    catch { }


                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("检查时出错");
                        Console.WriteLine(ex.InnerException.Message);
                        Console.WriteLine(ex.InnerException.StackTrace);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.Message);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.StackTrace);
                    }
                    Console.WriteLine("检查时出错");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    str += (System.Environment.NewLine);
                    str += (ex.Message);
                    str += (System.Environment.NewLine);
                    str += (ex.StackTrace);
                }


            }
            Log(str);

        }

        #endregion

        private void textBox2_Click(object sender, EventArgs e)
        {

        }

    } 

}
