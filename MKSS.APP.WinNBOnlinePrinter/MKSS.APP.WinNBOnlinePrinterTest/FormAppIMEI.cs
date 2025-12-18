using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Seagull.BarTender.Print;
using System;
using System.Configuration;
using System.Diagnostics;
using AepSdk.Apis;
using MKSS.Core.Services.SysCTWings;
using MKSS.Core.Common.Helper;
using MKSS.APP.WinNBTester;

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    public partial class FormAppIMEI : Form
    {
        bool FormLoading { get; set; }
        IotYunController IotYun = null;
        BindingList<pd_device> _lstdata = new BindingList<pd_device>();
        System.Media.SoundPlayer player_sucess = new System.Media.SoundPlayer();
        System.Media.SoundPlayer player_error = new System.Media.SoundPlayer();

        int total = 0;
        int total_add = 0;
        public QueryProductListItem SelectProduct
        {
            get; set;
        }

        public QueryNBProductListItem SelectNBProduct
        {
            get; set;
        }
        public FormAppIMEI()
        {
            SplashForm.Instance.Message = "正在启动，请稍候......";
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            SplashForm.Instance.Message = "正在启动，正在连接数据库......";
            //try
            //{
            //    string table = @"select * from pd_device s where F_SerialNO='test' ";
            //    bool sucess = false;
            //    List<pd_device> _lstClass = DatabaseObject.Main.QueryList<pd_device>(table);
            //}
            //catch (Exception ex)
            //{
            //    SplashForm.Instance.Visible = false;
            //    SplashForm.Instance.Message = "正在启动，连接数据库失败......";
            //    MessageBox.Show("连接数据库失败："+ex.Message);
            //}

            player_sucess.SoundLocation = Application.StartupPath + "\\Sound\\Windows Proximity Notification.wav";
            player_sucess.Load();

            player_error.SoundLocation = Application.StartupPath + "\\Sound\\Windows Exclamation.wav";
            player_error.Load();
            IotYun = new IotYunController();

            SplashForm.Instance.Message = "正在启动，正在读取数据......";
            bool rep = false;

            this.CbxProductList.Items.Clear();
            QueryProductList produc = IotYun.QueryProductList();
            this.CbxProductList.DataSource = produc.data;
            this.CbxProductList.SelectedItem = null;
            //foreach (QueryProductListItem item in produc.data)
            //{
            //    this.CbxProductList.Items.Add(item); 
            //}


            this.CbxNBProductList.Items.Clear();
            //QueryNBProductList producNB = IotYun.QueryNBProductList();
            //this.CbxNBProductList.DataSource = producNB.data;
            //this.CbxNBProductList.SelectedItem = null;
            //foreach (QueryNBProductListItem item in producNB.data)
            //{
            //    this.CbxNBProductList.Items.Add(item);
            //}

            SplashForm.Instance.Message = "读取完成......";
            SplashForm.Instance.Close();
            this.textBoxScan.Focus();
        }


        private void CbxProductList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CbxProductList.SelectedItem == null) SelectProduct = null;
            else SelectProduct = this.CbxProductList.SelectedItem as QueryProductListItem;
        }

        private void CbxNBProductList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CbxNBProductList.SelectedItem == null) SelectNBProduct = null;
            else SelectNBProduct = this.CbxNBProductList.SelectedItem as QueryNBProductListItem;
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            DoAppIMEI();
            this.textBoxScan.Focus();
        }
         
        pd_device _device;
        void DoAppIMEI()
        {
            Stopwatch sw = new Stopwatch();
            //开始计时  
            sw.Start();
            string msg = "未知";
            bool sucess = false;
            pd_device dev = null;
            try
            {
                this.textBoxScan.Focus();
                string scanContent = this.textBoxScan.Text;
                if (scanContent.IndexOf(";") >= 0)
                {
                    scanContent = scanContent.Split(';')[0];
                }
                if (scanContent.IndexOf("；") >= 0)
                {
                    scanContent = scanContent.Split('；')[0];
                }

                if (SelectProduct == null || SelectNBProduct == null)
                {
                    msg = string.Format("必须先选择产品类别");
                    sucess = false;
                    return;
                }


                List<pd_device> queryDevice = null;
                if ( scanContent.Length == 15)
                {

                    string ks =  "IMEI号";

                    this.labelSerialNo.Text = scanContent;
                    string table = @"select * from pd_device s where F_SerialNO='" + scanContent + "' or F_IMEI='" + scanContent + "' ";
                    queryDevice = DatabaseObject.Main.QueryList<pd_device>(table);
                    
                    if (queryDevice.Count >= 1)
                    {
                        msg = ks + "已经登记过，"+ queryDevice[0].DeviceStatusCN + "！";
                        sucess = true;
                        _lstdata.Add(queryDevice[0]);
                        total++;
                        return;
                    }


                    DatabaseObject.Main.ExecuteSQL(String.Format(
                        "INSERT INTO pd_device SET F_Id='{0}', F_DeviceId=null, F_IMEI='{0}', F_CreateDate='{1}', F_Status=0,F_ProductId='{2}', F_ProductName='{3}', F_NBProductId='{4}', F_NBProductName='{5}'",
                        scanContent, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        SelectProduct.ProductId, SelectProduct.ProductName, SelectNBProduct.ProductId, SelectNBProduct.ProductName
                        ));
                    total_add++;
                    queryDevice = DatabaseObject.Main.QueryList<pd_device>(table);
                    dev = queryDevice[0];
                    try
                    {
                        sucess = IotYun.RegisterNetInner(dev);
                        msg = sucess?"注册电信平台成功": "注册电信平台失败";
                        if (sucess) {
                            queryDevice = DatabaseObject.Main.QueryList<pd_device>(table);
                            dev = queryDevice[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                        sucess = false;
                    }
                    _lstdata.Add(dev);



                } 
                else {

                    msg = "IMEI号格式不正确！";
                    return;

                } 


               
            }
            catch (Exception ex)
            {
                 
            }
            finally
            {
                this.textBoxScan.Text = "";
                this.labelREsult.ForeColor = sucess ? Color.Green : Color.Red;
                this.labelREsult.Text = msg;
                //结束计时  
                sw.Stop();
                this.labelTime.Text = sw.Elapsed.TotalSeconds.ToString("f2")+"s";


                dataGridView1.Invoke((Action)delegate
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = _lstdata;
                    dataGridView1.Refresh();
                });

                this.labelCount.ForeColor = _lstdata.Count <= 50 ? Color.Green : Color.Red;
                this.labelCount.Text = String.Format("+{0}/{1}", total_add.ToString("0000"), total.ToString("0000"));

                if (sucess)
                {
                    player_sucess.Play();
                }
                else {

                    player_error.Play();
                }
                 
            }
            

        }
           
        private void textBoxScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                Console.WriteLine("------------------");
                DoAppIMEI();
                e.Handled = true;
            }
            else { 
            
            }
        }



        public static VersionPacking VersionPacking { get { return (VersionPacking)Enum.Parse(typeof(VersionPacking), ConfigurationManager.AppSettings["VersionPacking"] + ""); } }
        private void radioButton贴装_CheckedChanged(object sender, EventArgs e)
        {
            if (FormLoading) return;
            if (VersionPacking != VersionPacking.Assemble)
            {
                if (this.radioButton贴装.Checked)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["VersionPacking"].Value = VersionPacking.Assemble.ToString();
                    cfa.Save();
                    MessageBox.Show("已切换到贴码工序，请重开软件。");
                    this.Close();
                }
            }
            if (VersionPacking != VersionPacking.Packing)
            {
                if (this.radioButton装箱.Checked)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["VersionPacking"].Value = VersionPacking.Packing.ToString();
                    cfa.Save();
                    MessageBox.Show("已切换到装箱工序，请重开软件。");
                    this.Close();
                }
            }
            if (VersionPacking != VersionPacking.AppIMEI)
            {
                if (this.radioButtonIMEI登记.Checked)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["VersionPacking"].Value = VersionPacking.AppIMEI.ToString();
                    cfa.Save();
                    MessageBox.Show("已切换到IMEI号注册工序，请重开软件。");
                    this.Close();
                }
            }
        }
         

    }
     
}
