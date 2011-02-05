using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using AutoProxySwitcher.Rules;

namespace AutoProxySwitcher
{
    /// <summary>
    /// Liste de règles
    /// </summary>
    public class NetworkRulesSet : List<NetworkRule>
    {
    }

    /// <summary>
    /// Vérificateur de règles
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
            private bool match;
            private string reason;

            public RulesCheckerResult()
            {
                this.match = false;
                this.reason = null;
            }

            public RulesCheckerResult(bool match)
            {
                this.match = match;
                this.reason = null;
            }

            public RulesCheckerResult(bool match, string reason)
            {
                this.match = match;
                this.reason = reason;
            }

            public bool Match
            {
                get { return match; }
                set { match = value; }
            }

            public string Reason
            {
                get { return reason; }
                set { reason = value; }
            }
        }

        /// <summary>
        /// Détermine si les règles correspondent au réseau trouvé
        /// </summary>
        /// <param name="net"></param>
        /// <returns></returns>
        public RulesCheckerResult CheckRulesAgainstNetwork(NetworkInfo net)
        {
            // Si aucune règle, alors ça matche automatiquement
            if (m_NetworkRulesSet.Count == 0)
            {
                return new RulesCheckerResult(true, "default rule match");
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
                            return new RulesCheckerResult(true, "network DNS matches " + dns);
                        }
                    }
                }
                else if (rule is NetworkRuleSubnet)
                {
                    if ((rule as NetworkRuleSubnet).Subnet == net.IP)
                    {
                        return new RulesCheckerResult(true, "network subnet matches " + net.IP);
                    }
                }
            }

            return new RulesCheckerResult(false);
        }
    }
}
