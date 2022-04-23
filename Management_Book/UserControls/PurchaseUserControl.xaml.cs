using Management_Book.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for PurchaseUserControl.xaml
    /// </summary>
    public partial class PurchaseUserControl : UserControl
    {
        public PurchaseUserControl()
        {
            InitializeComponent();

            OrderEntities.getInstance().openConnection();
            statusEnum = OrderEntities.getInstance().getAllStatusEnum();
            OrderEntities.getInstance().closeConnection();

            ComboBoxStatusFilter.ItemsSource = statusEnum;

            _viewModel.PageSize = Convert.ToInt32(AppConfig.getValue(AppConfig.PageSize));
            _viewModel.CurrentPage = 1;
            updateDataSource();
        }

        public enum PurchaseAction
        {
            AddNewOrder,
            UpdateSelectedOrder,
            DeleteSelectedOrder
        };

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

        OrderModel.ViewModel _viewModel = new OrderModel.ViewModel();
        List<OrderModel.PurchaseStatusEnum> statusEnum = new List<OrderModel.PurchaseStatusEnum>();
        BindingList<OrderModel.PurchaseProduct> _listOrderProduct = new BindingList<OrderModel.PurchaseProduct>();

        OrderModel.Purchase selectedPurchaseRow = new OrderModel.Purchase();
        int selectedRowOrderId = -1;

        static int CANCELLED_ORDER_VALUE = 3;
        static int NEW_ORDER_VALUE = 1;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OrderEntities.getInstance().openConnection();
            statusEnum = OrderEntities.getInstance().getAllStatusEnum();
            OrderEntities.getInstance().closeConnection();

            ComboBoxStatusFilter.ItemsSource = statusEnum;

            _viewModel.PageSize = Convert.ToInt32(AppConfig.getValue(AppConfig.PageSize));
            _viewModel.CurrentPage = 1;

            currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;
            currentPagingComboBox.SelectedIndex = 0;

            updateDataSource();
            updateView();
        }

        public void reload()
        {
            _viewModel.PageSize = Convert.ToInt32(AppConfig.getValue(AppConfig.PageSize));
            updateView();

            MessageBox.Show("Reload Page Size View OK");
        }

        private void updateDataSource()
        {
            OrderEntities myDB = OrderEntities.getInstance();
            myDB.openConnection();

            _viewModel.Orders = myDB.getAllPurchase();

            foreach(var order in _viewModel.Orders)
            {
                OrderModel.Customer customer = myDB.getCustomerById(order.CustomerId);
                order.CustomerTel = customer.Tel;
                order.StatusDisplayText = statusEnum[statusEnum.FindIndex(stat => stat.Value == order.Status)].DisplayText;
            }

            GridData.ItemsSource = _viewModel.SelectedOrders;

            myDB.closeConnection();

            _viewModel.TotalPage = _viewModel.Orders.Count / _viewModel.PageSize +
                (_viewModel.Orders.Count % _viewModel.PageSize == 0 ? 0 : 1);

            updateView();
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

        private void currentPagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var next = currentPagingComboBox.SelectedItem as PagingRow;

            if (next != null)
            {
                _viewModel.CurrentPage = (int)next.Page;
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
     
        private void transDataToView(List<OrderModel.Purchase> source, BindingList<OrderModel.Purchase> view)
        {
            if (view.Count != 0) view.Clear();
            foreach (var order in source) view.Add(order);
        }

        private void updateView()
        {
            int oldTotalPage = _viewModel.TotalPage;
            _viewModel.TotalPage = _viewModel.Orders.Count / _viewModel.PageSize +
                (_viewModel.Orders.Count % _viewModel.PageSize == 0 ? 0 : 1);

            if (_viewModel.TotalPage != oldTotalPage)
            {
                currentPagingComboBox.ItemsSource = new PagingInfo(_viewModel.TotalPage).Items;

                if (_viewModel.CurrentPage > _viewModel.TotalPage)
                {
                    _viewModel.CurrentPage = _viewModel.TotalPage;
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

            transDataToView(_viewModel.Orders
                                .Skip((_viewModel.CurrentPage - 1) * _viewModel.PageSize)
                                .Take(_viewModel.PageSize)
                                .ToList(),
                            _viewModel.SelectedOrders);
        }

        private void updateDataChangedFromDatabase()
        {
            OrderEntities.getInstance().openConnection();

            _viewModel.Orders = OrderEntities.getInstance().getAllPurchase();

            foreach (var order in _viewModel.Orders)
            {
                OrderModel.Customer customer = OrderEntities.getInstance().getCustomerById(order.CustomerId);
                order.CustomerTel = customer.Tel;
                order.StatusDisplayText = statusEnum[statusEnum.FindIndex(stat => stat.Value == order.Status)].DisplayText;
            }

            GridData.ItemsSource = _viewModel.SelectedOrders;

            OrderEntities.getInstance().closeConnection();

            updateView();
        }

        public void HandleParentEvent(PurchaseAction action)
        {
            switch (action)
            {
                case PurchaseAction.AddNewOrder:
                    addNewOrder();
                    break;
                case PurchaseAction.UpdateSelectedOrder:
                    udpateSelectedOrder();
                    break;
                case PurchaseAction.DeleteSelectedOrder:
                    deleteSelectedOrder();
                    break;
            }
        }

        private void deleteSelectedOrder()
        {
            if (selectedPurchaseRow != null && GridData.SelectedIndex != -1)
            {
                if (MessageBox.Show("Bạn có muốn xóa đơn hàng này không ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    OrderEntities.getInstance().openConnection();

                    OrderEntities.getInstance().deletePurchaseDetail(selectedPurchaseRow.Id);

                    OrderEntities.getInstance().deletePurchase(selectedPurchaseRow.Id);

                    cancelOrder();

                    refreshInputFields();

                    OrderEntities.getInstance().closeConnection();

                    updateDataChangedFromDatabase();

                    for (int i = 0; i < GridData.Items.Count; i++)
                    {
                        OrderModel.Purchase row = GridData.Items[i] as OrderModel.Purchase;

                        if (row != null && row.Id == selectedRowOrderId)
                        {
                            GridData.SelectedIndex = i;
                        }
                    }

                    MessageBox.Show("Xóa đơn hàng thành công", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần xóa", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void udpateSelectedOrder()
        {
            OrderModel.PurchaseStatusEnum statusItem = ComboBox_Status.SelectedItem as OrderModel.PurchaseStatusEnum;

            if (selectedPurchaseRow != null && GridData.SelectedIndex != -1)
            {
                if (selectedPurchaseRow.Status != CANCELLED_ORDER_VALUE)
                {
                    OrderEntities.getInstance().openConnection();
                    MyShopEntities.getInstance().openConnection();

                    OrderEntities.getInstance().updateCustomer(new OrderModel.Customer()
                    {
                        Name = TextBox_CustomerName.Text,
                        Email = TextBox_Email.Text,
                        Address = TextBox_Address.Text,
                        Tel = TextBox_Tel.Text
                    });

                    List<OrderModel.PurchaseProduct> oldList = OrderEntities.getInstance().getPurchaseProductOf(selectedPurchaseRow.Id);

                    foreach (var purchaseProduct in _listOrderProduct)
                    {
                        MyShopModel.Product productInStock = MyShopEntities.getInstance().getOneProduct(purchaseProduct.ProductId);

                        if (oldList.Any(pro => pro.ProductId.Equals(purchaseProduct.ProductId))) // Product exist in order -> get product in oldList -> calculate -> update new
                        {
                            int findIdx = oldList.FindIndex(p => p.ProductId.Equals(purchaseProduct.ProductId));
                            int difference = purchaseProduct.Quantity - oldList[findIdx].Quantity;

                            if (difference != 0)
                            {
                                OrderModel.PurchaseProduct newPro = purchaseProduct;

                                newPro.Quantity = purchaseProduct.Quantity;
                                newPro.Total = purchaseProduct.Quantity * purchaseProduct.Price;

                                OrderEntities.getInstance().updateProductPurchaseDetail(newPro);
                                MyShopEntities.getInstance().updateQuantityProduct(purchaseProduct.ProductId, productInStock.Quantity - difference);
                            }

                            oldList.RemoveAt(findIdx);
                        }
                        else
                        {
                            int difference = productInStock.Quantity - purchaseProduct.Quantity;

                            OrderModel.PurchaseProduct purchaseDetail = new OrderModel.PurchaseProduct()
                            {
                                PurchaseId = selectedPurchaseRow.Id,
                                ProductId = purchaseProduct.ProductId,
                                Price = purchaseProduct.Price,
                            };

                            if (difference >= 0)
                            {
                                purchaseDetail.Quantity = purchaseProduct.Quantity;
                                purchaseDetail.Total = purchaseDetail.Quantity * purchaseDetail.Price;
                            }
                            else
                            {
                                purchaseDetail.Quantity = productInStock.Quantity;
                                purchaseDetail.Total = purchaseDetail.Quantity * purchaseDetail.Price;
                                difference = 0;
                            }

                            OrderEntities.getInstance().insertPurchaseDetail(purchaseDetail);
                            MyShopEntities.getInstance().updateQuantityProduct(purchaseProduct.ProductId, difference);
                        }
                    }

                    if (oldList.Count > 0)
                    {
                        foreach (var oldProduct in oldList)
                        {
                            MyShopModel.Product productInStock = MyShopEntities.getInstance().getOneProduct(oldProduct.ProductId);
                            OrderEntities.getInstance().deleteProductInPurchaseDetail(oldProduct.PurchaseId, oldProduct.ProductId);
                            MyShopEntities.getInstance().updateQuantityProduct(oldProduct.ProductId, productInStock.Quantity + oldProduct.Quantity);
                        }
                    }

                    selectedPurchaseRow.CreateDate = Convert.ToDateTime(TextBox_CreateDate.Text);
                    selectedPurchaseRow.Status = statusItem.Value;
                    selectedPurchaseRow.Total = 0;
                    selectedPurchaseRow.Profit = 0;

                    foreach (OrderModel.PurchaseProduct pro in OrderEntities.getInstance().getPurchaseProductOf(selectedPurchaseRow.Id))
                    {
                        selectedPurchaseRow.Total += pro.Total;
                        selectedPurchaseRow.Profit += pro.Total - (pro.Quantity * MyShopEntities.getInstance().getOneProduct(pro.ProductId).Cost);
                    }

                    OrderEntities.getInstance().updatePurchase(selectedPurchaseRow);

                    MyShopEntities.getInstance().closeConnection();
                    OrderEntities.getInstance().closeConnection();

                    updateDataChangedFromDatabase();

                    for (int i = 0; i < GridData.Items.Count; i++)
                    {
                        OrderModel.Purchase row = GridData.Items[i] as OrderModel.Purchase;

                        if (row != null && row.Id == selectedRowOrderId)
                        {
                            GridData.SelectedIndex = i;
                        }
                    }
                    MessageBox.Show("Cập nhật đơn hàng thành công", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Đơn hàng đã hủy không thể cập nhật", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cancelOrder()
        {
            if (_listOrderProduct.Count > 0)
            {
                MyShopEntities.getInstance().openConnection();
                foreach (var oldProduct in _listOrderProduct)
                {
                    MyShopModel.Product productInStock = MyShopEntities.getInstance().getOneProduct(oldProduct.ProductId);
                    MyShopEntities.getInstance().updateQuantityProduct(oldProduct.ProductId, productInStock.Quantity + oldProduct.Quantity);
                }
                MyShopEntities.getInstance().closeConnection();
            }
        }

        private void addNewOrder()
        {
            Debug.WriteLine("Add Order Event Click");

            Application curApp = Application.Current;
            Window window = curApp.MainWindow;

            InputNewOrder inputNewOrder = new InputNewOrder();
            inputNewOrder.Owner = window;
            inputNewOrder.ShowDialog();
            updateDataSource();
            updateView();
        }

        private void refreshInputFields()
        {

            TextBox_CustomerName.Text = "";
            TextBox_Address.Text = "";
            TextBox_Tel.Text = "";
            TextBox_Email.Text = "";
            ComboBox_Status.SelectedIndex = -1;
            _listOrderProduct.Clear();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            OrderEntities.getInstance().openConnection();

            _viewModel.Orders = OrderEntities.getInstance().getAllPurchase();
            foreach (var order in _viewModel.Orders)
            {
                OrderModel.Customer customer = OrderEntities.getInstance().getCustomerById(order.CustomerId);
                order.CustomerTel = customer.Tel;
                order.StatusDisplayText = statusEnum[statusEnum.FindIndex(stat => stat.Value == order.Status)].DisplayText;
            }

            OrderEntities.getInstance().closeConnection();

            ComboBoxStatusFilter.SelectedIndex = -1;

            refreshInputFields();

            updateView();
        }

        private void filterPrice_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateFrom = (DateTime)DatePickerFrom.SelectedDate;
            dateFrom = dateFrom.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dateTo = (DateTime)DatePickerTo.SelectedDate;
            dateTo = dateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            Debug.WriteLine(dateFrom);
            Debug.WriteLine(dateTo);

            OrderEntities.getInstance().openConnection();

            _viewModel.Orders = OrderEntities.getInstance().getPurchasesFilterByDate(dateFrom, dateTo);
            foreach (var order in _viewModel.Orders)
            {
                OrderModel.Customer customer = OrderEntities.getInstance().getCustomerById(order.CustomerId);
                order.CustomerTel = customer.Tel;
                order.StatusDisplayText = statusEnum[statusEnum.FindIndex(stat => stat.Value == order.Status)].DisplayText;
            }

            OrderEntities.getInstance().closeConnection();

            updateView();
        }

        private void GridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedPurchaseRow = GridData.SelectedItem as OrderModel.Purchase;
            
            if (selectedPurchaseRow != null) {
                selectedRowOrderId = selectedPurchaseRow.Id;
                OrderEntities.getInstance().openConnection();

                _listOrderProduct = new BindingList<OrderModel.PurchaseProduct>( OrderEntities.getInstance().getPurchaseProductOf(selectedPurchaseRow.Id));
                GridListProduct.ItemsSource = _listOrderProduct;

                OrderModel.Customer customer = OrderEntities.getInstance().getCustomerByTel(selectedPurchaseRow.CustomerTel);

                TextBox_CustomerName.Text = customer.Name;
                TextBox_Address.Text = customer.Address;
                TextBox_Tel.Text = customer.Tel;
                TextBox_Email.Text = customer.Email;

                OrderEntities.getInstance().closeConnection();

                ComboBox_Status.ItemsSource = statusEnum;
                ComboBox_Status.SelectedIndex = statusEnum.FindIndex(stat => stat.Value.Equals(selectedPurchaseRow.Status));
            }
        }

        private void Update_Status_Click(object sender, RoutedEventArgs e)
        {
            OrderModel.PurchaseStatusEnum statusItem = ComboBox_Status.SelectedItem as OrderModel.PurchaseStatusEnum;
            if (selectedPurchaseRow != null)
            {
                if (selectedPurchaseRow.Status != CANCELLED_ORDER_VALUE)
                {
                    if(statusItem.Value == CANCELLED_ORDER_VALUE)
                    {
                        cancelOrder();
                    }
                    OrderEntities.getInstance().openConnection();

                    OrderEntities.getInstance().updatePurchaseStatus(selectedPurchaseRow.Id, statusItem.Value);

                    OrderEntities.getInstance().closeConnection();

                    updateDataChangedFromDatabase();

                    for (int i = 0; i < GridData.Items.Count; i++)
                    {
                        OrderModel.Purchase row = GridData.Items[i] as OrderModel.Purchase;

                        if (row != null && row.Id == selectedRowOrderId)
                        {
                            GridData.SelectedIndex = i;
                        }
                    }

                    MessageBox.Show("Cập nhật trạng thái đơn hàng thành công", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                } 
                else
                {
                    MessageBox.Show("Đơn hàng đã hủy không thể cập nhật", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    

            }
        }

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderModel.PurchaseStatusEnum statusValue = ComboBoxStatusFilter.SelectedItem as OrderModel.PurchaseStatusEnum;
            if(statusValue != null)
            {
                OrderEntities.getInstance().openConnection();

                _viewModel.Orders = OrderEntities.getInstance().getPurchasesFilterByStatus(statusValue.Value);

                foreach (var order in _viewModel.Orders)
                {
                    OrderModel.Customer customer = OrderEntities.getInstance().getCustomerById(order.CustomerId);
                    order.CustomerTel = customer.Tel;
                    order.StatusDisplayText = statusEnum[statusEnum.FindIndex(stat => stat.Value == order.Status)].DisplayText;
                }

                OrderEntities.getInstance().closeConnection();

                updateView();
            }
        }

        private void Modify_Product_Click(object sender, RoutedEventArgs e)
        {
            
            if (selectedPurchaseRow != null && GridData.SelectedIndex != -1)
            {
                if (selectedPurchaseRow.Status != CANCELLED_ORDER_VALUE && selectedPurchaseRow.Status == NEW_ORDER_VALUE)
                {
                    Application curApp = Application.Current;
                    Window window = curApp.MainWindow;

                    ModifyProductInOrder modifyProductInOrder = new ModifyProductInOrder(selectedPurchaseRow.Id, _listOrderProduct.ToList());
                    modifyProductInOrder.Owner = window;
                    modifyProductInOrder.ShowDialog();
                    if (modifyProductInOrder.DialogResult == true)
                    {
                        _listOrderProduct.Clear();
                        foreach (OrderModel.PurchaseProduct p in modifyProductInOrder.getResult())
                        {
                            _listOrderProduct.Add(p);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Đơn hàng không thể chỉnh sửa", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
