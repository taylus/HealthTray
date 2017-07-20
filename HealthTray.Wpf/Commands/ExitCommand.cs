using System;
using System.Windows;
using System.Windows.Input;

namespace HealthTray.Wpf
{
    public class ExitCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
