using AutoProxySwitcherLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AutoProxySwitcherWPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        private NetworkChangeDetector _networkChangeDetector;
        private IList<NetworkConfiguration> _configurationsList;

        private IList<NetworkInfo> _currentNetworks = new List<NetworkInfo>();
        private NetworkInfo _currentNetwork = null;
        private ProxySettings _currentProxySettings = null;

        private DateTime lastUpdateTime;
        private NetworkConfiguration _activeConfiguration;
        private string addresses;
        private string interfaceName;
        private string reason;

        /// <summary>
        /// Default constructor, only for design mode
        /// </summary>
        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                // Generate some configurations
                _configurationsList = new List<NetworkConfiguration>();

                NetworkRulesSet rulesSet = new NetworkRulesSet();
                rulesSet.Rules.Add(new AutoProxySwitcherLib.Rules.NetworkRuleDNS("10.0.0.1"));

                for (int i = 0; i < 5; i++)
                {
                    _configurationsList.Add(new NetworkConfiguration("Config #" + i, rulesSet, new StandardProxySettings("http://test:8080", null, false)));
                }

                _activeConfiguration = _configurationsList[2];


                // Generate some networks
                for(int i = 0; i < 10; i++)
                {
                    _currentNetworks.Add(new NetworkInfo() { IfName = "Sample Network #" + i, IP = { "10.0.0.1" }, DNS = { "10.0.0.5" }, NetworkIP = { "10.0.0.0/24" } });
                }

                _currentNetwork = _currentNetworks[3];
                _currentProxySettings = _activeConfiguration.ProxySettings;

                //
                reason = "Network matching IP x.y.z.t";
            }
            else
            {
                string rulesFile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
                    + "\\AutoProxySwitcher\\"
                    + Environment.ExpandEnvironmentVariables(System.Configuration.ConfigurationManager.AppSettings["RulesFile"]);

                // Create default rules files if it doesn't exists
                if (!System.IO.File.Exists(rulesFile))
                {
                    System.IO.File.Copy("Examples/rules.xml", rulesFile);
                }

                // Load configurations
                _networkChangeDetector = new NetworkChangeDetector();
                _networkChangeDetector.ProxyChanged += _networkChangeDetector_ProxyChanged;
                _networkChangeDetector.LoadConfigurations(rulesFile);

                _configurationsList = _networkChangeDetector.Configurations;

                // Init commands relay
                DebugCommand = new RelayCommand<string>(m => MessageBox.Show("toto: " + m));
                SetConfigurationCommand = new RelayCommand<string>((conf) => _networkChangeDetector.SetConfiguration(conf));
                AutoDetectCommand = new RelayCommand(() => _networkChangeDetector.SetConfiguration(null));
                ExitCommand = new RelayCommand(() => { Messenger.Default.Send<ExitMessage>(new ExitMessage()); });
            }
        }

        public RelayCommand ExitCommand
        {
            get;
            private set;
        }

        public RelayCommand<string> DebugCommand
        {
            get;
            private set;
        }

        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            set { lastUpdateTime = value; }
        }

        public NetworkConfiguration ActiveConfiguration
        {
            get { return _activeConfiguration; }
            set { return; }
        }

        public string Addresses
        {
            get { return addresses; }
            set { addresses = value; }
        }

        public string InterfaceName
        {
            get { return interfaceName; }
            set { interfaceName = value; }
        }

        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }

        public IList<NetworkInfo> CurrentNetworks
        {
            get { return _currentNetworks; }
            set { _currentNetworks = value; }
        }

        public NetworkInfo CurrentNetwork
        {
            get { return _currentNetwork; }
        }

        public ProxySettings CurrentProxySettings
        {
            get { return _currentProxySettings; }
        }

        void _networkChangeDetector_ProxyChanged(string name, NetworkInfo networkInfo, ProxySettings proxySettings, string reason)
        {
            try
            {
                log.DebugFormat("ProxyChanged event: {0}, network {1}, proxy {2}, reason", name, networkInfo, proxySettings, reason);

                lastUpdateTime = DateTime.Now;
                _activeConfiguration = _configurationsList.First((c) => c.Name == name);
                this.interfaceName = networkInfo != null ? networkInfo.IfName : "N/A";
                this.addresses = networkInfo != null ? String.Join(", ", networkInfo.IP) : "";
                this.reason = reason;
                _currentNetworks = _networkChangeDetector.CurrentNetworks;
                _currentNetwork = networkInfo != null ? _currentNetworks.FirstOrDefault((n) => networkInfo.IfName == n.IfName) : null;
                _currentProxySettings = proxySettings;

                RaisePropertyChanged("LastUpdateTime");
                RaisePropertyChanged("ActiveConfiguration");
                RaisePropertyChanged("InterfaceName");
                RaisePropertyChanged("Addresses");
                RaisePropertyChanged("Reason");
                RaisePropertyChanged("CurrentNetworks");
                RaisePropertyChanged("CurrentNetwork");
                RaisePropertyChanged("CurrentProxySettings");

                log.Debug("Sending ChangeMessage");
                ChangeMessage msg = new ChangeMessage() { ConfName = name, Network = networkInfo, ProxySettings = proxySettings, Reason = reason };
                Messenger.Default.Send<ChangeMessage>(msg);
            }
            catch (Exception ex)
            {
                log.Error("Failure", ex);
            }
        }

        public IList<NetworkConfiguration> Configurations
        {
            get { return _configurationsList; }
            set { _configurationsList = value; }
        }

        public void StartMonitoring()
        {
            _networkChangeDetector.StartMonitoring();
        }

        /// <summary>
        /// Command to start autodetection
        /// </summary>
        public RelayCommand AutoDetectCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to set configuration
        /// </summary>
        public RelayCommand<string> SetConfigurationCommand
        {
            get;
            private set;
        }

        public class ChangeMessage : GalaSoft.MvvmLight.Messaging.MessageBase
        {
            public string ConfName { get; set; }
            public NetworkInfo Network { get; set; }
            public ProxySettings ProxySettings { get; set; }
            public string Reason { get; set; }
        }

        public class ExitMessage : GalaSoft.MvvmLight.Messaging.MessageBase
        {
        }
    }
}