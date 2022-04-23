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
using Management_Book.Model;

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

        private void Btn_Cancel(object sender, RoutedEventArgs e)
        {
            Frm_Login loginForm = new Frm_Login();
            loginForm.setInitial(0);
            loginForm.Show();
            this.Close();
        }

        private void Btn_OK(object sender, RoutedEventArgs e)
        {
            if(TxtPassword.Password == TxtConfirmPassword.Password) 
            {
                string username = TxtUsername.Text;
                string password = TxtPassword.Password;

                UserEntities.getInstance().openConnection();

                UserModel.Account acc = UserEntities.getInstance().getUser(username);

                if(acc.UserName == null)
                {
                    var passwordInBytes = Encoding.UTF8.GetBytes(password);
                    var entropy = new byte[20];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(entropy);
                    }

                    var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);
                    var cypherTextBase64 = Convert.ToBase64String(cypherText);
                    var entropyBase64 = Convert.ToBase64String(entropy);

                    UserEntities.getInstance().insertUser(username, cypherTextBase64, entropyBase64);

                    MessageBox.Show("Register Suscess", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Frm_Login loginForm = new Frm_Login();
                    loginForm.setInitial(0);
                    loginForm.Show();
                    this.Close();
                } else
                {
                    MessageBox.Show("Username is exist in system !!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Confirmation password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
