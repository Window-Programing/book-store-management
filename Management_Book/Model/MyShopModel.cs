using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Management_Book.Model
{
    public class MyShopModel
    {
        public class Product
        { 
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string Image { get; set; }
            public int Category_ID { get; set; }

            public Product(string name, decimal price, int quantity, string img, int category)
            {
                Name = name;
                Price = price;
                Quantity = quantity;
                Image = img;
                Category_ID = category;
            }

            
        }

        public class Category
        {
            public string Name { get; set; }

            public List<Product> Products { get; set; }
            
        }
    }
}
