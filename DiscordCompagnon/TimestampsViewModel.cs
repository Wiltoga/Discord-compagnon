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
    internal class TimestampsViewModel : ReactiveObject
    {
        public TimestampsViewModel()
        {
            Date = DateTime.Now;
            // check for ante meridiem
            Am = Date.Hour < 13;
            if (!IsAmPm || Am)
                Hours = Date.Hour.ToString("00");
            else
                Hours = (Date.Hour - 12).ToString("00");

            Minutes = Date.Minute.ToString("00");
            Seconds = "00";
            // user input validation for the hours of timestamp
            this.WhenAnyValue(o => o.Hours)
                .Select(hours =>
                {
                    if (hours.Length < 2)
                        return string.Concat(new string('0', 2 - hours.Length), hours);
                    var current = hours[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt <= 0)
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

            // user input validation for the minutes of timestamp
            this.WhenAnyValue(o => o.Minutes)
                .Select(minutes =>
                {
                    if (minutes.Length < 2)
                        return string.Concat(new string('0', 2 - minutes.Length), minutes);
                    var current = minutes[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt <= 0)
                            return "00";
                        if (currentInt > 59)
                            return "59";
                        return current;
                    }
                    return "00";
                })
                .BindTo(this, o => o.Minutes);

            // user input validation for the seconds of timestamp
            this.WhenAnyValue(o => o.Seconds)
                .Select(seconds =>
                {
                    if (seconds.Length < 2)
                        return string.Concat(new string('0', 2 - seconds.Length), seconds);
                    var current = seconds[..2];
                    if (int.TryParse(current, out var currentInt))
                    {
                        if (currentInt <= 0)
                            return "00";
                        if (currentInt > 59)
                            return "59";
                        return current;
                    }
                    return "00";
                })
                .BindTo(this, o => o.Seconds);

            SelectedTimestampType = TimestampTypes.First();
        }

        /// <summary>
        /// Current timestamp is AM
        /// </summary>
        [Reactive]
        public bool Am { get; set; }

        /// <summary>
        /// Selected timestamp day
        /// </summary>
        [Reactive]
        public DateTime Date { get; set; }

        /// <summary>
        /// Hours of the timestamp
        /// </summary>
        [Reactive]
        public string Hours { get; set; }

        /// <summary>
        /// If the current culture uses AM/PM
        /// </summary>
        public bool IsAmPm { get; } = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("tt");

        /// <summary>
        /// Minutes of the timestamp
        /// </summary>
        [Reactive]
        public string Minutes { get; set; }

        /// <summary>
        /// Seconds of the timestamp
        /// </summary>
        [Reactive]
        public string Seconds { get; set; }

        /// <summary>
        /// Selected timestamp render type
        /// </summary>
        [Reactive]
        public TimestampType SelectedTimestampType { get; set; }

        /// <summary>
        /// List of timestamp render types
        /// </summary>
        public TimestampType[] TimestampTypes => TimestampType.List;

        /// <summary>
        /// Copy the timestamp in the clipboard
        /// </summary>
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

        public void EditHours(int delta)
        {
            if (int.TryParse(Hours, out var integer))
            {
                integer += delta;
                Hours = integer.ToString("00");
            }
        }

        public void EditMinutes(int delta)
        {
            if (int.TryParse(Minutes, out var integer))
            {
                integer += delta;
                Minutes = integer.ToString("00");
            }
        }

        public void EditSeconds(int delta)
        {
            if (int.TryParse(Seconds, out var integer))
            {
                integer += delta;
                Seconds = integer.ToString("00");
            }
        }
    }

    /// <summary>
    /// Type of timestamp rendering
    /// </summary>
    internal class TimestampType
    {
        public TimestampType(string name, string format, string? example)
        {
            Name = name;
            Format = format;
            Example = example ?? DateTime.Now.ToString(format);
        }

        /// <summary>
        /// List of all types supported
        /// </summary>
        public static TimestampType[] List { get; } = new[]
        {
            new TimestampType("Short date", "d", "09/01/2023"),
            new TimestampType("Long date", "D", "September 1 2023"),
            new TimestampType("Short time", "t", "07:34 AM"),
            new TimestampType("Long time", "T", "07:34:25 AM"),
            new TimestampType("Date & time", "f", "September 1 2023 07:34 AM"),
            new TimestampType("Weekday, date & time", "F", "Friday, September 1 2023 07:34 AM"),
            new TimestampType("Time left", "R", "in 3 days"),
        };

        /// <summary>
        /// An example of this render type
        /// </summary>
        public string Example { get; }

        /// <summary>
        /// Discord format of the render type
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Description of the render type
        /// </summary>
        public string Name { get; }
    }
}