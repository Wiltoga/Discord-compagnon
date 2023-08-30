using Lazy;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiscordCompagnon
{
    internal class MainInterfaceViewModel
    {
        public MainInterfaceViewModel()
        {
            Settings = new SettingsViewModel();
            Timestamps = new TimestampsViewModel();
        }

        public SettingsViewModel Settings { get; }
        public TimestampsViewModel Timestamps { get; }
    }
}