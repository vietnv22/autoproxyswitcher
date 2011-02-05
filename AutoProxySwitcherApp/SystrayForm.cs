using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Diagnostics;
using System.Deployment.Application;
using System.Collections;
using System.Collections.Specialized;

namespace AutoProxySwitcher
{
    public partial class SystrayForm : Form
    {
        private StatusForm m_statusForm;

        public SystrayForm()
        {
            m_networkChangeDetector = new NetworkChangeDetector();
            InitializeComponent();

            m_statusForm = new StatusForm(m_networkChangeDetector);
            m_statusForm.FormClosing += new FormClosingEventHandler(m_statusForm_FormClosing);
            Hide();
        }

        void m_statusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                m_statusForm.Hide();
            }
        }

        #region TrayMenu
        private void toolStripMenuItemQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void m_networkChangeDetector_ProxyChanged(string name, NetworkInfo networkInfo, ProxySettings proxySettings, string reason)
        {
            // Afficher tooltip indiquant disponibilité
            if (networkInfo != null && proxySettings != null)
            {
                m_notifyIcon.BalloonTipText = String.Format("Réseau \"{0}\" disponible\nInterface: {1}, adresse: {2}\nRaison : {3}", name, networkInfo.Name, networkInfo.IP, reason);
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            }
            else if (networkInfo == null)
            {
                m_notifyIcon.BalloonTipText = String.Format("Configuration forcée du réseau \"{0}\"\nRaison : {1}", name, reason);
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            }
            else
            {
                m_notifyIcon.BalloonTipText = String.Format("Aucune règle trouvée, pas de changement");
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            }

            m_statusForm.Status = "Date de modification: " + System.DateTime.Now + "\n" + m_notifyIcon.BalloonTipText;
            m_notifyIcon.ShowBalloonTip(3000);
        }

        private void SystrayForm_Load(object sender, EventArgs e)
        {
            // Charger les règles
            m_networkChangeDetector.LoadRules(System.Configuration.ConfigurationSettings.AppSettings["RulesFile"]);
            m_networkChangeDetector.ProxyChanged += new NetworkChangeDetector.ProxyChangedEventHandler(m_networkChangeDetector_ProxyChanged);
            m_networkChangeDetector.StartMonitor();

            m_statusForm.comboProxy.Items.Add("auto");
            foreach (KeyValuePair<string, KeyValuePair<NetworkRulesSet, ProxySettings>> proxy in m_networkChangeDetector.Proxies)
            {
                m_statusForm.comboProxy.Items.Add(proxy.Key);
            }
            m_statusForm.comboProxy.SelectedIndex = 0;
        }

        private void m_notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_statusForm.Visible = !m_statusForm.Visible;
            }
        }

        private void statutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_statusForm.Visible = true;
        }
    }
}