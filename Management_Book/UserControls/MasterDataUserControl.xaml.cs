using Aspose.Cells;
using Management_Book.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
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
        MyShopModel.ViewModel _viewModel = new MyShopModel.ViewModel();
        BindingList<MyShopModel.Category> _categories;
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

            //DataGirdProduct.ItemsSource = _viewModel.SelectedProducts;
            _viewModel.PageSize = 10;

            MyShopEntities db = MyShopEntities.getInstance();
            db.openConnection();
            _categories = new BindingList<MyShopModel.Category>(db.getCategories());
            db.closeConnection();

            db.openConnection();
            foreach (var cat in _categories)
            {
                cat.Products = db.getProductsOf(cat.Name);
            }
            db.closeConnection();

            ComboBoxCategory.ItemsSource = _categories;
            grid.ItemsSource = _viewModel.SelectedProducts;
            
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

                transDataToView(_viewModel.Products, _viewModel.SelectedProducts);
                updatePaging(_viewModel);

                currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;
                currentPagingComboBox.SelectedIndex = 0;
            }
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = 1;
            currentPagingComboBox.SelectedIndex = 0;
            updatePaging(_viewModel);
        }
        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage > 1)
            {
                _viewModel.CurrentPage -= 1;
                currentPagingComboBox.SelectedIndex -= 1;
                updatePaging(_viewModel);
            }
        }
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CurrentPage < _viewModel.TotalPage)
            {
                _viewModel.CurrentPage += 1;
                currentPagingComboBox.SelectedIndex += 1;
                updatePaging(_viewModel);
            }
        }
        private void lastButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentPage = _viewModel.TotalPage;
            currentPagingComboBox.SelectedIndex = _viewModel.TotalPage - 1;
            updatePaging(_viewModel);
        }
        private void transDataToView(List<MyShopModel.Product> source, BindingList<MyShopModel.Product> view)
        {
            if (view.Count != 0) view.Clear();
            foreach (var product in source) view.Add(product);
        }
        private void updatePaging(MyShopModel.ViewModel view)
        {
            transDataToView(view.Products
                                .Skip((view.CurrentPage - 1) * view.PageSize)
                                .Take(view.PageSize)
                                .ToList(),
                            _viewModel.SelectedProducts);
        }

        private void currentPagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var next = currentPagingComboBox.SelectedItem as PagingRow;

            if(next != null)
            {
                _viewModel.CurrentPage = (int)next.Page;
                updatePaging(_viewModel);
            }
            

        }
    }
}
