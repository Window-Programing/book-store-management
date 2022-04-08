using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


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
            catch (Exception ex) { }
        }

        public void truncateTable(string tableName)
        {
            if(tableName == ProductTable || tableName == CategoryTable)
            {
                var sql = $"TRUNCATE TABLE {tableName}";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@tableName", SqlDbType.NText).Value = tableName;

                command.ExecuteNonQuery();
            }
        }

        public void insertProduct(MyShopModel.Product product)
        {
            var sql = $"INSERT INTO Product({ProductTableField.Name}, {ProductTableField.Price}, {ProductTableField.Quantity}, {ProductTableField.Image}, {ProductTableField.Category}) " +
                $"VALUES(@name, @price, @quantity, @image, @category)";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@name", SqlDbType.NText).Value = product.Name;
            command.Parameters.Add("@price", SqlDbType.Int).Value = product.Price;
            command.Parameters.Add("@quantity", SqlDbType.Int).Value = product.Quantity;
            command.Parameters.Add("@image", SqlDbType.Text).Value = product.Image;
            command.Parameters.Add("@category", SqlDbType.Int).Value = product.Category.Id;

            command.ExecuteNonQuery();
        }
        public void updateProduct(int productIDTarget, string name, int price, int quantity, string image, int category)
        {
            var sql = $"UPDATE Product SET {ProductTableField.Name}=@name, {ProductTableField.Price}= @price," +
                $" {ProductTableField.Quantity} = @quantity, {ProductTableField.Image} = @image, {ProductTableField.Category}= @category" +
                $"WHERE {ProductTableField.ID}=@id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = productIDTarget;
            command.Parameters.Add("@name", SqlDbType.NText).Value = name;
            command.Parameters.Add("@price", SqlDbType.Int).Value = price;
            command.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
            command.Parameters.Add("@image", SqlDbType.Text).Value = image;
            command.Parameters.Add("@category", SqlDbType.Int).Value = category;

            command.ExecuteNonQuery();
        }
        public void deleteProduct(int id)
        {
            var sql = $"DELETE FROM Product WHERE {ProductTableField.ID}=@id;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            command.ExecuteNonQuery();
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
                categories.Add(new MyShopModel.Category() { Name = reader[CategoryTableField.Name].ToString() });
            }

            command.Cancel();

            return categories;
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
                    Name = reader.GetString(1),
                    Price = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    Image = reader.GetString(4),
                    Category = new MyShopModel.Category() { Id = reader.GetInt32(5) }
                });
            }

            return products;
        }

        public List<MyShopModel.Product> getProductsOf(string categoryName)
        {
            var sql = $"SELECT * FROM Product JOIN Category on Product.category = Category.category_id WHERE Category.category_name LIKE @name";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@name", SqlDbType.NText).Value = categoryName;
            var reader = command.ExecuteReader();

            var products = new List<MyShopModel.Product>();

            while (reader.Read())
            {
                string name = reader.GetString(1);
                products.Add(new MyShopModel.Product()
                {
                    //Name = reader.GetString(1),
                    //Price = reader.GetInt32(2),
                    //Quantity = reader.GetInt32(3),
                    //Image = reader.GetString(4),
                    //Category = new MyShopModel.Category() { Id = reader.GetInt32(5) }
                });
            }
            command.Cancel();

            return products;
        }
    }
}
