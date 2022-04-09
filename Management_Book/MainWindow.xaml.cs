using DevExpress.Xpf.Core;
using Management_Book.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_Book
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void textEditor_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void import_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void add_category_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void delete_category_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void add_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void update_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void delete_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        public class TabDataItem
        {
            public string HeaderText { get; set; }
            public UserControl Content { get; set; }
        }

        class AppConfig
        {
            public static string Server = "Server";
            public static string Database = "Database";
            public static string Username = "Username";
            public static string Password = "Password";
            public static string Entropy = "Entropy";
            public static string GetValue(string key)
            {
                string value = ConfigurationManager.AppSettings[key];
                return value;
            }

            public static void SetValue(string key, string value)
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
                string server = AppConfig.GetValue(AppConfig.Server);
                string database = AppConfig.GetValue(AppConfig.Database);
                string username = AppConfig.GetValue(AppConfig.Username);
                string password = AppConfig.GetValue(AppConfig.Password);

                builder.DataSource = $"{server}";
                builder.InitialCatalog = database;
                builder.IntegratedSecurity = true;
                builder.ConnectTimeout = 6;

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
                }

                return result;
            }

            public void Connect()
            {
                _connection.Open();
            }
        }
        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var screen = new ObservableCollection<DXTabItem>()
            {
                new DXTabItem{Content = new MasterDataUserControl(), Header = "MasterData"},
                new DXTabItem{Content = new SaleUserControl(), Header = "Sale"},
                new DXTabItem{Content = new OrderUserControl(), Header = "Order"}
            };

            dXTabControl1.ItemsSource = screen;

            //Kết nối db
            //string connectionString = AppConfig.ConnectionString();
            //var dao = new SqlDataAccess(connectionString);

            //if (dao.CanConnect())
            //{
            //    dao.Connect();
            //    Title = "Connected";
            //    MessageBox.Show("Connected");
            //}
            //else
            //{
            //    MessageBox.Show("Cannot connect to database");
            //}

            //Đọc password
            var cypherText = AppConfig.GetValue(AppConfig.Password);
            var cypherTextInBytes = Convert.FromBase64String(cypherText);

            var entropyText = AppConfig.GetValue(AppConfig.Entropy);
            var entropyInBytes = Convert.FromBase64String(entropyText);

            var passwordInBytes = ProtectedData.Unprotect(cypherTextInBytes, entropyInBytes, DataProtectionScope.CurrentUser);
            var password = Encoding.UTF8.GetString(passwordInBytes);
            MessageBox.Show(password);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string password = "012345";

            var passwordInBytes = Encoding.UTF8.GetBytes(password);

            var entropy = new byte[20];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            var entropyBase64 = Convert.ToBase64String(entropy);

            var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);

            var cypherTextBase64 = Convert.ToBase64String(cypherText);

            AppConfig.SetValue(AppConfig.Password, cypherTextBase64);
            AppConfig.SetValue(AppConfig.Entropy, entropyBase64);
        }
    }
}
