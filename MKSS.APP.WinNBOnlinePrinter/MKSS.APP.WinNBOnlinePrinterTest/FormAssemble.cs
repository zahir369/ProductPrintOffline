using System;
using System.Collections.Generic;

using System.Windows.Forms;
using Seagull.BarTender.Print;
using System.Configuration;
using MKSS.APP.WinNBOnlinePrinterTest.Ass;
using System.Collections.Concurrent;
using Org.BouncyCastle.Ocsp;
using MySqlX.XDevAPI.Common;
using System.IO;
using RestSharp;
using MKSS.APP.WinNBOnlinePrinterTest.Model;
using Newtonsoft.Json;
using static MKSS.APP.WinNBOnlinePrinterTest.FormLogin;
using System.Linq;

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    public partial class FormAssemble : Form
    {
        public static FormAssemble Instance;
        bool FormLoading { get; set; }
        TmpPrintAbs TmpPrintAbs { get; set; }
        Engine btEngine = new Engine(true);
        public static string PrintTemplate { get { return (ConfigurationManager.AppSettings["PrintTemplate"] + ""); } }
        System.Media.SoundPlayer player_sucess = new System.Media.SoundPlayer();
        System.Media.SoundPlayer player_error = new System.Media.SoundPlayer();

        public static List<BatchModel.ListItem> orderList = new List<BatchModel.ListItem>();
        public static List<TaskModel.ListItem> orderList2 = new List<TaskModel.ListItem>();

        string orderSeleted = null;
        string orderSeleted2 = null;
        BatchModel.ListItem orderSeletedModel = null;
        TaskModel.ListItem orderSeletedModel2 = null;

        public FormAssemble()
        {
            SplashForm.Instance.Message = "正在启动，请稍候......";
            InitializeComponent();
            TmpPrintAbs.CreateAllInstancesOf<TmpPrintAbs>(btEngine, pictureBox1, printer_comboBox);
            foreach (System.IO.FileInfo item in (new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory())).GetFiles("*.btw"))
            {
                this.lbl_comboBox.Items.Add(item.Name);
                if (PrintTemplate == item.Name) {
                    lbl_comboBox.SelectedItem = item.Name;
                }
            }
            if (lbl_comboBox.SelectedItem == null) lbl_comboBox.SelectedIndex = 0;
            Lbl_comboBox_SelectedIndexChanged(null, null);

            lbl_comboBox.SelectedIndexChanged -= Lbl_comboBox_SelectedIndexChanged;
            lbl_comboBox.SelectedIndexChanged += Lbl_comboBox_SelectedIndexChanged;

                    

        }
       

        

        //

        private void Lbl_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (lbl_comboBox.SelectedItem+"" != PrintTemplate) {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["PrintTemplate"].Value = lbl_comboBox.SelectedItem + "";
                cfa.Save();
                MessageBox.Show("已切换到打印"+ lbl_comboBox.SelectedItem + "" + "模式，请重开软件。");
                this.Close();
            }
            TmpPrintAbs = TmpPrintAbs.Of(lbl_comboBox.SelectedItem + "");
        }

       

       

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.textBox1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            SplashForm.Instance.Message = "正在启动，正在连接数据库......";
            try
            {
                //string table = @"select * from pd_device s where F_SerialNO='test' ";
                //bool sucess = false;
                //List<pd_device> _lstClass = DatabaseObject.Main.QueryList<pd_device>(table);
            }
            catch (Exception ex)
            {
                SplashForm.Instance.Message = "正在启动，连接数据库失败......";
                SplashForm.Instance.Close();
                MessageBox.Show("连接数据库失败："+ex.Message);
                this.Close();
            }
           

            this.textBox1.Focus();
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                printer_comboBox.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                printer_comboBox.SelectedItem = printers.Default.PrinterName;
            }

            player_sucess.SoundLocation = Application.StartupPath + "\\Sound\\Windows Proximity Notification.wav";
            player_sucess.Load();

            player_error.SoundLocation = Application.StartupPath + "\\Sound\\Windows Exclamation.wav";
            player_error.Load();

            SplashForm.Instance.Message = "正在启动，正在连接打印机......";
            bool rep = false;
            RefreshData(null,ref rep);
            btEngine = new Engine(true);
            TmpPrintAbs.PrintBar(false);
            SplashForm.Instance.Message = "已连接......";

            SplashForm.Instance.Close();
            this.textBox1.Focus();

        }
        public class SnPostModel
        {
           public List<string> imeiList = new List<string>();
            public string workorderId { get; set; }

            public string taskId { get; set; }

            public bool iotPrintFlag { get; set; }

            public string remark { get; set; }
        }

        public class SnReciveModel
        {
            public string msg { get; set; }
            public int code { get; set; }
        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {
            Query();
        }
        int queryIndex = 0;
        void Query()
        {

            bool sucess = false;
         
            try
            {

                this.textBox1.Focus();

                string serialNO = this.textBox1.Text;
                if (serialNO.Contains("；"))
                {
                    serialNO = serialNO.Split('；')[0];

                }
                if (serialNO.Contains(";"))
                {
                    serialNO = serialNO.Split(';')[0];

                }
                pd_device dev = new pd_device();
                dev.F_IMEI = serialNO;
                dev.F_SerialNO = serialNO;
                //长度校验
                if (dev.F_IMEI.Length != 15 || !dev.F_IMEI.Substring(0,2).Equals("86"))
                {

                    MessageBox.Show("IMEI长度为15位且86开头，请重新扫码", "提示!!!");

                }
                else
                {
                    //存储打印记录到本地
                  

                    String fileName = "BW";
                    fileName = fileName + System.DateTime.Now.ToString("yyyyMMdd");

                    label3.Text = fileName;
                    string fileUriDic = $"C:\\BWP";
                    //判断文件夹是否存在
                    if (!Directory.Exists(fileUriDic))
                    {
                        //创建文件夹
                        try
                        {
                          Directory.CreateDirectory(fileUriDic);
                        }
                        catch (Exception e)
                        {
                        }
                    }



                    string fileUri = $"C:\\BWP\\{fileName}" + ".txt";
                //判断文件是否存在
                    if (!File.Exists(fileUri))
                    {
                        //创建文件
                        try
                        {
                           FileStream fs = File.Create(fileUri);
                            fs.Close();
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    // 读取文件的所有行
                    string[] lines = File.ReadAllLines(fileUri);
                    // 检查文件是否包含某个字符串
                    bool containsString = lines.Any(line => line.Contains(serialNO));
                    if (containsString)
                    {
                        //不存
                    }
                    else
                    {

                        StreamWriter sw = File.AppendText(fileUri); //保存到指定路径
                        sw.Write(serialNO + "\r\n");
                        sw.Flush();
                        sw.Close();

                    }

                    //count for now file.
                    label5.Text = File.ReadLines(fileUri).Count().ToString() + "PCS";

                    //打印
                    //赋值第一个标签
                    TmpPrintAbs.Print(dev, queryIndex, true);
                    queryIndex++;

                    //赋值第二个标签
                    TmpPrintAbs.Print(dev, queryIndex, true);
                    queryIndex++;
                                                      

                }
                    this.textBox1.Text = "";



                //if (serialNO.IndexOf(";") >= 0)
                //{
                //    serialNO = serialNO.Split(';')[0];
                //}
                //if (serialNO.IndexOf("；") >= 0)
                //{
                //    serialNO = serialNO.Split('；')[0];
                //}
                //this.labelSerialNo.Text = serialNO;
                //this.textBox1.Text = "";

                //if (serialNO.Length == 24){
                //    serialNO = TmpPrintJinKaXo.FromSerialNo24(serialNO);
                //}
                //pd_device dev = null;
                //string msg = "";
                //string table = @"select * from pd_device s where F_SerialNO='" + serialNO + "' or F_IMEI='"+ serialNO + "' ";

                //List<pd_device> _lstClass = DatabaseObject.Main.QueryList<pd_device>(table);
                //if (_lstClass.Count == 0)
                //{
                //    msg = "串号未找到！";
                //    sucess = false;
                //}
                //else if (_lstClass.Count > 1)
                //{
                //    msg = "串号重复！";
                //    sucess = false;
                //}
                //else
                //{
                //    dev = _lstClass[0];

                //    if (dev.F_BD_HEGE != 1)
                //    {
                //        msg = "不良品！";
                //        sucess = false;
                //    }
                //    else {
                //        string dapp = "";
                //        if (dev.DeliverStatus != DeliverStatus.NoPrint) dapp = "，" + dev.DeliverStatusCN;
                //        msg = "已上线，" + dev.DeviceStatusCN + dapp + "！";
                //        DatabaseObject.Main.ExecuteSQL(String.Format("update pd_device set F_PrintStatus=" + (int)DeliverStatus.Printed + ",F_PrintDate='{1}' where F_SerialNO='{0}' or F_IMEI='{0}'", serialNO, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                //        sucess = true;
                //    }

                //}

                //this.labelREsult.ForeColor = sucess ? Color.Green : Color.Red;
                //this.labelREsult.Text = msg;
                //if (sucess && dev != null)
                //{
                //    bool rep = false;
                //    int sudata = RefreshData(dev, ref rep);
                //    if (dev.F_PrintStatus == 1 || rep) { 
                //        msg = "重复扫码【"+ dev.F_PrintDate + "】，需手动打印！";
                //        this.labelREsult.Text = msg;
                //        rep = true;
                //    }
                //    sudata = sudata + 1;
                //    if(this.checkBoxForcePrint.Checked) rep = false;
                //    TmpPrintAbs.Print(dev, sudata,this.checkBoxPrint.Checked && !rep);
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {

                this.textBox1.Focus();
                if (sucess)
                {
                    player_sucess.Play();
                }
                else
                {

                    player_error.Play();
                }


            }
            

        }
        

        

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            TmpPrintAbs.PrintBar(true); 
            this.textBox1.Focus();
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Console.WriteLine("------------------");
                Query();
                Print();
            }
        }


        List<pd_device> _lstdata = new List<pd_device>();
        ConcurrentDictionary<string, pd_device> _cache = new ConcurrentDictionary<string, pd_device>();
        public int RefreshData(pd_device dev,ref bool rep)
        {
            try
            {
                
                //SqlSugarClient _db = MysqlDB.GetDB();
                //_lstClass = new List<Device>();
                //_lstClass = _db.Queryable<Device>().Where(w=>w.F_SerialNO!=null && w.F_SerialNO!="").OrderBy(w => w.F_CreateDate, OrderByType.Desc).ToList();
                if (dev != null)
                {
                    if (!_cache.ContainsKey(dev.F_Id))
                    {
                        _cache.TryAdd(dev.F_Id, dev);
                    }
                    else
                    {
                        rep = true;
                        return _lstdata.IndexOf(_cache[dev.F_Id]);//不重复添加
                    }
                    _lstdata.Add(dev);
                }
                //dataGridView1.DataSource = null;
                //dataGridView1.DataSource = _lstdata;
                //dataGridView1.Refresh();
                this.labelCount.Text = _lstdata.Count.ToString("0000");
                //Application.DoEvents();
            }
            catch (Exception ex)
            {
                this.labelREsult.Text = "数据查询失败!" + ex.Message;
            }
            if (dev == null) return 0;
            return _lstdata.IndexOf(_cache[dev.F_Id]);
        }

        void Print()
        {

        }

        private void printer_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
             
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public static VersionPacking VersionPacking { get { return (VersionPacking)Enum.Parse(typeof(VersionPacking),ConfigurationManager.AppSettings["VersionPacking"] + ""); } }
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

        private void lbl_comboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void order_comboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void order_comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void order_comboBox2_SelectedIndexChanged_2(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    public enum VersionPacking
    {
        AppIMEI,
        Assemble,
        Packing
    }
}
