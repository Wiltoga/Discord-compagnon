using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calendar = System.Windows.Controls.Calendar;

namespace DiscordCompagnon
{
    /// <summary>
    /// Interaction logic for MainInterface.xaml
    /// </summary>
    public partial class MainInterface : UserControl
    {
        public MainInterface()
        {
            InitializeComponent();
        }

        private MainInterfaceViewModel ViewModel => (MainInterfaceViewModel)DataContext;

        private void Calendar_GotMouseCapture(object sender, MouseEventArgs e)
        {
            var originalElement = (UIElement)e.OriginalSource;
            if (originalElement is CalendarDayButton || originalElement is CalendarItem)
            {
                originalElement.ReleaseMouseCapture();
            }
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            MainWindow.CloseWindow();
        }

        private void CopyTimestampButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CopyLink();
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.SelectAll();
        }

        private async void TriggerSettingsSave(object sender, RoutedEventArgs e)
        {
            await Task.Delay(300);
            ViewModel.SaveSettings();
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CloseContent();
        }
    }
}