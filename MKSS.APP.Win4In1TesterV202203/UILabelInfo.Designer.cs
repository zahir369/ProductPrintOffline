
namespace MKSS.APP.WinNBTester
{
    partial class UILabelInfo
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
            this.labelName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.labelUnit = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelName.Font = new System.Drawing.Font("宋体", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelName.Location = new System.Drawing.Point(0, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(97, 38);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "----";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("宋体", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(97, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 38);
            this.label2.TabIndex = 1;
            this.label2.Text = ":";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelValue.Font = new System.Drawing.Font("宋体", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelValue.Location = new System.Drawing.Point(133, 0);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(137, 38);
            this.labelValue.TabIndex = 2;
            this.labelValue.Text = "------";
            // 
            // labelUnit
            // 
            this.labelUnit.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelUnit.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelUnit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelUnit.Location = new System.Drawing.Point(270, 0);
            this.labelUnit.Name = "labelUnit";
            this.labelUnit.Size = new System.Drawing.Size(96, 44);
            this.labelUnit.TabIndex = 3;
            this.labelUnit.Text = "--";
            this.labelUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UILabelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelUnit);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelName);
            this.Name = "UILabelInfo";
            this.Size = new System.Drawing.Size(344, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label labelUnit;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
