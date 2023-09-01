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
using System.Windows.Interop;
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

            Handle = new WindowInteropHelper(this).Handle;
        }

        private enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        private enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        public IntPtr Handle { get; }

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
                        var showWindow = false;
                        // check Discord window dimensions and state
                        var windowSize = new RECT();
                        GetWindowRect(DiscordProcess.MainWindowHandle, ref windowSize);
                        var state = GetPlacement(DiscordProcess.MainWindowHandle);
                        if (!IsWindowVisible(DiscordProcess.MainWindowHandle))
                            state.showCmd = ShowWindowCommands.Hide;
                        switch (state.showCmd)
                        {
                            case ShowWindowCommands.Normal:
                                showWindow = true;
                                Instance.Top = windowSize.Top;
                                Instance.Left = windowSize.Left + 71;
                                break;

                            // somehow, even if the Discord window is closed, it still is in the "normal" state.
                            case ShowWindowCommands.Hide:
                            case ShowWindowCommands.Minimized:
                                showWindow = false;
                                break;

                            case ShowWindowCommands.Maximized:
                                showWindow = true;
                                Instance.Top = windowSize.Top + 7;
                                Instance.Left = windowSize.Left + 71;
                                break;
                        }

                        // check if another window is on top of the hot area of discord's window
                        IntPtr other = DiscordProcess.MainWindowHandle;
                        var compagnonBox = new RECT
                        {
                            Left = (int)Instance.Left,
                            Right = (int)Instance.Left + 100,
                            Top = (int)Instance.Top,
                            Bottom = (int)Instance.Top + 22,
                        };
                        while ((other = GetWindow(other, GetWindowType.GW_HWNDPREV)) != IntPtr.Zero)
                        {
                            if (other == Instance.Handle)
                                continue;
                            if (!IsWindowVisible(other))
                                continue;
                            var otherSize = new RECT();
                            var sb = new StringBuilder(256);
                            GetWindowText(other, sb, 256);
                            var title = sb.ToString();
#if DEBUG
                            // to prevent the debugging overlay from triggering the hiding
                            if (title == "")
                                continue;
#endif
                            var otherState = GetPlacement(other);
                            GetWindowRect(other, ref otherSize);

                            if (compagnonBox.Collision(otherSize))
                                showWindow = false;
                        }

                        if (showWindow)
                            Instance.Show();
                        else
                            Instance.Hide();
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

        [DllImport("user32")]
        private static extern IntPtr GetWindow(IntPtr hwnd, GetWindowType uCmd);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            public int Width { get => Right - Left; set => Right = value + Left; }
            public int Height { get => Bottom - Top; set => Bottom = value + Top; }

            public bool Collision(RECT hitbox)
            {
                // each corner to the other box
                foreach ((int Y, int X) point in new[]
                {
                    (Top, Left),
                    (Top, Right),
                    (Bottom, Left),
                    (Bottom, Right),
                })
                {
                    if (hitbox.Collision(point.X, point.Y))
                        return true;
                }
                // each other corner to the current box
                foreach ((int Y, int X) point in new[]
                {
                    (hitbox.Top, hitbox.Left),
                    (hitbox.Top, hitbox.Right),
                    (hitbox.Bottom, hitbox.Left),
                    (hitbox.Bottom, hitbox.Right),
                })
                {
                    if (Collision(point.X, point.Y))
                        return true;
                }

                // NOTE : there are other checks to do for a good rectangle collision, notably if the collision is like this: ➕,
                // but it's a very very small probability with windows, so I don't care

                return false;
            }

            public bool Collision(int x, int y)
            {
                return x >= Left && x <= Right && y >= Top && y <= Bottom;
            }

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