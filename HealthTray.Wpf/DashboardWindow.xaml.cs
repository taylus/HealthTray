using System;
using System.Linq;
using System.Windows;
using System.Net.Http;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthTray.Service;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    public partial class DashboardWindow : Window
    {
        const string refreshIntervalFormat = "Updated every {0} seconds";

        private AppConfig config;
        private DispatcherTimer refreshTimer;

        /// <summary>
        /// Settings page user control.
        /// </summary>
        public SettingsControl Settings { get; private set; }

        public IHealthTrayService Service { get; set; }

        /// <summary>
        /// Creates a dashboard window using the given healthchecks service.
        /// </summary>
        public DashboardWindow(IHealthTrayService service, AppConfig config)
        {
            Service = service;
            this.config = config;

            int refreshTimerSeconds = config.Get<int>("refresh-seconds");
            refreshTimer = ConfigureRefreshTimer(refreshTimerSeconds);
            refreshTimer.Start();

            Settings = new SettingsControl(config);
            InitializeComponent();
            dockPanel.Children.Add(Settings);
            UpdateRefreshTimerDisplay(refreshTimerSeconds);

            PreviewKeyDown += new KeyEventHandler(CloseOnEsc);
            PreviewKeyUp += new KeyEventHandler(RefreshOnF5);
            PreviewKeyUp += new KeyEventHandler(ShowDashboardOnF1);
            PreviewKeyUp += new KeyEventHandler(ShowSettingsOnF2);
#if DEBUG
            PreviewKeyUp += new KeyEventHandler(ShowTestNotificationOnF3);
#endif
        }

        /// <summary>
        /// Initializes and returns a dashboard refresh timer which fires every N seconds.
        /// </summary>
        private DispatcherTimer ConfigureRefreshTimer(int seconds)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(seconds);
            timer.Tick += async (sender, e) => await Refresh();
            return timer;
        }

        /// <summary>
        /// Updates the dashboard refresh timer to fire every N seconds.
        /// Reads the number of seconds from configuration if it is unspecified.
        /// </summary>
        public void UpdateRefreshTimer(int? seconds = null)
        {
            if (seconds == null) seconds = config.Get<int>("refresh-seconds");
            refreshTimer.Interval = TimeSpan.FromSeconds(seconds.Value);
            UpdateRefreshTimerDisplay(seconds.Value);
        }

        /// <summary>
        /// Updates the text displayed for how often the dashboard refreshes
        /// by reading it from configuration.
        /// </summary>
        private void UpdateRefreshTimerDisplay(int seconds)
        {
            RefreshIntervalDisplay.Content = string.Format(refreshIntervalFormat, seconds);
        }

        /// <summary>
        /// Initialize the dashboard with check data.
        /// </summary>
        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Settings.Visibility = Visibility.Collapsed;
            await Refresh();
        }

        /// <summary>
        /// Get fresh check data and repopulate the dashboard with it.
        /// </summary>
        public async Task Refresh()
        {
            CheckPanel.Children.Clear();
            //TODO: better to merge checks into existing controls based on ping url?

            try
            {
                var checks = await Service.GetChecks();
                foreach (var check in checks)
                {
                    CheckPanel.Children.Add(new CheckControl(check));
                }

                var overallStatus = DetermineOverallStatus(checks);
                Icon = CheckControl.GetIconFor(overallStatus);
                ((App)Application.Current).SetTaskbarIcon(Icon);
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show(httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Determine the overall status of the dashboard based on the given checks.
        /// E.g. it's up if everything is up, but down if anything is down, etc.
        /// </summary>
        /// <remarks>
        /// TODO: extract + unit test
        /// </remarks>
        private CheckStatus DetermineOverallStatus(IList<Check> checks)
        {
            if (checks.Count == 0) return CheckStatus.@new;
            if (checks.All(c => c.status == CheckStatus.up)) return CheckStatus.up;
            if (checks.Any(c => c.status == CheckStatus.down)) return CheckStatus.down;
            if (checks.Any(c => c.status == CheckStatus.late)) return CheckStatus.late;
            if (checks.Any(c => c.status == CheckStatus.paused)) return CheckStatus.paused;
            return CheckStatus.@new;
        }

        /// <summary>
        /// Keyboard handler: closes this window on pressing the escape key.
        /// </summary>
        private void CloseOnEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        /// <summary>
        /// Keyboard handler: refreshes the window on pressing F5.
        /// </summary>
        private async void RefreshOnF5(object sender, KeyEventArgs e)
        {
            if (CheckPanel.IsVisible && e.Key == Key.F5) await Refresh();
        }

        /// <summary>
        /// Keyboard handler: shows the dashboard on pressing F1.
        /// </summary>
        private void ShowDashboardOnF1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) new ShowDashboardCommand().Execute(null);
        }

        /// <summary>
        /// Keyboard handler: shows settings on pressing F2.
        /// </summary>
        private void ShowSettingsOnF2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2) new ShowSettingsCommand().Execute(null);
        }

        /// <summary>
        /// Keyboard handler: shows a test notification on pressing F3.
        /// </summary>
        private void ShowTestNotificationOnF3(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F3) return;

            var app = Application.Current as App;
            if (app == null) return;

            app.ShowBalloonNotification(new StatusChangePopup(Icon, "Test check"));
        }

        /// <summary>
        /// Intercept window closing and minimize to the system tray instead.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
#if !DEBUG
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
#endif
        }
    }
}
