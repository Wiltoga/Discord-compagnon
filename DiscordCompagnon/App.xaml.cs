using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
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
            // releasing single instance lock
            mutex?.ReleaseMutex();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // registering and checking for single instance app
            mutex = new Mutex(true, "{06749a37-1bcf-42aa-921d-7d7356e619b3}");
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex = null;
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }

        private void disp(byte[] bytes, bool copy)
        {
            var str = Encoding.Unicode.GetString(bytes);
            disp(str);
            if (copy)
                Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void disp(string str)
        {
            Console.WriteLine(str);
            foreach (var b in Encoding.Unicode.GetBytes(str))
            {
                for (int i = 7; i >= 0; --i)
                {
                    var shift = 0b1 << i;
                    if ((shift & b) == shift)
                        Console.Write("8");
                    else
                        Console.Write("^");
                }
                Console.Write('|');
            }
            Console.WriteLine();
        }
    }
}