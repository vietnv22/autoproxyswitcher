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

        public delegate void ProxyChangedEventHandler(string name, NetworkInfo networkInfo, ProxySettings proxySettings, string reason);

        public event ProxyChangedEventHandler ProxyChanged;

        /// <summary>
        /// Assigns or returns proxyNode/rules configurations
        /// </summary>
        public List<NetworkConfiguration> Configurations
        {
            get { return m_configurations; }
            set { m_configurations = value; }
        }

        /// <summary>
        /// Démarrer le monitoring de changement de réseaux
        /// </summary>
        public void StartMonitor()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            log.Info("Listening for address changes");
            SetProxyAccordingToNetwork();
        }

        /// <summary>
        /// Définir le configuration
        /// </summary>
        /// <param name="networkNode">Nom du réseau, mettre null pour autodétection</param>
        public void SetProxy(string name)
        {
            if (name == null)
            {
                SetProxyAccordingToNetwork();
            }
            else
            {
                NetworkConfiguration configuration = m_configurations.Find((c) => c.Name == name);
                configuration.ProxySettings.Configure();

                // Evénement
                if (ProxyChanged != null)
                {
                    ProxyChanged(name, null, configuration.ProxySettings, "Setting forced");
                }
            }
        }

        /// <summary>
        /// Loads configurations from file
        /// </summary>
        /// <returns></returns>
        public void LoadConfigurations(string fileName)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            List<NetworkConfiguration> configurations = new List<NetworkConfiguration>();

            log.Debug("Loading configurations");

            // Chargement des règles
            foreach (System.Xml.XmlNode networkNode in doc.SelectNodes("/Networks/Network"))
            {
                NetworkRulesSet networkRulesSet = new NetworkRulesSet();
                ProxySettings proxySettings = null;

                foreach (System.Xml.XmlNode ruleNode in networkNode.SelectNodes("Rules/*"))
                {
                    if (ruleNode.Name == "Subnet")
                    {
                        networkRulesSet.Add(new NetworkRuleSubnet(ruleNode.Attributes["ip"].Value));
                    }
                    else if (ruleNode.Name == "DNS")
                    {
                        networkRulesSet.Add(new NetworkRuleDNS(ruleNode.Attributes["ip"].Value));
                    }
                }

                // Chargement des paramètres du configuration
                System.Xml.XmlNode proxyNode = networkNode.SelectSingleNode("Proxy");
                
                switch (proxyNode.Attributes["type"].Value)
                {
                    case "PacFile":
                        proxySettings = new ProxyPACSettings(proxyNode.Attributes["url"].Value);
                        break;

                    case "Standard":
                        proxySettings = new StandardProxySettings(proxyNode.Attributes["url"].Value);
                        break;

                    case "None":
                        proxySettings = new NoProxySettings();
                        break;

                    default:
                        throw new Exception("Unknown ProxyType" + proxyNode.Attributes["type"].Value);
                }

                // Ajouter la configuration
                configurations.Add(new NetworkConfiguration(networkNode.Attributes["name"].Value, networkRulesSet, proxySettings));
            }

            log.DebugFormat("Configurations loaded: {0}", configurations.Count);

            m_configurations = configurations;
        }

        /// <summary>
        /// Méthode appelée lorsque l'adresse d'une interface change
        /// Cette méthode parcoure la liste des réseaux disponibles pour savoir s'il faut changer de configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressChangedCallback(object sender, EventArgs e)
        {
            log.Info("Adress changed");
            SetProxyAccordingToNetwork();
        }

        private void SetProxyAccordingToNetwork()
        {
            NetworkInfo matchingNetwork = null;
            NetworkConfiguration matchingConfiguration = null;
            RulesChecker.RulesCheckerResult result = new RulesChecker.RulesCheckerResult();
            int countMatches = 0;

            // Check each configuration
            foreach (NetworkConfiguration configuration in m_configurations)
            {
                log.InfoFormat("Checking config {0}", configuration.Name);

                // Check each network
                foreach (KeyValuePair<string, NetworkInfo> net in NetworkInfo.ListAvailableNetworks())
                {
                    log.InfoFormat("Checking network: {0} (IP: {1}, DNS: {2})", net.Key, string.Join(",", net.Value.NetworkIP), string.Join(",", net.Value.DNS));

                    RulesChecker rc = new RulesChecker(configuration.NetworkRulesSet);

                    RulesChecker.RulesCheckerResult tempResult = rc.CheckRulesAgainstNetwork(net.Value);
                    if (tempResult.Match)
                    {
                        log.InfoFormat("Config {0} match #{1}", configuration.Name, countMatches + 1);

                        if (countMatches == 0)
                        {
                            result = tempResult;
                            matchingConfiguration = configuration;
                            matchingNetwork = net.Value;
                        }

                        countMatches++;
                    }
                }
            }

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
