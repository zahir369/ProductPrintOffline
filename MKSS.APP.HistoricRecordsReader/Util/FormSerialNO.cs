using MKSS.APP.HistoricRecordsReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.HistoricRecordsReader
{
    public partial class FormSerialNO : Form
    {
        public FormSerialNO()
        {
            InitializeComponent(); 
        }

        public static FormSerialNO Instance { get; set; }
        FormSensor FormSensor { get; set; } 
        public static void Show(FormSensor form,ulong s )
        {
            if (Instance == null || Instance.IsDisposed)
            {
                Instance = new FormSerialNO();
            }
            else {
                return;
            }
            Instance.FormSensor = form;
             
            Instance.TopMost = true;
            Instance.Owner = form;
            Instance.StartPosition = FormStartPosition.CenterScreen;
            Instance.Text = string.Format("修改设备序列号");
            Instance.textBox1.Text = s.ToString();

            CommandExecuter.Tasks.Clear();
            Instance.FormSensor.RefreshBtnStatus();
            Instance.Show();

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            ulong uu = 0;
            if (!ulong.TryParse(this.textBox1.Text,out uu) || (!this.checkBox1.Checked && (uu<100000000000 || uu > 999999999999)))
            {
                MessageBox.Show("请输入12位设备序列号，只支持数字！" );
                return;
            }

            if (!ulong.TryParse(this.textBox1.Text, out uu) || (this.checkBox1.Checked && (uu < 1000000000000 || uu > 99999999999999)))
            {
                MessageBox.Show("请输入14位设备序列号，只支持数字！");
                return;
            }

            bool sucess = this.FormSensor.SetSerialNo(uu);
            if (sucess)
            {

                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.SerialNo, Par1 = CommandExt.Par1Default });
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("修改序列号失败:"+ this.textBox1.Text );
            this.Close();

        }

        private void FormModify_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (CommandExt item in FormSensor.Commands.Commands.Values)
            {
                if (!item.Reponsed)
                {
                    CommandExecuter.EnqueueTask(item);
                }
            }
            FormSensor.RefreshBtnStatus();
        }

        private void FormModify_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked) {
                if (this.textBox1.Text.Length >= 13) {
                    this.textBox1.Text = MKSS.Service.UIBiaoDing.X01.X0(this.textBox1.Text.Substring(0,13), MKSS.Service.UIBiaoDing.X01.N1);
                }
            }
        }
    }
}
