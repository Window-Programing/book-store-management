using Management_Book.Model;
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
    /// Interaction logic for InputNewCategory.xaml
    /// </summary>
    public partial class InputNewCategory : Window
    {
        public InputNewCategory()
        {
            InitializeComponent();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        internal MyShopModel.Category getNewCategory()
        {
            return new MyShopModel.Category()
            {
                Name = TextBox_Name.Text
            };
        }
    }
}
