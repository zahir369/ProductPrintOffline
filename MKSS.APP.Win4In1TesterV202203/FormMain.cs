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
                 return PROTOCOL.V1;
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
            }

            
   

        }
          

        private void FormMain_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SelectCOM(i);
            }
            RefreshData();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in list)
            {
                if (item.Started) {
                    MessageBox.Show("请先停止数据读取！");
                    e.Cancel = true;
                    return;
                }
                //item.Saver.Dispose();
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
