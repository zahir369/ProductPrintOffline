using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.ConfigTool
{
    public partial class FormModify : Form
    {
        public FormModify()
        {
            InitializeComponent();
        }
        AddrValue AddrValue { get; set; }
        FormMain FormMain { get; set; }
        bool ComMode { get { return this.AddrValue.DropdownList != null && this.AddrValue.DropdownList.Length > 0; } }
        public FormModify(AddrValue addr, FormMain form)
        {
            InitializeComponent();
            AddrValue = addr; FormMain = form;
            this.Text = string.Format("修改{0}的值", AddrValue.Name);
            this.textBoxName.Text = AddrValue.Name;
            if (ComMode)
            {
                this.textBoxValue.Visible = false;
                this.comboBoxValue.Visible = true;
                AddrConfigItem sel = null;
                foreach (var item in this.AddrValue.DropdownList)
                {
                    this.comboBoxValue.Items.Add(item);
                    if (item.Value == AddrValue.Value) sel = item;
                }
                if (sel != null) this.comboBoxValue.SelectedItem = sel;
            }
            else {

                this.textBoxValue.Visible = true;
                this.comboBoxValue.Visible = false;
                this.textBoxValue.Text = AddrValue.Value.ToString();

            }


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
                bool sucess = this.FormMain.WriteValue(AddrValue, (this.comboBoxValue.SelectedItem as AddrConfigItem).Value);
                if (sucess) { 
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
                bool sucess = this.FormMain.WriteValue(AddrValue, ushort.Parse(this.textBoxValue.Text));
                if (sucess)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else MessageBox.Show("修改" + string.Format("{0}的值失败", AddrValue.Name));
            } 

        }
    }
}
