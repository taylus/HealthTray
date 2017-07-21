using System;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;

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

        /// <summary>
        /// Loads settings from App.config and populates the form.
        /// </summary>
        private void LoadSettings()
        {
            string apiDocsUrl = ConfigurationManager.AppSettings["healthchecks-api-docs"];
            Uri apiDocsUri;
            if (Uri.TryCreate(apiDocsUrl, UriKind.Absolute, out apiDocsUri))
            {
                apiDocsLink.NavigateUri = apiDocsUri;
            }

            apiUrl.Text = ConfigurationManager.AppSettings["healthchecks-api-url"];
            apiKey.Text = ConfigurationManager.AppSettings["healthchecks-api-key"];
            refreshSeconds.Text = ConfigurationManager.AppSettings["refresh-seconds"];
        }

        /// <summary>
        /// Saves settings in the form to App.config.
        /// </summary>
        private void SaveSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //TODO: validation
            //refresh seconds must be a positive integer # w/ some lower limit (30 seconds?)

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                SetAppSetting(config, "healthchecks-api-url", apiUrl.Text);
                SetAppSetting(config, "healthchecks-api-key", apiKey.Text);
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
    }
}
