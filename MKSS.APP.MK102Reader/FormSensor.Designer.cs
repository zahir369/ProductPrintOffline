
namespace MKSS.APP.MK102Reader
{
    partial class FormSensor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("探测器一");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("探测器二");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("探测器三");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("探测器四");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("探测器五");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("探测器六");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("探测器七");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("探测器八");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("所有设备", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8});
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSensor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnExport = new System.Windows.Forms.Button();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.CbxSerialPorts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabelTimeDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.RecordTimeShowBox = new System.Windows.Forms.ToolStripStatusLabel();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.DateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nongDuDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commandExtBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandExtBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::MKSS.APP.MK102Reader.Properties.Resources.BackgroundImage;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.BtnExport);
            this.panel1.Controls.Add(this.textBoxInterval);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.BtnConnect);
            this.panel1.Controls.Add(this.CbxSerialPorts);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(875, 112);
            this.panel1.TabIndex = 0;
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExport.Location = new System.Drawing.Point(791, 84);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(75, 23);
            this.BtnExport.TabIndex = 10;
            this.BtnExport.Text = "导出(&E)";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInterval.Location = new System.Drawing.Point(506, 86);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(44, 21);
            this.textBoxInterval.TabIndex = 9;
            this.textBoxInterval.Text = "3000ms";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(690, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(173, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "波特率：9600 无校验 1位停止 ";
            // 
            // BtnConnect
            // 
            this.BtnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConnect.Location = new System.Drawing.Point(683, 84);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(102, 23);
            this.BtnConnect.TabIndex = 6;
            this.BtnConnect.Text = "连接设备(&C)";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // CbxSerialPorts
            // 
            this.CbxSerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CbxSerialPorts.FormattingEnabled = true;
            this.CbxSerialPorts.Location = new System.Drawing.Point(556, 86);
            this.CbxSerialPorts.Name = "CbxSerialPorts";
            this.CbxSerialPorts.Size = new System.Drawing.Size(121, 20);
            this.CbxSerialPorts.TabIndex = 5;
            this.CbxSerialPorts.Click += new System.EventHandler(this.CbxSerialPorts_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(190, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Combustible Gas Alarm Controller Reader";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::MKSS.APP.MK102Reader.Properties.Resources.pictureBox1;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(27, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(84, 84);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 23F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(117, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(389, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "可燃气体报警控制器读取器";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(206, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(669, 297);
            this.panel2.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.nongDuDataGridViewTextBoxColumn,
            this.DateTime});
            this.dataGridView1.DataSource = this.commandExtBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(669, 297);
            this.dataGridView1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "节点探测器一";
            treeNode1.Tag = "Sensor1";
            treeNode1.Text = "探测器一";
            treeNode2.Name = "探测器二";
            treeNode2.Tag = "Sensor2";
            treeNode2.Text = "探测器二";
            treeNode3.Name = "探测器三";
            treeNode3.Tag = "Sensor3";
            treeNode3.Text = "探测器三";
            treeNode4.Name = "探测器四";
            treeNode4.Tag = "Sensor4";
            treeNode4.Text = "探测器四";
            treeNode5.Name = "探测器五";
            treeNode5.Tag = "Sensor5";
            treeNode5.Text = "探测器五";
            treeNode6.Name = "探测器六";
            treeNode6.Tag = "Sensor6";
            treeNode6.Text = "探测器六";
            treeNode7.Name = "探测器七";
            treeNode7.Tag = "Sensor7";
            treeNode7.Text = "探测器七";
            treeNode8.Name = "探测器八";
            treeNode8.Tag = "Sensor8";
            treeNode8.Text = "探测器八";
            treeNode9.Name = "节点所有设备";
            treeNode9.Tag = "None";
            treeNode9.Text = "所有设备";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode9});
            this.treeView1.Size = new System.Drawing.Size(206, 297);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.treeView1);
            this.panel3.Controls.Add(this.textBoxLogger);
            this.panel3.Controls.Add(this.statusStrip1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 112);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(875, 420);
            this.panel3.TabIndex = 2;
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxLogger.Location = new System.Drawing.Point(0, 297);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(875, 101);
            this.textBoxLogger.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelTimeDesc,
            this.toolStripStatusLabel2,
            this.StatusLabelTime,
            this.RecordTimeShowBox});
            this.statusStrip1.Location = new System.Drawing.Point(0, 398);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(875, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabelTimeDesc
            // 
            this.StatusLabelTimeDesc.Name = "StatusLabelTimeDesc";
            this.StatusLabelTimeDesc.Size = new System.Drawing.Size(80, 17);
            this.StatusLabelTimeDesc.Text = "未连接设备！";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel2.Text = "当前时间：";
            // 
            // StatusLabelTime
            // 
            this.StatusLabelTime.Name = "StatusLabelTime";
            this.StatusLabelTime.Size = new System.Drawing.Size(126, 17);
            this.StatusLabelTime.Text = "0000-00-00 00:00:00";
            this.StatusLabelTime.Click += new System.EventHandler(this.StatusLabelTime_DoubleClick);
            // 
            // RecordTimeShowBox
            // 
            this.RecordTimeShowBox.Name = "RecordTimeShowBox";
            this.RecordTimeShowBox.Size = new System.Drawing.Size(188, 17);
            this.RecordTimeShowBox.Text = "欢迎使用报警器历史记录读取器！";
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // DateTime
            // 
            this.DateTime.DataPropertyName = "DateTime";
            dataGridViewCellStyle1.Format = "G";
            dataGridViewCellStyle1.NullValue = null;
            this.DateTime.DefaultCellStyle = dataGridViewCellStyle1;
            this.DateTime.HeaderText = "刷新时间";
            this.DateTime.Name = "DateTime";
            this.DateTime.ReadOnly = true;
            this.DateTime.Width = 200;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "名称";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "状态";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nongDuDataGridViewTextBoxColumn
            // 
            this.nongDuDataGridViewTextBoxColumn.DataPropertyName = "NongDu";
            this.nongDuDataGridViewTextBoxColumn.HeaderText = "浓度";
            this.nongDuDataGridViewTextBoxColumn.Name = "nongDuDataGridViewTextBoxColumn";
            this.nongDuDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // commandExtBindingSource
            // 
            this.commandExtBindingSource.DataSource = typeof(MKSS.APP.MK102Reader.SensorEntity);
            // 
            // FormSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 532);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSensor";
            this.Text = "MK-102可燃气体报警控制器读取器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSensor_FormClosing);
            this.Load += new System.EventHandler(this.FormSensor_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandExtBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CbxSerialPorts;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label5;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.BindingSource commandExtBindingSource;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelTime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel RecordTimeShowBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn par1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelTimeDesc;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nongDuDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateTime;
    }
}