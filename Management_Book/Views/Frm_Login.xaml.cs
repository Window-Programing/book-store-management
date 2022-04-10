using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
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
        bool IsDone = false;
        public Frm_Login()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {

            if (TxtUsername.Text == string.Empty)
            {
                TxtUsername.SelectAll();
                return;
            }
            if (TxtPassword.Text == string.Empty)
            {
                TxtPassword.SelectAll();
                return;
            }
            if (TxtUsername.Text != string.Empty && TxtPassword.Text != string.Empty && TxtUsername.Text == "admin" && TxtPassword.Text == "123")
            {
                IsDone = true;
            }
            else
                IsDone = false;
            if (!IsDone)
            {
                MessageBox.Show("Error in your information", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        public static bool IsActive()
        {
            Frm_Login login = new Frm_Login();
            login.ShowDialog();
            return login.IsDone ? true : false;
        }
    }
}
