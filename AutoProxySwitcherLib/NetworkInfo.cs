using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using log4net;

namespace AutoProxySwitcherLib
{
    public class NetworkInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkInfo));
        
        private string m_name;
        private List<string> m_IP;
        private List<string> m_NetworkIP;
        private List<string> m_DNS;

        public NetworkInfo()
        {
            m_DNS = new List<string>();
            m_NetworkIP = new List<string>();
            m_IP = new List<string>();
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public List<string> IP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }

        public List<string> NetworkIP
        {
            get { return m_NetworkIP; }
            set { m_NetworkIP = value; }
        }

        public List<string> DNS
        {
            get { return m_DNS; }
            set { m_DNS = value; }
        }

        /// <summary>
        /// Returns a list of available connected networks. The network address is given in text format: "a.b.c.d/bits"
        /// This list is build from the list of main IPs of each network interface
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, NetworkInfo> ListAvailableNetworks()
        {
            Dictionary<string, NetworkInfo> networks = new Dictionary<string, NetworkInfo>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface n in adapters)
            {
                NetworkInfo net = new NetworkInfo();
                int networkLength = 0;

                // Stores interface address and network address
                // Sometimes, it lacks masks (ex: loopback)
                foreach (UnicastIPAddressInformation uipai in n.GetIPProperties().UnicastAddresses)
                {
                    // Adds IP address
                    net.IP.Add(uipai.Address.ToString());

                    // Adds network IP adress with netmask
                    if (uipai.IPv4Mask != null && uipai.IPv4Mask.ToString() != "0.0.0.0")
                    {
                        string netIP = ""; // Network address as string

                        if (null != uipai.IPv4Mask)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                byte ba = uipai.Address.GetAddressBytes()[i];
                                byte bm = uipai.IPv4Mask.GetAddressBytes()[i];

                                netIP += ba & bm;
                                if (i < 3) netIP += ".";
                                networkLength += (int)System.Math.Truncate(System.Math.Log(bm + 1, 2));
                            }
                            netIP += "/" + networkLength;
                        }
                        else
                        {
                            netIP = uipai.Address.ToString() + "/32";
                        }

                        net.NetworkIP.Add(netIP);
                    }
                }

                // Order addresses by length (shortest first)
                net.IP = net.IP.OrderBy((s) => s.Length).ToList();

                net.Name = n.Name;

                foreach(System.Net.IPAddress ipAddress in n.GetIPProperties().DnsAddresses)
                {
                    net.DNS.Add(ipAddress.ToString());
                }

                networks.Add(n.Name, net);
            }

            return networks;
        }

    }
}
