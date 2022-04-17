using Management_Book.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for InputNewOrder.xaml
    /// </summary>
    public partial class InputNewOrder : Window, INotifyPropertyChanged
    {
        public InputNewOrder()
        {
            InitializeComponent();
        }

        MyShopModel.ViewModel _viewModel = new MyShopModel.ViewModel();
        BindingList<MyShopModel.Category> _categories;

        Dictionary<string, int> _categoriesDictionary = new Dictionary<string, int>();

        MyShopModel.Category _selectedCategory = new MyShopModel.Category();

        BindingList<OrderModel.PurchaseProduct> _selectedProduct= new BindingList<OrderModel.PurchaseProduct>();

        OrderModel.Purchase _currentOrder = new OrderModel.Purchase();
        float _totalOrder;
        float TotalOrder
        {
            get => _totalOrder;
            set
            {
                _totalOrder = value;
                OnPropertyChanged("TotalOrder");
            }
        }

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.PageSize = 24;
            updateDataSource();
        }

        public void updateDataSource()
        {
            MyShopEntities myDB = MyShopEntities.getInstance();
            myDB.openConnection();

            _categories = new BindingList<MyShopModel.Category>(myDB.getCategories());
            foreach (var category in _categories)
            {
                _categoriesDictionary[category.Name] = category.Id;
            }

            foreach (var cat in _categories)
            {
                cat.Products = myDB.getProductsOf(cat.Id);
            }
            myDB.closeConnection();

            ComboBoxCategory.ItemsSource = _categories;
            GridData.ItemsSource = _viewModel.SelectedProducts;
            GridListProduct.ItemsSource = _selectedProduct;

            _currentOrder.Total = 0;
            AllProduct_Click(null, null);
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

            _selectedCategory = ComboBoxCategory.SelectedItem as MyShopModel.Category;
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

            if (next != null)
            {
                _viewModel.CurrentPage = (int)next.Page;
                updateView();
            }
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
                    _viewModel.CurrentPage = currentPagingComboBox.Items.Count - 1;
                }


                if (_viewModel.CurrentPage < 1)
                {
                    currentPagingComboBox.SelectedIndex = 0;
                }
                else
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

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchInput.Text.ToString().Trim();
            MyShopEntities.getInstance().openConnection();
            if(_selectedCategory != null)
            {
                _viewModel.Products = MyShopEntities.getInstance().getProductsLike(_selectedCategory.Id, searchText);
            }
            else
            {
                _viewModel.Products = MyShopEntities.getInstance().getProductsLike(searchText);
            }
            
            MyShopEntities.getInstance().closeConnection();
            updateView();
        }

        private void AllProduct_Click(object sender, RoutedEventArgs e)
        {
            MyShopEntities.getInstance().openConnection();
            _viewModel.Products = MyShopEntities.getInstance().getAllProducts();
            MyShopEntities.getInstance().closeConnection();
            _viewModel.CurrentPage = 1;
            _viewModel.TotalPage = _viewModel.Products.Count / _viewModel.PageSize +
                (_viewModel.Products.Count % _viewModel.PageSize == 0 ? 0 : 1);

            updateView();

            currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;
            currentPagingComboBox.SelectedIndex = 0;
            ComboBoxCategory.SelectedIndex = -1;
            _selectedCategory = null;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyShopModel.Product product = GridData.SelectedItem as MyShopModel.Product;
                if(product != null)
                {
                    OrderModel.PurchaseProduct selectedProduct = new OrderModel.PurchaseProduct()
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = Convert.ToInt32(QuantityInput.Text),
                        Total = product.Price * Convert.ToInt32(QuantityInput.Text)
                    };

                    if(_selectedProduct.Any(prd => prd.Name.Equals(selectedProduct.Name))){
                        int idx = _selectedProduct.ToList().FindIndex(prd => prd.Name.Equals(selectedProduct.Name));
             
                        if(_selectedProduct[idx].Quantity + selectedProduct.Quantity > product.Quantity)
                        {
                            MessageBox.Show("Vượt quá số lượng cho phép của sản phẩm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            _selectedProduct[idx].Quantity += selectedProduct.Quantity;
                            _selectedProduct[idx].Total += selectedProduct.Quantity * selectedProduct.Price;
                            _currentOrder.Total += selectedProduct.Quantity * selectedProduct.Price;
                        }
                    }
                    else
                    {
                        if (selectedProduct.Quantity > product.Quantity)
                        {
                            MessageBox.Show("Vượt quá số lượng cho phép của sản phẩm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            _selectedProduct.Add(selectedProduct);
                            _currentOrder.Total += selectedProduct.Total;
                        }
                    }
                    
                    QuantityInput.Text = "";
                    GridData.UnselectAll();
                } else
                {
                    MessageBox.Show("Vui lòng chọn 1 sản phẩm để thêm vào", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            } catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập liệu, số lượng phải là số nguyên [0-9]", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            OrderModel.PurchaseProduct selectedProduct = GridListProduct.SelectedItem as OrderModel.PurchaseProduct;
            if (selectedProduct != null)
            {
                int idx = _selectedProduct.ToList().FindIndex(prd => prd.Name.Equals(selectedProduct.Name));

                MyShopEntities.getInstance().openConnection();
                MyShopModel.Product product = MyShopEntities.getInstance().getOneProduct(selectedProduct.ProductId);
                MyShopEntities.getInstance().closeConnection();

                if (_selectedProduct[idx].Quantity + 1 > product.Quantity)
                {
                    MessageBox.Show("Vượt quá số lượng cho phép của sản phẩm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    _selectedProduct[idx].Quantity++;
                    _selectedProduct[idx].Total += selectedProduct.Price;
                    _currentOrder.Total += selectedProduct.Price;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm trong đơn hàng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            OrderModel.PurchaseProduct selectedProduct = GridListProduct.SelectedItem as OrderModel.PurchaseProduct;
            if (selectedProduct != null)
            {
                int idx = _selectedProduct.ToList().FindIndex(prd => prd.Name.Equals(selectedProduct.Name));
                if (_selectedProduct[idx].Quantity - 1 < 1)
                {
                    if(MessageBox.Show("Bạn có muốn xóa sản phẩm này khỏi đơn hàng không?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        _currentOrder.Total -= _selectedProduct[idx].Total;
                        _selectedProduct.RemoveAt(idx);
                    }
                }
                else
                {
                    _selectedProduct[idx].Quantity--;
                    _selectedProduct[idx].Total -= selectedProduct.Price;
                    _currentOrder.Total -= selectedProduct.Price;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm trong đơn hàng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            OrderModel.PurchaseProduct selectedProduct = GridListProduct.SelectedItem as OrderModel.PurchaseProduct;
            if (selectedProduct != null)
            {
                int idx = _selectedProduct.ToList().FindIndex(prd => prd.Name.Equals(selectedProduct.Name));
                if (MessageBox.Show("Bạn có muốn xóa sản phẩm này khỏi đơn hàng không?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _currentOrder.Total -= _selectedProduct[idx].Total;
                    _selectedProduct.RemoveAt(idx);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm trong đơn hàng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            OrderEntities.getInstance().openConnection();
            MyShopEntities.getInstance().openConnection();

            OrderModel.Customer customer = new OrderModel.Customer()
            {
                Name = TextBox_Name.Text,
                Address = TextBox_Address.Text,
                Email = TextBox_Email.Text,
                Tel = TextBox_Tel.Text
            };
            int customerId = OrderEntities.getInstance().insertCustomer(customer);

            _currentOrder.CustomerId = customerId;
            int newPurchaseId = OrderEntities.getInstance().insertPurchase(_currentOrder);

            foreach (var purchaseProduct in _selectedProduct)
            {
                OrderModel.PurchaseProduct purchaseDetail = new OrderModel.PurchaseProduct()
                {
                    PurchaseId = newPurchaseId,
                    ProductId = purchaseProduct.ProductId,
                    Price = purchaseProduct.Price,
                    Quantity = purchaseProduct.Quantity,
                    Total = purchaseProduct.Total
                };
                OrderEntities.getInstance().insertPurchaseDetail(purchaseDetail);
            }

            MyShopEntities.getInstance().closeConnection();

            MessageBox.Show("Tạo đơn hàng mới thành công", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void TextBox_Tel_LostFocus(object sender, RoutedEventArgs e)
        {
            OrderEntities.getInstance().openConnection();

            OrderModel.Customer customer = OrderEntities.getInstance().getCustomerByTel(TextBox_Tel.Text);

            if(customer != null)
            {
                TextBox_Name.Text = customer.Name;
                TextBox_Address.Text = customer.Address;
                TextBox_Email.Text = customer.Email;
            }
            else
            {
                TextBox_Name.Text = "";
                TextBox_Address.Text = "";
                TextBox_Email.Text = "";
            }

            OrderEntities.getInstance().closeConnection();
        }
    }
}
