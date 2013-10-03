using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using AutoProxySwitcherLib.Rules;
using System.Text.RegularExpressions;
using log4net;

namespace AutoProxySwitcherLib
{
    /// <summary>
    /// Matching (or not matching) reasons
    /// </summary>
    public enum Reasons
    {
        MATCH,      // A ruleNode match
        DEFAULT,    // The default configuration match (the one with no rules)
        NOMATCH
    };

    /// <summary>
    /// Rules checker. Contains a ruleNode set which can be checks against a given network information
    /// </summary>
    public class RulesChecker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RulesChecker));

        private NetworkRulesSet m_NetworkRulesSet;

        public RulesChecker(NetworkRulesSet networkRulesSet)
        {
            m_NetworkRulesSet = networkRulesSet;
        }

        internal NetworkRulesSet NetworkRulesSet
        {
            get { return m_NetworkRulesSet; }
            set { m_NetworkRulesSet = value; }
        }

        public class RulesCheckerResult
        {
            private Reasons reason;
            private string textReason;

            public RulesCheckerResult()
            {
                this.textReason = null;
                this.reason = Reasons.NOMATCH;
            }

            public RulesCheckerResult(Reasons reason, string reasonString)
            {
                this.reason = reason;
                this.textReason = reasonString;
            }

            public bool Match
            {
                get { return reason != Reasons.NOMATCH; }
            }

            public Reasons Reason
            {
                get { return reason; }
                set { reason = value; }
            }

            public string ReasonString
            {
                get { return textReason; }
                set { textReason = value; }
            }
        }

        /// <summary>
        /// Find rule matching the given network informations
        /// </summary>
        /// <param name="net">Network information</param>
        /// <returns>Result containing reason of match</returns>
        public RulesCheckerResult CheckRulesAgainstNetwork(NetworkInfo net)
        {
            // Si aucune règle, alors ça matche automatiquement
            if (m_NetworkRulesSet.Rules.Count == 0)
            {
                return new RulesCheckerResult(Reasons.DEFAULT, "default rule match");
            }

            // Parcourir les règles de détection
            RulesCheckerResult res = new RulesCheckerResult(Reasons.NOMATCH, "");
            bool result = CheckRuleAgainstNetwork(m_NetworkRulesSet, net, ref res);
            return res;
        }

        /// <summary>
        /// Checks (recursively) if a rule matches a given network information
        /// </summary>
        /// <param name="rule">Rule to check</param>
        /// <param name="net">Network information</param>
        /// <param name="res">Detailed result if match</param>
        /// <returns>true if rule matches</returns>
        private bool CheckRuleAgainstNetwork(NetworkRule rule, NetworkInfo net, ref RulesCheckerResult res)
        {
            if (rule is NetworkRulesSet)
            {
                bool thisRes = false;
                foreach (var rule2 in (rule as NetworkRulesSet).Rules)
                {
                    thisRes = CheckRuleAgainstNetwork(rule2, net, ref res);
                    if ((rule as NetworkRulesSet).Op == Operator.And && !thisRes) { thisRes = false; break; }
                    if ((rule as NetworkRulesSet).Op == Operator.Or && thisRes) { thisRes = true; break; }
                    if ((rule as NetworkRulesSet).Op == Operator.Not) { thisRes = !thisRes; break; }
                }

                res = new RulesCheckerResult(thisRes ? Reasons.MATCH : Reasons.NOMATCH, res != null ? res.ReasonString : "complex rule used");
                return thisRes;
            }
            else if (rule is NetworkRuleDNS)
            {
                foreach (string dns in net.DNS)
                {
                    if ((rule as NetworkRuleDNS).DNS == dns)
                    {
                        res = new RulesCheckerResult(Reasons.MATCH, "network DNS matches " + dns);
                        return true;
                    }
                }
            }
            else if (rule is NetworkRuleSubnet)
            {
                foreach (string ip in net.NetworkIP)
                {
                    if ((rule as NetworkRuleSubnet).Subnet == ip)
                    {
                        res = new RulesCheckerResult(Reasons.MATCH, "network subnet matches " + ip);
                        return true;
                    }
                }
            }
            else if (rule is NetworkRuleIfName)
            {
                string regex = (rule as NetworkRuleIfName).InterfaceName;

                if (Regex.IsMatch(net.IfName, regex))
                {
                    res = new RulesCheckerResult(Reasons.MATCH, "interface name matches " + regex);
                    return true;
                }
            }
            else if (rule is NetworkRulePingable)
            {
                string ip = (rule as NetworkRulePingable).IP;

                log.Debug("Pinging " + ip);
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(ip, 5000);
                log.Info("Ping status: " + reply.Status);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    res = new RulesCheckerResult(Reasons.MATCH, "machine " + ip + " is pingable");
                    return true;
                }
            }
            return false;
        }
    }
}
