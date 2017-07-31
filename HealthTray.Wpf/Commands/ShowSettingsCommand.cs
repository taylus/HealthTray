using System;
using System.Windows;
using System.Windows.Input;

namespace HealthTray.Wpf
{
    public class ShowSettingsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            var dashboard = Application.Current.MainWindow as DashboardWindow;
            if (dashboard != null)
            {
                dashboard.Settings.Visibility = Visibility.Visible;
                dashboard.CheckPanel.Visibility = Visibility.Collapsed;
                dashboard.RefreshIntervalDisplay.Visibility = Visibility.Collapsed;
                dashboard.Settings.Clear();
                dashboard.Settings.LoadFromConfig();
                dashboard.Show();
                dashboard.Activate();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
