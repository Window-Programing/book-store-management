using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Management_Book.Model
{
    public class MyShopEntities
    {
        public static string ProductTable = "Product";
        public static string CategoryTable = "Category";
        static string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ToString();
        static SqlConnection connection = new SqlConnection(connectionString);

        private static MyShopEntities Instance;
        private static readonly object _lock = new object();

        private MyShopEntities()
        {

        }
        public static MyShopEntities getInstance()
        {
            if(Instance == null)
            {
                lock (_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new MyShopEntities();
                    }  
                } 
            }
            return Instance;
        }

        enum PurchaseStatus
        {
            All = -1,
            New = 1,
            Cancelled = 2,
            Completed = 3,
            Shipping = 4
        }
        public class CategoryTableField
        {
            static public string ID = "category_id";
            static public string Name = "category_name";
        }
        public class ProductTableField
        {
            static public string ID = "product_id";
            static public string Name = "product_name";
            static public string Price = "price";
            static public string Cost = "cost";
            static public string Quantity = "quantity";
            static public string Image = "image";
            static public string Category = "category";
        }
        public SqlConnection openConnection()
        {
            try
            {
                connection.Open();
                return connection;
            }

            catch (Exception ex) { }
            return null;
        }
        public void closeConnection()
        {
            try
            {
                if (connection.State  == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            catch (Exception ex) { string message =  ex.Message; }
        }

        public void resetTable(string tableName)
        {
            if(tableName == ProductTable || tableName == CategoryTable)
            {
                var sql = $"DELETE {tableName}; DBCC CHECKIDENT('{tableName}', RESEED, 0)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@tableName", SqlDbType.NText).Value = tableName;

                command.ExecuteNonQuery();
            }
        }
        public int insertCategory(MyShopModel.Category category)
        {
            var sql = $"INSERT INTO Category({CategoryTableField.Name}) VALUES(@name); SELECT SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@name", SqlDbType.NText).Value = category.Name;

            int id = Convert.ToInt32(command.ExecuteScalar());

            return id;
        }
        public void updateCategory(int categoryIDTarget, string name)
        {
            var sql = $"UPDATE Category SET {CategoryTableField.Name}=@name WHERE {CategoryTableField.ID}=@id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.NText).Value = categoryIDTarget;
            command.Parameters.Add("@name", SqlDbType.NText).Value = name;
      
            command.ExecuteNonQuery();
        }
        public void deleteCategory(int id)
        {
            var sql = $"DELETE FROM Category WHERE {CategoryTableField.ID}=@id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            command.ExecuteNonQuery();
        }
        public List<MyShopModel.Category> getCategories()
        {
            var sql = $"SELECT * FROM Category";
            
            SqlCommand command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var categories = new List<MyShopModel.Category>();

            while (reader.Read())
            {
                categories.Add(new MyShopModel.Category() { 
                    Id = (int) reader[CategoryTableField.ID],
                    Name = reader[CategoryTableField.Name].ToString() });
            }

            command.Cancel();

            return categories;
        }

        public void insertProduct(MyShopModel.Product product)
        {
            var sql = $"INSERT INTO Product({ProductTableField.Name}, {ProductTableField.Price}, {ProductTableField.Cost}, {ProductTableField.Quantity}, {ProductTableField.Image}, {ProductTableField.Category}) " +
                $"VALUES(@name, @price, @cost, @quantity, @image, @category)";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@cost", product.Cost);
            command.Parameters.AddWithValue("@quantity", product.Quantity);
            command.Parameters.AddWithValue("@image", product.Image);
            command.Parameters.AddWithValue("@category", product.Category.Id);

            command.ExecuteNonQuery();
        }
        public void updateProduct(MyShopModel.Product product)
        {
            var sql = $"UPDATE Product SET {ProductTableField.Name}=@name, {ProductTableField.Price}= @price, " +
                $"{ProductTableField.Cost} = @cost, {ProductTableField.Quantity} = @quantity, " +
                $"{ProductTableField.Image} = @image, {ProductTableField.Category} = @category " +
                $"WHERE {ProductTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", product.Id);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@cost", product.Cost);
            command.Parameters.AddWithValue("@quantity", product.Quantity);
            command.Parameters.AddWithValue("@image", product.Image);
            command.Parameters.AddWithValue("@category", product.Category.Id);

            command.ExecuteNonQuery();
        }
        public void updateQuantityProduct(int productId, int quantity)
        {
            var sql = $"UPDATE Product SET {ProductTableField.Quantity} = @quantity WHERE {ProductTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", productId);
            command.Parameters.AddWithValue("@quantity", quantity);

            command.ExecuteNonQuery();
        }
        public void deleteProduct(int id)
        {
            var sql = $"DELETE FROM Product WHERE {ProductTableField.ID}=@id;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            command.ExecuteNonQuery();
        }
        public List<MyShopModel.Product> getAllProducts()
        {
            var sql = $"SELECT * FROM Product";

            SqlCommand command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                products.Add(new MyShopModel.Product() {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDouble(2),
                    Cost = reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Image = reader.GetString(5),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(6) }
                });
            }

            return products;
        }
        public List<MyShopModel.Product> getProductsLike(int categoryId, string name)
        {
            var sql = $"SELECT * FROM Product WHERE {ProductTableField.Category} = @category AND {ProductTableField.Name} LIKE @name";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@category", categoryId);
            command.Parameters.AddWithValue("@name", name + "%");
            var reader = command.ExecuteReader();

            Debug.WriteLine(command.CommandText.ToString());

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                products.Add(new MyShopModel.Product()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDouble(2),
                    Cost = reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Image = reader.GetString(5),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(6) }
                });
            }

            return products;
        }
        public List<MyShopModel.Product> getProductsLike(string name)
        {
            var sql = $"SELECT * FROM Product WHERE {ProductTableField.Name} LIKE @name";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name + "%");
            var reader = command.ExecuteReader();

            Debug.WriteLine(command.CommandText.ToString());

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                products.Add(new MyShopModel.Product()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDouble(2),
                    Cost = reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Image = reader.GetString(5),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(6) }
                });
            }

            return products;
        }

        public List<MyShopModel.Product> getProductsOf(int categoryId)
        {
            var sql = $"SELECT * FROM Product WHERE {ProductTableField.Category} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", categoryId);
            var reader = command.ExecuteReader();

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                products.Add(new MyShopModel.Product()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = (double)reader.GetDouble(2),
                    Cost = (double)reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Image = reader.GetString(5),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(6) }
                });
            }
            command.Cancel();

            return products;
        }
        public MyShopModel.Product getOneProduct(int productId)
        {
            var sql = $"SELECT * FROM Product WHERE {ProductTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", productId);
            var reader = command.ExecuteReader();

            var products = new MyShopModel.Product();

            while (reader.Read())
            {
                products.Id = reader.GetInt32(0);
                products.Name = reader.GetString(1);
                products.Price = reader.GetDouble(2);
                products.Cost = reader.GetDouble(3);
                products.Quantity = reader.GetInt32(4);
                products.Image = reader.GetString(5);
                products.Category = new MyShopModel.Category() { Id = reader.GetInt32(6) };
            }

            return products;
        }

        public void deleteProductsOf(int categoryId)
        {
            var sql = $"DELETE FROM Product WHERE {ProductTableField.Category} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", categoryId);
            command.ExecuteNonQuery();
            command.Cancel();

        }

        public List<MyShopModel.Product> getProductsFilterByPrice(string name, int categoryId, int fromPrice, int toPrice)
        {
            var sql =$"SELECT * " +
                $"FROM Product " +
                $"WHERE {ProductTableField.Category} = @category AND {ProductTableField.Name} LIKE @name and " +
                $"{ProductTableField.Price} >= @fromPrice and {ProductTableField.Price} <= @toPrice " +
                $"ORDER BY price ASC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@category", categoryId);
            command.Parameters.AddWithValue("@name", name + "%");
            command.Parameters.AddWithValue("@fromPrice", fromPrice);
            command.Parameters.AddWithValue("@toPrice", toPrice);
            var reader = command.ExecuteReader();

            Debug.WriteLine(command.CommandText.ToString());

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                products.Add(new MyShopModel.Product()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = (double)reader.GetDouble(2),
                    Cost = (double)reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Image = reader.GetString(5),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(6) }
                });
            }

            return products;
        }
    }
}
