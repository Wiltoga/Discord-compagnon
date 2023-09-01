using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCompagnon
{
    internal class AboutViewModel
    {
        public AboutViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version?.ToString() ?? "";
        }

        public string Version { get; }
    }
}