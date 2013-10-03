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
            this.comboProxy = new System.Windows.Forms.ComboBox();
            this.buttonSetProxy = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_status = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboProxy
            // 
            this.comboProxy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboProxy.FormattingEnabled = true;
            this.comboProxy.Location = new System.Drawing.Point(3, 261);
            this.comboProxy.Name = "comboProxy";
            this.comboProxy.Size = new System.Drawing.Size(284, 21);
            this.comboProxy.TabIndex = 10;
            // 
            // buttonSetProxy
            // 
            this.buttonSetProxy.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonSetProxy.AutoSize = true;
            this.buttonSetProxy.Location = new System.Drawing.Point(293, 261);
            this.buttonSetProxy.Name = "buttonSetProxy";
            this.buttonSetProxy.Size = new System.Drawing.Size(136, 30);
            this.buttonSetProxy.TabIndex = 11;
            this.buttonSetProxy.Text = "Choose this configuration";
            this.buttonSetProxy.UseVisualStyleBackColor = true;
            this.buttonSetProxy.Click += new System.EventHandler(this.buttonSetProxy_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.m_status, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSetProxy, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboProxy, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 294);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // m_status
            // 
            this.m_status.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.m_status, 2);
            this.m_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_status.Location = new System.Drawing.Point(3, 3);
            this.m_status.Name = "m_status";
            this.m_status.ReadOnly = true;
            this.m_status.Size = new System.Drawing.Size(426, 252);
            this.m_status.TabIndex = 13;
            this.m_status.Text = "";
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 294);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(448, 333);
            this.Name = "StatusForm";
            this.Text = "Status";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSetProxy;
        public System.Windows.Forms.ComboBox comboProxy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox m_status;
    }
}