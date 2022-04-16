using Management_Book.Model;
using System;
using System.Collections.Generic;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
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


            if (inputNewOrder.DialogResult == true)
            {
               
            }
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

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void currentPagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lastButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void filterPrice_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
