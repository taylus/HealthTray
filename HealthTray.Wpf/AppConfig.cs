using System;
using System.Reflection;
using System.Configuration;

namespace HealthTray.Wpf
{
    /// <summary>
    /// Centralizes App.config I/O.
    /// This application writes settings to its App.config file at runtime.
    /// </summary>
    public class AppConfig
    {
        public Configuration Configuration { get; private set; }

        public AppConfig() : this(GetExeConfig()) { }

        public AppConfig(Configuration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Adds or updates the given key-value pair.
        /// </summary>
        public void Set(string key, string value)
        {
            var setting = Configuration.AppSettings.Settings[key];
            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {

                Configuration.AppSettings.Settings.Add(key, value);
            }
        }

        /// <summary>
        /// Returns the specified key's value.
        /// Returns the type's default value if the key does not exist.
        /// </summary>
        public T Get<T>(string key)
        {
            string value = Configuration.AppSettings.Settings[key]?.Value;
            if (string.IsNullOrWhiteSpace(value)) return default(T);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Writes settings out to the current configuration file.
        /// </summary>
        public void Save()
        {
            Configuration.Save();
        }

        /// <summary>
        /// Returns the configuration file for this application.
        /// </summary>
        private static Configuration GetExeConfig()
        {
            return ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }
    }
}
