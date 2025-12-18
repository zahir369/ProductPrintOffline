using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKSS.APP.Win4In1Tester
{
    public partial class FormMain : Form
    {

        public static FormMain Instance;
        List<UIConnection> list = new List<UIConnection>();
        public PROTOCOL Protocal
        {
            get
            {
                if (this.rV1.Checked) return PROTOCOL.V1;
                if (this.rV2.Checked) return PROTOCOL.V2;
                if (this.rV3.Checked) return PROTOCOL.V3;
                return PROTOCOL.No;
            }
        }
        public FormMain()
        {
            InitializeComponent();
            Instance = this;
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
   

        }
          

        private void FormMain_Load(object sender, EventArgs e)
        {
            RefreshData();
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
         
        public void RefreshData() {
            try
            {
               
            }
            catch (Exception ex)
            {
                this.uiConnection1.UpdateText = "数据查询失败!" + ex.Message + System.Environment.NewLine;
            }
        }
         

    }

}
