using log4net;
using System.Collections.Generic;

namespace AutoProxySwitcherLib
{
    /// <summary>
    /// Stores informations of a network
    /// </summary>
    public class NetworkInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NetworkInfo));

        public NetworkInfo()
        {
            DNS = new List<string>();
            NetworkIP = new List<string>();
            IP = new List<string>();
        }

        public string IfName { get; set; }

        public List<string> IP { get; set; }

        public List<string> NetworkIP { get; set; }

        public List<string> DNS { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    }
}
