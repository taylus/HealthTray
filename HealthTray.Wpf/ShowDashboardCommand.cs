using System;
using System.Windows;
using System.Windows.Input;

namespace HealthTray.Wpf
{
    public class ShowDashboardCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { throw new NotSupportedException(); } remove { } }

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.Show();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
