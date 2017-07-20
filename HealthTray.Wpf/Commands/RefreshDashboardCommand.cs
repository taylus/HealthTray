using System;
using System.Windows;
using System.Windows.Input;

namespace HealthTray.Wpf
{
    public class RefreshDashboardCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public async void Execute(object parameter)
        {
            var dashboard = Application.Current.MainWindow as DashboardWindow;
            if (dashboard != null) await dashboard.Refresh();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
