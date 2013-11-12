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
using AutoProxySwitcherLib;
using log4net;

namespace AutoProxySwitcher
{
    public partial class SystrayForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SystrayForm));

        private AutoProxySwitcherLib.NetworkChangeDetector m_networkChangeDetector;
        private StatusForm m_statusForm;

        public SystrayForm()
        {
            m_networkChangeDetector = new NetworkChangeDetector();
            InitializeComponent();


            // Start monitoring
            m_networkChangeDetector.ProxyChanged += new NetworkChangeDetector.ProxyChangedEventHandler(m_networkChangeDetector_ProxyChanged);
            m_networkChangeDetector.StartMonitoring();
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

        private void UIA(Action action) { this.Invoke(action); }

        private void m_networkChangeDetector_ProxyChanged(string name, NetworkInfo networkInfo, ProxySettings proxySettings, string reason)
        {
            string info;
            string detailedInfo;

            log.Info("Proxy Changed");

            // Afficher tooltip indiquant disponibilité
            if (networkInfo != null && proxySettings != null)
            {
                info = String.Format("Configuration \"{0}\" available\nInterface name: {1}\nReason: {2}", name, networkInfo.IfName, reason);
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            }
            else if (networkInfo == null)
            {
                info = String.Format("Configuration \"{0}\" available\nReason: {1}", name, reason);
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            }
            else
            {
                info = String.Format("No rule found, no change");
                m_notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            }

            m_notifyIcon.BalloonTipText = info;

            detailedInfo = "Update date: " + System.DateTime.Now;
            detailedInfo += "\n\n" + info;

            if (networkInfo != null)
            {
                detailedInfo += "\n\nNetwork details: ";
                detailedInfo += "\n  Interface name: " + networkInfo.IfName;
                detailedInfo += "\n  Type: " + networkInfo.Type;
                detailedInfo += "\n  Description: " + networkInfo.Description;
                detailedInfo += "\n  IP(s): " + String.Join(", ", networkInfo.IP);
                detailedInfo += "\n  Network(s): " + String.Join(", ", networkInfo.NetworkIP);
                detailedInfo += "\n  DNS: " + String.Join(", ", networkInfo.DNS);
            }

            if (proxySettings != null)
            {
                detailedInfo += "\n\nProxy details: ";
                detailedInfo += "\n  " + proxySettings;
            }

            if (InvokeRequired)
            {
                UIA(() => m_statusForm.Status = detailedInfo);
            }
            else
            {
                m_statusForm.Status = detailedInfo;
            }

            m_notifyIcon.ShowBalloonTip(3000);
        }

        private void SystrayForm_Load(object sender, EventArgs e)
        {
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) + "\\AutoProxySwitcher\\";
            System.IO.Directory.CreateDirectory(folder);
            string rulesFile = folder
                + Environment.ExpandEnvironmentVariables(System.Configuration.ConfigurationManager.AppSettings["RulesFile"]);

            log.Info("rules file expected location is " + rulesFile);

            // Create default rules files if it doesn't exists
            if (!System.IO.File.Exists(rulesFile))
            {
                log.Info("rules file doesn't exists, creating one from example");

                try
                {
                    System.IO.File.Copy("Examples/rules.xml", rulesFile);
                }
                catch (Exception ex)
                {
                    log.Error("Failed to create rules file", ex);
                    throw;
                }
            }

            // Build status form
            m_statusForm = new StatusForm(m_networkChangeDetector);
            m_statusForm.FormClosing += new FormClosingEventHandler(m_statusForm_FormClosing);

            // Charger les règles
            try
            {
                m_networkChangeDetector.LoadConfigurations(rulesFile);
            }
            catch (Exception ex)
            {
                m_statusForm.Status = ex.Message + ":\n\n" + ex.InnerException.Message;
                m_notifyIcon.BalloonTipText = m_statusForm.Status;
                m_notifyIcon.ShowBalloonTip(3000);
            }

            // Build configuration list
            m_statusForm.comboProxy.Items.Add("auto");
            foreach (NetworkConfiguration configuration in m_networkChangeDetector.Configurations)
            {
                m_statusForm.comboProxy.Items.Add(configuration.Name);
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