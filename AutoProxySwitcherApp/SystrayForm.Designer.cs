namespace AutoProxySwitcher
{
    partial class SystrayForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystrayForm));
            this.m_notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.m_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_toolStripMenuItemQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_notifyIcon
            // 
            this.m_notifyIcon.BalloonTipTitle = "Auto Proxy Switcher";
            this.m_notifyIcon.ContextMenuStrip = this.m_contextMenuStrip;
            this.m_notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("m_notifyIcon.Icon")));
            this.m_notifyIcon.Text = "Auto Proxy Switcher";
            this.m_notifyIcon.Visible = true;
            this.m_notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.m_notifyIcon_MouseClick);
            // 
            // m_contextMenuStrip
            // 
            this.m_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statutToolStripMenuItem,
            this.toolStripSeparator1,
            this.m_toolStripMenuItemQuit});
            this.m_contextMenuStrip.Name = "m_contextMenuStrip";
            this.m_contextMenuStrip.Size = new System.Drawing.Size(112, 54);
            // 
            // statutToolStripMenuItem
            // 
            this.statutToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.statutToolStripMenuItem.Name = "statutToolStripMenuItem";
            this.statutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.statutToolStripMenuItem.Text = "Status";
            this.statutToolStripMenuItem.Click += new System.EventHandler(this.statutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(108, 6);
            // 
            // m_toolStripMenuItemQuit
            // 
            this.m_toolStripMenuItemQuit.Name = "m_toolStripMenuItemQuit";
            this.m_toolStripMenuItemQuit.Size = new System.Drawing.Size(111, 22);
            this.m_toolStripMenuItemQuit.Text = "Exit";
            this.m_toolStripMenuItemQuit.Click += new System.EventHandler(this.toolStripMenuItemQuit_Click);
            // 
            // SystrayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(168, 148);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SystrayForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "Auto Proxy Switcher";
            this.Load += new System.EventHandler(this.SystrayForm_Load);
            this.m_contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenuStrip m_contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem m_toolStripMenuItemQuit;
        private System.Windows.Forms.ToolStripMenuItem statutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}

