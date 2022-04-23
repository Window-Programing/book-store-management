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
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

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

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                var filename = screen.FileName;

                // Lấy thư mục hiện hành
                var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                var imgSubFolder = exeFolder + "Images";

                // Tạo thư mục Images để chứa ảnh
                if (!Directory.Exists(imgSubFolder))
                {
                    Directory.CreateDirectory(imgSubFolder);
                }
        
                var sourceInfo = new FileInfo(filename);
                var extension = sourceInfo.Extension; // Trích xuất phần đuôi
                var newName = Guid.NewGuid() + extension; // Tự phát sinh id duy nhất toàn hệ thống
                var destination = $"{imgSubFolder}\\{newName}";

                File.Copy(filename, destination);

                // Cập nhật tên mới để lưu vào CSDL
                TextBox_Image.Text = newName;

            }
        }
    }
}
