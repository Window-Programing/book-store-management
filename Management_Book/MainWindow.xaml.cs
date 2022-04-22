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
using System.Threading;
using DevExpress.Xpf.Ribbon;

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

        private void Button_Logout_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Frm_Login mv = new Frm_Login();
            mv.Show();
            this.Close();
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
                    db.resetTable(MyShopEntities.ProductTable);
                    db.resetTable(MyShopEntities.CategoryTable);

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
                        float price = productSheet.Cells[$"D{row}"].FloatValue;
                        float cost = productSheet.Cells[$"E{row}"].FloatValue;
                        int quantity = productSheet.Cells[$"F{row}"].IntValue;
                        string image = productSheet.Cells[$"G{row}"].StringValue;

                        var product = new MyShopModel.Product()
                        {
                            Name = name,
                            Price = price,
                            Cost = cost,
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
                } catch(Exception )
                {
                    MessageBox.Show($"Không thể định dạng dữ liệu hoặc file không tồn tại", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.updateDataSource();
        }

        private void add_category_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Add Category Button Clicked");
            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.HandleParentEvent(MasterDataUserControl.MasterDataAction.AddNewCategory);
        }

        private void delete_category_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Update Category Button Clicked");
            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.HandleParentEvent(MasterDataUserControl.MasterDataAction.DeleteSelectedCategory);
        }

        private void add_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Add Product Button Clicked");
            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.HandleParentEvent(MasterDataUserControl.MasterDataAction.AddNewProduct);
        }

        private void update_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Update Product Button Clicked");
            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.HandleParentEvent(MasterDataUserControl.MasterDataAction.UpdateSelectedProduct);

        }

        private void delete_product_btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Delete Product Button Clicked");
            var tab = dXTabControl1.Items[0] as DXTabItem;
            var usercontrol = tab.Content as MasterDataUserControl;
            usercontrol.HandleParentEvent(MasterDataUserControl.MasterDataAction.DeleteSelectedProduct);
        }

        private void Create_Order_Btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Create Order Button Clicked");
            var tab = dXTabControl1.Items[1] as DXTabItem;
            var usercontrol = tab.Content as PurchaseUserControl;
            usercontrol.HandleParentEvent(PurchaseUserControl.PurchaseAction.AddNewOrder);
        }

        private void Update_Order_Btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Update Order Button Clicked");
            var tab = dXTabControl1.Items[1] as DXTabItem;
            var usercontrol = tab.Content as PurchaseUserControl;
            usercontrol.HandleParentEvent(PurchaseUserControl.PurchaseAction.UpdateSelectedOrder);
        }

        private void Delete_Order_Btn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Debug.WriteLine("Delete Order Button Clicked");
            var tab = dXTabControl1.Items[1] as DXTabItem;
            var usercontrol = tab.Content as PurchaseUserControl;
            usercontrol.HandleParentEvent(PurchaseUserControl.PurchaseAction.DeleteSelectedOrder);
        }

        public class TabDataItem
        {
            public string HeaderText { get; set; }
            public UserControl Content { get; set; }
        }

        private void Config_Database_Button_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }

        private void Page_Size_Button_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            ConfigPageSize configPageSize = new ConfigPageSize();
            configPageSize.Owner = this;
            configPageSize.ShowDialog();

            int pageSize = configPageSize.getResult();

            if (configPageSize.DialogResult == true && pageSize != -1)
            {
                AppConfig.setValue(AppConfig.PageSize, pageSize.ToString());

            }

            ((MasterDataUserControl)((DXTabItem)dXTabControl1.Items[0]).Content).reload();
        }
        private void ThemedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var screen = new ObservableCollection<DXTabItem>()
                {
                    new DXTabItem{Content = new MasterDataUserControl(), Header = "MasterData"},
                    new DXTabItem{Content = new PurchaseUserControl(), Header = "Sale"},
                    new DXTabItem{Content = new ReportUserControl(), Header = "Report Purchase"},
                    new DXTabItem{Content = new ReportProductControl(), Header = "Report Product"}
                };

            dXTabControl1.ItemsSource = screen;
            dXTabControl1.SelectedIndex = Convert.ToInt32(AppConfig.getValue(AppConfig.Tab));

            String namePageSave = AppConfig.getValue(AppConfig.PageRibbon);

            foreach (RibbonPage page in groupPageRibbon.Pages)
            {
                if (page.Name == namePageSave)
                {
                    controlRibbon.SelectedPage = page;
                }
            }
        }

        private void ThemedWindow_Closed(object sender, EventArgs e)
        {
            int idx1 = dXTabControl1.SelectedIndex;

            AppConfig.setValue(AppConfig.Tab, idx1.ToString());

            foreach(RibbonPage page in groupPageRibbon.Pages)
            {
                if (page.IsSelected)
                {
                    AppConfig.setValue(AppConfig.PageRibbon, page.Name);
                }
            }
        }
    }
}
