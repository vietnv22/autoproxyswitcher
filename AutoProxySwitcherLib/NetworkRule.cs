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

        public override string ToString()
        {
            return "DNS IP is " + m_DNS;
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

        public override string ToString()
        {
            return "subnet is " + m_Subnet;
        }
    }

    public class NetworkRuleIfName : NetworkRule
    {
        private string _ifName;

        public NetworkRuleIfName(string ifName)
        {
            _ifName = ifName;
        }

        public string InterfaceName {
            get { return _ifName; }
            set { _ifName = value; }
        }

        public override string ToString()
        {
            return "interface name is " + _ifName;
        }
    }

    public class NetworkRulePingable : NetworkRule
    {
        private string _ip;

        public NetworkRulePingable(string ip)
        {
            _ip = ip;
        }

        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public override string ToString()
        {
            return "machine " + _ip + " is pingable";
        }
    }
}
