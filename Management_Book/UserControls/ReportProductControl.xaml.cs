using Aspose.Cells.Charts;
using LiveCharts;
using LiveCharts.Wpf;
using Management_Book.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
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
using SeriesCollection = LiveCharts.SeriesCollection;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for ReportProductControl.xaml
    /// </summary>
    public partial class ReportProductControl : UserControl
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        public SeriesCollection SeriesCollection { get; set; }
        public string[] CreateDate { get; set; }
        public Func<double, string> Formatter { get; set; }

        OrderModel.ViewModel _viewModel = new OrderModel.ViewModel();
        public ReportProductControl()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void GridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataContext = null;

            OrderModel.ReportProduct selectedItem = GridData.SelectedItem as OrderModel.ReportProduct;

            DateTime dateFrom = (DateTime)DatePickerFrom.SelectedDate;
            dateFrom = dateFrom.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dateTo = (DateTime)DatePickerTo.SelectedDate;
            dateTo = dateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            if (selectedItem != null)
            {
                OrderEntities.getInstance().openConnection();

                List<OrderModel.ReportProduct> singleProduct = OrderEntities.getInstance().getSingleProductFilterByDate(selectedItem.ProductId, dateFrom, dateTo);

                OrderEntities.getInstance().closeConnection();
                List<double> Quantity = new List<double>();
                List<string> listDate = new List<string>();

                foreach (OrderModel.ReportProduct row in singleProduct)
                {
                    Quantity.Add(row.Quantity);
                    listDate.Add(row.CreateDate.ToString().Substring(0, 10));
                }

                SeriesCollection = new SeriesCollection();

                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Quantity product sold",
                    Values = new ChartValues<double>(Quantity)
                });

                CreateDate = listDate.ToArray();
                Formatter = value => value.ToString("C");

                DataContext = this;
            }
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GridData_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

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

            _viewModel.ReProduct = OrderEntities.getInstance().getReportProductFilterByDate(dateFrom, dateTo);

            OrderEntities.getInstance().closeConnection();

            GridData.ItemsSource = _viewModel.ReProduct;
        }
    }
}
