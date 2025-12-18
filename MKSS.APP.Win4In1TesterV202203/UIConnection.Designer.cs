
namespace MKSS.APP.Win4In1Tester
{
    partial class UIConnection
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnExport = new System.Windows.Forms.Button();
            this.labelV = new System.Windows.Forms.Label();
            this.COM = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.panel3 = new System.Windows.Forms.Panel();
            this.MinusYMax = new System.Windows.Forms.Button();
            this.MinusYMin = new System.Windows.Forms.Button();
            this.AddYMin = new System.Windows.Forms.Button();
            this.AddYMax = new System.Windows.Forms.Button();
            this.IC2 = new MKSS.APP.WinNBTester.UILabelInfo();
            this.IC5 = new MKSS.APP.WinNBTester.UILabelInfo();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnStart);
            this.panel1.Controls.Add(this.BtnExport);
            this.panel1.Controls.Add(this.labelV);
            this.panel1.Controls.Add(this.COM);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(723, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(447, 48);
            this.panel1.TabIndex = 1;
            // 
            // BtnStart
            // 
            this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStart.Location = new System.Drawing.Point(285, 16);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 23);
            this.BtnStart.TabIndex = 4;
            this.BtnStart.Text = "开始/停止";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExport.Location = new System.Drawing.Point(366, 16);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(75, 23);
            this.BtnExport.TabIndex = 3;
            this.BtnExport.Text = "导出";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // labelV
            // 
            this.labelV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelV.AutoSize = true;
            this.labelV.Location = new System.Drawing.Point(389, 0);
            this.labelV.Name = "labelV";
            this.labelV.Size = new System.Drawing.Size(11, 12);
            this.labelV.TabIndex = 2;
            this.labelV.Text = "N";
            this.labelV.Visible = false;
            // 
            // COM
            // 
            this.COM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.COM.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.COM.FormattingEnabled = true;
            this.COM.Location = new System.Drawing.Point(196, 18);
            this.COM.Name = "COM";
            this.COM.Size = new System.Drawing.Size(83, 20);
            this.COM.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1173, 74);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.IC2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.IC5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1173, 74);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1173, 353);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.MinusYMax);
            this.panel3.Controls.Add(this.MinusYMin);
            this.panel3.Controls.Add(this.AddYMin);
            this.panel3.Controls.Add(this.AddYMax);
            this.panel3.Controls.Add(this.elementHost1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 74);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1173, 353);
            this.panel3.TabIndex = 6;
            this.panel3.SizeChanged += new System.EventHandler(this.panel3_SizeChanged);
            // 
            // MinusYMax
            // 
            this.MinusYMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinusYMax.Location = new System.Drawing.Point(1146, 39);
            this.MinusYMax.Name = "MinusYMax";
            this.MinusYMax.Size = new System.Drawing.Size(18, 20);
            this.MinusYMax.TabIndex = 4;
            this.MinusYMax.Text = "-";
            this.MinusYMax.UseVisualStyleBackColor = true;
            this.MinusYMax.Click += new System.EventHandler(this.MinusYMax_Click);
            // 
            // MinusYMin
            // 
            this.MinusYMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MinusYMin.Location = new System.Drawing.Point(1146, 294);
            this.MinusYMin.Name = "MinusYMin";
            this.MinusYMin.Size = new System.Drawing.Size(18, 20);
            this.MinusYMin.TabIndex = 3;
            this.MinusYMin.Text = "-";
            this.MinusYMin.UseVisualStyleBackColor = true;
            this.MinusYMin.Click += new System.EventHandler(this.MinusYMin_Click);
            // 
            // AddYMin
            // 
            this.AddYMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddYMin.Location = new System.Drawing.Point(1146, 274);
            this.AddYMin.Name = "AddYMin";
            this.AddYMin.Size = new System.Drawing.Size(18, 20);
            this.AddYMin.TabIndex = 2;
            this.AddYMin.Text = "+";
            this.AddYMin.UseVisualStyleBackColor = true;
            this.AddYMin.Click += new System.EventHandler(this.AddYMin_Click);
            // 
            // AddYMax
            // 
            this.AddYMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddYMax.Location = new System.Drawing.Point(1146, 19);
            this.AddYMax.Name = "AddYMax";
            this.AddYMax.Size = new System.Drawing.Size(18, 20);
            this.AddYMax.TabIndex = 1;
            this.AddYMax.Text = "+";
            this.AddYMax.UseVisualStyleBackColor = true;
            this.AddYMax.Click += new System.EventHandler(this.AddYMax_Click);
            // 
            // IC2
            // 
            this.IC2.AutoSize = true;
            this.IC2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IC2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IC2.LabelColor = System.Drawing.SystemColors.ControlText;
            this.IC2.Location = new System.Drawing.Point(374, 24);
            this.IC2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.IC2.Name = "IC2";
            this.IC2.Size = new System.Drawing.Size(342, 46);
            this.IC2.TabIndex = 1;
            // 
            // IC5
            // 
            this.IC5.AutoSize = true;
            this.IC5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IC5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IC5.LabelColor = System.Drawing.SystemColors.ControlText;
            this.IC5.Location = new System.Drawing.Point(24, 24);
            this.IC5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.IC5.Name = "IC5";
            this.IC5.Size = new System.Drawing.Size(342, 46);
            this.IC5.TabIndex = 4;
            // 
            // UIConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "UIConnection";
            this.Size = new System.Drawing.Size(1173, 427);
            this.Load += new System.EventHandler(this.UIConnection_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private WinNBTester.UILabelInfo IC2;
        private WinNBTester.UILabelInfo IC5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelV;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.ComboBox COM;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button AddYMax;
        private System.Windows.Forms.Button MinusYMax;
        private System.Windows.Forms.Button MinusYMin;
        private System.Windows.Forms.Button AddYMin;
    }
}
