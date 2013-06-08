using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoProxySwitcherLib.Rules
{
    public abstract class NetworkRule { };

    public class NetworkRuleDNS : NetworkRule
    {
        private string m_DNS;

        public NetworkRuleDNS(string DNS)
        {
            m_DNS = DNS;
        }

        public string DNS
        {
            get { return m_DNS; }
            set { m_DNS = value; }
        }
    }

    public class NetworkRuleSubnet : NetworkRule
    {
        private string m_Subnet;

        public NetworkRuleSubnet(string subnet)
        {
            m_Subnet = subnet;
        }

        public string Subnet
        {
            get { return m_Subnet; }
            set { m_Subnet = value; }
        }
    }
}
