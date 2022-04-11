using Aspose.Cells;
using DevExpress.Xpf.Core;
using Management_Book.Model;
using Management_Book.UserControls;

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Management_Book.Views;
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
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                try
                {

                    var filename = screen.FileName;
                    var workbook = new Workbook(filename);

                    Debug.WriteLine(filename);
                    var categorySheet = workbook.Worksheets[0];

                    Dictionary<string, int> categoriesDictionary = new Dictionary<string, int>();
                    var categories = new List<MyShopModel.Category>();

                    MyShopEntities db = MyShopEntities.getInstance();
                    db.openConnection();
                    db.truncateTable(MyShopEntities.ProductTable);
                    db.truncateTable(MyShopEntities.CategoryTable);

                    var row = 2;
                    var cell = categorySheet.Cells[$"B{row}"];
                    do
                    {
                        var name = cell.StringValue;
                        var category = new MyShopModel.Category() { Name = name };

                        int id = db.insertCategory(category);
                        category.Id = id;

                        categories.Add(category);
                        categoriesDictionary.Add(category.Name, category.Id);

                        cell = categorySheet.Cells[$"B{++row}"];
                    } while (cell.Value != null);

                    var productSheet = workbook.Worksheets[1];
                    row = 2;
                    cell = productSheet.Cells[$"B{row}"];
                    var productCount = 0;
                    do
                    {
                        string categoryName = cell.StringValue;
                        string name = productSheet.Cells[$"C{row}"].StringValue;
                        int price = productSheet.Cells[$"D{row}"].IntValue;
                        int quantity = productSheet.Cells[$"E{row}"].IntValue;
                        string image = productSheet.Cells[$"F{row}"].StringValue;

                        var product = new MyShopModel.Product()
                        {
                            Name = name,
                            Price = price,
                            Quantity = quantity,
                            Image = image,
                            Category = new MyShopModel.Category() { Id = categoriesDictionary[categoryName], Name = categoryName }
                        };

                        db.insertProduct(product);

                        productCount++;
                        cell = productSheet.Cells[$"B{++row}"];
                    } while (cell.Value != null);

                    db.closeConnection();

                    MessageBox.Show($"Đã thêm vào hệ thống {categories.Count} loại sản phẩm và {productCount} sản phẩm", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch(Exception ex)
                {
                    MessageBox.Show($"Không thể định dạng dữ liệu hoặc file không tồn tại", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
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
        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var screen = new ObservableCollection<DXTabItem>()
                {
                    new DXTabItem{Content = new MasterDataUserControl(), Header = "MasterData"},
                    new DXTabItem{Content = new SaleUserControl(), Header = "Sale"},
                    new DXTabItem{Content = new OrderUserControl(), Header = "Order"}
                };

            dXTabControl1.ItemsSource = screen;
        }
    }
}
