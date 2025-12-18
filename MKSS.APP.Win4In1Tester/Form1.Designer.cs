
namespace MKSS.APP.Win4In1Tester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.uiConnection1 = new MKSS.APP.Win4In1Tester.UIConnection();
            this.uiConnection2 = new MKSS.APP.Win4In1Tester.UIConnection();
            this.uiConnection5 = new MKSS.APP.Win4In1Tester.UIConnection();
            this.uiConnection6 = new MKSS.APP.Win4In1Tester.UIConnection();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rV1 = new System.Windows.Forms.RadioButton();
            this.rV2 = new System.Windows.Forms.RadioButton();
            this.rV3 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.uiConnection1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.uiConnection2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.uiConnection5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.uiConnection6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1465, 590);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // uiConnection1
            // 
            this.uiConnection1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection1.Location = new System.Drawing.Point(3, 3);
            this.uiConnection1.Name = "uiConnection1";
            this.uiConnection1.Port = null;
            this.uiConnection1.Size = new System.Drawing.Size(726, 274);
            this.uiConnection1.TabIndex = 0;
            // 
            // uiConnection2
            // 
            this.uiConnection2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection2.Location = new System.Drawing.Point(735, 3);
            this.uiConnection2.Name = "uiConnection2";
            this.uiConnection2.Port = null;
            this.uiConnection2.Size = new System.Drawing.Size(727, 274);
            this.uiConnection2.TabIndex = 1;
            // 
            // uiConnection5
            // 
            this.uiConnection5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection5.Location = new System.Drawing.Point(3, 283);
            this.uiConnection5.Name = "uiConnection5";
            this.uiConnection5.Port = null;
            this.uiConnection5.Size = new System.Drawing.Size(726, 274);
            this.uiConnection5.TabIndex = 4;
            // 
            // uiConnection6
            // 
            this.uiConnection6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection6.Location = new System.Drawing.Point(735, 283);
            this.uiConnection6.Name = "uiConnection6";
            this.uiConnection6.Port = null;
            this.uiConnection6.Size = new System.Drawing.Size(727, 274);
            this.uiConnection6.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rV1);
            this.panel1.Controls.Add(this.rV2);
            this.panel1.Controls.Add(this.rV3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(735, 563);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(727, 24);
            this.panel1.TabIndex = 6;
            // 
            // rV1
            // 
            this.rV1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rV1.AutoSize = true;
            this.rV1.Checked = true;
            this.rV1.Location = new System.Drawing.Point(399, 5);
            this.rV1.Name = "rV1";
            this.rV1.Size = new System.Drawing.Size(83, 16);
            this.rV1.TabIndex = 2;
            this.rV1.TabStop = true;
            this.rV1.Text = "V1(基础版)";
            this.rV1.UseVisualStyleBackColor = true;
            // 
            // rV2
            // 
            this.rV2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rV2.AutoSize = true;
            this.rV2.Location = new System.Drawing.Point(495, 4);
            this.rV2.Name = "rV2";
            this.rV2.Size = new System.Drawing.Size(131, 16);
            this.rV2.TabIndex = 1;
            this.rV2.Text = "V2(增加PM1.0 PM10)";
            this.rV2.UseVisualStyleBackColor = true;
            // 
            // rV3
            // 
            this.rV3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rV3.AutoSize = true;
            this.rV3.Location = new System.Drawing.Point(632, 4);
            this.rV3.Name = "rV3";
            this.rV3.Size = new System.Drawing.Size(83, 16);
            this.rV3.TabIndex = 0;
            this.rV3.Text = "V3(modbus)";
            this.rV3.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1465, 590);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "美克盛世 多合一传感器测试工具V1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private UIConnection uiConnection1;
        private UIConnection uiConnection2;
        private UIConnection uiConnection5;
        private UIConnection uiConnection6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rV1;
        private System.Windows.Forms.RadioButton rV2;
        private System.Windows.Forms.RadioButton rV3;
    }
}

