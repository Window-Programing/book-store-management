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

        public MasterDataUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            DataGirdProduct.ItemsSource = _viewModel.SelectedProducts;
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
        }
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

            view.TotalItems = view.Products.Count;
            view.TotalPage = view.TotalItems / view.PageSize +
                (view.Products.Count % view.PageSize == 0 ? 0 : 1);

            currentPagingTextBlock.Text = $"{view.CurrentPage}/{view.TotalPage}";
        }
        
    }
}
