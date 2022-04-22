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
using System.Globalization;
using System.Net;

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
                    new DXTabItem{Content = new ReportUserControl(), Header = "Report"}
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

        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                if (url == null)
                url = CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }

        static object GetRegistryValue() => Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\BookManagementt", "Configuration", string.Empty);

        public static DateTime GetNistTime()
        {
            using (WebResponse response = WebRequest.Create("https://www.microsoft.com").GetResponse())
                return DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal);
        }
        private void checkTrialAndCreateRegistry()
        {
            if (CheckForInternetConnection())
            {
                dynamic registryValue = GetRegistryValue();

                registryValue = registryValue != null ? registryValue.ToString() : string.Empty;

                //Check whether the Registry is already configured. IF not, create the registry
                if (String.IsNullOrWhiteSpace(registryValue))
                {
                    registryValue = GetNistTime().ToString("dd/MM/yyyy");
                    var softwareSubkey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                    var keyData = softwareSubkey.CreateSubKey("BookManagementt");
                    keyData.SetValue("Configuration", registryValue);
                    keyData.Close();
                    MessageBox.Show($"Trial date started - {registryValue}");
                }
                else
                {
                    DateTime date = DateTime.ParseExact(registryValue, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime curTime = GetNistTime();
                    TimeSpan daysLeft = date.AddDays(15).Subtract(curTime);
                    bool expired = date.AddDays(15) >= curTime ? false : true;
                    if(!expired)
                    {
                        MessageBox.Show($"You have {daysLeft.Days} days {daysLeft.Hours} hours trial left.");
                    } else
                    {
                        MessageBox.Show("Your trial period has expired!");
                        Environment.Exit(0);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Internet connection is required to use this program!");
                Environment.Exit(0);
            }

        }

        private void ThemedWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            checkTrialAndCreateRegistry();
        }
    }
}
