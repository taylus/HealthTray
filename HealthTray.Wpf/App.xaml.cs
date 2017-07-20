using System.Windows;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using HealthTray.Service;

namespace HealthTray.Wpf
{
    public partial class App : Application
    {
        private TaskbarIcon tb;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            tb = (TaskbarIcon)FindResource("TaskbarIcon"); //taskbar icon stays in the system tray until disposed/application exits

            //TODO: read API key and URL from config + make a settings window
            var service = new HealthTrayService("https://healthchecks.io/api/v1/", "");
            var dashboard = new DashboardWindow(service);
            dashboard.Show();
        }

        public void SetTaskbarIcon(ImageSource icon)
        {
            tb.IconSource = icon;
        }
    }
}
