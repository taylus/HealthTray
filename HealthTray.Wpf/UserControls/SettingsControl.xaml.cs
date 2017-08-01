using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Security.Cryptography;
using HealthTray.Service;
using HealthTray.Security;

namespace HealthTray.Wpf
{
    /// <summary>
    /// Displays application settings and allows the user to edit them.
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private AppConfig config;

        public SettingsControl(AppConfig config)
        {
            this.config = config;
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        /// <summary>
        /// Clears the settings form of all inputs.
        /// </summary>
        public void Clear()
        {
            apiUrl.Clear();
            apiKey.Clear();
            refreshSeconds.Clear();
        }

        /// <summary>
        /// Loads settings from App.config and populates the form.
        /// </summary>
        public void LoadFromConfig()
        {
            try
            {
                string apiDocsUrl = config.Get<string>("healthchecks-api-docs");
                Uri apiDocsUri;
                if (Uri.TryCreate(apiDocsUrl, UriKind.Absolute, out apiDocsUri))
                {
                    apiDocsLink.NavigateUri = apiDocsUri;
                }

                apiUrl.Text = config.Get<string>("healthchecks-api-url");
                refreshSeconds.Text = config.Get<string>("refresh-seconds");

                string salt = config.Get<string>("healthtray-salt");
                if (!string.IsNullOrWhiteSpace(salt))
                {
                    apiKey.Text = Crypto.Decrypt(config.Get<string>("healthchecks-api-key"), salt);
                }
            }
            catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
            {
                MessageBox.Show("Something went wrong decrypting your API key. :(\n\nCould you please supply a new one?\n\nThe error was: " +
                    ex.GetType() + ": " + ex.Message, "HealthTray - Error Decrypting API Key", MessageBoxButton.OK, MessageBoxImage.Error);
                apiKey.Clear();
                apiKey.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves button click handler.
        /// </summary>
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Saves settings in the form to App.config.
        /// </summary>
        public void Save()
        {
            if (!Validate()) return;

            try
            {
                string salt = Crypto.GenerateSalt(32);
                config.Set("healthtray-salt", salt);
                config.Set("healthchecks-api-key", Crypto.Encrypt(apiKey.Text, salt));
                config.Set("healthchecks-api-url", apiUrl.Text);
                config.Set("refresh-seconds", refreshSeconds.Text);
                config.Save();

                var dashboard = Application.Current.MainWindow as DashboardWindow;
                if (dashboard != null) dashboard.Service = new HealthTrayService(apiUrl.Text, apiKey.Text);

                new ShowDashboardCommand().Execute(null);
                new RefreshDashboardCommand().Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Validates the form for saving, returning false if it is invalid.
        /// </summary>
        /// <remarks>
        /// TODO: Look into data binding + more sophisticated validation.
        /// </remarks>
        private bool Validate()
        {
            int refreshSecondsValue;
            if (!int.TryParse(refreshSeconds.Text, out refreshSecondsValue))
            {
                MessageBox.Show("Refresh seconds must be an integer.", "Error saving settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                refreshSeconds.Focus();
                refreshSeconds.SelectAll();
                return false;
            }

            return true;
        }
    }
}
