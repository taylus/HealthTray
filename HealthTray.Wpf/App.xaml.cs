using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Hardcodet.Wpf.TaskbarNotification;
using HealthTray.Service;
using HealthTray.Security;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    public partial class App : Application
    {
        private TaskbarIcon tb;

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
            var service = new StubHealthTrayService(new List<Check>
            {
                new Check()
                {
                    name = "A test check",
                    last_ping = DateTime.Now.AddMinutes(-1.5)
                },
                new Check()
                {
                    name = "Another test check",
                    last_ping = DateTime.Now
                },
                new Check()
                {
                    name = "Yet another test check",
                    last_ping = DateTime.Now.AddHours(-30.5),
                    status = CheckStatus.late
                },
                new Check()
                {
                    name = "Oh no it's down",
                    last_ping = DateTime.Now.AddDays(-10),
                    status = CheckStatus.down
                },
            });

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
                if (MessageBox.Show("It looks like you haven't set an API key yet. Enter one now?", "First run?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    goToSettings = true;
                }
            }
            else
            {
                var apiKey = Crypto.Decrypt(config.Get<string>("healthchecks-api-key"), apiKeySalt);
                service = new HealthTrayService(apiUrl, apiKey);
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
    }
}
