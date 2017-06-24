using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace HealthTray
{
    public partial class App : Application
    {
        private TaskbarIcon tb;

        protected override void OnStartup(StartupEventArgs e)
        {
            tb = (TaskbarIcon)FindResource("TaskbarIcon");
            //taskbar icon stays in the system tray until disposed/application exits
        }

        private void TaskbarIcon_Menu_OpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            Current.MainWindow.Show();
        }

        private void TaskbarIcon_Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
