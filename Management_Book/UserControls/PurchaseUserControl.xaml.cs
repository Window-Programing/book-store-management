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
        List<OrderModel.PurchaseProduct> _listOrderProduct = new List<OrderModel.PurchaseProduct>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OrderEntities.getInstance().openConnection();
            statusEnum = OrderEntities.getInstance().getAllStatusEnum();
            OrderEntities.getInstance().closeConnection();

            _viewModel.PageSize = 24;
            _viewModel.CurrentPage = 1;
            updateDataSource();
        }

        public void updateDataSource()
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

            transDataToView(_viewModel.Orders
                                .Skip((_viewModel.CurrentPage - 1) * _viewModel.PageSize)
                                .Take(_viewModel.PageSize)
                                .ToList(),
                            _viewModel.SelectedOrders);
        }

        public void HandleParentEvent(PurchaseAction action)
        {
        //    MyShopModel.Product productChoose = new MyShopModel.Product();
        //    if (GridData.SelectedItem != null)
        //    {
        //        productChoose = GridData.SelectedItem as MyShopModel.Product;
        //    }

        //    MyShopEntities.getInstance().openConnection();

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
            throw new NotImplementedException();
        }

        private void udpateSelectedOrder()
        {
            throw new NotImplementedException();
        }

        private void addNewOrder()
        {
            Debug.WriteLine("Add Order Event Click");

            Application curApp = Application.Current;
            Window window = curApp.MainWindow;

            InputNewOrder inputNewOrder = new InputNewOrder();
            inputNewOrder.Owner = window;
            inputNewOrder.ShowDialog();
        }

        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void filterPrice_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OrderModel.Purchase orderRow = GridData.SelectedItem as OrderModel.Purchase;
            if(orderRow != null) {
                OrderEntities.getInstance().openConnection();

                _listOrderProduct = OrderEntities.getInstance().getPurchaseProductOf(orderRow.Id);
                GridListProduct.ItemsSource = _listOrderProduct;

                OrderModel.Customer customer = OrderEntities.getInstance().getCustomerByTel(orderRow.CustomerTel);

                TextBox_CustomerName.Text = customer.Name;
                TextBox_Address.Text = customer.Address;
                TextBox_Tel.Text = customer.Tel;
                TextBox_Email.Text = customer.Email;

                OrderEntities.getInstance().closeConnection();
            }
           
        }
    }
}
