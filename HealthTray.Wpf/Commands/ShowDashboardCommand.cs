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
            var window = Application.Current.MainWindow;
            window.Show();
            window.Focus();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
