using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoProxySwitcherLib
{
    public abstract class ProxySettings
    {
        public abstract void Configure();
    }

    public class ProxyPACSettings : ProxySettings
    {
        private string pacFile;

        public ProxyPACSettings(string pacFile)
        {
            this.pacFile = pacFile;
        }

        public string PacFile
        {
            get { return pacFile; }
            set { pacFile = value; }
        }

        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();
            pc.SetProxyPac(pacFile);
        }

        public override string ToString()
        {
            return "PAC file " + pacFile;
        }
    }

    public class StandardProxySettings : ProxySettings
    {
        private string proxyAddress;
        private string exceptions;
        private bool? bypassLocalAddresses;

        /// <summary>
        /// Creates a standard proxy setting
        /// </summary>
        /// <param name="proxyAddress"></param>
        /// <param name="exceptions">null if no change required</param>
        /// <param name="bypassLocalAddresses">true to bypass local addresses</param>
        public StandardProxySettings(string proxyAddress, string exceptions, bool? bypassLocalAddresses)
        {
            this.proxyAddress = proxyAddress;
            this.exceptions = exceptions;
            this.bypassLocalAddresses = bypassLocalAddresses;
        }

        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();

            string bypassList = null;

            if (bypassLocalAddresses.HasValue || exceptions != null)
            {
                bypassList = "";

                if (exceptions != null)
                {
                    bypassList += exceptions;
                }

                if (bypassLocalAddresses.HasValue && bypassLocalAddresses == true)
                {
                    if (bypassList.Length > 0)
                    {
                        bypassList += ";";
                    }
                    
                    bypassList += "<local>";
                }
            }

            pc.SetProxy(proxyAddress, bypassList);
        }

        public override string ToString()
        {
            string info = "";

            if (bypassLocalAddresses != null || exceptions != null)
            {
                info = " clear exceptions";

                if (!string.IsNullOrWhiteSpace(exceptions))
                {
                    info = " with exceptions";
                }

                if (bypassLocalAddresses != null && bypassLocalAddresses == true)
                {
                    info += " and bypass proxy for local addresses";
                }
            }
            else
            {
                info = " exceptions untouched";
            }

            return proxyAddress + info;
        }
    }

    public class NoProxySettings : ProxySettings
    {
        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();
            pc.ResetProxy();
        }

        public override string ToString()
        {
            return "No proxy";
        }
    }
}
