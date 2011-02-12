using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Collections.Specialized;
using AutoProxySwitcher.Rules;

namespace AutoProxySwitcher
{
    public class NetworkChangeDetector
    {
        Dictionary<string, KeyValuePair<NetworkRulesSet, ProxySettings>> m_proxies;

        public delegate void ProxyChangedEventHandler(string name, NetworkInfo networkInfo, ProxySettings proxySettings, string reason);

        public event ProxyChangedEventHandler ProxyChanged;

        /// <summary>
        /// Assigne ou retourne les règles
        /// </summary>
        public Dictionary<string, KeyValuePair<NetworkRulesSet, ProxySettings>> Proxies
        {
            get { return m_proxies; }
            set { m_proxies = value; }
        }

        /// <summary>
        /// Démarrer le monitoring de changement de réseaux
        /// </summary>
        public void StartMonitor()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            Debug.WriteLine("Listening for address changes. Press any key to exit.");
            SetProxyAccordingToNetwork();
        }

        /// <summary>
        /// Définir le proxy
        /// </summary>
        /// <param name="network">Nom du réseau, mettre null pour autodétection</param>
        public void SetProxy(string name)
        {
            if (name == null)
            {
                SetProxyAccordingToNetwork();
            }
            else
            {
                m_proxies[name].Value.Configure();

                // Evénement
                if (ProxyChanged != null)
                {
                    ProxyChanged(name, null, m_proxies[name].Value, "Setting forced");
                }
            }
        }

        /// <summary>
        /// Chargement des règles dans le fichier de configuration
        /// </summary>
        /// <returns></returns>
        public void LoadRules(string fileName)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            Dictionary<string, KeyValuePair<NetworkRulesSet, ProxySettings>> proxies = new Dictionary<string, KeyValuePair<NetworkRulesSet, ProxySettings>>();

            // Chargement des règles
            foreach (System.Xml.XmlNode network in doc.SelectNodes("/Networks/Network"))
            {
                NetworkRulesSet networkRulesSet = new NetworkRulesSet();
                ProxySettings proxySettings = null;

                foreach (System.Xml.XmlNode rule in network.SelectNodes("Rules/*"))
                {
                    if (rule.Name == "Subnet")
                    {
                        networkRulesSet.Add(new NetworkRuleSubnet(rule.Attributes["ip"].Value));
                    }
                    else if (rule.Name == "DNS")
                    {
                        networkRulesSet.Add(new NetworkRuleDNS(rule.Attributes["ip"].Value));
                    }
                }

                // Chargement des paramètres du proxy
                System.Xml.XmlNode proxy = network.SelectSingleNode("Proxy");
                
                switch (proxy.Attributes["type"].Value)
                {
                    case "PacFile":
                        proxySettings = new ProxyPACSettings(proxy.Attributes["url"].Value);
                        break;

                    case "Standard":
                        proxySettings = new StandardProxySettings(proxy.Attributes["url"].Value);
                        break;

                    case "None":
                        proxySettings = new NoProxySettings();
                        break;

                    default:
                        throw new Exception("Unknown ProxyType" + proxy.Attributes["type"].Value);
                }

                // Ajouter la configuration
                proxies.Add(network.Attributes["name"].Value, new KeyValuePair<NetworkRulesSet, ProxySettings>(networkRulesSet, proxySettings));
            }

            m_proxies = proxies;
        }

        /// <summary>
        /// Méthode appelée lorsque l'adresse d'une interface change
        /// Cette méthode parcoure la liste des réseaux disponibles pour savoir s'il faut changer de proxy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressChangedCallback(object sender, EventArgs e)
        {
            Trace.TraceInformation("Changement d'adresses");
            SetProxyAccordingToNetwork();
        }

        private void SetProxyAccordingToNetwork()
        {
            string name = "";
            NetworkInfo availableNetwork = null;
            NetworkRulesSet networkRulesSet = null;
            ProxySettings proxySetting = null;
            RulesChecker.RulesCheckerResult result = new RulesChecker.RulesCheckerResult();

            // Checker chaque réseau par rapport à la liste des règles
            foreach (KeyValuePair<string, NetworkInfo> net in NetworkInfo.ListAvailableNetworks())
            {
                Debug.WriteLine("Réseau: " + net.Key + " (IP:" + net.Value.IP + ")" + " (DNS:" + net.Value.DNS + ")");

                foreach (KeyValuePair<string, KeyValuePair<NetworkRulesSet, ProxySettings>> proxy in m_proxies)
                {
                    RulesChecker rc = new RulesChecker(proxy.Value.Key);

                    result = rc.CheckRulesAgainstNetwork(net.Value);
                    if (result.Match)
                    {
                        name = proxy.Key;
                        availableNetwork = net.Value;
                        networkRulesSet = proxy.Value.Key;
                        proxySetting = proxy.Value.Value;
                        break;
                    }
                }

                if (availableNetwork != null) break;
            }

            // Configurer le proxy en conséquence
            if (proxySetting != null)
            {
                proxySetting.Configure();
            }

            // Evénement
            if (ProxyChanged != null)
            {
                ProxyChanged(name, availableNetwork, proxySetting, result.Reason);
            }
        }
    }
}
