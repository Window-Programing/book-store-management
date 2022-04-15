using Management_Book.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for InputNewProduct.xaml
    /// </summary>
    public partial class InputNewProduct : Window
    {
        public InputNewProduct()
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

        public MyShopModel.Product getNewProduct()
        {
            if (DialogResult == true)
            {
                MyShopModel.Category cat = Combobox_category.SelectedItem as MyShopModel.Category;
                int category_id = cat.Id;

                return new MyShopModel.Product()
                {
                    Name = TextBox_Name.Text.ToString(),
                    Price = (float)Convert.ToDouble(TextBox_Price.Text),
                    Cost = (float)Convert.ToDouble(TextBox_Cost.Text),
                    Quantity = Convert.ToInt32(TextBox_Quantity.Text),
                    Image = TextBox_Image.Text.ToString(),
                    Category = new MyShopModel.Category() { Id = category_id }
                };
            }
            else
            {
                return null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyShopEntities.getInstance().openConnection();
            Combobox_category.ItemsSource = MyShopEntities.getInstance().getCategories();
            MyShopEntities.getInstance().closeConnection();
        }
    }
}
