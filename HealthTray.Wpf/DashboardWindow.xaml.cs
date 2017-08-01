using System;
using System.Windows;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using HealthTray.Service;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    /// <summary>
    /// Displays zero or more "checks", which are simple indications of whether
    /// a specific healthchecks.io URL has been visited recently.<para/>
    /// 
    /// Checks are retrieved through a <see cref="IHealthTrayService"/>, the
    /// standard implementation of which calls healthchecks.io's REST API.
    /// </summary>
    public partial class DashboardWindow : Window
    {
        const string refreshIntervalFormat = "Updated every {0} seconds";

        private AppConfig config;
        private DispatcherTimer refreshTimer;

        private CheckStatus lastOverallStatus = CheckStatus.@new;
        private CheckStatus currentOverallStatus = CheckStatus.@new;

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
            RefreshIntervalDisplay.Text = string.Format(refreshIntervalFormat, seconds);
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
            try
            {
                var checks = await Service.GetChecks();
                CheckPanel.Children.Clear();
                foreach (var check in checks)
                {
                    CheckPanel.Children.Add(new CheckControl(check));
                }

                SetOverallStatus(StatusCalculator.CalculateOverallStatusFrom(checks));
            }
            catch (HttpRequestException httpEx)
            {
                refreshTimer.Stop();
                SetOverallStatus(CheckStatus.@new);
                MessageBox.Show("Error refreshing dashboard: " + httpEx.Message, "HealthTray Error", MessageBoxButton.OK, MessageBoxImage.Error);
                refreshTimer.Start();
            }
        }

        /// <summary>
        /// Sets overall dashboard status to the given status.
        /// If it changed from the last status, the user will be informed via popup.
        /// </summary>
        private void SetOverallStatus(CheckStatus status)
        {
            lastOverallStatus = currentOverallStatus;
            currentOverallStatus = status;

            Icon = CheckControl.GetIconFor(status);
            ((App)Application.Current).SetTaskbarIcon(Icon);

            if (currentOverallStatus != lastOverallStatus)
            {
                var app = Application.Current as App;
                app.ShowBalloonNotification(new StatusChangePopup(Icon, "HealthTray status changed."));
            }
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

            app.ShowBalloonNotification(new StatusChangePopup(Icon, "Test popup"));
        }

        /// <summary>
        /// Click handler for the "refresh now" dashboard link.
        /// </summary>
        private async void RefreshNowLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            await Refresh();
            e.Handled = true;
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
