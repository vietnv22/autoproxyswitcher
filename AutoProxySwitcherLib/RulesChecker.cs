using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using AutoProxySwitcherLib.Rules;

namespace AutoProxySwitcherLib
{
    /// <summary>
    /// Set of rules
    /// </summary>
    public class NetworkRulesSet : List<NetworkRule>
    {
    }

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
        /// Find ruleNode matching the given network informations
        /// </summary>
        /// <param name="net">Network information</param>
        /// <returns>Result containing reason of match</returns>
        public RulesCheckerResult CheckRulesAgainstNetwork(NetworkInfo net)
        {
            // Si aucune règle, alors ça matche automatiquement
            if (m_NetworkRulesSet.Count == 0)
            {
                return new RulesCheckerResult(Reasons.DEFAULT, "default rule match");
            }

            // Parcourir les règles de détection
            foreach (NetworkRule rule in m_NetworkRulesSet)
            {
                if (rule is NetworkRuleDNS)
                {
                    foreach (string dns in net.DNS)
                    {
                        if ((rule as NetworkRuleDNS).DNS == dns)
                        {
                            return new RulesCheckerResult(Reasons.MATCH, "network DNS matches " + dns);
                        }
                    }
                }
                else if (rule is NetworkRuleSubnet)
                {
                    foreach (string ip in net.NetworkIP)
                    {
                        if ((rule as NetworkRuleSubnet).Subnet == ip)
                        {
                            return new RulesCheckerResult(Reasons.MATCH, "network subnet matches " + ip);
                        }
                    }
                }
            }

            return new RulesCheckerResult(Reasons.NOMATCH, "");
        }
    }
}
