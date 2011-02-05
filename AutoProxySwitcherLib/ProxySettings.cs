using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoProxySwitcher
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
    }

    public class StandardProxySettings : ProxySettings
    {
        private string proxyAddress;

        public StandardProxySettings(string proxyAddress)
        {
            this.proxyAddress = proxyAddress;
        }

        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();
            pc.SetProxy(proxyAddress);
        }
    }

    public class NoProxySettings : ProxySettings
    {
        public override void Configure()
        {
            SystemProxy.ProxyConfigurator pc = new SystemProxy.ProxyConfigurator();
            pc.ResetProxy();
        }
    }
}
