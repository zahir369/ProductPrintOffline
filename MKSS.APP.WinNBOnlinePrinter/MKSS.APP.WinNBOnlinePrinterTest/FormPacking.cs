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

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    public partial class FormPacking : Form
    {
        public static FormAssemble Instance;
        bool FormLoading { get; set; }
        package_entity package { get; set; }

        System.Media.SoundPlayer player_sucess = new System.Media.SoundPlayer();
        System.Media.SoundPlayer player_error = new System.Media.SoundPlayer();
        public FormPacking()
        {
            SplashForm.Instance.Message = "正在启动，请稍候......";
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            SplashForm.Instance.Message = "正在启动，正在连接数据库......";
            try
            {
                string table = @"select * from pd_device s where F_SerialNO='test' ";
                bool sucess = false;
                List<pd_device> _lstClass = DatabaseObject.Main.QueryList<pd_device>(table);
            }
            catch (Exception ex)
            {
                SplashForm.Instance.Visible = false;
                SplashForm.Instance.Message = "正在启动，连接数据库失败......";
                MessageBox.Show("连接数据库失败："+ex.Message);
            }

            player_sucess.SoundLocation = Application.StartupPath + "\\Sound\\Windows Proximity Notification.wav";
            player_sucess.Load();

            player_error.SoundLocation = Application.StartupPath + "\\Sound\\Windows Exclamation.wav";
            player_error.Load();


            SplashForm.Instance.Message = "正在启动，正在读取数据......";
            bool rep = false;
            package = new package_entity(this.labelPackageNo.Text, labelREsult,labelCount,this.dataGridView1);
            package.LoadPackage();
            package.LoadDeviceList();
            SplashForm.Instance.Message = "读取完成......";
            SplashForm.Instance.Close();
            this.textBoxScan.Focus();
        }


        private void BtnQuery_Click(object sender, EventArgs e)
        {
            DoPackage();
            this.textBoxScan.Focus();
        }

        pd_device_package _package;
        pd_device _device;


        void DoPackage()
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


                List<pd_device> queryDevice = null;
                if (scanContent.Length == 12|| scanContent.Length == 14 || scanContent.Length == 15)
                {

                    string ks = (scanContent.Length == 12|| scanContent.Length == 14) ? "串号" : "IMEI号";
                    if (string.IsNullOrEmpty(this.labelPackageNo.Text) || 
                        (!string.IsNullOrEmpty(this.labelPackageNo.Text) && this.labelPackageNo.Text.Length != 9)) {
                        msg = "请先扫包装箱码！";
                        sucess = false;
                        return;
                    }

                    if (package.list.Count >= 50) {
                        msg = "包装箱已满！";
                        sucess = false;
                        return;
                    }

                    this.labelSerialNo.Text = scanContent;
                    string table = @"select * from pd_device s where F_SerialNO='" + scanContent + "' or F_IMEI='" + scanContent + "' ";
                    queryDevice = DatabaseObject.Main.QueryList<pd_device>(table);
                    if (queryDevice.Count == 0)
                    {
                        msg = ks + "未找到！";
                        sucess = false;
                        return;
                    }

                    if (queryDevice.Count > 1)
                    {
                        msg = ks + "重复存在！";
                        sucess = false;
                        return;
                    }

                    dev = queryDevice[0];
                    if (dev.DeliverStatus == DeliverStatus.NoPrint)
                    {
                        //msg = "设备未贴码，无法装箱。";
                        //sucess = false;
                        //return;
                    }

                    if (dev.DeliverStatus == DeliverStatus.Packaged)
                    {
                        if (dev.F_PackageNo == this.labelPackageNo.Text)
                        {
                            msg = "重复装箱！";
                            sucess = false;
                            return;
                        }
                        else
                        {
                            msg = "已装到其他箱：" + dev.F_PackageNo + "！";
                            sucess = false;
                            return;
                        }
                    }

                    if (dev.DeliverStatus == DeliverStatus.Deliverd)
                    {
                        msg = "已发货！";
                        sucess = false;
                        return;
                    }

                    string dapp = "";
                    this.package.AddDevice(dev);
                    if (dev.DeliverStatus != DeliverStatus.NoPrint) dapp = "，" + dev.DeliverStatusCN;
                    
                    msg = "装箱成功，" + dev.DeviceStatusCN + dapp + "！";
                    sucess = true;


                }
                else if (scanContent.Length == 9)
                {
                    this.labelPackageNo.Text = scanContent;
                    this.package = new package_entity(scanContent, labelREsult, this.labelCount, this.dataGridView1);
                    msg = "开始装箱：" + this.labelPackageNo.Text + "！";
                    sucess = true; 
                    return;

                }
                else {

                    msg = "箱号、串号、IMEI号不存在！";
                    return;

                }

                if (string.IsNullOrEmpty(this.labelPackageNo.Text))
                {
                    msg = "请先扫箱子二维码！"; 
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
                DoPackage();
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

        private void button删除报警器_Click(object sender, EventArgs e)
        {

            if (this.dataGridView1.SelectedRows.Count == 0) {
                MessageBox.Show("请先选择要删除装箱的报警器！");
                this.textBoxScan.Focus();
                return;
            }

            if (MessageBox.Show(
                  string.Format("确定要删除装箱报警器吗？", this.labelPackageNo.Text),
                  "确认删除", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                this.package.RemoveDevice();
            }

            this.textBoxScan.Focus();
        }

        private void button删除包装箱_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(
                   string.Format("确定要删除{0}货柜数据吗？", this.labelPackageNo.Text),
                   "确认删除", MessageBoxButtons.YesNoCancel ) ==  DialogResult.Yes)
            {
                this.package.RemovePackage();
                this.labelPackageNo.Text = "-";
                package = new package_entity(this.labelPackageNo.Text, labelREsult, labelCount, this.dataGridView1);
                package.LoadDeviceList();
            }

            this.textBoxScan.Focus();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

    }

    public class package_entity
    {
        string package_no;
        System.Windows.Forms.Label labelREsult;
        System.Windows.Forms.Label labelCount;
        System.Windows.Forms.DataGridView dataGridView1;
        BindingList<pd_device> _lstdata = new BindingList<pd_device>();
        public pd_device_package package { get; set; }
        public BindingList<pd_device> list { get { return _lstdata; } }
        public package_entity(string _package_no,
            System.Windows.Forms.Label l, System.Windows.Forms.Label lc, System.Windows.Forms.DataGridView grid) {
            _lstdata = new BindingList<pd_device>();

            labelREsult = l;
            labelCount = lc;
            dataGridView1 = grid;
            package_no = _package_no;
            LoadPackage();
            LoadDeviceList();
        }


        public void LoadPackage()
        {

            Task.Run(() =>
            {
                try
                {
                    string table = @"select * from pd_device_package s where F_PackageNo='" + package_no + "' ";
                    var _lstpa = DatabaseObject.Main.QueryList<pd_device_package>(table);
                    if (_lstpa.Count == 0)
                    {
                        DatabaseObject.Main.ExecuteSQL(String.Format(
                            "INSERT INTO pd_device_package SET F_PackageNo='{0}', F_CreateDate='{1}', F_DeviceCount=0, F_DeliverBatchNo=null, F_DeliverDate=null",
                            package_no, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                }
                catch (Exception ex)
                {
                    labelREsult.Invoke((Action)delegate
                    {
                        this.labelREsult.Text = "数据查询失败!" + ex.Message;
                    });
                }
            });


        }

        public void LoadDeviceList()
        {

            Task.Run(() =>
            {
                try
                {

                    string table = @"select * from pd_device s where F_PackageNo='" + package_no + "' order by F_PackageXh desc";
                    var _lstdatax = DatabaseObject.Main.QueryList<pd_device>(table);
                    foreach (var item in _lstdatax)
                    {
                        this._lstdata.Add(item);
                    }
                    dataGridView1.Invoke((Action)delegate
                    {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = _lstdata;
                        dataGridView1.Refresh();
                        this.labelCount.ForeColor = _lstdata.Count <= 50 ? Color.Green : Color.Red;
                        this.labelCount.Text = String.Format("{0}/50", _lstdata.Count.ToString());
                    });

                    //Application.DoEvents();
                }
                catch (Exception ex)
                {
                    labelREsult.Invoke((Action)delegate
                    {
                        this.labelREsult.Text = "数据查询失败!" + ex.Message;
                    });

                }
            });

        }

        public void AddDevice(pd_device dev) {
            try
            {

                string str1 =(String.Format("update pd_device set F_PrintStatus=" + (int)DeliverStatus.Packaged + ",F_PackageTime='{1}',F_PackageNo='{2}',F_PackageXh={3} where F_SerialNO='{0}'",
                        dev.F_SerialNO, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), package_no, _lstdata.Count + 1));
                string str2 = (String.Format(
                       "update pd_device_package SET F_DeviceCount={1} where F_PackageNo='{0}'",
                       package_no, _lstdata.Count + 1));
                int i1 = DatabaseObject.Main.ExecuteSQL(str1);
                int i2 = DatabaseObject.Main.ExecuteSQL(str2);
                if (i1 == 0 || i2 == 0) {
                    throw new Exception("装箱失败！");
                }
                bool find = _lstdata.Where(w => w.F_SerialNO == dev.F_SerialNO).Any();
                if (!find)
                {
                    dev.DeliverStatus = DeliverStatus.Packaged;
                    dev.F_PackageNo = package_no;
                    dev.F_PackageTime = DateTime.Now;
                    dev.F_PackageXh = _lstdata.Count + 1;
                    _lstdata.Add(dev);
                    dataGridView1.Invoke((Action)delegate
                    {
                        //dataGridView1.DataSource = null;
                        //dataGridView1.DataSource = _lstdata;
                        dataGridView1.Update();
                        this.labelCount.ForeColor = _lstdata.Count <= 50 ? Color.Green : Color.Red;
                        this.labelCount.Text = String.Format("{0}/50", _lstdata.Count.ToString());
                    });
                }
            }
            catch (Exception ex)
            {
                labelREsult.Invoke((Action)delegate
                {
                    this.labelREsult.Text = "数据查询失败!" + ex.Message;
                });

            }
        }

        public void RemoveDevice()
        {
            try
            {
                List<pd_device> del = new List<pd_device>();
                foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
                {
                    pd_device dev = item.DataBoundItem as pd_device;
                    if (dev != null)
                    {
                        int i1 = DatabaseObject.Main.ExecuteSQL(String.Format(
              "update pd_device set F_PrintStatus=1,F_PackageNo=null,F_PackageTime=null,F_PackageXh=null where F_SerialNO='{0}'", dev.F_SerialNO));
                        int i2 = DatabaseObject.Main.ExecuteSQL(String.Format(
                              "update pd_device_package SET F_DeviceCount={1} where F_PackageNo='{0}'",
                              this.package_no, _lstdata.Count - del.Count - 1));
                         
                        if (i1 == 0 || i2 == 0)
                        {
                            throw new Exception("删除装箱失败！");
                        }
                        bool find = _lstdata.Where(w => w.F_SerialNO == dev.F_SerialNO).Any();
                        if (find)
                        {
                            del.Add(dev);
                        }
                    }
                }

                foreach (var item in del)
                {
                    _lstdata.Remove(item);
                }
                dataGridView1.Invoke((Action)delegate
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = _lstdata;
                    dataGridView1.Refresh();
                    this.labelCount.ForeColor = _lstdata.Count <= 50 ? Color.Green : Color.Red;
                    this.labelCount.Text = String.Format("{0}/50", _lstdata.Count.ToString());
                });

                this.labelREsult.ForeColor = Color.Red;
                this.labelREsult.Text = "删除成功！";

            }
            catch (Exception ex)
            {
                labelREsult.Invoke((Action)delegate
                {
                    this.labelREsult.Text = "数据查询失败!" + ex.Message;
                });

            }
        }


        public void RemovePackage()
        {
            try
            {
                DatabaseObject.Main.ExecuteSQL(String.Format(
            "update pd_device set F_PrintStatus=1,F_PackageNo=null,F_PackageTime=null,F_PackageXh=null where F_PackageNo='{0}'", package_no));
                DatabaseObject.Main.ExecuteSQL(String.Format(
                  "delete from pd_device_package where F_PackageNo='{0}'", this.package_no)); 
                this.labelREsult.ForeColor = Color.Red;
                this.labelREsult.Text = "删除成功！";

            }
            catch (Exception ex)
            {
                labelREsult.Invoke((Action)delegate
                {
                    this.labelREsult.Text = "数据查询失败!" + ex.Message;
                });

            }
        }
    }
}
