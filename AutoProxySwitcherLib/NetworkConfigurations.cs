using AutoProxySwitcherLib.Rules;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AutoProxySwitcherLib
{
    public class NetworkConfigurations
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkConfigurations));

        /// <summary>
        /// Load configuration from a string
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<NetworkConfiguration> FromString(string xml)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            return FromXmlDocument(doc);
        }
        
        /// <summary>
        /// Load configuration from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<NetworkConfiguration> FromFile(string fileName)
        {
            log.Info("Loading configuration from file " + fileName);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            return FromXmlDocument(doc);
        }

        /// <summary>
        /// Log configuration from XmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<NetworkConfiguration> FromXmlDocument(System.Xml.XmlDocument doc)
        {
            List<NetworkConfiguration> configurations = new List<NetworkConfiguration>();

            log.Debug("Loading configurations");

            // Chargement des règles
            foreach (System.Xml.XmlNode networkNode in doc.SelectNodes("/Networks/Network"))
            {
                NetworkRulesSet networkRulesSet = new NetworkRulesSet();
                ProxySettings proxySettings = null;

                networkRulesSet = ReadRules(networkNode.SelectNodes("Rules/*"));

                // Chargement des paramètres du configuration
                System.Xml.XmlNode proxyNode = networkNode.SelectSingleNode("Proxy");

                switch (proxyNode.Attributes["type"].Value)
                {
                    case "PacFile":
                        proxySettings = new ProxyPACSettings(proxyNode.Attributes["url"].Value);
                        break;

                    case "Standard":
                        proxySettings = new StandardProxySettings(proxyNode.Attributes["url"].Value, proxyNode.Attributes["exceptions"] != null ? proxyNode.Attributes["exceptions"].Value : null);
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

            return configurations;
        }

        /// <summary>
        /// Recursively read rules
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        static private NetworkRulesSet ReadRules(XmlNodeList nodeList)
        {
            NetworkRulesSet networkRulesSet = new NetworkRulesSet();

            foreach (System.Xml.XmlNode ruleNode in nodeList)
            {
                NetworkRule rule = null;

                if (ruleNode.Name == "And")
                {
                    NetworkRulesSet rs2 = ReadRules(ruleNode.SelectNodes("*"));
                    rs2.Op = Operator.And;
                    rule = rs2;
                }
                else if (ruleNode.Name == "Or")
                {
                    NetworkRulesSet rs2 = ReadRules(ruleNode.SelectNodes("*"));
                    rs2.Op = Operator.Or;
                    rule = rs2;
                }
                else if (ruleNode.Name == "Not")
                {
                    NetworkRulesSet rs2 = ReadRules(ruleNode.SelectNodes("*"));
                    rs2.Op = Operator.Not;
                    rule = rs2;
                }
                else if (ruleNode.Name == "Subnet")
                {
                    rule = new NetworkRuleSubnet(ruleNode.Attributes["ip"].Value);
                }
                else if (ruleNode.Name == "DNS")
                {
                    rule = new NetworkRuleDNS(ruleNode.Attributes["ip"].Value);
                }
                else if (ruleNode.Name == "Interface")
                {
                    rule = new NetworkRuleIfName(ruleNode.Attributes["name"].Value);
                }
                else if (ruleNode.Name == "Ping")
                {
                    rule = new NetworkRulePingable(ruleNode.Attributes["ip"].Value);
                }
                else
                {
                    log.Warn("Unknown rule " + ruleNode.Name);
                }

                if (rule != null)
                {
                    networkRulesSet.Rules.Add(rule);
                }
            }

            return networkRulesSet;
        }
    }
}
