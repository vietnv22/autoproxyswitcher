using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace AutoProxySwitcherLib
{
    public class NetworkManager
    {
        /// <summary>
        /// Returns a list of available connected networks. The network address is given in text format: "a.b.c.d/bits"
        /// This list is build from the list of main IPs of each network interface
        /// </summary>
        /// <returns></returns>
        public static IList<NetworkInfo> ListAvailableNetworks()
        {
            List<NetworkInfo> networks = new List<NetworkInfo>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface n in adapters)
            {
                NetworkInfo net = new NetworkInfo();

                if (n.OperationalStatus == OperationalStatus.Up)
                {
                    // Stores basic attributes
                    net.IfName = n.Name;
                    net.Description = n.Description;
                    net.Type = n.NetworkInterfaceType.ToString();

                    // Stores interface address and network address
                    // Sometimes, it lacks masks (ex: loopback)
                    foreach (UnicastIPAddressInformation uipai in n.GetIPProperties().UnicastAddresses)
                    {
                        // Adds IP address
                        net.IP.Add(uipai.Address.ToString());

                        // Adds network IP adress with netmask
                        if (uipai.IPv4Mask != null && uipai.IPv4Mask.ToString() != "0.0.0.0")
                        {
                            net.NetworkIP.Add(GetNetworkAddressAsString(uipai.Address, uipai.IPv4Mask));
                        }
                    }

                    // Order addresses by length (shortest first)
                    net.IP = net.IP.OrderBy((s) => s.Length).ToList();

                    foreach (System.Net.IPAddress ipAddress in n.GetIPProperties().DnsAddresses)
                    {
                        net.DNS.Add(ipAddress.ToString());
                    }

                    networks.Add(net);
                }
            }

            return networks;
        }

        public static string GetNetworkAddressAsString(IPAddress address, IPAddress mask)
        {
            string netIP = ""; // Network address as string
            int networkLength = 0;

            if (null != mask)
            {
                for (int i = 0; i < 4; i++)
                {
                    byte ba = address.GetAddressBytes()[i];
                    byte bm = mask.GetAddressBytes()[i];

                    netIP += ba & bm;
                    if (i < 3) netIP += ".";
                    networkLength += 8 - (int)System.Math.Truncate(System.Math.Log(256 - bm, 2));
                }
                netIP += "/" + networkLength;
            }
            else
            {
                netIP = address.ToString() + "/32";
            }

            return netIP;
        }
    }
}
