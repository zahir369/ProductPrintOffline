using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.ConfigToolMulti
{
    public partial class FormModify : Form
    {
        public FormModify()
        {
            InitializeComponent(); 
        }

        public static FormModify Instance { get; set; }
        AddrValueListTable DataArray { get; set; }
        AddrValueList AddrValueList { get; set; }
        AddrValue AddrValue { get; set; }
        FormMain FormMain { get; set; }
        bool ComMode { get { return this.AddrValue.DropdownList != null && this.AddrValue.DropdownList.Length > 0; } }
        public static void Show(AddrValue addr, FormMain form, AddrValueList _AddrValueList,AddrValueListTable _DataArray)
        {

            if (Instance == null || Instance.IsDisposed)
            {
                Instance = new FormModify();
            }
            else {
                return;
            }

            FormMain.WritingValues = true;
            Instance.TopMost = true;
            Instance.Owner = form;
            Instance.StartPosition = FormStartPosition.CenterScreen;
            Instance.AddrValue = addr; Instance.FormMain = form; Instance.AddrValueList = _AddrValueList; Instance.DataArray = _DataArray;
            Instance.Text = string.Format("修改{0}=>{1}的值", _AddrValueList.DeviceAddr, Instance.AddrValue.Name);
            Instance.textBoxName.Text = Instance.AddrValue.Name;
            if (Instance.ComMode)
            {
                Instance.textBoxValue.Visible = false;
                Instance.comboBoxValue.Visible = true;
                AddrConfigItem sel = null;
                foreach (var item in Instance.AddrValue.DropdownList)
                {
                    Instance.comboBoxValue.Items.Add(item);
                    if (item.Value == Instance.AddrValue.Value) sel = item;
                }
                if (sel != null) Instance.comboBoxValue.SelectedItem = sel;
            }
            else
            {

                Instance.textBoxValue.Visible = true;
                Instance.comboBoxValue.Visible = false;
                Instance.textBoxValue.Text = Instance.AddrValue.Value.ToString();

            }
            Instance.Show();

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {

            if (ComMode)
            {
                if (this.comboBoxValue.SelectedItem == null) {
                    MessageBox.Show("请选择"+ string.Format("{0}的值", AddrValue.Name));
                    return;
                }
                bool sucess = this.FormMain.WriteValue(AddrValue, (this.comboBoxValue.SelectedItem as AddrConfigItem).Value, this.checkBox1.Checked, AddrValueList.DeviceAddr);
                if (sucess)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else MessageBox.Show("修改" + string.Format("{0}的值失败", AddrValue.Name));

            }
            else
            {
                if (string.IsNullOrEmpty(this.textBoxValue.Text))
                {
                    MessageBox.Show("请输入" + string.Format("{0}的值", AddrValue.Name));
                    return;
                }
                bool sucess = this.FormMain.WriteValue(AddrValue, ushort.Parse(this.textBoxValue.Text), this.checkBox1.Checked, AddrValueList.DeviceAddr);
                if (sucess)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else MessageBox.Show("修改" + string.Format("{0}的值失败", AddrValue.Name));
            }
            this.Close();
        }

        private void FormModify_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormModify_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.WritingValues = false;
        }
    }
}
