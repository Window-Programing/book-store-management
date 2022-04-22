using DevExpress.Xpf.Core;
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
        public Frm_Login()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(connectionString);
            try
            {

                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                String query = "SELECT * FROM UserInformation WHERE Username=@Username";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@Username", TxtUsername.Text);
                SqlDataReader dr = sqlCmd.ExecuteReader();
                string cypherText = "";
                string entropyText = "";
                if (dr.Read())
                {
                    cypherText = dr.GetString(2);
                    entropyText = dr.GetString(3);
                }

                var cypherTextInBytes = Convert.FromBase64String(cypherText);
                var entropyTextInBytes = Convert.FromBase64String(entropyText);
                var passwordInBytes = ProtectedData.Unprotect(cypherTextInBytes, entropyTextInBytes, DataProtectionScope.CurrentUser);
                string sourcePassword = Encoding.UTF8.GetString(passwordInBytes);
                if ( TxtUsername.Text == "")
                {
                    MessageBox.Show("Input Username and Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (TxtPassword.Password == sourcePassword)
                {
                    MessageBox.Show(sourcePassword + " - " + TxtUsername.Text);
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