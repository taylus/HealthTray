using System.Windows;
using System.Windows.Media;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            tb = (TaskbarIcon)FindResource("TaskbarIcon"); //taskbar icon stays in the system tray until disposed/application exits

            var config = new AppConfig();
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

        public void SetTaskbarIcon(ImageSource icon)
        {
            tb.IconSource = icon;
        }
    }
}
