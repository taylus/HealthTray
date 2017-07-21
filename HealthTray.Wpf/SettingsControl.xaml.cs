using System;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HealthTray.Wpf
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void LoadSettings()
        {
            string apiDocsUrl = ConfigurationManager.AppSettings["healthchecks-api-docs"];
            Uri apiDocsUri;
            if(Uri.TryCreate(apiDocsUrl, UriKind.Absolute, out apiDocsUri))
            {
                apiDocsLink.NavigateUri = apiDocsUri;
            }

            apiUrl.Text = ConfigurationManager.AppSettings["healthchecks-api-url"];
            apiKey.Text = ConfigurationManager.AppSettings["healthchecks-api-key"];
            refreshSeconds.Text = ConfigurationManager.AppSettings["refresh-seconds"];
        }
    }
}
