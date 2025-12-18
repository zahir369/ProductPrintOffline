
namespace MKSS.APP.WinNBOnlinePrinterTest
{
    partial class FormAppIMEI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAppIMEI));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.fPackageXhDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fPackageNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deliverStatusCNDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fPackageTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pddeviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.labelCount = new System.Windows.Forms.Label();
            this.textBoxScan = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelSerialNo = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnQuery = new System.Windows.Forms.Button();
            this.labelREsult = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CbxNBProductList = new System.Windows.Forms.ComboBox();
            this.CbxProductList = new System.Windows.Forms.ComboBox();
            this.radioButtonIMEI登记 = new System.Windows.Forms.RadioButton();
            this.radioButton装箱 = new System.Windows.Forms.RadioButton();
            this.radioButton贴装 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pddeviceBindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(597, 186);
            this.panel1.TabIndex = 8;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fPackageXhDataGridViewTextBoxColumn,
            this.fPackageNoDataGridViewTextBoxColumn,
            this.dataGridViewTextBoxColumn6,
            this.deliverStatusCNDataGridViewTextBoxColumn,
            this.fPackageTimeDataGridViewTextBoxColumn,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10});
            this.dataGridView1.DataSource = this.pddeviceBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(2025, 845);
            this.dataGridView1.TabIndex = 7;
            // 
            // fPackageXhDataGridViewTextBoxColumn
            // 
            this.fPackageXhDataGridViewTextBoxColumn.DataPropertyName = "F_PackageXh";
            this.fPackageXhDataGridViewTextBoxColumn.HeaderText = "序号";
            this.fPackageXhDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.fPackageXhDataGridViewTextBoxColumn.Name = "fPackageXhDataGridViewTextBoxColumn";
            this.fPackageXhDataGridViewTextBoxColumn.ReadOnly = true;
            this.fPackageXhDataGridViewTextBoxColumn.Width = 150;
            // 
            // fPackageNoDataGridViewTextBoxColumn
            // 
            this.fPackageNoDataGridViewTextBoxColumn.DataPropertyName = "F_PackageNo";
            this.fPackageNoDataGridViewTextBoxColumn.HeaderText = "箱号";
            this.fPackageNoDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.fPackageNoDataGridViewTextBoxColumn.Name = "fPackageNoDataGridViewTextBoxColumn";
            this.fPackageNoDataGridViewTextBoxColumn.ReadOnly = true;
            this.fPackageNoDataGridViewTextBoxColumn.Width = 150;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "F_SerialNO";
            this.dataGridViewTextBoxColumn6.HeaderText = "串号";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 150;
            // 
            // deliverStatusCNDataGridViewTextBoxColumn
            // 
            this.deliverStatusCNDataGridViewTextBoxColumn.DataPropertyName = "DeliverStatusCN";
            this.deliverStatusCNDataGridViewTextBoxColumn.HeaderText = "状态";
            this.deliverStatusCNDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.deliverStatusCNDataGridViewTextBoxColumn.Name = "deliverStatusCNDataGridViewTextBoxColumn";
            this.deliverStatusCNDataGridViewTextBoxColumn.ReadOnly = true;
            this.deliverStatusCNDataGridViewTextBoxColumn.Width = 150;
            // 
            // fPackageTimeDataGridViewTextBoxColumn
            // 
            this.fPackageTimeDataGridViewTextBoxColumn.DataPropertyName = "F_PackageTime";
            this.fPackageTimeDataGridViewTextBoxColumn.HeaderText = "装箱时间";
            this.fPackageTimeDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.fPackageTimeDataGridViewTextBoxColumn.Name = "fPackageTimeDataGridViewTextBoxColumn";
            this.fPackageTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.fPackageTimeDataGridViewTextBoxColumn.Width = 150;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "F_IMEI";
            this.dataGridViewTextBoxColumn3.HeaderText = "F_IMEI";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "F_IMSI";
            this.dataGridViewTextBoxColumn4.HeaderText = "F_IMSI";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "F_ICCID";
            this.dataGridViewTextBoxColumn5.HeaderText = "F_ICCID";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // dataGridViewTextBoxColumn16
            // 
            this.dataGridViewTextBoxColumn16.DataPropertyName = "DeviceStatusCN";
            this.dataGridViewTextBoxColumn16.HeaderText = "联网状态";
            this.dataGridViewTextBoxColumn16.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            this.dataGridViewTextBoxColumn16.Width = 150;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "F_ProductName";
            this.dataGridViewTextBoxColumn9.HeaderText = "云平台型号";
            this.dataGridViewTextBoxColumn9.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Width = 150;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "F_NBProductName";
            this.dataGridViewTextBoxColumn10.HeaderText = "电信平台型号";
            this.dataGridViewTextBoxColumn10.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Width = 150;
            // 
            // pddeviceBindingSource
            // 
            this.pddeviceBindingSource.DataSource = typeof(MKSS.APP.WinNBOnlinePrinterTest.pd_device);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new System.Drawing.Font("宋体", 64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCount.Location = new System.Drawing.Point(1306, 45);
            this.labelCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(694, 128);
            this.labelCount.TabIndex = 1;
            this.labelCount.Text = "+0000/0000";
            // 
            // textBoxScan
            // 
            this.textBoxScan.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxScan.Location = new System.Drawing.Point(291, 45);
            this.textBoxScan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxScan.Name = "textBoxScan";
            this.textBoxScan.Size = new System.Drawing.Size(676, 80);
            this.textBoxScan.TabIndex = 9;
            this.textBoxScan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxScan_KeyPress);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelTime);
            this.panel2.Controls.Add(this.labelSerialNo);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.BtnQuery);
            this.panel2.Controls.Add(this.labelREsult);
            this.panel2.Controls.Add(this.labelCount);
            this.panel2.Controls.Add(this.textBoxScan);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2025, 249);
            this.panel2.TabIndex = 2;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(993, 106);
            this.labelTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(26, 18);
            this.labelTime.TabIndex = 135;
            this.labelTime.Text = "0s";
            // 
            // labelSerialNo
            // 
            this.labelSerialNo.AutoSize = true;
            this.labelSerialNo.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSerialNo.Location = new System.Drawing.Point(279, 160);
            this.labelSerialNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialNo.Name = "labelSerialNo";
            this.labelSerialNo.Size = new System.Drawing.Size(59, 64);
            this.labelSerialNo.TabIndex = 12;
            this.labelSerialNo.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(24, 160);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(283, 64);
            this.label3.TabIndex = 14;
            this.label3.Text = "IMEI号：";
            // 
            // BtnQuery
            // 
            this.BtnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnQuery.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnQuery.Location = new System.Drawing.Point(1048, 56);
            this.BtnQuery.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.Size = new System.Drawing.Size(248, 74);
            this.BtnQuery.TabIndex = 11;
            this.BtnQuery.Text = "扫码";
            this.BtnQuery.UseVisualStyleBackColor = true;
            this.BtnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // labelREsult
            // 
            this.labelREsult.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelREsult.AutoSize = true;
            this.labelREsult.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelREsult.Location = new System.Drawing.Point(984, 160);
            this.labelREsult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelREsult.Name = "labelREsult";
            this.labelREsult.Size = new System.Drawing.Size(59, 64);
            this.labelREsult.TabIndex = 10;
            this.labelREsult.Text = "?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(24, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(283, 64);
            this.label1.TabIndex = 15;
            this.label1.Text = "扫码枪：";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.CbxNBProductList);
            this.panel3.Controls.Add(this.CbxProductList);
            this.panel3.Controls.Add(this.radioButtonIMEI登记);
            this.panel3.Controls.Add(this.radioButton装箱);
            this.panel3.Controls.Add(this.radioButton贴装);
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 249);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2025, 845);
            this.panel3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1316, 803);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(188, 18);
            this.label4.TabIndex = 139;
            this.label4.Text = "请选择电信网平台型号";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1316, 763);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(188, 18);
            this.label5.TabIndex = 138;
            this.label5.Text = "请选择物联网平台型号";
            // 
            // CbxNBProductList
            // 
            this.CbxNBProductList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CbxNBProductList.FormattingEnabled = true;
            this.CbxNBProductList.Location = new System.Drawing.Point(1512, 797);
            this.CbxNBProductList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CbxNBProductList.Name = "CbxNBProductList";
            this.CbxNBProductList.Size = new System.Drawing.Size(493, 26);
            this.CbxNBProductList.TabIndex = 137;
            this.CbxNBProductList.SelectedIndexChanged += new System.EventHandler(this.CbxNBProductList_SelectedIndexChanged);
            // 
            // CbxProductList
            // 
            this.CbxProductList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CbxProductList.FormattingEnabled = true;
            this.CbxProductList.Location = new System.Drawing.Point(1512, 757);
            this.CbxProductList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CbxProductList.Name = "CbxProductList";
            this.CbxProductList.Size = new System.Drawing.Size(493, 26);
            this.CbxProductList.TabIndex = 136;
            this.CbxProductList.SelectedIndexChanged += new System.EventHandler(this.CbxProductList_SelectedIndexChanged);
            // 
            // radioButtonIMEI登记
            // 
            this.radioButtonIMEI登记.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonIMEI登记.AutoSize = true;
            this.radioButtonIMEI登记.Checked = true;
            this.radioButtonIMEI登记.Location = new System.Drawing.Point(18, 806);
            this.radioButtonIMEI登记.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonIMEI登记.Name = "radioButtonIMEI登记";
            this.radioButtonIMEI登记.Size = new System.Drawing.Size(105, 22);
            this.radioButtonIMEI登记.TabIndex = 135;
            this.radioButtonIMEI登记.TabStop = true;
            this.radioButtonIMEI登记.Text = "IMEI登记";
            this.radioButtonIMEI登记.UseVisualStyleBackColor = true;
            this.radioButtonIMEI登记.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // radioButton装箱
            // 
            this.radioButton装箱.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton装箱.AutoSize = true;
            this.radioButton装箱.Location = new System.Drawing.Point(260, 806);
            this.radioButton装箱.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButton装箱.Name = "radioButton装箱";
            this.radioButton装箱.Size = new System.Drawing.Size(105, 22);
            this.radioButton装箱.TabIndex = 133;
            this.radioButton装箱.Text = "装箱工序";
            this.radioButton装箱.UseVisualStyleBackColor = true;
            this.radioButton装箱.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // radioButton贴装
            // 
            this.radioButton贴装.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton贴装.AutoSize = true;
            this.radioButton贴装.Location = new System.Drawing.Point(144, 806);
            this.radioButton贴装.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButton贴装.Name = "radioButton贴装";
            this.radioButton贴装.Size = new System.Drawing.Size(105, 22);
            this.radioButton贴装.TabIndex = 132;
            this.radioButton贴装.Text = "贴码工序";
            this.radioButton贴装.UseVisualStyleBackColor = true;
            this.radioButton贴装.CheckedChanged += new System.EventHandler(this.radioButton贴装_CheckedChanged);
            // 
            // FormAppIMEI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2025, 1094);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormAppIMEI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "北湾电子 NB报警器IMEI登记工具V1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pddeviceBindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.TextBox textBoxScan;
        private System.Windows.Forms.Panel panel2;
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
        private System.Windows.Forms.RadioButton radioButton装箱;
        private System.Windows.Forms.RadioButton radioButton贴装;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource pddeviceBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn fPackageXhDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fPackageNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn deliverStatusCNDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fPackageTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.RadioButton radioButtonIMEI登记;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox CbxNBProductList;
        private System.Windows.Forms.ComboBox CbxProductList;
    }
}
