using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Configuration;
using AutoProxySwitcherLib;

namespace AutoProxySwitcher
{
    public partial class StatusForm : Form
    {
        private NetworkChangeDetector m_networkChangeDetector;

        public string Status
        {
            get { return m_status.Text; }
            set { m_status.Text = value; }
        }

        public StatusForm(NetworkChangeDetector networkChangeDetector)
        {
            InitializeComponent();
            m_networkChangeDetector = networkChangeDetector;
        }

        private void m_buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSetProxy_Click(object sender, EventArgs e)
        {
            if (comboProxy.Text == "auto")
            {
                m_networkChangeDetector.SetConfiguration(null);
            }
            else
            {
                m_networkChangeDetector.SetConfiguration(comboProxy.Text);
            }
        }
    }
}
