using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Hardcodet.Wpf.TaskbarNotification;
using HealthTray.Service;
using HealthTray.Security;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    /// <summary>
    /// Represents the overall HealthTray application. Mainly responsible for
    /// initial startup and system tray icon (<see cref="TaskbarIcon"/>) management.
    /// </summary>
    /// <remarks>
    /// Big thanks for Philipp Sumi for developing the WPF-based NotifyIcon package!
    /// http://www.hardcodet.net/wpf-notifyicon
    /// </remarks>
    public partial class App : Application
    {
        private TaskbarIcon tb;

        /// <summary>
        /// Creates a new instance of this application.
        /// </summary>
        public App() : base()
        {
            Dispatcher.UnhandledException += UnhandledExceptionHandler;
        }

        /// <summary>
        /// Application startup. Put global initialization logic here.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            tb = (TaskbarIcon)FindResource("TaskbarIcon"); //taskbar icon stays in the system tray until disposed/application exits

            var config = new AppConfig();
            var useMockData = config.Get<bool>("use-mock-data");
            if (useMockData)
            {
                StartupWithMockService(config);
            }
            else
            {
                StartupWithLiveService(config);
            }
        }

        /// <summary>
        /// Start up the application using a fake Healthchecks service (for testing).
        /// </summary>
        private void StartupWithMockService(AppConfig config)
        {
            var service = new FileBasedHealthTrayService(@"Testing\checks.json");
            var dashboard = new DashboardWindow(service, config);
            dashboard.Show();
        }

        /// <summary>
        /// Start up the application using the live Healthchecks service (for release).
        /// </summary>
        private void StartupWithLiveService(AppConfig config)
        {
            var apiUrl = config.Get<string>("healthchecks-api-url");
            var apiKeySalt = config.Get<string>("healthtray-salt");

            IHealthTrayService service = null;
            bool goToSettings = false;
            if (string.IsNullOrWhiteSpace(apiUrl) || string.IsNullOrWhiteSpace(apiKeySalt))
            {
                service = new StubHealthTrayService(new List<Check>());
                if (MessageBox.Show("It looks like you haven't set an API URL and/or key yet. Enter one now?", "First run?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    goToSettings = true;
                }
            }
            else
            {
                try
                {
                    var apiKey = Crypto.Decrypt(config.Get<string>("healthchecks-api-key"), apiKeySalt);
                    service = new HealthTrayService(apiUrl, apiKey);
                }
                catch
                {
                    service = new StubHealthTrayService(new List<Check>());
                    goToSettings = true; //settings page should inform user about the decryption error
                }
            }

            var dashboard = new DashboardWindow(service, config);
            dashboard.Show();

            if (goToSettings) new ShowSettingsCommand().Execute(null);
        }

        /// <summary>
        /// Sets the taskbar icon to the given image.
        /// </summary>
        public void SetTaskbarIcon(ImageSource icon)
        {
            tb.IconSource = icon;
        }

        /// <summary>
        /// Shows a custom balloon popup/notification on the taskbar icon.
        /// Used to alert the user e.g. "your check status just changed."
        /// </summary>
        public void ShowBalloonNotification(UIElement popup, PopupAnimation animation = PopupAnimation.Slide, int? timeoutMilliseconds = 4000)
        {
            tb.ShowCustomBalloon(popup, animation, timeoutMilliseconds);
        }

        /// <summary>
        /// A global error handler for any exceptions that aren't otherwise handled.
        /// </summary>
        private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMsg = string.Format("Whoops. An unexpected error occurred and HealthTray must close.\n" +
                "The error was: {0}: {1}", e.Exception.GetType(), e.Exception.Message);
            MessageBox.Show(errorMsg, "HealthTray Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Environment.Exit(-1);
        }
    }
}
