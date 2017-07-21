using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using Hardcodet.Wpf.TaskbarNotification;
using HealthTray.Service;
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

            //TODO: use real API, read API key and URL from config, and make a settings window for the user to specify them
            //var service = new HealthTrayService("https://healthchecks.io/api/v1/", "");
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
            var dashboard = new DashboardWindow(service, new AppConfig());
            dashboard.Show();
        }

        public void SetTaskbarIcon(ImageSource icon)
        {
            tb.IconSource = icon;
        }
    }
}
