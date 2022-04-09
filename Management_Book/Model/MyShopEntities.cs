using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book.Model
{
    public class MyShopEntities
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ToString();
        static SqlConnection connection = new SqlConnection(connectionString);

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
        public static void openConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex) { string message = ex.Message; }
        }
        public static void closeConnection()
        {
            try
            {
                if (connection != null)
                    connection.Close();
            }
            catch (Exception ex) { string message =  ex.Message; }
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
            command.Parameters.Add("@category", SqlDbType.Int).Value = product.Category_ID;

            command.ExecuteNonQuery();
        }
        public void updateProduct(int productIDTarget, string name, int price, int quantity, string image, int category)
        {
            var sql = $"UPDATE Product SET {ProductTableField.Name}=@name, {ProductTableField.Price}= @price," +
                $" {ProductTableField.Quantity} = @quantity, {ProductTableField.Image} = @image, {ProductTableField.Category}= @category" +
                $"WHERE {ProductTableField.ID}=@id" +
                $"VALUES(@name, @price, @quantity, @image, @category)";

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
            var sql = $"DELETE FROM Product WHERE {ProductTableField.ID}=@id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            command.ExecuteNonQuery();
        }
        //public void insertCategory(MyShopModel.Category category)
        //{
        //    var sql = $"INSERT INTO Category({CategoryTableField.Name}) VALUES(@name)";

        //    SqlCommand command = new SqlCommand(sql, connection);
        //    command.Parameters.Add("@name", SqlDbType.NText).Value = category.Name;

        //    command.ExecuteNonQuery();
        //}
        //public void updateCategory(int productIDTarget, string name, int price, int quantity, string image, int category)
        //{
        //    var sql = $"UPDATE Product SET {ProductTableField.Name}=@name, {ProductTableField.Price}= @price," +
        //        $" {ProductTableField.Quantity} = @quantity, {ProductTableField.Image} = @image, {ProductTableField.Category}= @category" +
        //        $"WHERE {ProductTableField.ID}=@id" +
        //        $"VALUES(@name, @price, @quantity, @image, @category)";

        //    SqlCommand command = new SqlCommand(sql, connection);
        //    command.Parameters.Add("@id", SqlDbType.Int).Value = productIDTarget;
        //    command.Parameters.Add("@name", SqlDbType.NText).Value = name;
        //    command.Parameters.Add("@price", SqlDbType.Int).Value = price;
        //    command.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
        //    command.Parameters.Add("@image", SqlDbType.Text).Value = image;
        //    command.Parameters.Add("@category", SqlDbType.Int).Value = category;

        //    command.ExecuteNonQuery();
        //}
        //public void deleteCategory(int id)
        //{
        //    var sql = $"DELETE FROM Product WHERE {ProductTableField.ID}=@id";

        //    SqlCommand command = new SqlCommand(sql, connection);
        //    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

        //    command.ExecuteNonQuery();
        //}
    }
}
