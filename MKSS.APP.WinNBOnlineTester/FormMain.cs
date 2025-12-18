
using MKSS.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKSS.APP.WinNBTester
{
    public partial class FormMain : Form
    {


        public static FormMain Instance; 
        bool FormLoading { get; set; } 
        public FormMain()
        {
            FormLoading = true;
            InitializeComponent();
            Instance = this;
             

        }

        private void BtnQuery_Click(object sender, EventArgs e)
        {

            Query();
        }

        void Query() {

            string serialNO = this.textBox1.Text;
            if (serialNO.IndexOf(";") >= 0) {
                serialNO= serialNO.Split(';')[0];
            }
            this.labelSerialNo.Text = serialNO;
            this.textBox1.Text="";
            Device dev = null;
            string msg = "";
            bool sucess = MysqlDB.QueryDevice(serialNO, ref dev, ref msg);
            this.labelREsult.ForeColor = sucess ? Color.Green : Color.Red;
            this.labelREsult.Text = msg;
            if (sucess)
            {
                RefreshData(dev);
            }

        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                Console.WriteLine("------------------");
                Query();
                Print();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            RefreshData(null); 
        }

        List<Device> _lstClass = new List<Device>();
        public void RefreshData(Device dev)
        {
            try
            {
                //SqlSugarClient _db = MysqlDB.GetDB();
                //_lstClass = new List<Device>();
                //_lstClass = _db.Queryable<Device>().Where(w=>w.F_SerialNO!=null && w.F_SerialNO!="").OrderBy(w => w.F_CreateDate, OrderByType.Desc).ToList();
                if(dev!=null) _lstClass.Insert(0,dev);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _lstClass;
                dataGridView1.Refresh();
                this.labelCount.Text = _lstClass.Count.ToString("0000");
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                this.labelREsult.Text = "数据查询失败!" + ex.Message ;
            }
        }

        void Print() {
            using (Seagull.BarTender.Print.Engine btEngine = new Seagull.BarTender.Print.Engine(true))
            {
                Seagull.BarTender.Print.LabelFormatDocument labelFormat = btEngine.Documents.Open(System.IO.Directory.GetCurrentDirectory()+ "\\test.btw");

                try
                {
                    labelFormat.SubStrings.SetSubString("name", "songgj");
                    labelFormat.SubStrings.SetSubString("age", "songgj");
                    labelFormat.SubStrings.SetSubString("ID", "songgj");
                    labelFormat.SubStrings.SetSubString("code", "songgj");
                }

                catch (Exception ex)
                {
                    MessageBox.Show("修改内容出错 " + ex.Message, "操作提示");
                }

                if (labelFormat != null)
                {
                    //Generate a thumbnail for it.
                    labelFormat.ExportImageToFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp" , 
                        Seagull.BarTender.Print.ImageType.BMP, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit, 
                        new Seagull.BarTender.Print.Resolution(407, 407 ), Seagull.BarTender.Print.OverwriteOptions.Overwrite);

                    System.Drawing.Image image = System.Drawing.Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp");
                    Bitmap NmpImage = new Bitmap(image);
                    pictureBox1.Image = NmpImage;
                    image.Dispose();
                }
                else
                {
                    MessageBox.Show("生成图片错误", "操作提示");
                }

                //if (isPreView) return;

                //if (_PrinterName != "")
                //{
                //    labelFormat.PrintSetup.PrinterName = _PrinterName;
                //    labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                //}
                //else
                //{
                //    MessageBox.Show("请先选择打印机", "操作提示");
                //}
            } 
        }

    }

}
