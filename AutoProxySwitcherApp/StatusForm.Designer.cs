namespace AutoProxySwitcher
{
    partial class StatusForm
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
            this.m_statusStrip = new System.Windows.Forms.StatusStrip();
            this.m_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_labelStatus = new System.Windows.Forms.Label();
            this.comboProxy = new System.Windows.Forms.ComboBox();
            this.buttonSetProxy = new System.Windows.Forms.Button();
            this.m_statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_toolStripStatusLabel});
            this.m_statusStrip.Location = new System.Drawing.Point(0, 194);
            this.m_statusStrip.Name = "m_statusStrip";
            this.m_statusStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.m_statusStrip.Size = new System.Drawing.Size(307, 22);
            this.m_statusStrip.SizingGrip = false;
            this.m_statusStrip.TabIndex = 7;
            // 
            // m_toolStripStatusLabel
            // 
            this.m_toolStripStatusLabel.Name = "m_toolStripStatusLabel";
            this.m_toolStripStatusLabel.Size = new System.Drawing.Size(16, 17);
            this.m_toolStripStatusLabel.Text = "...";
            // 
            // m_labelStatus
            // 
            this.m_labelStatus.Location = new System.Drawing.Point(4, 9);
            this.m_labelStatus.Name = "m_labelStatus";
            this.m_labelStatus.Size = new System.Drawing.Size(297, 108);
            this.m_labelStatus.TabIndex = 9;
            // 
            // comboProxy
            // 
            this.comboProxy.FormattingEnabled = true;
            this.comboProxy.Location = new System.Drawing.Point(7, 160);
            this.comboProxy.Name = "comboProxy";
            this.comboProxy.Size = new System.Drawing.Size(150, 21);
            this.comboProxy.TabIndex = 10;
            // 
            // buttonSetProxy
            // 
            this.buttonSetProxy.Location = new System.Drawing.Point(185, 154);
            this.buttonSetProxy.Name = "buttonSetProxy";
            this.buttonSetProxy.Size = new System.Drawing.Size(110, 30);
            this.buttonSetProxy.TabIndex = 11;
            this.buttonSetProxy.Text = "Set Proxy";
            this.buttonSetProxy.UseVisualStyleBackColor = true;
            this.buttonSetProxy.Click += new System.EventHandler(this.buttonSetProxy_Click);
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 216);
            this.Controls.Add(this.buttonSetProxy);
            this.Controls.Add(this.comboProxy);
            this.Controls.Add(this.m_statusStrip);
            this.Controls.Add(this.m_labelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StatusForm";
            this.Text = "Status";
            this.Load += new System.EventHandler(this.StatusForm_Load);
            this.m_statusStrip.ResumeLayout(false);
            this.m_statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel m_toolStripStatusLabel;
        private System.Windows.Forms.Label m_labelStatus;
        private System.Windows.Forms.Button buttonSetProxy;
        public System.Windows.Forms.ComboBox comboProxy;
    }
}