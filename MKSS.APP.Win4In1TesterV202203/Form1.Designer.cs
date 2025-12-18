
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
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.uiConnection1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.uiConnection2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1465, 590);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // uiConnection1
            // 
            this.uiConnection1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection1.Location = new System.Drawing.Point(3, 3);
            this.uiConnection1.Name = "uiConnection1";
            this.uiConnection1.Size = new System.Drawing.Size(1459, 289);
            this.uiConnection1.Started = false;
            this.uiConnection1.TabIndex = 0;
            // 
            // uiConnection2
            // 
            this.uiConnection2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiConnection2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiConnection2.Location = new System.Drawing.Point(3, 298);
            this.uiConnection2.Name = "uiConnection2";
            this.uiConnection2.Size = new System.Drawing.Size(1459, 289);
            this.uiConnection2.Started = false;
            this.uiConnection2.TabIndex = 1;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1465, 590);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "美克盛世 二合一传感器测试工具V1.1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private UIConnection uiConnection1;
        private UIConnection uiConnection2;
    }
}

