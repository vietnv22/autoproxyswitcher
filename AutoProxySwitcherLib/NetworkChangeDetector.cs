using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Collections.Specialized;
using AutoProxySwitcherLib.Rules;
using log4net;

namespace AutoProxySwitcherLib
{
    public class NetworkChangeDetector
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkChangeDetector));

        private List<NetworkConfiguration> m_configurations;
        private IList<NetworkInfo> _currentNetworks;

        public delegate void ProxyChangedEventHandler(string configurationName, NetworkInfo networkInfo, ProxySettings proxySettings, string reason);

        public event ProxyChangedEventHandler ProxyChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public NetworkChangeDetector()
        {
            m_configurations = new List<NetworkConfiguration>();
        }

        /// <summary>
        /// Assigns or returns proxyNode/rules configurations
        /// </summary>
        public List<NetworkConfiguration> Configurations
        {
            get { return m_configurations; }
            set { m_configurations = value; }
        }

        /// <summary>
        /// Returns current available networks
        /// </summary>
        public IList<NetworkInfo> CurrentNetworks
        {
            get { return _currentNetworks;  }
        }

        /// <summary>
        /// Starts monitoring for changes
        /// </summary>
        public void StartMonitoring(string filename = null)
        {
            log.Info("StartMonitoring");
            if (filename != null)
            {
                LoadConfigurations(filename);
            }

            // Start listening for changes
            log.Info("Listening for address changes");
            try
            {
                NetworkChange.NetworkAddressChanged += AddressChangedCallback;
            }
            catch (Exception ex)
            {
                log.Error("Failed to listen for changes", ex);
            }

            SetConfiguration(null);
        }

        /// <summary>
        /// Loads configurations from file
        /// </summary>
        /// <returns></returns>
        public void LoadConfigurations(string fileName)
        {
            log.Info("Loading configurations from file " + fileName);

            try
            {
                m_configurations = NetworkConfigurations.FromFile(fileName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load configuration file", ex);
            }
        }

        /// <summary>
        /// Manually set configuration
        /// </summary>
        /// <param name="name">Configuration name, set null to autodetect</param>
        public void SetConfiguration(string name)
        {
            // Force first check (in a thread, since it can last some time
            log.Info("Starting setconfiguration thread for " + name);
            System.Threading.Thread thread = new System.Threading.Thread(() => { log.Info("setconfiguration thread for " + name); AddressChangedCallback(name, null); });
            thread.Start();
        }

        /// <summary>
        /// Returns the configuration that matches the given network
        /// </summary>
        /// <param name="configurations"></param>
        public NetworkConfiguration FindMatchingConfiguration(IList<NetworkInfo> networks, out RulesChecker.RulesCheckerResult result, out NetworkInfo matchingNetwork)
        {
            int countMatches = 0;
            NetworkConfiguration matchingConfiguration = null;
            result = null;
            matchingNetwork = null;

            // Check each configuration
            foreach (NetworkConfiguration configuration in m_configurations)
            {
                using (log4net.ThreadContext.Stacks["NDC"].Push(String.Format("config {0}", configuration.Name)))
                {
                    log.InfoFormat("Checking config {0}", configuration.Name); 

                    // Check each network
                    foreach (NetworkInfo net in networks)
                    {
                        log.InfoFormat("Checking network: {0} (IP: {1}, DNS: {2})", net.IfName, string.Join(",", net.NetworkIP), string.Join(",", net.DNS));

                        RulesChecker rc = new RulesChecker(configuration.NetworkRulesSet);

                        RulesChecker.RulesCheckerResult tempResult = rc.CheckRulesAgainstNetwork(net);
                        if (tempResult.Match)
                        {
                            log.InfoFormat("Match #{1}: config {0}", configuration.Name, countMatches + 1);

                            if (countMatches == 0)
                            {
                                result = tempResult;
                                matchingConfiguration = configuration;
                                matchingNetwork = net;
                            }

                            countMatches++;
                        }
                    }
                }
            }

            return matchingConfiguration;
        }

        /// <summary>
        /// Méthode appelée lorsque l'adresse d'une interface change
        /// Cette méthode parcoure la liste des réseaux disponibles pour savoir s'il faut changer de configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressChangedCallback(object sender, EventArgs e)
        {
            try
            {
                log.Info("Address changed");
                SetProxyAccordingToNetwork(sender is string ? (string)sender : null);
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
            }
        }

        /// <summary>
        /// Sets proxy according to current available networks and configurations rules
        /// </summary>
        private void SetProxyAccordingToNetwork(string configName)
        {
            if (configName != null)
            {
                log.Info("Set configuration " + configName);

                NetworkConfiguration configuration = m_configurations.Find((c) => c.Name == configName);
                configuration.ProxySettings.Configure();

                // Evénement
                if (ProxyChanged != null)
                {
                    ProxyChanged(configName, null, configuration.ProxySettings, "Setting forced");
                }
            }
            else
            {
                NetworkInfo matchingNetwork = null;
                NetworkConfiguration matchingConfiguration = null;
                RulesChecker.RulesCheckerResult result = new RulesChecker.RulesCheckerResult();

                // Get current networks
                IList<NetworkInfo> currentNetworks = NetworkManager.ListAvailableNetworks();
                _currentNetworks = currentNetworks;
                foreach (var networkInfo in currentNetworks)
                {
                    log.Debug("Network " + networkInfo);
                }

                // Find matching configuration
                matchingConfiguration = FindMatchingConfiguration(currentNetworks, out result, out matchingNetwork);

                // Configurer le configuration en conséquence
                if (matchingConfiguration != null)
                {
                    matchingConfiguration.ProxySettings.Configure();
                }

                // Evénement
                if (ProxyChanged != null)
                {
                    ProxyChanged(matchingConfiguration.Name, result.Reason != Reasons.DEFAULT ? matchingNetwork : null, matchingConfiguration.ProxySettings, result.ReasonString);
                }
            }
        }
    }
}
