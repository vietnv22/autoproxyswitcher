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

        /// <summary>
        /// Creates a standard proxy setting
        /// </summary>
        /// <param name="proxyAddress"></param>
        /// <param name="exceptions">null if no change required</param>
        public StandardProxySettings(string proxyAddress, string exceptions)
        {
            this.proxyAddress = proxyAddress;
            this.exceptions = exceptions;
        }

        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();
            pc.SetProxy(proxyAddress, exceptions);
        }

        public override string ToString()
        {
            string proxyStr = proxyAddress;
            if (exceptions == null)
            {
                proxyStr += " without exceptions";
            }
            else if (exceptions == "")
            {
                proxyStr += " and clear exceptions";
            }
            else
            {
                proxyStr += " with exceptions";
            }

            return proxyStr;
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
