
namespace MKSS.APP.ConfigTool
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtAddr = new System.Windows.Forms.TextBox();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.CbxBandRate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CbxSerialPorts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.分类 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.值 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.地址 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.原始值 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefreshTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReadOnlyStr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.修改 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataUnitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addrValueBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addrValueBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnRefresh);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.TxtAddr);
            this.panel1.Controls.Add(this.BtnConnect);
            this.panel1.Controls.Add(this.CbxBandRate);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.CbxSerialPorts);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(930, 44);
            this.panel1.TabIndex = 0;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Location = new System.Drawing.Point(527, 10);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(75, 23);
            this.BtnRefresh.TabIndex = 10;
            this.BtnRefresh.Text = "读取数据";
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(620, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(108, 16);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "1000ms自动刷新";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(349, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "地址";
            // 
            // TxtAddr
            // 
            this.TxtAddr.Location = new System.Drawing.Point(384, 11);
            this.TxtAddr.Name = "TxtAddr";
            this.TxtAddr.Size = new System.Drawing.Size(56, 21);
            this.TxtAddr.TabIndex = 7;
            this.TxtAddr.Text = "1";
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(446, 10);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(75, 23);
            this.BtnConnect.TabIndex = 4;
            this.BtnConnect.Text = "连接";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // CbxBandRate
            // 
            this.CbxBandRate.FormattingEnabled = true;
            this.CbxBandRate.Location = new System.Drawing.Point(222, 11);
            this.CbxBandRate.Name = "CbxBandRate";
            this.CbxBandRate.Size = new System.Drawing.Size(121, 20);
            this.CbxBandRate.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "波特率";
            // 
            // CbxSerialPorts
            // 
            this.CbxSerialPorts.FormattingEnabled = true;
            this.CbxSerialPorts.Location = new System.Drawing.Point(48, 11);
            this.CbxSerialPorts.Name = "CbxSerialPorts";
            this.CbxSerialPorts.Size = new System.Drawing.Size(121, 20);
            this.CbxSerialPorts.TabIndex = 1;
            this.CbxSerialPorts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CbxSerialPorts_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "串口";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxLogger);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 350);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(930, 100);
            this.panel2.TabIndex = 1;
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLogger.Location = new System.Drawing.Point(0, 0);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLogger.Size = new System.Drawing.Size(930, 100);
            this.textBoxLogger.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 44);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(930, 306);
            this.panel3.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeight = 30;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.分类,
            this.名称,
            this.值,
            this.dataUnitDataGridViewTextBoxColumn,
            this.地址,
            this.原始值,
            this.RefreshTime,
            this.ReadOnlyStr,
            this.修改});
            this.dataGridView1.DataSource = this.addrValueBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(930, 306);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.SizeChanged += new System.EventHandler(this.dataGridView1_SizeChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // 分类
            // 
            this.分类.DataPropertyName = "Type";
            this.分类.HeaderText = "分类";
            this.分类.Name = "分类";
            this.分类.ReadOnly = true;
            // 
            // 名称
            // 
            this.名称.DataPropertyName = "Name";
            this.名称.HeaderText = "名称";
            this.名称.Name = "名称";
            this.名称.ReadOnly = true;
            // 
            // 值
            // 
            this.值.DataPropertyName = "ShowValue";
            this.值.HeaderText = "值";
            this.值.Name = "值";
            this.值.ReadOnly = true;
            // 
            // 地址
            // 
            this.地址.DataPropertyName = "AddressStr";
            this.地址.HeaderText = "地址";
            this.地址.Name = "地址";
            this.地址.ReadOnly = true;
            // 
            // 原始值
            // 
            this.原始值.DataPropertyName = "Value";
            this.原始值.HeaderText = "原始值";
            this.原始值.Name = "原始值";
            this.原始值.ReadOnly = true;
            // 
            // RefreshTime
            // 
            this.RefreshTime.DataPropertyName = "RefreshTimeStr";
            this.RefreshTime.HeaderText = "刷新时间";
            this.RefreshTime.Name = "RefreshTime";
            this.RefreshTime.ReadOnly = true;
            // 
            // ReadOnlyStr
            // 
            this.ReadOnlyStr.DataPropertyName = "ReadOnlyStr";
            this.ReadOnlyStr.HeaderText = "可写";
            this.ReadOnlyStr.Name = "ReadOnlyStr";
            this.ReadOnlyStr.ReadOnly = true;
            // 
            // 修改
            // 
            this.修改.DataPropertyName = "ReadOnlyBtnText";
            this.修改.HeaderText = "修改";
            this.修改.Name = "修改";
            this.修改.ReadOnly = true;
            this.修改.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataUnitDataGridViewTextBoxColumn
            // 
            this.dataUnitDataGridViewTextBoxColumn.DataPropertyName = "DataUnit";
            this.dataUnitDataGridViewTextBoxColumn.HeaderText = "单位";
            this.dataUnitDataGridViewTextBoxColumn.Name = "dataUnitDataGridViewTextBoxColumn";
            this.dataUnitDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // addrValueBindingSource
            // 
            this.addrValueBindingSource.DataSource = typeof(MKSS.APP.ConfigTool.AddrValue);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 450);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "美克盛世 XXX";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addrValueBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox CbxBandRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CbxSerialPorts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox TxtAddr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource addrValueBindingSource;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 分类;
        private System.Windows.Forms.DataGridViewTextBoxColumn 名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 值;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataUnitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 地址;
        private System.Windows.Forms.DataGridViewTextBoxColumn 原始值;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefreshTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReadOnlyStr;
        private System.Windows.Forms.DataGridViewButtonColumn 修改;
    }
}

