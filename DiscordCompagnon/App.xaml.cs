using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiscordCompagnon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? mutex;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            mutex?.ReleaseMutex();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "{06749a37-1bcf-42aa-921d-7d7356e619b3}");
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex = null;
                Environment.Exit(0);
            }
            base.OnStartup(e);
        }
    }
}