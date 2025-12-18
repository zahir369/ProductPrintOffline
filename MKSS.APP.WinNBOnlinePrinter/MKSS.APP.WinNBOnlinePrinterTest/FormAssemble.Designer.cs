
namespace MKSS.APP.WinNBOnlinePrinterTest
{
    partial class FormAssemble
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAssemble));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelCount = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_comboBox = new System.Windows.Forms.ComboBox();
            this.labelSerialNo = new System.Windows.Forms.Label();
            this.printer_comboBox = new System.Windows.Forms.ComboBox();
            this.checkBoxPrint = new System.Windows.Forms.CheckBox();
            this.BtnQuery = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelREsult = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBoxForcePrint = new System.Windows.Forms.CheckBox();
            this.radioButtonIMEI登记 = new System.Windows.Forms.RadioButton();
            this.radioButton装箱 = new System.Windows.Forms.RadioButton();
            this.radioButton贴装 = new System.Windows.Forms.RadioButton();
            this.BtnPrint = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(18, 408);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1815, 388);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1214, 198);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(597, 186);
            this.panel1.TabIndex = 8;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(226, 46);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.Visible = false;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new System.Drawing.Font("宋体", 64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCount.Location = new System.Drawing.Point(1696, 14);
            this.labelCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(310, 128);
            this.labelCount.TabIndex = 1;
            this.labelCount.Text = "0000";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(384, 36);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(444, 80);
            this.textBox1.TabIndex = 9;
            this.textBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBox1_PreviewKeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbl_comboBox);
            this.panel2.Controls.Add(this.labelSerialNo);
            this.panel2.Controls.Add(this.printer_comboBox);
            this.panel2.Controls.Add(this.checkBoxPrint);
            this.panel2.Controls.Add(this.BtnQuery);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.labelCount);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2025, 300);
            this.panel2.TabIndex = 2;
            // 
            // lbl_comboBox
            // 
            this.lbl_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_comboBox.Font = new System.Drawing.Font("宋体", 16F);
            this.lbl_comboBox.FormattingEnabled = true;
            this.lbl_comboBox.Location = new System.Drawing.Point(1422, 211);
            this.lbl_comboBox.Margin = new System.Windows.Forms.Padding(4);
            this.lbl_comboBox.Name = "lbl_comboBox";
            this.lbl_comboBox.Size = new System.Drawing.Size(400, 41);
            this.lbl_comboBox.TabIndex = 133;
            this.lbl_comboBox.Text = "请选择打印模板";
            this.lbl_comboBox.SelectedIndexChanged += new System.EventHandler(this.lbl_comboBox_SelectedIndexChanged_1);
            // 
            // labelSerialNo
            // 
            this.labelSerialNo.AutoSize = true;
            this.labelSerialNo.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSerialNo.Location = new System.Drawing.Point(838, 46);
            this.labelSerialNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialNo.Name = "labelSerialNo";
            this.labelSerialNo.Size = new System.Drawing.Size(59, 64);
            this.labelSerialNo.TabIndex = 12;
            this.labelSerialNo.Text = "-";
            // 
            // printer_comboBox
            // 
            this.printer_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.printer_comboBox.Font = new System.Drawing.Font("宋体", 16F);
            this.printer_comboBox.FormattingEnabled = true;
            this.printer_comboBox.Location = new System.Drawing.Point(959, 211);
            this.printer_comboBox.Margin = new System.Windows.Forms.Padding(4);
            this.printer_comboBox.Name = "printer_comboBox";
            this.printer_comboBox.Size = new System.Drawing.Size(400, 41);
            this.printer_comboBox.TabIndex = 128;
            this.printer_comboBox.Text = "请选择打印机";
            this.printer_comboBox.SelectedIndexChanged += new System.EventHandler(this.printer_comboBox_SelectedIndexChanged);
            // 
            // checkBoxPrint
            // 
            this.checkBoxPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPrint.AutoSize = true;
            this.checkBoxPrint.Checked = true;
            this.checkBoxPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrint.Location = new System.Drawing.Point(1904, 269);
            this.checkBoxPrint.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxPrint.Name = "checkBoxPrint";
            this.checkBoxPrint.Size = new System.Drawing.Size(106, 22);
            this.checkBoxPrint.TabIndex = 129;
            this.checkBoxPrint.Text = "直接打印";
            this.checkBoxPrint.UseVisualStyleBackColor = true;
            // 
            // BtnQuery
            // 
            this.BtnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnQuery.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnQuery.Location = new System.Drawing.Point(1422, 46);
            this.BtnQuery.Margin = new System.Windows.Forms.Padding(4);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.Size = new System.Drawing.Size(248, 74);
            this.BtnQuery.TabIndex = 11;
            this.BtnQuery.Text = "查询";
            this.BtnQuery.UseVisualStyleBackColor = true;
            this.BtnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(24, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(347, 64);
            this.label2.TabIndex = 10;
            this.label2.Text = "设备串号：";
            // 
            // labelREsult
            // 
            this.labelREsult.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelREsult.AutoSize = true;
            this.labelREsult.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelREsult.Location = new System.Drawing.Point(47, 71);
            this.labelREsult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelREsult.Name = "labelREsult";
            this.labelREsult.Size = new System.Drawing.Size(59, 64);
            this.labelREsult.TabIndex = 10;
            this.labelREsult.Text = "?";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.checkBoxForcePrint);
            this.panel3.Controls.Add(this.radioButtonIMEI登记);
            this.panel3.Controls.Add(this.radioButton装箱);
            this.panel3.Controls.Add(this.labelREsult);
            this.panel3.Controls.Add(this.radioButton贴装);
            this.panel3.Controls.Add(this.BtnPrint);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 300);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2025, 794);
            this.panel3.TabIndex = 3;
            // 
            // checkBoxForcePrint
            // 
            this.checkBoxForcePrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxForcePrint.AutoSize = true;
            this.checkBoxForcePrint.Location = new System.Drawing.Point(1904, 9);
            this.checkBoxForcePrint.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxForcePrint.Name = "checkBoxForcePrint";
            this.checkBoxForcePrint.Size = new System.Drawing.Size(106, 22);
            this.checkBoxForcePrint.TabIndex = 134;
            this.checkBoxForcePrint.Text = "强制打印";
            this.checkBoxForcePrint.UseVisualStyleBackColor = true;
            // 
            // radioButtonIMEI登记
            // 
            this.radioButtonIMEI登记.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonIMEI登记.AutoSize = true;
            this.radioButtonIMEI登记.Location = new System.Drawing.Point(36, 754);
            this.radioButtonIMEI登记.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonIMEI登记.Name = "radioButtonIMEI登记";
            this.radioButtonIMEI登记.Size = new System.Drawing.Size(105, 22);
            this.radioButtonIMEI登记.TabIndex = 132;
            this.radioButtonIMEI登记.Text = "IMEI登记";
            this.radioButtonIMEI登记.UseVisualStyleBackColor = true;
            this.radioButtonIMEI登记.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // radioButton装箱
            // 
            this.radioButton装箱.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton装箱.AutoSize = true;
            this.radioButton装箱.Location = new System.Drawing.Point(266, 754);
            this.radioButton装箱.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton装箱.Name = "radioButton装箱";
            this.radioButton装箱.Size = new System.Drawing.Size(105, 22);
            this.radioButton装箱.TabIndex = 131;
            this.radioButton装箱.Text = "装箱工序";
            this.radioButton装箱.UseVisualStyleBackColor = true;
            this.radioButton装箱.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // radioButton贴装
            // 
            this.radioButton贴装.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton贴装.AutoSize = true;
            this.radioButton贴装.Checked = true;
            this.radioButton贴装.Location = new System.Drawing.Point(150, 754);
            this.radioButton贴装.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton贴装.Name = "radioButton贴装";
            this.radioButton贴装.Size = new System.Drawing.Size(105, 22);
            this.radioButton贴装.TabIndex = 130;
            this.radioButton贴装.TabStop = true;
            this.radioButton贴装.Text = "贴码工序";
            this.radioButton贴装.UseVisualStyleBackColor = true;
            this.radioButton贴装.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // BtnPrint
            // 
            this.BtnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPrint.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnPrint.Location = new System.Drawing.Point(1588, 665);
            this.BtnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.Size = new System.Drawing.Size(366, 92);
            this.BtnPrint.TabIndex = 130;
            this.BtnPrint.Text = "打印";
            this.BtnPrint.UseVisualStyleBackColor = true;
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(136, 162);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1800, 453);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(459, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 33);
            this.label1.TabIndex = 134;
            this.label1.Text = "文件名：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(610, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 33);
            this.label3.TabIndex = 135;
            this.label3.Text = "0000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(1072, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 33);
            this.label4.TabIndex = 136;
            this.label4.Text = "统计个数：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(1255, 62);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 33);
            this.label5.TabIndex = 137;
            this.label5.Text = "0 PCS";
            // 
            // FormAssemble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2025, 1094);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormAssemble";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "北湾电子 NB报警器上线组装打印工具V1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelREsult;
        private System.Windows.Forms.Button BtnQuery;
        private System.Windows.Forms.Label labelSerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn fIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fDeviceIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fIMEIDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fIMSIDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fICCIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fSerialNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fProductIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fNBProductIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fProductNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fNBProductNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fCreateDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fRegisterTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fLastRefreshTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceStatusCNDataGridViewTextBoxColumn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox printer_comboBox;
        private System.Windows.Forms.CheckBox checkBoxPrint;
        private System.Windows.Forms.Button BtnPrint;
        private System.Windows.Forms.RadioButton radioButton装箱;
        private System.Windows.Forms.RadioButton radioButton贴装;
        private System.Windows.Forms.RadioButton radioButtonIMEI登记;
        private System.Windows.Forms.ComboBox lbl_comboBox;

        private System.Windows.Forms.CheckBox checkBoxForcePrint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
    }
}
