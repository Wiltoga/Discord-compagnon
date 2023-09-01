using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void CopyModifiedTextButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ViewModel.TextModifier.PreviewText, TextDataFormat.UnicodeText);
        }

        private void CopyTimestampButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Timestamps.CopyLink();
        }

        private void HoursTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewModel.Timestamps.EditHours(e.Delta > 0 ? 1 : -1);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }

        private void ImportTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
                ViewModel.TextModifier.InputText = Clipboard.GetText();
        }

        private void MinutesTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewModel.Timestamps.EditMinutes(e.Delta > 0 ? 1 : -1);
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SecondsTextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewModel.Timestamps.EditSeconds(e.Delta > 0 ? 1 : -1);
        }

        private void TextModifierButton_MouseEnter(object sender, MouseEventArgs e)
        {
            var textModifier = (ITextChanger)((FrameworkElement)sender).DataContext;
            ViewModel.TextModifier.Preview(textModifier);
        }

        private void TimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.SelectAll();
        }

        private async void TriggerSettingsSave(object sender, RoutedEventArgs e)
        {
            await Task.Delay(300);
            ViewModel.Settings.SaveSettings();
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CloseContent();
        }
    }
}