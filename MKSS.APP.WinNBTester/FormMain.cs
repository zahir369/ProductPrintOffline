using MKSS.Core.Common.HttpRestSharp;
using MKSS.Core.Model.SysBase;
using MKSS.Model;
using MKSS.Services;
using RestSharp;
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
        public IotYunController IotYun = new IotYunController();
        List<UIConnection> list = new List<UIConnection>();

        bool FormLoading { get; set; }
        public static bool VersionOffLine { get { return (ConfigurationManager.AppSettings["VersionOffLine"] +"").ToLower()=="true"; } }
        public static bool VersionOnline { get { return !VersionOffLine; } }
        public FormMain()
        {
            IniTasks();

            //IotYun.Test("861248055839073");
            //IotYun.Test("861248055844636");
            FormLoading = true;
            InitializeComponent();
            Instance = this;
            this.Text += (VersionOnline ? "联机版" : "脱机版");
            this.radioButton联网版.Checked = VersionOnline;
            this.radioButton脱机版.Checked = VersionOffLine;
            comboBoxNB.SelectedIndex = 0;//不同型号，休眠模式不一样
            comboBoxNB_SelectedIndexChanged(null,null);

            for (int r = 1; r <= 16; r++)
            {
                string n = string.Format("uiConnection{0}", r );
                UIConnection t1 = this.tableLayoutPanel1.Controls.Find(n,false).FirstOrDefault() as UIConnection;
                if (t1 != null)
                {
                    list.Add(t1);
                }
            }

            foreach (var item in list)
            {
                item.Port = null;
            }

            int ii = 0;
            foreach (var item in SerialPort.GetPortNames())
            {
                list[ii].Port = item;
                list[ii].Start();
                ii++;
            }

            if (VersionOnline)
            {
                this.CbxProductList.Items.Clear();
                QueryProductList produc = IotYun.QueryProductList();
                this.CbxProductList.DataSource = produc.data;
                this.CbxProductList.SelectedItem = null;
                //foreach (QueryProductListItem item in produc.data)
                //{
                //    this.CbxProductList.Items.Add(item); 
                //}


                this.CbxNBProductList.Items.Clear();
                QueryNBProductList producNB = IotYun.QueryNBProductList();
                this.CbxNBProductList.DataSource = producNB.data;
                this.CbxNBProductList.SelectedItem = null;
                //foreach (QueryNBProductListItem item in producNB.data)
                //{
                //    this.CbxNBProductList.Items.Add(item);
                //}
            }
            else {
                this.CbxProductList.Enabled = false;
                this.CbxNBProductList.Enabled = false;
            }


            //NBTesterSaver save = new NBTesterSaver(null);
            //save.BtnConnectCommand();

        }
         
        public QueryProductListItem SelectProduct
        {
            get;set;
        }

        public QueryNBProductListItem SelectNBProduct
        {
            get; set;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            RefreshData();
            FormLoading = false;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in list)
            {
                item.Saver.Dispose();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        List<Device> _lstClass = new List<Device>();
        public void RefreshData() {
            try
            {
                SqlSugarClient _db = MysqlDB.GetDB();
                _lstClass = new List<Device>();
                _lstClass = _db.Queryable<Device>().OrderBy(w=>w.F_CreateDate, OrderByType.Desc).Take(100).ToList();
                dataGridView1.DataSource = _lstClass;
                this.labelCount.Text = _db.Queryable<Device>().Count().ToString("0000");
            }
            catch (Exception ex)
            {
                this.uiConnection1.UpdateText = "数据查询失败!" + ex.Message + System.Environment.NewLine;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> _lstID = new List<string>();
                int _deleNum = 0;
                if (dataGridView1.CurrentRow.Index >= 0)
                {

                    StringBuilder _sbMsg = new StringBuilder("确定删除吗?\r\n");
                    DialogResult result = MessageBox.Show(_sbMsg.ToString(), "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        for (int i = this.dataGridView1.SelectedRows.Count; i > 0; i--)
                        {
                            string _iTagID =  (dataGridView1.SelectedRows[i - 1].Cells["F_Id"].Value)+"";
                            _lstID.Add(_iTagID);
                            bool b= FormMain.Instance.IotYun.Delete(_iTagID);
                            _deleNum = _deleNum + (b?1:0);
                        }
                        this.uiConnection1.UpdateText = "删除成功!" + _deleNum + System.Environment.NewLine;

                        RefreshData();
                    }
                    else
                    {
                        this.uiConnection1.UpdateText = "选择放弃删除!" + System.Environment.NewLine;

                    }

                }
            }
            catch (Exception ex)
            {
                this.uiConnection1.UpdateText = "删除失败!" + ex.Message + System.Environment.NewLine;
            }
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

        private void radioButton脱机版_CheckedChanged(object sender, EventArgs e)
        {
            if (FormLoading) return;
            if (VersionOffLine)
            {
                if (this.radioButton联网版.Checked) {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["VersionOffLine"].Value = false.ToString();
                    cfa.Save();
                    MessageBox.Show("已切换到联机版，请重开软件。");
                    this.Close();
                }
            }
            else {
                if (this.radioButton脱机版.Checked)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["VersionOffLine"].Value = true.ToString();
                    cfa.Save();
                    MessageBox.Show("已切换到脱机版，请重开软件。");
                    this.Close();
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            {
                string name = "NB"+DateTime.Now.ToString("yyyyMMddHHmmss");
                var dlg = new SaveFileDialog()
                {
                    Title = name + "-另存为",
                    DefaultExt = "txt",
                    Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                    FileName = name
                };
                if (dlg.ShowDialog() ==  DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        ExportExcel(dlg.FileName, name);
                        Cursor = Cursors.Arrow;
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }
            }
        }


        public void ExportExcel(string path, string _Batch)
        {

            try
            {

                SqlSugarClient _db = MysqlDB.GetDB();
                var tableAll = _db.Queryable<Device>().ToDataTable();

                 
                ECTester.EPPlusHelper.ImportExcel(path, tableAll.DataSet);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void comboBoxNB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNB.SelectedItem + "" == "BC260Y") {
                NBTesterSaver.PAR[(int)(NBTesterSaver.CommandType.设置睡眠模式)][0] = "AT+QCFG=\"wakeupRXD\",0";
                NBTesterSaver.PAR[(int)(NBTesterSaver.CommandType.禁用串口唤醒睡眠)][0] = "AT+QCFG=\"wakeupRXD\",0";
            }
            if (comboBoxNB.SelectedItem + "" == "BC25")
            {
                NBTesterSaver.PAR[(int)(NBTesterSaver.CommandType.设置睡眠模式)][0] = "AT+QSCLK=0";
                NBTesterSaver.PAR[(int)(NBTesterSaver.CommandType.禁用串口唤醒睡眠)][0] = "AT+QSCLK=0";
            }
        }


        void IniTasks()
        {
            StringBuilder sb = new StringBuilder();
            SqlSugarClient _db = MysqlDB.GetDB();
            string sql1 = "SELECT  distinct left(F_SerialNO,7) from pd_device where F_SerialNO is not null";
            DataTable table = _db.Ado.GetDataTable(sql1);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow dr = table.Rows[i];
                string F_SerialNO = dr[0] + "";

                string year = "20" + F_SerialNO.Substring(0, 2);//任务单年份
                string scrwd_xh = F_SerialNO.Substring(2, 5);//任务单序号
                List<ScrwEntity> list = GetScrwListOf(year, scrwd_xh);

                ScrwEntity scrwEntity = null;
                if (list.Count > 0)
                {
                    scrwEntity = list[0];
                }
                if (scrwEntity != null)
                {

                    try
                    {
                        string sql1Update =
                        "update pd_device set F_SCRWD_ProductCode='" + scrwEntity.ProductCode +
                        "'  ,F_SCRWD_ProductType='" + scrwEntity.ProductType +
                        "'  ,F_SCRWD_ProductFullName='" + scrwEntity.ProductFullName +
                        "'  ,F_SCRWD_OrderNumber='" + scrwEntity.OrderNumber +
                        "' where F_SerialNO is not null and left(F_SerialNO,7)='" + F_SerialNO + "';";
                        sb.Append(sql1Update);
                        //_IDeviceServices.BaseDal.Db.Ado.ExecuteCommand(sql1Update);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }

            }


        }


        /// <summary>
        /// 获取生产任务
        /// </summary> 
        /// <param name="key">名称</param>
        /// <param name="finished">是否完成</param>
        /// <param name="depName">部门名称</param>
        /// <param name="userName">销售人员名称</param>
        /// <param name="monthrange">订单开始结束月份</param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns> 
        List<ScrwEntity> GetScrwListOf(string year, string xh)
        {
             
            string GraspAshxsub1 = "http://117.160.239.252:30003/" + "AppWorkingOrderOf.ashx";
            string param = string.Format("year={0}&xh={1}", year, xh);
            //http://localhost:8899/AppWorkingOrder.ashx?finished=false&depName=&userName=&monthFrom=&monthTo
            var client = new RestSharpClient("http://117.160.239.252:30003/");
            var request = client.Execute(new RestRequest($"{GraspAshxsub1}?{param}", Method.GET));


            try
            {
                MesResult temp = Newtonsoft.Json.JsonConvert.DeserializeObject<MesResult>(request.Content);
                return temp.data;
            }
            catch (Exception ex)
            {
                return new List<ScrwEntity>();
            }

            return new List<ScrwEntity>();


        }


    }

}
