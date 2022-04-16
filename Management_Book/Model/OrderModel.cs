using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book.Model
{
    public class OrderModel
    {
        public class Customer : INotifyPropertyChanged
        {
            private int _id;
            private string _name;
            private string _address;
            private string _email;
            private string _tel;
            public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
            public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
            public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
            public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
            public string Tel { get => _tel; set { _tel = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class Purchase : INotifyPropertyChanged
        {
            private int _id;
            private int _purchaseDetailId;
            private int _customerlId;
            private float _total;
            private string _createDate;
            private string _customerTel;
            private int _status;
            public int Id { get => _id; set{_id = value;OnPropertyChanged();} }
            public int CustomerId { get => _customerlId; set { _customerlId = value; OnPropertyChanged(); } }
            public int PurchaseId { get => _purchaseDetailId; set { _purchaseDetailId = value; OnPropertyChanged(); } }
            public float Total { get => _total; set { _total = value; OnPropertyChanged(); } }
            public string CustomerTel { get => _customerTel; set { _customerTel = value; OnPropertyChanged(); } }
            public int Status { get => _status; set { _status = value; OnPropertyChanged(); } }
            public string CreateDate { get => _createDate; set { _createDate = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public class PurchaseProduct : INotifyPropertyChanged
        { 
            private int _productId;
            private string _name;
            private float _price;
            private int _quantity;
            private float _total;
            private int _purchaseId;

            public int ProductId { get => _productId; set { _productId = value; OnPropertyChanged(); } }
            public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
            public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }
            public float Price { get => _price; set { _price = value; OnPropertyChanged(); } }
            public float Total { get => _total; set { _total = value ; OnPropertyChanged(); } }
            public int PurchaseId { get => _purchaseId; set { _purchaseId = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class PurchaseDetail : INotifyPropertyChanged
        {
            private int _id;
            private int _purchaseId;
            private List<PurchaseProduct> _purchaseProducts;

            public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
            public int PurchaseId { get => _purchaseId; set { _purchaseId = value; OnPropertyChanged(); } }
            public List<PurchaseProduct> PurchaseProducts { get => _purchaseProducts; set { _purchaseProducts = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public class ViewModel : INotifyPropertyChanged
        {
            int _currentPage = 0, _pageSize = 0, _totalPage = 0, _totalItems = 0;
            public List<Purchase> Order { get; set; } = new List<Purchase>();
            public BindingList<Purchase> SelectedOrders { get; set; } = new BindingList<Purchase>();
            
            public int CurrentPage { get => _currentPage; set { _currentPage = value; OnPropertyChanged(); } }
            public int PageSize { get => _pageSize; set { _pageSize = value; OnPropertyChanged(); } }
            public int TotalPage { get => _totalPage; set { _totalPage = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

    }
}
