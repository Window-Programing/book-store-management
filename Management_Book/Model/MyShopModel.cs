using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Management_Book.Model
{
    public class MyShopModel
    {
        public class Category : INotifyPropertyChanged, INotifyCollectionChanged
        {
            private int _id;
            private string _name;
            private List<Product> _products;
            public int Id
            {
                get => _id;
                set
                {
                    _id = value;
                    OnPropertyChanegd();
                }
            }
            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanegd();
                }
            }

            public List<Product> Products
            {
                get => _products;
                set
                {
                    _products = value;
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            private void OnPropertyChanegd([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class Product : INotifyPropertyChanged
        {
            private int _id;
            private string _name;
            private int _price;
            private int _quantity;
            private string _image;
            private Category _category;

            public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
            public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
            public int Price { get => _price; set { _price = value; OnPropertyChanged(); } }
            public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }
            public string Image { get => _image; set { _image = value; OnPropertyChanged(); } }
            public Category Category { get => _category; set { _category = value; OnPropertyChanged(); } }


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
            public List<Product> Products { get; set; } = new List<Product>();
            public BindingList<Product> SelectedProducts { get; set; } = new BindingList<Product>();

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
