using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Management_Book.Views
{
    /// <summary>
    /// Interaction logic for Frm_Login.xaml
    /// </summary>
    public partial class Frm_Login : ThemedWindow
    {
        public Frm_Login()
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


        class SqlDataAccess
        {
            private SqlConnection _connection;
            public SqlDataAccess(string connectionString)
            {
                _connection = new SqlConnection(connectionString);
            }

            public bool CanConnect()
            {
                bool result = true;

                try
                {
                    _connection.Open();
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    result = false;
                    MessageBox.Show(ex.Message);
                }

                return result;
            }

            public void Connect()
            {
                _connection.Open();
            }
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = AppConfig.ConnectionString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                String query = "SELECT COUNT(1) FROM UserInformation WHERE Username=@Username AND Password=@Password";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@Username", TxtUsername.Text);
                sqlCmd.Parameters.AddWithValue("@Password", TxtPassword.Password);
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count == 1)
                {
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Btn_Regis(object sender, RoutedEventArgs e)
        {
            Frm_Regis regis = new Frm_Regis();
            regis.Show();
            this.Close();
        }
    }
}