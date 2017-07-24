using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    public partial class CheckControl : UserControl
    {
        private Check check;

        /// <summary>
        /// Creates a default instance of this control.
        /// </summary>
        public CheckControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates an instance of this control populated from the given check data.
        /// </summary>
        public CheckControl(Check check) : this()
        {
            Refresh(check);
        }

        /// <summary>
        /// Updates this control based on the given check data.
        /// </summary>
        public void Refresh(Check check)
        {
            this.check = check;
            checkName.Content = check.name;
            checkLastPing.Content = string.Format("Last ping: {0}", FormatForDisplay(check.SinceLastPing()));
            checkStatus.Source = GetIconFor(check.status);
        }

        /// <summary>
        /// Returns the icon corresponding to the given CheckStatus.
        /// </summary>
        public static ImageSource GetIconFor(CheckStatus status)
        {
            string icon;
            if (status == CheckStatus.up) icon = "Icons/up.ico";
            else if (status == CheckStatus.down) icon = "Icons/down.ico";
            else if (status == CheckStatus.late) icon = "Icons/late.ico";
            else if (status == CheckStatus.@new) icon = "Icons/new.ico";
            else if (status == CheckStatus.paused) icon = "Icons/paused.ico";
            else throw new ArgumentException("No icon exists for status: " + status, nameof(status));
            return new BitmapImage(new Uri("pack://application:,,,/HealthTray;component/" + icon));
        }

        /// <summary>
        /// Returns a human-readable version of the given "check last pinged" TimeSpan.
        /// </summary>
        /// <remarks>
        /// TODO: unit test
        /// </remarks>
        private static string FormatForDisplay(TimeSpan? ts)
        {
            if (ts == null) return "never";

            var t = ts.Value;
            var sb = new StringBuilder();
            if (t.Days >= 1) sb.AppendFormat("{0}d ", t.Days);
            if (t.Hours >= 1) sb.AppendFormat("{0}h ", t.Hours);
            if (t.Minutes >= 1) sb.AppendFormat("{0}m ", t.Minutes);
            if (t.Seconds >= 1) sb.AppendFormat("{0}s ", t.Seconds);
            string display = sb.ToString();

            return string.IsNullOrWhiteSpace(display) ? "just now" : display + "ago";
        }
    }
}
