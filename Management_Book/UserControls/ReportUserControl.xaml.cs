using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using Management_Book.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System.Diagnostics;
using System.Globalization;

namespace Management_Book.UserControls
{
    /// <summary>
    /// Interaction logic for ReportUserControl.xaml
    /// </summary>
    public partial class ReportUserControl : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] CreateDate { get; set; }
        public Func<double, string> Formatter { get; set; }

        OrderModel.ViewModel _viewModel = new OrderModel.ViewModel();
        public ReportUserControl()
        {
            InitializeComponent();
        }

        private void filterPrice_Click(object sender, RoutedEventArgs e)
        {
            DataContext = null;

            if (DatePickerFrom.SelectedDate == null || DatePickerTo.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn các mốc thời gian cần xem report", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                DateTime dateFrom = (DateTime)DatePickerFrom.SelectedDate;
                DateTime dateTo = (DateTime)DatePickerTo.SelectedDate;
                dateFrom = dateFrom.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                dateTo = dateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                OrderEntities.getInstance().openConnection();

                _viewModel.Orders = OrderEntities.getInstance().getReportPurchaseFilterByDate(dateFrom, dateTo);
                foreach(var item in _viewModel.Orders)
                {
                    item.CreateDate = item.CreateDate.Date;
                }

                OrderEntities.getInstance().closeConnection();

                GridData.ItemsSource = _viewModel.Orders;

                List<double> Total = new List<double>();
                List<double> Profit = new List<double>();
                List<string> listDate = new List<string>();

                foreach (OrderModel.Purchase row in _viewModel.Orders)
                {
                    Total.Add(row.Total);
                    Profit.Add(row.Profit);
                    listDate.Add(row.CreateDate.ToShortDateString());
                }

                SeriesCollection = new SeriesCollection() {
                new LineSeries
                {
                    Title="Revenue",
                    Values = new ChartValues<double> (Total)
                },
                new LineSeries
                {
                    Title = "Profit",
                    Values = new ChartValues<double>(Profit)
                }

            };

                CreateDate = listDate.ToArray();

                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                Formatter = value => value.ToString("#,###", cul.NumberFormat);

                DataContext = this;
            }
        }
    }
}
