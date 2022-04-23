using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book
{
    internal class AppConfig
    {
        public static string Server = "Server";
        public static string Database = "Database";
        public static string Username = "Username";
        public static string Password = "Password";
        public static string Entropy = "Entropy";
        public static string AutoLogin = "AutoLogin";
        public static string PageSize = "PageSize";
        public static string Tab = "Tab";
        public static string PageRibbon = "PageRibbon";
        
        public static string getValue(string key)
        {
            ConfigurationManager.RefreshSection("appSettings");
            string value = ConfigurationManager.AppSettings[key];
            return value;
        }
        
        public static void setValue(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            settings[key].Value = value;
            configFile.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
