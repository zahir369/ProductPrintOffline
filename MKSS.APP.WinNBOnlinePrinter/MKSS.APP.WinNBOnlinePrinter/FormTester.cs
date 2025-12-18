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

namespace BarTender_Dev_Dome
{
    public partial class FormTester : Form
    {
        public static FormTester Instance;
        bool FormLoading { get; set; }
        public FormTester()
        {
            InitializeComponent();
        }

        Engine btEngine;
        private void FormMain_Load(object sender, EventArgs e)
        {

            this.textBox1.Focus();
            Printers printers = new Printers();
            foreach (Printer printer in printers)
            {
                printer_comboBox.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                // Automatically select the default printer.
                printer_comboBox.SelectedItem = printers.Default.PrinterName;
            }

            bool rep = false;
            RefreshData(null,ref rep);

            if (TryStartBarTenderEngine())
            {
                PrintBar(null, null, false);
            }
            else
            {
                BtnPrint.Enabled = false;
                checkBoxPrint.Checked = false;
                checkBoxPrint.Enabled = false;
            }
        }


        private void BtnQuery_Click(object sender, EventArgs e)
        {

            Query();
        }

        pd_device D1;
        pd_device D2;
        void Query()
        {
            this.textBox1.Focus();
            string serialNO = this.textBox1.Text;
            if (serialNO.IndexOf(";") >= 0)
            {
                serialNO = serialNO.Split(';')[0];
            }
            if (serialNO.IndexOf("；") >= 0)
            {
                serialNO = serialNO.Split('；')[0];
            }
            this.labelSerialNo.Text = serialNO;
            this.textBox1.Text = "";
            pd_device dev = null;
            string msg = "";
            string table = @"select * from pd_device s where F_SerialNO='" + serialNO + "' ";

            bool sucess = false;
            List < pd_device > _lstClass = DatabaseObject.Main.QueryList<pd_device>(table);
            if (_lstClass.Count == 0)
            {
                msg = "串号未找到！";
                sucess = false;
            }
            else if (_lstClass.Count > 1)
            {
                msg = "串号重复！";
                sucess = false;
            }
            else
            {
                dev = _lstClass[0];
                msg = "已上线，"+ dev.DeviceStatusCN + "！";
                sucess = true;
            } 

            this.labelREsult.ForeColor = sucess ? Color.Green : Color.Red;
            this.labelREsult.Text = msg;
            if (sucess && dev!=null)
            {
                bool rep = false;
                int sudata = RefreshData(dev,ref rep);
                if (rep)
                {
                    msg = "重复扫码！";
                    this.labelREsult.Text = msg; 
                }

                sudata = sudata + 1;
                if (sudata % 2 == 1)
                {

                    D1 = dev;
                }
                if (sudata % 2 == 0)
                {
                    D2 = dev;
                }


                if (D1 != null || D2 != null)
                {
                    bool print = false;
                    if(checkBoxPrint.Checked && (D1 != null && D2 != null)) print = true;
                    PrintBar(D1, D2, print);
                     
                }

            }

        }


        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintBar(D1, D2, true);
        }


        bool PrintBar(pd_device dev1, pd_device dev2, bool isPrint = false)
        {

            if (btEngine == null)
            {
                if (isPrint)
                {
                    MessageBox.Show("BarTender 打印组件未初始化，无法打印，请检查是否已安装并重新启动程序。", "操作提示");
                }
                return false;
            }

            {
                LabelFormatDocument labelFormat = btEngine.Documents.Open(System.IO.Directory.GetCurrentDirectory() + "\\NB带验证码标签.btw");

                try
                {
                    if (dev1 != null)
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE1", dev1.F_SerialNO + ";" + dev1.F_SerialValidCode);
                        labelFormat.SubStrings.SetSubString("TEXT1", dev1.F_SerialNO);
                    }
                    else {
                        labelFormat.SubStrings.SetSubString("QRCODE1", "000000000000");
                        labelFormat.SubStrings.SetSubString("TEXT1", "000000000000");
                    }

                    if (dev2 != null)
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE2", dev2.F_SerialNO + ";" + dev2.F_SerialValidCode);
                        labelFormat.SubStrings.SetSubString("TEXT2", dev2.F_SerialNO);
                    }
                    else
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE2", "000000000000");
                        labelFormat.SubStrings.SetSubString("TEXT2", "000000000000");
                    }
           
                }

                catch (Exception ex)
                {
                    MessageBox.Show("修改内容出错 " + ex.Message, "操作提示");
                }

                if (labelFormat != null)
                {
                    //Generate a thumbnail for it.
                    labelFormat.ExportImageToFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp", ImageType.BMP, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit,
                        new Resolution(600, 150), OverwriteOptions.Overwrite);

                    System.Drawing.Image image = System.Drawing.Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp");
                    Bitmap NmpImage = new Bitmap(image);
                    pictureBox1.Image = NmpImage;
                    image.Dispose();

                }
                else
                {
                    MessageBox.Show("生成图片错误", "操作提示");
                }

                if (!isPrint) return true;

                if (printer_comboBox.Text != "")
                {
                    labelFormat.PrintSetup.PrinterName = printer_comboBox.Text;
                    labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                }
                else
                {
                    MessageBox.Show("请先选择打印机", "操作提示");
                }
                D1 = null;
                D2 = null;
                return true;
            }
        }

        private bool TryStartBarTenderEngine()
        {
            try
            {
                btEngine = new Engine(true);
                return true;
            }
            catch (Exception ex)
            {
                btEngine = null;
                MessageBox.Show("BarTender 打印组件启动失败，请确认已安装 BarTender 或相关运行组件。" + System.Environment.NewLine + ex.Message, "启动失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
        Dictionary<string, pd_device> _cache = new Dictionary<string, pd_device>();
        public int RefreshData(pd_device dev,ref bool rep)
        {
            try
            {
                
                //SqlSugarClient _db = MysqlDB.GetDB();
                //_lstClass = new List<Device>();
                //_lstClass = _db.Queryable<Device>().Where(w=>w.F_SerialNO!=null && w.F_SerialNO!="").OrderBy(w => w.F_CreateDate, OrderByType.Desc).ToList();
                if (dev != null)
                {
                    if (!_cache.ContainsKey(dev.F_DeviceId))
                    {
                        _cache.Add(dev.F_DeviceId, dev);
                    }
                    else
                    {
                        rep = true;
                        return _lstdata.IndexOf(_cache[dev.F_DeviceId]);//不重复添加
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
            return _lstdata.IndexOf(_cache[dev.F_DeviceId]);
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
    }
}
