using Aspose.Cells;
using Management_Book.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
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
        class Category : INotifyPropertyChanged, INotifyCollectionChanged
        {
            private string _name;
            private List<Product> _products;
            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanegd();
                }
            }

            public List<Product> Products
            {
                get => _products;
                set
                {
                    _products = value;
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            private void OnPropertyChanegd([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        class Product : INotifyPropertyChanged
        {
            private string _name;
            private int _price;
            private Category _category;
            public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
            public int Price { get => _price; set { _price = value; OnPropertyChanged(); } }
            public Category Category { get => _category; set { _category = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        class ViewModel : INotifyPropertyChanged
        {
            int _currentPage = 0, _pageSize = 0, _totalPage = 0, _totalItems = 0;
            public List<Product> Products { get; set; } = new List<Product>();
            public BindingList<Product> SelectedProducts { get; set; } = new BindingList<Product>();

            public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); } }
            public int PageSize { get => _pageSize; set { _pageSize = value; OnPropertyChanged(); } }
            public int TotalPage { get => _totalPage; set { _totalPage = value; OnPropertyChanged(); } }
            public int TotalItems { get => _totalItems; set { _totalItems = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        BindingList<Category> _categories = new BindingList<Category>();
        ViewModel _viewModel = new ViewModel();

        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ComboBoxCategory.SelectedIndex;
            if (i >= 0)
            {
                _viewModel.Products = _categories[i].Products;
                _viewModel.CurrentPage = 1;

                transDataToView(_viewModel.Products, _viewModel.SelectedProducts);
                updatePaging(_viewModel);
            }
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = 1;
            updatePaging(_viewModel);
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage > 1)
            {
                _viewModel.CurrentPage -= 1;
                updatePaging(_viewModel);
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage < _viewModel.TotalPage)
            {
                _viewModel.CurrentPage += 1;
                updatePaging(_viewModel);
            }
        }

        private void lastButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = _viewModel.TotalPage;
            updatePaging(_viewModel);
        }

        private void transDataToView(List<Product> source, BindingList<Product> view)
        {
            if (view.Count != 0) view.Clear();
            foreach (var product in source) view.Add(product);
        }
        private void updatePaging(ViewModel view)
        {
            transDataToView(view.Products
                                .Skip((view.CurrentPage - 1) * view.PageSize)
                                .Take(view.PageSize)
                                .ToList(),
                            _viewModel.SelectedProducts);

            view.TotalItems = view.Products.Count;
            view.TotalPage = view.TotalItems / view.PageSize +
                (view.Products.Count % view.PageSize == 0 ? 0 : 1);

            currentPagingTextBlock.Text = $"{view.CurrentPage}/{view.TotalPage}";
        }
        public MasterDataUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBoxCategory.ItemsSource = _categories;
            ListViewProduct.ItemsSource = _viewModel.SelectedProducts;
            _viewModel.PageSize = 10;

            var workbook = new Workbook("C:\\Users\\minht\\Desktop\\GitHub\\book-store-management\\Management_Book\\BookData.xlsx");
            var categorySheet = workbook.Worksheets[0];

            Dictionary<string, int> categoriesDictionary = new Dictionary<string, int>();

            var row = 2;
            var cell = categorySheet.Cells[$"B{row}"];

            do
            {
                var name = cell.StringValue;



                row++;
                cell = categorySheet.Cells[$"B{row}"];
            }
            while (cell.Value != null);

            //foreach (var tab in tabs)
            //{
            //    Category category = new Category()
            //    {
            //        Name = tab.Name,
            //        Products = new List<Product>()
            //    };

            //    var row = 2;
            //    var cell = tab.Cells[$"B{row}"];

            //    while (cell.Value != null)
            //    {
            //        string name = cell.StringValue;
            //        int price = tab.Cells[$"C{row}"].IntValue;

            //        var product = new Product()
            //        {
            //            Name = name,
            //            Price = price,
            //            Category = category
            //        };
            //        category.Products.Add(product);

            //        row++;
            //        cell = tab.Cells[$"B{row}"];
            //    }

            //    _categories.Add(category);
            //}
        }
    }
}
