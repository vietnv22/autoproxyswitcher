using AutoProxySwitcherLib;
using AutoProxySwitcherWPF.View;
using AutoProxySwitcherWPF.ViewModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoProxySwitcherWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        //private TaskbarIcon tb;
        //private MainWindow mw;

        //<!--<tb:TaskbarIcon x:Key="MyNotifyIcon" DataContext="{Binding Source={StaticResource Locator}, Path=Main}" x:Name="MyNotifyIcon" IconSource="View/Icons/network.ico" ToolTipText="AutoProxySwitcher" />
        //<v:MainWindow x:Key="MainWindow" />-->
        protected override void OnStartup(StartupEventArgs e)
        {
            log.Info("Application start");

            base.OnStartup(e);

            // Get ViewModel, Window and Taskbar icon and connect everything
            ViewModelLocator locator = (ViewModelLocator)FindResource("Locator");

            var mw = (MainWindow)FindResource("MainWindow");
            mw.Show();
            (FindResource("MyNotifyIcon") as TaskbarIcon).LeftClickCommand = new RelayCommand
            (
                () =>
                {
                    log.Debug("LeftClickCommand:");
                    mw.Show();
                }
            );

            Messenger.Default.Register<MainViewModel.ExitMessage>
            (
                 this,
                 (msg) =>
                 {
                     try
                     {
                         log.Info("Shutdown requested");
                         Shutdown();
                     }
                     catch (Exception ex)
                     {
                         log.Error("failed to shutdown", ex);
                     }
                 }
            );

            Messenger.Default.Register<MainViewModel.ChangeMessage>
            (
                 this,
                 (msg) =>
                 {
                     try
                     {
                         TaskbarIcon tb = FindResource("MyNotifyIcon") as TaskbarIcon;

                         log.Debug("Change message received");

                         StringBuilder message = new StringBuilder();
                         message.AppendFormat("Configuration changed to {0}\n", msg.ConfName);

                         if (msg.Network != null)
                         {
                             message.AppendFormat("Connected to network {0}\n", msg.Network.IfName);
                         }

                         message.AppendFormat("Proxy set to {0}\n", msg.ProxySettings.ToString());
                         message.AppendFormat("Reason: {0}", msg.Reason);

                         tb.ShowBalloonTip("AutoProxySwitcher", message.ToString(), BalloonIcon.Info);
                     }
                     catch (Exception ex)
                     {
                         log.Error("failed to display tooltip", ex);
                     }
                 }
            );

            locator.Main.StartMonitoring();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            log.Info("Application end");
 	        base.OnExit(e);
        }
    }
}
