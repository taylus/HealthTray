using System;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Security.Cryptography;
using HealthTray.Security;

namespace HealthTray.Wpf
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
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
                Configuration config = GetConfig();
                string apiDocsUrl = config.AppSettings.Settings["healthchecks-api-docs"].Value;
                Uri apiDocsUri;
                if (Uri.TryCreate(apiDocsUrl, UriKind.Absolute, out apiDocsUri))
                {
                    apiDocsLink.NavigateUri = apiDocsUri;
                }

                apiUrl.Text = config.AppSettings.Settings["healthchecks-api-url"].Value;
                refreshSeconds.Text = config.AppSettings.Settings["refresh-seconds"].Value;

                string salt = config.AppSettings.Settings["healthtray-salt"].Value;
                apiKey.Text = Crypto.Decrypt(config.AppSettings.Settings["healthchecks-api-key"].Value, salt);
            }
            catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
            {
                MessageBox.Show("Something went wrong decrypting your API key. :(\n\nPlease supply a new one.\n\nThe error was: "
                    + ex.Message, "Error decrypting API key", MessageBoxButton.OK, MessageBoxImage.Error);
                apiKey.Clear();
                apiKey.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves settings in the form to App.config.
        /// </summary>
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //TODO: validation
            //refresh seconds must be a positive integer # w/ some lower limit (30 seconds?)

            try
            {
                Configuration config = GetConfig();
                string salt = Crypto.GenerateSalt(32);
                SetAppSetting(config, "healthtray-salt", salt);
                SetAppSetting(config, "healthchecks-api-key", Crypto.Encrypt(apiKey.Text, salt));
                SetAppSetting(config, "healthchecks-api-url", apiUrl.Text);
                SetAppSetting(config, "refresh-seconds", refreshSeconds.Text);
                config.Save();

                new ShowDashboardCommand().Execute(null);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sets a specified AppSetting key-value pair. Throws a <see cref="KeyNotFoundException"/>
        /// if the key does not exist in the configuration's AppSettings.
        /// </summary>
        private static void SetAppSetting(Configuration config, string key, string value)
        {
            var setting = config.AppSettings.Settings[key];
            if (setting == null) throw new KeyNotFoundException(string.Format("AppSetting \"{0}\" was not found in configuration.", key));
            setting.Value = value;
        }

        /// <summary>
        /// Returns the configuration file for this application.
        /// </summary>
        private static Configuration GetConfig()
        {
            return ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }
    }
}
