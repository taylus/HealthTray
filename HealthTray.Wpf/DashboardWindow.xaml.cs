using System.Windows;
using System.ComponentModel;

namespace HealthTray.Wpf
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            for(int i = 0; i < 4; i++)
            {
                CheckPanel.Children.Add(new CheckControl());
            }
        }

        //minimize to system tray when closed
        protected override void OnClosing(CancelEventArgs e)
        {
#if !DEBUG
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
#endif
        }
    }
}
