
namespace EggCounter
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.最先时间 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LabelCount1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Xh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CaseNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnDisConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnConnect = new System.Windows.Forms.Button();
            this.TxtSerialPorts = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this._serial = new System.IO.Ports.SerialPort(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.LabelCount5 = new System.Windows.Forms.Label();
            this.LabelCount3 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.dataGridView2);
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 99);
            this.panel2.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1383, 762);
            this.panel2.TabIndex = 1;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.最先时间});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(458, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(925, 552);
            this.dataGridView2.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Xh";
            this.dataGridViewTextBoxColumn2.HeaderText = "笼编号";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 97;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Count";
            this.dataGridViewTextBoxColumn3.HeaderText = "累计产蛋";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 117;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "TimeTo";
            this.dataGridViewTextBoxColumn4.HeaderText = "最后时间";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 117;
            // 
            // 最先时间
            // 
            this.最先时间.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.最先时间.DataPropertyName = "TimeFrom";
            this.最先时间.HeaderText = "最先时间";
            this.最先时间.Name = "最先时间";
            this.最先时间.Width = 117;
            // 
            // LabelCount1
            // 
            this.LabelCount1.AutoSize = true;
            this.LabelCount1.Font = new System.Drawing.Font("微软雅黑", 50F);
            this.LabelCount1.Location = new System.Drawing.Point(3, 7);
            this.LabelCount1.Name = "LabelCount1";
            this.LabelCount1.Size = new System.Drawing.Size(155, 88);
            this.LabelCount1.TabIndex = 1;
            this.LabelCount1.Text = "100";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Xh,
            this.Count,
            this.Dt,
            this.CaseNo});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(458, 552);
            this.dataGridView1.TabIndex = 2;
            // 
            // Xh
            // 
            this.Xh.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Xh.DataPropertyName = "Xh";
            this.Xh.HeaderText = "序号";
            this.Xh.Name = "Xh";
            this.Xh.Width = 77;
            // 
            // Count
            // 
            this.Count.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Count.DataPropertyName = "Count";
            this.Count.HeaderText = "蛋个数";
            this.Count.Name = "Count";
            this.Count.Width = 97;
            // 
            // Dt
            // 
            this.Dt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Dt.DataPropertyName = "Dt";
            this.Dt.HeaderText = "时间";
            this.Dt.Name = "Dt";
            this.Dt.Width = 77;
            // 
            // CaseNo
            // 
            this.CaseNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CaseNo.DataPropertyName = "CaseNo";
            this.CaseNo.HeaderText = "笼号";
            this.CaseNo.Name = "CaseNo";
            this.CaseNo.Width = 77;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox1.Location = new System.Drawing.Point(0, 552);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1383, 210);
            this.textBox1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnDisConnect);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.BtnConnect);
            this.panel1.Controls.Add(this.TxtSerialPorts);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1383, 99);
            this.panel1.TabIndex = 0;
            // 
            // BtnDisConnect
            // 
            this.BtnDisConnect.Location = new System.Drawing.Point(580, 26);
            this.BtnDisConnect.Name = "BtnDisConnect";
            this.BtnDisConnect.Size = new System.Drawing.Size(150, 52);
            this.BtnDisConnect.TabIndex = 3;
            this.BtnDisConnect.Text = "停止(&D)";
            this.BtnDisConnect.UseVisualStyleBackColor = true;
            this.BtnDisConnect.Click += new System.EventHandler(this.BtnDisConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 27);
            this.label1.TabIndex = 2;
            this.label1.Text = "通信端口";
            // 
            // BtnConnect
            // 
            this.BtnConnect.Location = new System.Drawing.Point(408, 26);
            this.BtnConnect.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.BtnConnect.Name = "BtnConnect";
            this.BtnConnect.Size = new System.Drawing.Size(150, 52);
            this.BtnConnect.TabIndex = 1;
            this.BtnConnect.Text = "开始(&C)";
            this.BtnConnect.UseVisualStyleBackColor = true;
            this.BtnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // TxtSerialPorts
            // 
            this.TxtSerialPorts.FormattingEnabled = true;
            this.TxtSerialPorts.Location = new System.Drawing.Point(144, 36);
            this.TxtSerialPorts.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.TxtSerialPorts.Name = "TxtSerialPorts";
            this.TxtSerialPorts.Size = new System.Drawing.Size(238, 35);
            this.TxtSerialPorts.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // _serial
            // 
            this._serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.LabelCount3);
            this.panel3.Controls.Add(this.LabelCount5);
            this.panel3.Controls.Add(this.LabelCount1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1018, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(365, 552);
            this.panel3.TabIndex = 4;
            // 
            // LabelCount5
            // 
            this.LabelCount5.AutoSize = true;
            this.LabelCount5.Font = new System.Drawing.Font("微软雅黑", 50F);
            this.LabelCount5.Location = new System.Drawing.Point(3, 300);
            this.LabelCount5.Name = "LabelCount5";
            this.LabelCount5.Size = new System.Drawing.Size(155, 88);
            this.LabelCount5.TabIndex = 2;
            this.LabelCount5.Text = "100";
            // 
            // LabelCount3
            // 
            this.LabelCount3.AutoSize = true;
            this.LabelCount3.Font = new System.Drawing.Font("微软雅黑", 50F);
            this.LabelCount3.Location = new System.Drawing.Point(3, 157);
            this.LabelCount3.Name = "LabelCount3";
            this.LabelCount3.Size = new System.Drawing.Size(155, 88);
            this.LabelCount3.TabIndex = 3;
            this.LabelCount3.Text = "100";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1383, 861);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "FormMain";
            this.Text = "鸡蛋计数";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnConnect;
        private System.Windows.Forms.ComboBox TxtSerialPorts;
        private System.Windows.Forms.Timer timer1;
        private System.IO.Ports.SerialPort _serial;
        private System.Windows.Forms.Label LabelCount1;
        private System.Windows.Forms.Button BtnDisConnect;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Xh;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn Dt;
        private System.Windows.Forms.DataGridViewTextBoxColumn CaseNo;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn 最先时间;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label LabelCount5;
        private System.Windows.Forms.Label LabelCount3;
    }
}

