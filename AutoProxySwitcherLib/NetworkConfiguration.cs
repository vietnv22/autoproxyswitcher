using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoProxySwitcherLib
{
    /// <summary>
    /// Handles a configuration: a set of network rules and a proxyNode setting
    /// </summary>
    public class NetworkConfiguration
    {
        private string name;
        private ProxySettings proxySettings;
        private NetworkRulesSet networkRulesSet;

        public NetworkConfiguration(string name, NetworkRulesSet networkRulesSet, ProxySettings proxySettings)
        {
            this.name = name;
            this.networkRulesSet = networkRulesSet;
            this.proxySettings = proxySettings;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public NetworkRulesSet NetworkRulesSet
        {
            get { return networkRulesSet; }
            set { networkRulesSet = value; }
        }

        public ProxySettings ProxySettings
        {
            get { return proxySettings; }
            set { proxySettings = value; }
        }
    }
}
