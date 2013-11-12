using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoProxySwitcherLib;
using System.Net;
using System.Resources;
using System.Collections.Generic;
using log4net;

namespace AutoProxySwitcherTests
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UnitTest1));

        [TestInitialize]
        public void Init()
        {
            log.Info("Init");
        }

        /// <summary>
        /// Test network address to string conversion
        /// </summary>
        [TestMethod]
        [TestCategory("IPAddress")]
        public void TestNetworkAddressConversions()
        {
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.255.255")), "192.168.0.1/32");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.255.240")), "192.168.0.0/28"); 
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.255.0")), "192.168.0.0/24");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.254.0")), "192.168.0.0/23");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.252.0")), "192.168.0.0/22");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("23.98.176.0"), IPAddress.Parse("255.255.252.0")), "23.98.176.0/22");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.248.0")), "192.168.0.0/21");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.128.0")), "192.168.0.0/17");
            Assert.AreEqual(NetworkManager.GetNetworkAddressAsString(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("248.0.0.0")), "192.0.0.0/5");
        }

        [TestMethod]
        public void TestSimpleRule()
        {
            AutoProxySwitcherLib.NetworkChangeDetector ncd = new NetworkChangeDetector();
            RulesChecker.RulesCheckerResult result;
            NetworkInfo networkInfo;
            ncd.LoadConfigurations("Samples/SimpleRule.xml");

            NetworkInfo ni = new NetworkInfo();
            ni.Description = "Test Network";
            ni.DNS = new List<string> { "192.168.1.252" };
            ni.IP = new List<string> { "192.168.1.254" };
            ni.NetworkIP = new List<string> { "192.168.0.0/16" };
            ni.Type = "Wi-Fi";
            ni.IfName = "Test1";

            List<NetworkInfo> networks = new List<NetworkInfo> { ni };

            NetworkConfiguration configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            Console.WriteLine(result.ReasonString);
            Assert.AreSame(configuration, ncd.Configurations[0]);
            Assert.IsTrue(result.ReasonString.Equals("network DNS matches 192.168.1.252"));
        }

        [TestMethod]
        public void TestComplexRule()
        {
            RulesChecker.RulesCheckerResult result;
            NetworkInfo networkInfo;
            AutoProxySwitcherLib.NetworkChangeDetector ncd = new NetworkChangeDetector();
            ncd.LoadConfigurations("Samples/ComplexRule.xml");

            NetworkInfo ni = new NetworkInfo();
            ni.Description = "Test Network";
            ni.DNS = new List<string> { "192.168.1.252" };
            ni.IP = new List<string> { "192.168.1.254" };
            ni.NetworkIP = new List<string> { "192.168.0.0/16" };
            ni.Type = "Wi-Fi";
            ni.IfName = "Test1";

            List<NetworkInfo> networks = new List<NetworkInfo> { ni };

            NetworkConfiguration configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            if (result != null)
            {
                Console.WriteLine(result.ReasonString);
            }
            Console.WriteLine("Test");
            Assert.AreSame(configuration, ncd.Configurations[0]);
            //Assert.IsTrue(result.ReasonString.Equals("network DNS matches 192.168.1.252"));
        }

        [TestMethod]
        public void TestPingRule()
        {
            RulesChecker.RulesCheckerResult result;
            NetworkInfo networkInfo;
            AutoProxySwitcherLib.NetworkChangeDetector ncd = new NetworkChangeDetector();
            ncd.LoadConfigurations("Samples/PingRule.xml");

            NetworkInfo ni = new NetworkInfo();
            ni.Description = "Test Network";
            ni.DNS = new List<string> { "192.168.1.252" };
            ni.IP = new List<string> { "192.168.1.254" };
            ni.NetworkIP = new List<string> { "192.168.0.0/16" };
            ni.Type = "Wi-Fi";
            ni.IfName = "Test1";

            List<NetworkInfo> networks = new List<NetworkInfo> { ni };

            NetworkConfiguration configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            if (result != null)
            {
                Console.WriteLine(result.ReasonString);
            }
            Console.WriteLine("Test");
            Assert.AreSame(configuration, ncd.Configurations[0]);
            //Assert.IsTrue(result.ReasonString.Equals("network DNS matches 192.168.1.252"));
        }

        /// <summary>
        /// Checks that the right config is taken among many
        /// </summary>
        [TestMethod]
        public void TestManyConfigs()
        {
            AutoProxySwitcherLib.NetworkChangeDetector ncd = new NetworkChangeDetector();
            RulesChecker.RulesCheckerResult result;
            NetworkInfo networkInfo;
            ncd.LoadConfigurations("Samples/ManyConfigs.xml");

            NetworkInfo ni = new NetworkInfo();
            ni.Description = "Test Network";
            ni.DNS = new List<string> { "172.17.4.80", "172.17.4.81" };
            ni.IP = new List<string> { "172.17.4.145" };
            ni.NetworkIP = new List<string> { "172.17.4.0/24" };
            ni.Type = "Local Area Connection";
            ni.IfName = "test";

            List<NetworkInfo> networks;
            NetworkConfiguration configuration;

            networks = new List<NetworkInfo> { ni };
            configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            Console.WriteLine(result.ReasonString);
            Console.WriteLine(configuration.ProxySettings);
            Assert.AreEqual(configuration.Name, "Config 1");
            Assert.IsTrue(result.ReasonString.Equals("network subnet matches 172.17.4.0/24"));
            

            ni.Description = "Test Network";
            ni.DNS = new List<string> { "172.17.80.80", "172.17.80.81" };
            ni.IP = new List<string> { "172.17.80.15" };
            ni.NetworkIP = new List<string> { "172.17.80.0/24" };
            ni.Type = "Local Area Connection";
            ni.IfName = "test";

            networks = new List<NetworkInfo> { ni };
            configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            Console.WriteLine(result.ReasonString);
            Console.WriteLine(configuration.ProxySettings);
            Assert.AreEqual(configuration.Name, "Config 4");
            Assert.IsTrue(result.ReasonString.Equals("network subnet matches 172.17.80.0/24"));

            ni.Description = "Test Network";
            ni.DNS = new List<string> { "172.16.0.5" };
            ni.IP = new List<string> { "172.16.0.78" };
            ni.NetworkIP = new List<string> { "172.16.2/24" };
            ni.Type = "Local Area Connection";
            ni.IfName = "test";

            networks = new List<NetworkInfo> { ni };
            configuration = ncd.FindMatchingConfiguration(networks, out result, out networkInfo);

            Console.WriteLine(result.ReasonString);
            Console.WriteLine(configuration.ProxySettings); 
            Assert.AreEqual(configuration.Name, "Default Config");
            Assert.IsTrue(result.ReasonString.Equals("default rule match"));
        }

    }
}
