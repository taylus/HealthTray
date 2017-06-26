using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace HealthTray.Wpf
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(CloseOnEsc);

            for(int i = 0; i < 4; i++)
            {
                CheckPanel.Children.Add(new CheckControl());
            }
        }

        private void CloseOnEsc(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape) Close();
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
