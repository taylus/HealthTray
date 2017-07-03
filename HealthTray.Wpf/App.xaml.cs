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

        private void TaskbarIcon_Menu_OpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            Current.MainWindow.Show();
        }

        private async void TaskbarIcon_Menu_RefreshDashboard_Click(object sender, RoutedEventArgs e)
        {
            var dashboard = Current.MainWindow as DashboardWindow;
            if (dashboard != null) await dashboard.Refresh();
        }

        private void TaskbarIcon_Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        public void SetTaskbarIcon(ImageSource icon)
        {
            tb.IconSource = icon;
        }
    }
}
