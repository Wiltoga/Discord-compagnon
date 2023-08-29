using Lazy;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiscordCompagnon
{
    internal class MainInterfaceViewModel : ReactiveObject
    {
        public MainInterfaceViewModel()
        {
            Timer = Properties.Settings.Default.Timer.ToString();
            Date = DateTime.Now;
            Am = Date.Hour < 13;
            if (!IsAmPm || Am)
                Hours = Date.Hour.ToString("00");
            else
                Hours = (Date.Hour - 12).ToString("00");
            Minutes = Date.Minute.ToString("00");
            Seconds = "00";
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

            this.WhenAnyValue(o => o.Hours)
                .Select(hours =>
                {
                    if (hours.Length < 2)
                        return string.Concat(new string('0', 2 - hours.Length), hours);
                    var current = hours[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt < 0)
                            return "00";
                        if (!IsAmPm && currentInt > 23)
                            return "23";
                        if (IsAmPm && currentInt > 12)
                            return "12";
                        return current;
                    }
                    return "00";
                })
                .BindTo(this, o => o.Hours);

            this.WhenAnyValue(o => o.Minutes)
                .Select(minutes =>
                {
                    if (minutes.Length < 2)
                        return string.Concat(new string('0', 2 - minutes.Length), minutes);
                    var current = minutes[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt < 0)
                            return "00";
                        if (currentInt > 59)
                            return "23";
                        return current;
                    }
                    return "00";
                })
                .BindTo(this, o => o.Minutes);

            this.WhenAnyValue(o => o.Seconds)
                .Select(seconds =>
                {
                    if (seconds.Length < 2)
                        return string.Concat(new string('0', 2 - seconds.Length), seconds);
                    var current = seconds[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt < 0)
                            return "00";
                        if (currentInt > 59)
                            return "23";
                        return current;
                    }
                    return "00";
                })
                .BindTo(this, o => o.Seconds);

            SelectedTimestampType = TimestampTypes.First();
        }

        [Reactive]
        public bool Am { get; set; }

        [Reactive]
        public DateTime Date { get; set; }

        [Reactive]
        public string Hours { get; set; }

        public bool IsAmPm { get; } = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("tt");

        [Reactive]
        public string Minutes { get; set; }

        [Reactive]
        public string Seconds { get; set; }

        [Reactive]
        public TimestampType SelectedTimestampType { get; set; }

        [Reactive]
        public string Timer { get; set; }

        public TimestampType[] TimestampTypes => TimestampType.List;

        public void CopyLink()
        {
            if (int.TryParse(Hours, out var intHours) && int.TryParse(Minutes, out var intMinutes) && int.TryParse(Seconds, out var intSeconds))
            {
                try
                {
                    var targetDate = new DateTimeOffset(Date.Date + new TimeSpan(intHours + (IsAmPm && !Am ? 12 : 0), intMinutes, intSeconds));
                    Clipboard.SetText($"<t:{targetDate.ToUnixTimeSeconds()}:{SelectedTimestampType.Format}>");
                }
                catch { }
            }
        }

        public void SaveSettings()
        {
            if (int.TryParse(Timer, out var resultTimer))
                Properties.Settings.Default.Timer = resultTimer;

            Properties.Settings.Default.Save();
        }
    }

    internal class TimestampType
    {
        public TimestampType(string name, string format, string? example)
        {
            Name = name;
            Format = format;
            Example = example ?? DateTime.Now.ToString(format);
        }

        public static TimestampType[] List { get; } = new[]
        {
            new TimestampType("Short date", "d", null),
            new TimestampType("Long date", "D", null),
            new TimestampType("Short time", "t", null),
            new TimestampType("Long time", "T", null),
            new TimestampType("Date & time", "f", null),
            new TimestampType("Weekday, date & time", "f", $"{DateTime.Now:dddd}, {DateTime.Now:f}"),
            new TimestampType("Time left", "R", "in 3 days"),
        };

        public string Example { get; }
        public string Format { get; }
        public string Name { get; }
    }
}