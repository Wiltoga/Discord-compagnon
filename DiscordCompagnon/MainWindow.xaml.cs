using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordCompagnon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer timer;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            CloseContent();

            // initializing the process parser
            timer = new Timer(Properties.Settings.Default.Timer);
            timer.Elapsed += (_, _) => Dispatcher.Invoke(ResetPosition);
            timer.Start();
        }

        private enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        /// <summary>
        /// Saved discord process
        /// </summary>
        private static Process? DiscordProcess { get; set; }

        /// <summary>
        /// Instance of this window
        /// </summary>
        private static MainWindow? Instance { get; set; }

        /// <summary>
        /// Shrinks the window
        /// </summary>
        public static void CloseContent()
        {
            if (Instance is not null)
            {
                Instance.activated.Visibility = Visibility.Hidden;
                Instance.deactivated.Visibility = Visibility.Visible;
                ResetPosition();
            }
        }

        /// <summary>
        /// Closes the app
        /// </summary>
        public static void CloseWindow()
        {
            if (Instance is not null)
            {
                Instance.Close();
            }
        }

        /// <summary>
        /// Expand the window
        /// </summary>
        public static void OpenContent()
        {
            if (Instance is not null)
            {
                Instance.activated.Visibility = Visibility.Visible;
                Instance.deactivated.Visibility = Visibility.Hidden;
                ResetPosition();
            }
        }

        /// <summary>
        /// Check the position of the window relative to the Discord window
        /// </summary>
        public static void ResetPosition()
        {
            if (Instance is not null)
            {
                if (DiscordProcess is null || DiscordProcess.HasExited)
                {
                    var allProcesses = Process.GetProcesses();
                    DiscordProcess = allProcesses.FirstOrDefault(process =>
                    {
                        try
                        {
                            if (process.ProcessName == "Discord" && process.MainWindowHandle != IntPtr.Zero)
                                return true;
                        }
                        catch { }
                        return false;
                    });
                }
                if (DiscordProcess is not null)
                {
                    try
                    {
                        var windowSize = new RECT();
                        GetWindowRect(DiscordProcess.MainWindowHandle, ref windowSize);
                        var state = GetPlacement(DiscordProcess.MainWindowHandle);
                        switch (state.showCmd)
                        {
                            case ShowWindowCommands.Normal:
                                Instance.Show();
                                Instance.Top = windowSize.Top;
                                Instance.Left = windowSize.Left + 71;
                                break;

                            case ShowWindowCommands.Hide:
                            case ShowWindowCommands.Minimized:
                                Instance.Hide();
                                break;

                            case ShowWindowCommands.Maximized:
                                Instance.Show();
                                Instance.Top = windowSize.Top + 7;
                                Instance.Left = windowSize.Left + 71;
                                break;
                        }
                    }
                    catch { }
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            timer.Stop();
        }

        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            public int Width { get => Right - Left; set => Right = value + Left; }
            public int Height { get => Bottom - Top; set => Bottom = value + Top; }

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
    }
}