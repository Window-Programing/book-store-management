using DevExpress.Xpf.Core;
using Management_Book.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Windows.Shapes;


namespace Management_Book.Views
{
    /// <summary>
    /// Interaction logic for Frm_Login.xaml
    /// </summary>
    public partial class Frm_Login : ThemedWindow
    {
        int initial = 1;
        public Frm_Login()
        {
            InitializeComponent();
        }

        public void setInitial(int status)
        {
            this.initial = status;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {

            if (TxtUsername.Text == "" || TxtPassword.Password == "")
            {
                MessageBox.Show("Input Username and Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                UserEntities.getInstance().openConnection();

                UserModel.Account account = UserEntities.getInstance().getUser(TxtUsername.Text);

                if (account.UserName != null)
                {
                    string cypherText = account.Password;
                    var cypherTextInBytes = Convert.FromBase64String(cypherText);

                    string entropyText = account.Entropy;
                    var entropyTextInBytes = Convert.FromBase64String(entropyText);

                    var passwordInBytes = ProtectedData.Unprotect(cypherTextInBytes, entropyTextInBytes, DataProtectionScope.CurrentUser);
                    string password = Encoding.UTF8.GetString(passwordInBytes);

                    if (TxtPassword.Password == password)
                    {
                        var server = AppConfig.getValue(AppConfig.Server);
                        var database = AppConfig.getValue(AppConfig.Database);

                        string connectionString =
                            $"Server={server};Database={database};User Id={account.UserName};Password={password};MultipleActiveResultSets=true;";

                        SqlConnection connection = new SqlConnection(connectionString);
                        try
                        {
                            connection.Open();

                            AppConfig.setValue(AppConfig.Username, account.UserName);
                            AppConfig.setValue(AppConfig.Password, account.Password);
                            AppConfig.setValue(AppConfig.Entropy, account.Entropy);
                            if (checkLogin.IsChecked == true)
                            {
                                AppConfig.setValue(AppConfig.AutoLogin, "1");
                            }
                            else if (checkLogin.IsChecked == false)
                            {
                                AppConfig.setValue(AppConfig.AutoLogin, "0");
                            }

                            MainWindow dashboard = new MainWindow();
                            dashboard.Show();
                            this.Close();
                        } catch(Exception ex)
                        {
                            MessageBox.Show("Your account cannot accesss database, Please contact with your admin and register a new account !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }  
                    }
                }
                else
                {
                    MessageBox.Show("Username is not exists in system, please register", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private void Btn_Regis(object sender, RoutedEventArgs e)
        {
            Frm_Regis regis = new Frm_Regis();
            regis.Show();
            this.Close();
        }

        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (AppConfig.getValue(AppConfig.AutoLogin) == "1")
            {
                checkLogin.IsChecked = true;
            }
            else
            {
                checkLogin.IsChecked = false;
            }

            if (AppConfig.getValue(AppConfig.AutoLogin) == "1" && initial == 1)
            {
                var server = AppConfig.getValue(AppConfig.Server);
                var database = AppConfig.getValue(AppConfig.Database);
                var username = AppConfig.getValue(AppConfig.Username);
                var password = AppConfig.getValue(AppConfig.Password);
                string cypherText = password;
                var cypherTextInBytes = Convert.FromBase64String(cypherText);

                string entropyText = AppConfig.getValue(AppConfig.Entropy);
                var entropyTextInBytes = Convert.FromBase64String(entropyText);

                var passwordInBytes = ProtectedData.Unprotect(cypherTextInBytes, entropyTextInBytes, DataProtectionScope.CurrentUser);
                string realPassword = Encoding.UTF8.GetString(passwordInBytes);
                string connectionString =
                    $"Server={server};Database={database};User Id={username};Password={realPassword};";

                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();

                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Your account cannot accesss database, Please contact with your admin and register a new account !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } 
     
        }
    }
}