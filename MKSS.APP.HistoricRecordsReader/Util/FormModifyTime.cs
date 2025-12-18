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
    public partial class FormModifyTime : Form
    {
        public FormModifyTime()
        {
            InitializeComponent(); 
        }

        public static FormModifyTime Instance { get; set; }
        FormSensor FormSensor { get; set; } 
        public static void Show(FormSensor form )
        {
            if (Instance == null || Instance.IsDisposed)
            {
                Instance = new FormModifyTime();
            }
            else {
                return;
            }
            Instance.FormSensor = form;
             
            Instance.TopMost = true;
            Instance.Owner = form;
            Instance.StartPosition = FormStartPosition.CenterScreen;
            Instance.Text = string.Format("修改设备时间" );

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

            if (this.dateTimePicker1.Value == DateTime.MinValue)
            {
                MessageBox.Show("请选择" + string.Format("{0}", comboBoxValue.SelectedValue + ""));
                return;
            }
            bool sucess = this.FormSensor.SetDeviceTime(this.dateTimePicker1.Value, comboBoxValue.SelectedValue + "");
            if (sucess)
            {
                CommandExecuter.EnqueueTask(new CommandExt() { CommandType = CommandType.DateTimeNow, Par1 = CommandExt.Par1Default });
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("修改" + string.Format("{0}的值失败", comboBoxValue.SelectedValue+""));
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

    }
}
