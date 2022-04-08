using DevExpress.Xpf.Core;
using Management_Book.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
