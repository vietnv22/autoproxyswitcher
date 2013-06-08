using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoProxySwitcher
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //log4net.Config.XmlConfigurator.Configure();
                log.Info("Starting");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SystrayForm());
                log.Info("End");
            }
            catch (Exception ex)
            {
                log.Error("Fatal error", ex);
            }
        }
    }
}
