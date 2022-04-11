using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace Management_Book.Views
{
    /// <summary>
    /// Interaction logic for Frm_Regis.xaml
    /// </summary>
    public partial class Frm_Regis : ThemedWindow
    {
        public Frm_Regis()
        {
            InitializeComponent();
        }

        class AppConfig
        {
            public static string Server = "Server";
            public static string Instance = "Instance";
            public static string Database = "Database";
            public static string getValue(string key)
            {
                string value = (string)ConfigurationManager.AppSettings[key];
                return value;
            }
            public static void setValue(string key, string value)
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                settings[key].Value = value;

                configFile.Save(ConfigurationSaveMode.Minimal);
            }
            public static string ConnectionString()
            {
                string result = "";

                var builder = new SqlConnectionStringBuilder();
                string server = AppConfig.getValue(AppConfig.Server);
                string instance = AppConfig.getValue(AppConfig.Instance);
                string database = AppConfig.getValue(AppConfig.Database);

                //builder.DataSource = $"{server}\\{instance}";
                builder.DataSource = $"{instance}";
                builder.InitialCatalog = database;
                builder.IntegratedSecurity = true;
                builder.ConnectTimeout = 5;

                result = builder.ToString();
                return result;
            }
        }

        private void Btn_Cancel(object sender, RoutedEventArgs e)
        {
            Frm_Login regis = new Frm_Login();
            regis.Show();
            this.Close();
        }

        private void Btn_OK(object sender, RoutedEventArgs e)
        {
            if(TxtPassword.Password == TxtConfirmPassword.Password) 
            { 
                string connectionString = AppConfig.ConnectionString();
                SqlConnection sqlCon = new SqlConnection(connectionString);
                try
                {
                    var passwordInBytes = Encoding.UTF8.GetBytes(TxtPassword.Password);
                    var entropy = new byte[20];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(entropy);
                    }
                    var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);

                    var entropyBase64 = Convert.ToBase64String(entropy);
                    var cypherTextBase64 = Convert.ToBase64String(cypherText);


                    if (sqlCon.State == ConnectionState.Closed)
                        sqlCon.Open();
                    String query = "INSERT INTO [UserInformation](Username, Password, Entropy) VALUES (@Username,@Password,@Entropy)";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@Username", TxtUsername.Text);
                    sqlCmd.Parameters.AddWithValue("@Password", cypherTextBase64);
                    sqlCmd.Parameters.AddWithValue("@Entropy", entropyBase64);
                    int count = sqlCmd.ExecuteNonQuery();
                    if (count == 1)
                    {
                        MessageBox.Show(TxtUsername.Text + " - " + cypherTextBase64.Length + " - " + entropyBase64.Length);
                        Frm_Login login = new Frm_Login();
                        login.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Register failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlCon.Close();
                }
            }
            else
                MessageBox.Show("Confirmation password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
