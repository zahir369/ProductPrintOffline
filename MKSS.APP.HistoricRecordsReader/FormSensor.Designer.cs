
namespace MKSS.APP.HistoricRecordsReader
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("报警记录");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("报警恢复记录");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("故障记录");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("故障恢复记录");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("掉电记录");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("上电记录");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("失效记录");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("所有记录", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
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
            this.commandExtBindingSource = new System.Windows.Forms.BindingSource();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button写时间 = new System.Windows.Forms.Button();
            this.button写串号 = new System.Windows.Forms.Button();
            this.button标Span点 = new System.Windows.Forms.Button();
            this.button标定零点 = new System.Windows.Forms.Button();
            this.labelStatusLabelGGDesc = new System.Windows.Forms.Label();
            this.labelStatusLabelTimeDesc = new System.Windows.Forms.Label();
            this.labelNongdu = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelSpan = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.labelZero = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelStatusLabelTime = new System.Windows.Forms.Label();
            this.labelStatusLabelSerialNo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabelTimeDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelSerialNo = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelGGDesc = new System.Windows.Forms.ToolStripStatusLabel();
            this.RecordTimeShowBox = new System.Windows.Forms.ToolStripStatusLabel();
            this.ConfigStateLable = new System.Windows.Forms.ToolStripStatusLabel();
            this.RecordNumberShowBox = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.BtnPause = new System.Windows.Forms.ToolStripSplitButton();
            this.serialPort = new System.IO.Ports.SerialPort();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandExtBindingSource)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::MKSS.APP.HistoricRecordsReader.Properties.Resources.BackgroundImage;
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
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1490, 168);
            this.panel1.TabIndex = 0;
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExport.Location = new System.Drawing.Point(1364, 126);
            this.BtnExport.Margin = new System.Windows.Forms.Padding(4);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(112, 34);
            this.BtnExport.TabIndex = 10;
            this.BtnExport.Text = "导出(&E)";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInterval.Location = new System.Drawing.Point(936, 129);
            this.textBoxInterval.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(64, 28);
            this.textBoxInterval.TabIndex = 9;
            this.textBoxInterval.Text = "180ms";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(1221, 66);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(251, 54);
            this.label5.TabIndex = 7;
            this.label5.Text = "波特率：4800 偶校验 1位停止\r\n符合[GB15322.2-2019]中历史\r\n记录读取装置相关通信要求。";
            // 
            // BtnConnect
            // 
            this.BtnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConnect.Location = new System.Drawing.Point(1202, 126);
            this.BtnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(153, 34);
            this.BtnConnect.TabIndex = 6;
            this.BtnConnect.Text = "连接设备(&C)";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // CbxSerialPorts
            // 
            this.CbxSerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CbxSerialPorts.FormattingEnabled = true;
            this.CbxSerialPorts.Location = new System.Drawing.Point(1011, 129);
            this.CbxSerialPorts.Margin = new System.Windows.Forms.Padding(4);
            this.CbxSerialPorts.Name = "CbxSerialPorts";
            this.CbxSerialPorts.Size = new System.Drawing.Size(180, 26);
            this.CbxSerialPorts.TabIndex = 5;
            this.CbxSerialPorts.Click += new System.EventHandler(this.CbxSerialPorts_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(351, 116);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(343, 28);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sensor Historic Records Reader";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::MKSS.APP.HistoricRecordsReader.Properties.Resources.pictureBox1;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(40, 18);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(126, 126);
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
            this.label1.Location = new System.Drawing.Point(218, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(618, 59);
            this.label1.TabIndex = 2;
            this.label1.Text = "传感器历史记录读取器V3.2N";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(307, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1183, 449);
            this.panel2.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.DataSource = this.commandExtBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(751, 449);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button写时间);
            this.panel4.Controls.Add(this.button写串号);
            this.panel4.Controls.Add(this.button标Span点);
            this.panel4.Controls.Add(this.button标定零点);
            this.panel4.Controls.Add(this.labelStatusLabelGGDesc);
            this.panel4.Controls.Add(this.labelStatusLabelTimeDesc);
            this.panel4.Controls.Add(this.labelNongdu);
            this.panel4.Controls.Add(this.label13);
            this.panel4.Controls.Add(this.labelSpan);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.labelZero);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.labelStatusLabelTime);
            this.panel4.Controls.Add(this.labelStatusLabelSerialNo);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(751, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(432, 449);
            this.panel4.TabIndex = 1;
            // 
            // button写时间
            // 
            this.button写时间.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button写时间.Location = new System.Drawing.Point(284, 246);
            this.button写时间.Margin = new System.Windows.Forms.Padding(4);
            this.button写时间.Name = "button写时间";
            this.button写时间.Size = new System.Drawing.Size(135, 34);
            this.button写时间.TabIndex = 15;
            this.button写时间.Text = "写时间(&V)";
            this.button写时间.UseVisualStyleBackColor = true;
            this.button写时间.Visible = false;
            this.button写时间.Click += new System.EventHandler(this.button写时间_Click);
            // 
            // button写串号
            // 
            this.button写串号.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button写串号.Location = new System.Drawing.Point(284, 201);
            this.button写串号.Margin = new System.Windows.Forms.Padding(4);
            this.button写串号.Name = "button写串号";
            this.button写串号.Size = new System.Drawing.Size(135, 34);
            this.button写串号.TabIndex = 14;
            this.button写串号.Text = "写串号(&C)";
            this.button写串号.UseVisualStyleBackColor = true;
            this.button写串号.Visible = false;
            this.button写串号.Click += new System.EventHandler(this.button写串号_Click);
            // 
            // button标Span点
            // 
            this.button标Span点.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button标Span点.Location = new System.Drawing.Point(284, 93);
            this.button标Span点.Margin = new System.Windows.Forms.Padding(4);
            this.button标Span点.Name = "button标Span点";
            this.button标Span点.Size = new System.Drawing.Size(135, 34);
            this.button标Span点.TabIndex = 13;
            this.button标Span点.Text = "标Span点(&X)";
            this.button标Span点.UseVisualStyleBackColor = true;
            this.button标Span点.Visible = false;
            this.button标Span点.Click += new System.EventHandler(this.button标Span点_Click);
            // 
            // button标定零点
            // 
            this.button标定零点.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button标定零点.Location = new System.Drawing.Point(284, 39);
            this.button标定零点.Margin = new System.Windows.Forms.Padding(4);
            this.button标定零点.Name = "button标定零点";
            this.button标定零点.Size = new System.Drawing.Size(135, 34);
            this.button标定零点.TabIndex = 12;
            this.button标定零点.Text = "标定零点(&Z)";
            this.button标定零点.UseVisualStyleBackColor = true;
            this.button标定零点.Visible = false;
            this.button标定零点.Click += new System.EventHandler(this.button标定零点_Click);
            // 
            // labelStatusLabelGGDesc
            // 
            this.labelStatusLabelGGDesc.AutoSize = true;
            this.labelStatusLabelGGDesc.Location = new System.Drawing.Point(36, 312);
            this.labelStatusLabelGGDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatusLabelGGDesc.Name = "labelStatusLabelGGDesc";
            this.labelStatusLabelGGDesc.Size = new System.Drawing.Size(98, 18);
            this.labelStatusLabelGGDesc.TabIndex = 11;
            this.labelStatusLabelGGDesc.Text = "状态未知！";
            // 
            // labelStatusLabelTimeDesc
            // 
            this.labelStatusLabelTimeDesc.AutoSize = true;
            this.labelStatusLabelTimeDesc.Location = new System.Drawing.Point(34, 288);
            this.labelStatusLabelTimeDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatusLabelTimeDesc.Name = "labelStatusLabelTimeDesc";
            this.labelStatusLabelTimeDesc.Size = new System.Drawing.Size(116, 18);
            this.labelStatusLabelTimeDesc.TabIndex = 10;
            this.labelStatusLabelTimeDesc.Text = "未连接设备！";
            // 
            // labelNongdu
            // 
            this.labelNongdu.AutoSize = true;
            this.labelNongdu.Location = new System.Drawing.Point(36, 154);
            this.labelNongdu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNongdu.Name = "labelNongdu";
            this.labelNongdu.Size = new System.Drawing.Size(17, 18);
            this.labelNongdu.TabIndex = 9;
            this.labelNongdu.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(18, 128);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(103, 18);
            this.label13.TabIndex = 8;
            this.label13.Text = "实时浓度：";
            // 
            // labelSpan
            // 
            this.labelSpan.AutoSize = true;
            this.labelSpan.Location = new System.Drawing.Point(36, 100);
            this.labelSpan.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSpan.Name = "labelSpan";
            this.labelSpan.Size = new System.Drawing.Size(17, 18);
            this.labelSpan.TabIndex = 7;
            this.labelSpan.Text = "0";
            this.labelSpan.Click += new System.EventHandler(this.labelSpan_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(18, 74);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 18);
            this.label11.TabIndex = 6;
            this.label11.Text = "标定SPAN：";
            // 
            // labelZero
            // 
            this.labelZero.AutoSize = true;
            this.labelZero.Location = new System.Drawing.Point(36, 46);
            this.labelZero.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelZero.Name = "labelZero";
            this.labelZero.Size = new System.Drawing.Size(17, 18);
            this.labelZero.TabIndex = 5;
            this.labelZero.Text = "0";
            this.labelZero.Click += new System.EventHandler(this.labelZero_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(18, 20);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 18);
            this.label9.TabIndex = 4;
            this.label9.Text = "标定零点：";
            // 
            // labelStatusLabelTime
            // 
            this.labelStatusLabelTime.AutoSize = true;
            this.labelStatusLabelTime.Location = new System.Drawing.Point(34, 262);
            this.labelStatusLabelTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatusLabelTime.Name = "labelStatusLabelTime";
            this.labelStatusLabelTime.Size = new System.Drawing.Size(179, 18);
            this.labelStatusLabelTime.TabIndex = 3;
            this.labelStatusLabelTime.Text = "0000-00-00 00:00:00";
            this.labelStatusLabelTime.Click += new System.EventHandler(this.StatusLabelTime_Click);
            // 
            // labelStatusLabelSerialNo
            // 
            this.labelStatusLabelSerialNo.AutoSize = true;
            this.labelStatusLabelSerialNo.Location = new System.Drawing.Point(36, 208);
            this.labelStatusLabelSerialNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStatusLabelSerialNo.Name = "labelStatusLabelSerialNo";
            this.labelStatusLabelSerialNo.Size = new System.Drawing.Size(116, 18);
            this.labelStatusLabelSerialNo.TabIndex = 2;
            this.labelStatusLabelSerialNo.Text = "000000000000";
            this.labelStatusLabelSerialNo.Click += new System.EventHandler(this.StatusLabelSerialNo_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(18, 182);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 18);
            this.label4.TabIndex = 1;
            this.label4.Text = "设备串号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(18, 236);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "设备时间：";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "节点报警记录";
            treeNode1.Tag = "Warn";
            treeNode1.Text = "报警记录";
            treeNode2.Name = "节点报警恢复记录";
            treeNode2.Tag = "WarnRecover";
            treeNode2.Text = "报警恢复记录";
            treeNode3.Name = "节点故障记录";
            treeNode3.Tag = "Error";
            treeNode3.Text = "故障记录";
            treeNode4.Name = "节点故障恢复记录";
            treeNode4.Tag = "ErrorRecover";
            treeNode4.Text = "故障恢复记录";
            treeNode5.Name = "节点掉电记录";
            treeNode5.Tag = "PowerOff";
            treeNode5.Text = "掉电记录";
            treeNode6.Name = "节点上电记录";
            treeNode6.Tag = "PowerOffRecover";
            treeNode6.Text = "上电记录";
            treeNode7.Name = "节点失效记录";
            treeNode7.Tag = "Overdue";
            treeNode7.Text = "失效记录";
            treeNode8.Name = "节点所有记录";
            treeNode8.Tag = "None";
            treeNode8.Text = "所有记录";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode8});
            this.treeView1.Size = new System.Drawing.Size(307, 449);
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
            this.panel3.Location = new System.Drawing.Point(0, 168);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1490, 630);
            this.panel3.TabIndex = 2;
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxLogger.Location = new System.Drawing.Point(0, 449);
            this.textBoxLogger.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(1490, 150);
            this.textBoxLogger.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelTimeDesc,
            this.toolStripStatusLabel2,
            this.StatusLabelTime,
            this.toolStripStatusLabel1,
            this.StatusLabelSerialNo,
            this.StatusLabelGGDesc,
            this.RecordTimeShowBox,
            this.ConfigStateLable,
            this.RecordNumberShowBox,
            this.toolStripProgressBar1,
            this.BtnPause});
            this.statusStrip1.Location = new System.Drawing.Point(0, 599);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1490, 31);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabelTimeDesc
            // 
            this.StatusLabelTimeDesc.Name = "StatusLabelTimeDesc";
            this.StatusLabelTimeDesc.Size = new System.Drawing.Size(118, 24);
            this.StatusLabelTimeDesc.Text = "未连接设备！";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(100, 24);
            this.toolStripStatusLabel2.Text = "设备时间：";
            // 
            // StatusLabelTime
            // 
            this.StatusLabelTime.Name = "StatusLabelTime";
            this.StatusLabelTime.Size = new System.Drawing.Size(193, 24);
            this.StatusLabelTime.Text = "0000-00-00 00:00:00";
            this.StatusLabelTime.Click += new System.EventHandler(this.StatusLabelTime_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(53, 24);
            this.toolStripStatusLabel1.Text = "SN：";
            // 
            // StatusLabelSerialNo
            // 
            this.StatusLabelSerialNo.Name = "StatusLabelSerialNo";
            this.StatusLabelSerialNo.Size = new System.Drawing.Size(142, 24);
            this.StatusLabelSerialNo.Text = "000000000000";
            this.StatusLabelSerialNo.Click += new System.EventHandler(this.StatusLabelSerialNo_Click);
            // 
            // StatusLabelGGDesc
            // 
            this.StatusLabelGGDesc.Name = "StatusLabelGGDesc";
            this.StatusLabelGGDesc.Size = new System.Drawing.Size(100, 24);
            this.StatusLabelGGDesc.Text = "状态未知！";
            // 
            // RecordTimeShowBox
            // 
            this.RecordTimeShowBox.Name = "RecordTimeShowBox";
            this.RecordTimeShowBox.Size = new System.Drawing.Size(280, 24);
            this.RecordTimeShowBox.Text = "欢迎使用报警器历史记录读取器！";
            // 
            // ConfigStateLable
            // 
            this.ConfigStateLable.Name = "ConfigStateLable";
            this.ConfigStateLable.Size = new System.Drawing.Size(14, 24);
            this.ConfigStateLable.Text = ".";
            // 
            // RecordNumberShowBox
            // 
            this.RecordNumberShowBox.Name = "RecordNumberShowBox";
            this.RecordNumberShowBox.Size = new System.Drawing.Size(14, 24);
            this.RecordNumberShowBox.Text = ".";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 24);
            this.toolStripProgressBar1.Visible = false;
            // 
            // BtnPause
            // 
            this.BtnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnPause.Image = global::MKSS.APP.HistoricRecordsReader.Properties.Resources.pause;
            this.BtnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnPause.Name = "BtnPause";
            this.BtnPause.Size = new System.Drawing.Size(45, 28);
            this.BtnPause.Text = "toolStripSplitButton1";
            this.BtnPause.Visible = false;
            this.BtnPause.ButtonClick += new System.EventHandler(this.BtnPause_ButtonClick);
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 4800;
            this.serialPort.Parity = System.IO.Ports.Parity.Even;
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // FormSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1490, 798);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSensor";
            this.Text = "报警器历史记录读取器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSensor_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commandExtBindingSource)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label label5;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.BindingSource commandExtBindingSource;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelTime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel ConfigStateLable;
        private System.Windows.Forms.ToolStripStatusLabel RecordNumberShowBox;
        private System.Windows.Forms.ToolStripStatusLabel RecordTimeShowBox;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelTimeDesc;
        private System.Windows.Forms.ToolStripSplitButton BtnPause;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelGGDesc;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelSerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn par1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commandTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label labelNongdu;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelSpan;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelZero;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelStatusLabelTime;
        private System.Windows.Forms.Label labelStatusLabelSerialNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelStatusLabelTimeDesc;
        private System.Windows.Forms.Label labelStatusLabelGGDesc;
        private System.Windows.Forms.Button button写时间;
        private System.Windows.Forms.Button button写串号;
        private System.Windows.Forms.Button button标Span点;
        private System.Windows.Forms.Button button标定零点;
    }
}