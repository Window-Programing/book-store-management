using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for ConfigPageSize.xaml
    /// </summary>
    public partial class ConfigPageSize : Window
    {
        private int rs = -1;
        public ConfigPageSize()
        {
            InitializeComponent();
        }

        private void Apply_Btn_Click(object sender, RoutedEventArgs e)
        {
            bool ok = true;
            try
            {
                rs = Convert.ToInt32(TextBox_PerPage.Text);
            }
            catch (Exception ex)
            {
                ok = false;

                Application curApp = Application.Current;
                Window window = curApp.MainWindow;
                MessageBox.Show(window, "Vui lòng nhập vào giá trị là số nguyên", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (ok)
            {
                DialogResult = true;
            }
            
        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal int getResult()
        {
            return rs;
        }
    }
}
