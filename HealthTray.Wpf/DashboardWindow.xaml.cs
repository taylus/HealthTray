using System;
using System.Linq;
using System.Windows;
using System.Net.Http;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthTray.Service;
using HealthTray.Service.Model;

namespace HealthTray.Wpf
{
    public partial class DashboardWindow : Window
    {
        private IHealthTrayService service;

        /// <summary>
        /// Creates a dashboard window using the given healthchecks service.
        /// </summary>
        public DashboardWindow(IHealthTrayService service)
        {
            this.service = service;

            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(CloseOnEsc);
            PreviewKeyUp += new KeyEventHandler(RefreshOnF5);
        }

        /// <summary>
        /// Initialize the dashboard with check data.
        /// </summary>
        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await Refresh();

            /*
            for (int i = 0; i < 4; i++)
            {
                CheckPanel.Children.Add(new CheckControl());
            }
            */
        }

        /// <summary>
        /// Get fresh check data and repopulate the dashboard with it.
        /// </summary>
        public async Task Refresh()
        {
            CheckPanel.Children.Clear();
            //TODO: better to merge checks into existing controls based on ping url?

            try
            {
                var checks = await service.GetChecks();
                foreach (var check in checks)
                {
                    CheckPanel.Children.Add(new CheckControl(check));
                }

                var overallStatus = DetermineOverallStatus(checks);
                Icon = CheckControl.GetIconFor(overallStatus);
                ((App)Application.Current).SetTaskbarIcon(Icon);
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show(httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Determine the overall status of the dashboard based on the given checks.
        /// E.g. it's up if everything is up, but down if anything is down, etc.
        /// </summary>
        /// <remarks>
        /// TODO: extract + unit test
        /// </remarks>
        private CheckStatus DetermineOverallStatus(IList<Check> checks)
        {
            if (checks.All(c => c.status == CheckStatus.up)) return CheckStatus.up;
            if (checks.Any(c => c.status == CheckStatus.down)) return CheckStatus.down;
            if (checks.Any(c => c.status == CheckStatus.late)) return CheckStatus.late;
            if (checks.Any(c => c.status == CheckStatus.paused)) return CheckStatus.paused;
            return CheckStatus.@new;
        }

        /// <summary>
        /// Keyboard handler: closes this window on pressing the escape key.
        /// </summary>
        private void CloseOnEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        /// <summary>
        /// Keyboard handler: refreshes the window on pressing F5.
        /// </summary>
        private async void RefreshOnF5(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) await Refresh();
        }

        /// <summary>
        /// Intercept window closing and minimize to the system tray instead.
        /// </summary>
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
