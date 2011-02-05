using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace AutoProxySwitcher
{
    public class NetworkInfo
    {
        private string m_name;
        private string m_IP;
        private List<string> m_DNS;

        public NetworkInfo()
        {
            m_DNS = new List<string>();
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        
        public string IP
        {
            get { return m_IP; }
            set { m_IP = value; }
        }

        public List<string> DNS
        {
            get { return m_DNS; }
            set { m_DNS = value; }
        }

        /// <summary>
        /// Retourne la liste des réseau IP disponibles, au format texte: "a.b.c.d/bits"
        /// Cette liste est obtenue à partie de la liste des IP principales de chaque interface
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, NetworkInfo> ListAvailableNetworks()
        {
            Dictionary<string, NetworkInfo> networks = new Dictionary<string, NetworkInfo>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface n in adapters)
            {
                string netIP = ""; // IP du réseau sous forme de chaîne
                NetworkInfo net = new NetworkInfo();
                int networkLength = 0;

                // Calcul de l'IP du réseau en appliquant le masque à l'adresse
                // parfois, il n'y a pas de masque (ex: loopback)
                if (n.GetIPProperties().UnicastAddresses.Count == 0) continue;

                if (null != n.GetIPProperties().UnicastAddresses[0].IPv4Mask)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        byte ba = n.GetIPProperties().UnicastAddresses[0].Address.GetAddressBytes()[i];
                        byte bm = n.GetIPProperties().UnicastAddresses[0].IPv4Mask.GetAddressBytes()[i];

                        netIP += ba & bm;
                        if (i < 3) netIP += ".";
                        networkLength += (int)System.Math.Truncate(System.Math.Log(bm + 1, 2));
                    }
                    netIP += "/" + networkLength;
                }
                else
                {
                    netIP = n.GetIPProperties().UnicastAddresses[0].Address.ToString() + "/32";
                }

                net.Name = n.Name;

                foreach(System.Net.IPAddress ipAddress in n.GetIPProperties().DnsAddresses)
                {
                    net.DNS.Add(ipAddress.ToString());
                }

                net.IP = netIP;

                networks.Add(n.Name, net);
            }

            return networks;
        }

    }
}
