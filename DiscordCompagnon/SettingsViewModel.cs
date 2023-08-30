using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCompagnon
{
    internal class SettingsViewModel : ReactiveObject
    {
        private const string StartupRegistryKeyName = @"DiscordCompagnon";
        private const string StartupRegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public SettingsViewModel()
        {
            // duration between process parsings
            Timer = Properties.Settings.Default.Timer.ToString();

            // checking the run on startup value
            {
                using var startupKey = Registry.CurrentUser.OpenSubKey(StartupRegistryKeyPath);
                RunsOnStartup = startupKey?.GetValue(StartupRegistryKeyName, null) is not null;
            }
            // user input validation for the timer
            this.WhenAnyValue(o => o.Timer)
                .Do(value =>
                {
                    if (int.TryParse(value, out var result))
                    {
                        if (result < 100)
                            Timer = "100";
                    }
                    else
                        Timer = "2000";
                })
                .Subscribe();
        }

        /// <summary>
        /// Runs on startup
        /// </summary>
        [Reactive]
        public bool RunsOnStartup { get; set; }

        /// <summary>
        /// Time between process parsings
        /// </summary>
        [Reactive]
        public string Timer { get; set; }

        /// <summary>
        /// Saves the settings of the app
        /// </summary>
        public void SaveSettings()
        {
            if (int.TryParse(Timer, out var resultTimer))
                Properties.Settings.Default.Timer = resultTimer;

            Properties.Settings.Default.Save();

            try
            {
                using var startupKey = Registry.CurrentUser.OpenSubKey(StartupRegistryKeyPath, true);
                using var currentProcess = Process.GetCurrentProcess();
                if (startupKey is not null)
                {
                    var exePath = Path.Combine(AppContext.BaseDirectory, currentProcess.ProcessName + ".exe");
#if RELEASE
                    if (RunsOnStartup)
                        startupKey.SetValue(StartupRegistryKeyName, exePath);
                    else
                        startupKey.DeleteValue(StartupRegistryKeyName, false);
#endif
                }
            }
            catch { }
        }
    }
}