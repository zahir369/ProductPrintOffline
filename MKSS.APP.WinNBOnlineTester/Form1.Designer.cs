
namespace MKSS.APP.WinNBTester
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelCount = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelREsult = new System.Windows.Forms.Label();
            this.BtnQuery = new System.Windows.Forms.Button();
            this.labelSerialNo = new System.Windows.Forms.Label();
            this.fIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fDeviceIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fIMEIDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fIMSIDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fICCIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fSerialNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fProductIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fNBProductIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fProductNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fNBProductNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fCreateDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fRegisterTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fStatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fLastRefreshTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceStatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceStatusCNDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 272);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1210, 259);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fIdDataGridViewTextBoxColumn,
            this.fDeviceIdDataGridViewTextBoxColumn,
            this.fIMEIDataGridViewTextBoxColumn,
            this.fIMSIDataGridViewTextBoxColumn,
            this.fICCIDDataGridViewTextBoxColumn,
            this.fSerialNODataGridViewTextBoxColumn,
            this.fProductIdDataGridViewTextBoxColumn,
            this.fNBProductIdDataGridViewTextBoxColumn,
            this.fProductNameDataGridViewTextBoxColumn,
            this.fNBProductNameDataGridViewTextBoxColumn,
            this.fCreateDateDataGridViewTextBoxColumn,
            this.fRegisterTimeDataGridViewTextBoxColumn,
            this.fStatusDataGridViewTextBoxColumn,
            this.fLastRefreshTimeDataGridViewTextBoxColumn,
            this.deviceStatusDataGridViewTextBoxColumn,
            this.deviceStatusCNDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.deviceBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1272, 517);
            this.dataGridView1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(809, 132);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(398, 124);
            this.panel1.TabIndex = 8;
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new System.Drawing.Font("宋体", 64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCount.Location = new System.Drawing.Point(1094, 9);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(166, 86);
            this.labelCount.TabIndex = 1;
            this.labelCount.Text = "000";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(256, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(297, 56);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "000000000000";
            this.textBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textBox1_PreviewKeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelSerialNo);
            this.panel2.Controls.Add(this.BtnQuery);
            this.panel2.Controls.Add(this.labelREsult);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.labelCount);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1272, 99);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 99);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1272, 517);
            this.panel3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(16, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(234, 43);
            this.label2.TabIndex = 10;
            this.label2.Text = "设备串号：";
            // 
            // labelREsult
            // 
            this.labelREsult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelREsult.AutoSize = true;
            this.labelREsult.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelREsult.Location = new System.Drawing.Point(702, 37);
            this.labelREsult.Name = "labelREsult";
            this.labelREsult.Size = new System.Drawing.Size(41, 43);
            this.labelREsult.TabIndex = 10;
            this.labelREsult.Text = "?";
            // 
            // BtnQuery
            // 
            this.BtnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnQuery.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnQuery.Location = new System.Drawing.Point(923, 31);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.Size = new System.Drawing.Size(165, 49);
            this.BtnQuery.TabIndex = 11;
            this.BtnQuery.Text = "查询";
            this.BtnQuery.UseVisualStyleBackColor = true;
            this.BtnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // labelSerialNo
            // 
            this.labelSerialNo.AutoSize = true;
            this.labelSerialNo.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSerialNo.Location = new System.Drawing.Point(559, 31);
            this.labelSerialNo.Name = "labelSerialNo";
            this.labelSerialNo.Size = new System.Drawing.Size(41, 43);
            this.labelSerialNo.TabIndex = 12;
            this.labelSerialNo.Text = "-";
            // 
            // fIdDataGridViewTextBoxColumn
            // 
            this.fIdDataGridViewTextBoxColumn.DataPropertyName = "F_Id";
            this.fIdDataGridViewTextBoxColumn.HeaderText = "F_Id";
            this.fIdDataGridViewTextBoxColumn.Name = "fIdDataGridViewTextBoxColumn";
            // 
            // fDeviceIdDataGridViewTextBoxColumn
            // 
            this.fDeviceIdDataGridViewTextBoxColumn.DataPropertyName = "F_DeviceId";
            this.fDeviceIdDataGridViewTextBoxColumn.HeaderText = "F_DeviceId";
            this.fDeviceIdDataGridViewTextBoxColumn.Name = "fDeviceIdDataGridViewTextBoxColumn";
            // 
            // fIMEIDataGridViewTextBoxColumn
            // 
            this.fIMEIDataGridViewTextBoxColumn.DataPropertyName = "F_IMEI";
            this.fIMEIDataGridViewTextBoxColumn.HeaderText = "F_IMEI";
            this.fIMEIDataGridViewTextBoxColumn.Name = "fIMEIDataGridViewTextBoxColumn";
            // 
            // fIMSIDataGridViewTextBoxColumn
            // 
            this.fIMSIDataGridViewTextBoxColumn.DataPropertyName = "F_IMSI";
            this.fIMSIDataGridViewTextBoxColumn.HeaderText = "F_IMSI";
            this.fIMSIDataGridViewTextBoxColumn.Name = "fIMSIDataGridViewTextBoxColumn";
            // 
            // fICCIDDataGridViewTextBoxColumn
            // 
            this.fICCIDDataGridViewTextBoxColumn.DataPropertyName = "F_ICCID";
            this.fICCIDDataGridViewTextBoxColumn.HeaderText = "F_ICCID";
            this.fICCIDDataGridViewTextBoxColumn.Name = "fICCIDDataGridViewTextBoxColumn";
            // 
            // fSerialNODataGridViewTextBoxColumn
            // 
            this.fSerialNODataGridViewTextBoxColumn.DataPropertyName = "F_SerialNO";
            this.fSerialNODataGridViewTextBoxColumn.HeaderText = "F_SerialNO";
            this.fSerialNODataGridViewTextBoxColumn.Name = "fSerialNODataGridViewTextBoxColumn";
            // 
            // fProductIdDataGridViewTextBoxColumn
            // 
            this.fProductIdDataGridViewTextBoxColumn.DataPropertyName = "F_ProductId";
            this.fProductIdDataGridViewTextBoxColumn.HeaderText = "F_ProductId";
            this.fProductIdDataGridViewTextBoxColumn.Name = "fProductIdDataGridViewTextBoxColumn";
            // 
            // fNBProductIdDataGridViewTextBoxColumn
            // 
            this.fNBProductIdDataGridViewTextBoxColumn.DataPropertyName = "F_NBProductId";
            this.fNBProductIdDataGridViewTextBoxColumn.HeaderText = "F_NBProductId";
            this.fNBProductIdDataGridViewTextBoxColumn.Name = "fNBProductIdDataGridViewTextBoxColumn";
            // 
            // fProductNameDataGridViewTextBoxColumn
            // 
            this.fProductNameDataGridViewTextBoxColumn.DataPropertyName = "F_ProductName";
            this.fProductNameDataGridViewTextBoxColumn.HeaderText = "F_ProductName";
            this.fProductNameDataGridViewTextBoxColumn.Name = "fProductNameDataGridViewTextBoxColumn";
            // 
            // fNBProductNameDataGridViewTextBoxColumn
            // 
            this.fNBProductNameDataGridViewTextBoxColumn.DataPropertyName = "F_NBProductName";
            this.fNBProductNameDataGridViewTextBoxColumn.HeaderText = "F_NBProductName";
            this.fNBProductNameDataGridViewTextBoxColumn.Name = "fNBProductNameDataGridViewTextBoxColumn";
            // 
            // fCreateDateDataGridViewTextBoxColumn
            // 
            this.fCreateDateDataGridViewTextBoxColumn.DataPropertyName = "F_CreateDate";
            this.fCreateDateDataGridViewTextBoxColumn.HeaderText = "F_CreateDate";
            this.fCreateDateDataGridViewTextBoxColumn.Name = "fCreateDateDataGridViewTextBoxColumn";
            // 
            // fRegisterTimeDataGridViewTextBoxColumn
            // 
            this.fRegisterTimeDataGridViewTextBoxColumn.DataPropertyName = "F_RegisterTime";
            this.fRegisterTimeDataGridViewTextBoxColumn.HeaderText = "F_RegisterTime";
            this.fRegisterTimeDataGridViewTextBoxColumn.Name = "fRegisterTimeDataGridViewTextBoxColumn";
            // 
            // fStatusDataGridViewTextBoxColumn
            // 
            this.fStatusDataGridViewTextBoxColumn.DataPropertyName = "F_Status";
            this.fStatusDataGridViewTextBoxColumn.HeaderText = "F_Status";
            this.fStatusDataGridViewTextBoxColumn.Name = "fStatusDataGridViewTextBoxColumn";
            // 
            // fLastRefreshTimeDataGridViewTextBoxColumn
            // 
            this.fLastRefreshTimeDataGridViewTextBoxColumn.DataPropertyName = "F_LastRefreshTime";
            this.fLastRefreshTimeDataGridViewTextBoxColumn.HeaderText = "F_LastRefreshTime";
            this.fLastRefreshTimeDataGridViewTextBoxColumn.Name = "fLastRefreshTimeDataGridViewTextBoxColumn";
            // 
            // deviceStatusDataGridViewTextBoxColumn
            // 
            this.deviceStatusDataGridViewTextBoxColumn.DataPropertyName = "DeviceStatus";
            this.deviceStatusDataGridViewTextBoxColumn.HeaderText = "DeviceStatus";
            this.deviceStatusDataGridViewTextBoxColumn.Name = "deviceStatusDataGridViewTextBoxColumn";
            // 
            // deviceStatusCNDataGridViewTextBoxColumn
            // 
            this.deviceStatusCNDataGridViewTextBoxColumn.DataPropertyName = "DeviceStatusCN";
            this.deviceStatusCNDataGridViewTextBoxColumn.HeaderText = "DeviceStatusCN";
            this.deviceStatusCNDataGridViewTextBoxColumn.Name = "deviceStatusCNDataGridViewTextBoxColumn";
            this.deviceStatusCNDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // deviceBindingSource
            // 
            this.deviceBindingSource.DataSource = typeof(MKSS.Model.Device);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(467, 144);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(473, 270);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 616);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "美克盛世 NB报警器上线测试工具V1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
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
        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

