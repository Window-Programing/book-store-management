using Aspose.Cells;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using Management_Book.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for MasterDataUserControl.xaml
    /// </summary>
    public partial class MasterDataUserControl : UserControl
    {
        public enum MasterDataAction
        {
            AddNewCategory,               
            DeleteSelectedCategory,  
            AddNewProduct,	
            UpdateSelectedProduct, 
            DeleteSelectedProduct
        };

        MyShopModel.ViewModel _viewModel = new MyShopModel.ViewModel();
        BindingList<MyShopModel.Category> _categories;

        Dictionary<string, int> categoriesDictionary = new Dictionary<string, int>();

        MyShopModel.Category selectedCategory = new MyShopModel.Category();

        class PagingRow
        {
            public int Page { get; set; }
            public int TotalPages { get; set; }
        }
        class PagingInfo
        {
            public List<PagingRow> Items { get; set; }
            public PagingInfo(int totalPages)
            {
                Items = new List<PagingRow>();

                for (int i = 1; i <= totalPages; i++)
                {
                    Items.Add(new PagingRow()
                    {
                        Page = i,
                        TotalPages = totalPages
                    });
                }
            }
        }

        public MasterDataUserControl()
        {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.PageSize = Convert.ToInt32(AppConfig.getValue(AppConfig.PageSize));
            updateDataSource();
        }

        public void reload()
        {
            _viewModel.PageSize = Convert.ToInt32(AppConfig.getValue(AppConfig.PageSize));
            updateView();

            MessageBox.Show("Reload Page Size View OK");
        }
        public void updateDataSource()
        {
            MyShopEntities db = MyShopEntities.getInstance();
            db.openConnection();

            _categories = new BindingList<MyShopModel.Category>(db.getCategories());
            foreach (var category in _categories)
            {
                categoriesDictionary[category.Name] = category.Id;
            }

            foreach (var cat in _categories)
            {
                cat.Products = db.getProductsOf(cat.Id);
            }
            db.closeConnection();

            ComboBoxCategory.ItemsSource = _categories;
            if (ComboBoxCategory.Items.Count > 0)
            {
                ComboBoxCategory.SelectedIndex = 0;
                selectedCategory = ComboBoxCategory.SelectedItem as MyShopModel.Category;
                GridData.ItemsSource = _viewModel.SelectedProducts;
            }
            else
            {
                GridData.ItemsSource = new BindingList<MyShopModel.Product>();
            }
        }

        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ComboBoxCategory.SelectedIndex;
            if (i >= 0)
            {
                _viewModel.Products = _categories[i].Products;
                _viewModel.CurrentPage = 1;
                _viewModel.TotalPage = _viewModel.Products.Count / _viewModel.PageSize +
                    (_viewModel.Products.Count % _viewModel.PageSize == 0 ? 0 : 1);

                updateView();

                currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;
                currentPagingComboBox.SelectedIndex = 0;
            }

            selectedCategory = ComboBoxCategory.SelectedItem as MyShopModel.Category;
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = 1;
            currentPagingComboBox.SelectedIndex = 0;
            updateView();
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage > 1)
            {
                _viewModel.CurrentPage -= 1;
                currentPagingComboBox.SelectedIndex -= 1;
                updateView();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage < _viewModel.TotalPage)
            {
                _viewModel.CurrentPage += 1;
                currentPagingComboBox.SelectedIndex += 1;
                updateView();
            }
        }

        private void lastButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = _viewModel.TotalPage;
            currentPagingComboBox.SelectedIndex = _viewModel.TotalPage - 1;
            updateView();
        }

        private void currentPagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var next = currentPagingComboBox.SelectedItem as PagingRow;

            if(next != null)
            {
                _viewModel.CurrentPage = (int)next.Page;
                updateView();
            }
        }

        public void HandleParentEvent(MasterDataAction action)
        {
            MyShopModel.Product productChoose = new MyShopModel.Product();
            if (GridData.SelectedItem != null)
            {
                productChoose = GridData.SelectedItem as MyShopModel.Product;
            }

            MyShopEntities.getInstance().openConnection();

            switch (action)
            {
                case MasterDataAction.AddNewCategory:
                    addNewCategory();
                    break;
                case MasterDataAction.DeleteSelectedCategory:
                    deleteSelectedCategory();
                    break;
                case MasterDataAction.AddNewProduct:
                    AddNewProduct();
                    break;
                case MasterDataAction.UpdateSelectedProduct:
                    UpdateSelectedProduct(productChoose);
                    break;
                case MasterDataAction.DeleteSelectedProduct:
                    
                    DeleteSelectedProduct(productChoose.Id);
                    break;
            }

            MyShopEntities.getInstance().closeConnection();
        }

        private void deleteSelectedCategory()
        {
            Debug.WriteLine("Delete Category Event Click");

            MyShopEntities.getInstance().deleteProductsOf(selectedCategory.Id);
            MyShopEntities.getInstance().deleteCategory(selectedCategory.Id);
           
            updateDataSource();
        }

        private void addNewCategory()
        {
            Debug.WriteLine("Add Category Event Click");

            Application curApp = Application.Current;
            Window window = curApp.MainWindow;

            InputNewCategory inputNewCategory = new InputNewCategory();
            inputNewCategory.Owner = window;
            inputNewCategory.ShowDialog();

            MyShopModel.Category newCategory = inputNewCategory.getNewCategory();
            if (inputNewCategory.DialogResult == true && newCategory != null)
            {
                MyShopEntities.getInstance().insertCategory(newCategory);
                _categories.Add(newCategory);
            }
        }

        private void AddNewProduct()
        {
            Debug.WriteLine("Add Product Event Click");

            Application curApp = Application.Current;
            Window window = curApp.MainWindow;

            InputNewProduct inputNewProduct = new InputNewProduct();
            inputNewProduct.Owner = window;
            inputNewProduct.ShowDialog();

            MyShopModel.Product newProduct = inputNewProduct.getNewProduct();

            if (inputNewProduct.DialogResult == true && newProduct != null)
            {
                MyShopEntities.getInstance().openConnection();
                MyShopEntities.getInstance().insertProduct(newProduct);
            }

            updateDataChangedFromDatabase();
        }

        private void UpdateSelectedProduct(MyShopModel.Product targetProduct)
        {
            Debug.WriteLine("Update Product Event Click");

            int id_category = ComboBoxCategory.SelectedIndex;
            double cost, price;
            double.TryParse(TextBox_Cost.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("vi-VN").NumberFormat, out cost);
            double.TryParse(TextBox_Price.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("vi-VN").NumberFormat, out price);

            targetProduct.Name = TextBox_Name.Text;
            targetProduct.Price = price;
            targetProduct.Cost = cost;
            targetProduct.Quantity = Convert.ToInt32(TextBox_Quantity.Text);
            targetProduct.Image = TextBox_Image.Text;
            targetProduct.Category = new MyShopModel.Category() { Id = categoriesDictionary[_categories[id_category].Name] };

            foreach (var product in _viewModel.SelectedProducts)
            {
                if (product.Id == targetProduct.Id)
                {
                    MyShopEntities.getInstance().updateProduct(targetProduct);   
                }
            }
            updateDataChangedFromDatabase();
        }

        private void DeleteSelectedProduct(int id)
        {
            Debug.WriteLine("Delete Product Event Click");
            
            for (var i = 0; i < _viewModel.SelectedProducts.Count; i++)
            {
                if (_viewModel.SelectedProducts[i].Id == id)
                {
                    _viewModel.SelectedProducts.RemoveAt(i);
                    MyShopEntities.getInstance().deleteProduct(id);

                   
                }
            }
            updateDataChangedFromDatabase();
        }

        private void transDataToView(List<MyShopModel.Product> source, BindingList<MyShopModel.Product> view)
        {
            if (view.Count != 0) view.Clear();
            foreach (var product in source) view.Add(product);
        }

        private void updateView()
        {
            int oldTotalPage = _viewModel.TotalPage;
            _viewModel.TotalPage = _viewModel.Products.Count / _viewModel.PageSize +
                (_viewModel.Products.Count % _viewModel.PageSize == 0 ? 0 : 1);

            if (_viewModel.TotalPage != oldTotalPage)
            {
                currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;

                if (_viewModel.CurrentPage > _viewModel.TotalPage)
                {
                    _viewModel.CurrentPage = _viewModel.TotalPage;
                }

                if(_viewModel.CurrentPage < 1)
                {
                    currentPagingComboBox.SelectedIndex = 0;
                } else
                {
                    currentPagingComboBox.SelectedIndex = _viewModel.CurrentPage - 1;
                }

            }

            transDataToView(_viewModel.Products
                                .Skip((_viewModel.CurrentPage - 1) * _viewModel.PageSize)
                                .Take(_viewModel.PageSize)
                                .ToList(),
                            _viewModel.SelectedProducts);
        }

        private void updateDataChangedFromDatabase()
        {
            _viewModel.Products = MyShopEntities.getInstance().getProductsOf(selectedCategory.Id);
            updateView();
        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchInput.Text.ToString().Trim();
            MyShopEntities.getInstance().openConnection();
            _viewModel.Products = MyShopEntities.getInstance().getProductsLike(selectedCategory.Id, searchText);
            MyShopEntities.getInstance().closeConnection();
            updateView();
        }

        private void filterPrice_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchInput.Text.ToString().ToLower().Trim();
            MyShopEntities.getInstance().openConnection();
            int from = Convert.ToInt32(fromPrice.Text);
            int to = Convert.ToInt32(toPrice.Text);
            _viewModel.Products = MyShopEntities.getInstance()
               .getProductsFilterByPrice(searchText, selectedCategory.Id, from, to);

            MyShopEntities.getInstance().closeConnection();
            updateView();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            MyShopEntities.getInstance().openConnection();
            _viewModel.Products = MyShopEntities.getInstance().getProductsOf(selectedCategory.Id);
            MyShopEntities.getInstance().closeConnection();
            updateView();

            SearchInput.Text = "";
            fromPrice.Text = "";
            toPrice.Text = "";
        }
    }
}
