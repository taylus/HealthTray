using System;
using System.Windows;
using System.Windows.Input;

namespace HealthTray.Wpf
{
    public class ShowDashboardCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            var dashboard = Application.Current.MainWindow as DashboardWindow;
            if (dashboard != null)
            {
                dashboard.Settings.Visibility = Visibility.Collapsed;
                dashboard.CheckPanel.Visibility = Visibility.Visible;
                dashboard.RefreshDisplay.Visibility = Visibility.Visible;
                dashboard.UpdateRefreshTimer();
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
