using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book.Model
{
    public class OrderEntities
    {
        public static string PurchaseTable = "Purchase";
        public static string PurchaseDetailTable = "PurchaseDetail";
        public static string CustomerTable = "Customer";
        static string connectionString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ToString();
        static SqlConnection connection = new SqlConnection(connectionString);

        private static OrderEntities Instance;
        private static readonly object _lock = new object();
        public static OrderEntities getInstance()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new OrderEntities();
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

        public class CustomerTableField
        {
            static public string ID = "customer_id";
            static public string Name = "customer_name";
            static public string Address = "address";
            static public string Email = "email";
            static public string Tel = "tel";
        }

        public class PurchaseTableField
        {
            static public string ID = "purchase_id";
            static public string CreateAt = "create_at";
            static public string Total = "total";
            static public string CustomerTel = "customer_id";
            static public string Status = "status";
        }

        public class PurchaseDetailTableField
        {
            static public string ID = "purchaseDetail_id";
            static public string PurchaseID = "purchase_id";
            static public string ProductID = "product_id";
            static public string Price = "price";
            static public string Quantity = "quantity";
            static public string Total = "total";
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
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            catch (Exception ex) { string message = ex.Message; }
        }

        public void resetTable(string tableName)
        {
            if (tableName == PurchaseTable || tableName == PurchaseDetailTable)
            {
                var sql = $"DELETE {tableName}; DBCC CHECKIDENT('{tableName}', RESEED, 0)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@tableName", SqlDbType.NText).Value = tableName;

                command.ExecuteNonQuery();
            }
        }

        public int insertPurchase(OrderModel.Purchase purchase)
        {
            var sql = $"INSERT INTO Purchase ({PurchaseTableField.CreateAt}, {PurchaseTableField.Total}, {PurchaseTableField.CustomerTel}, {PurchaseTableField.Status}) " +
                $"VALUES(@create_at, @total, @tel, @stat); SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@create_at", DateTime.Now);
            command.Parameters.AddWithValue("@total", purchase.Total);
            command.Parameters.AddWithValue("@tel", purchase.CustomerId);
            command.Parameters.AddWithValue("@stat", PurchaseStatus.New);

            int id = Convert.ToInt32(command.ExecuteScalar());
            return id;
        }

        public List<OrderModel.Purchase> getAllPurchase()
        {
            var sql = $"SELECT * FROM Purchase";

            SqlCommand command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            List<OrderModel.Purchase> listPurchase = new List<OrderModel.Purchase>();

            while (reader.Read())
            {
                listPurchase.Add(new OrderModel.Purchase()
                {
                    Id = reader.GetInt32(0),
                    Total = (float)reader.GetDouble(1),
                    CreateDate = reader.GetDateTime(2),
                    CustomerId = reader.GetInt32(3),
                    Status = reader.GetInt32(4),
                });
            }

            return listPurchase;
        }

        public void insertPurchaseDetail(OrderModel.PurchaseProduct purchaseProduct)
        {
            var sql = $"INSERT INTO PurchaseDetail ({PurchaseDetailTableField.PurchaseID}, {PurchaseDetailTableField.ProductID}, {PurchaseDetailTableField.Price}, {PurchaseDetailTableField.Quantity}, {PurchaseDetailTableField.Total}) " +
                $"VALUES(@purchase_id, @product_id, @price, @quantity, @total)";



            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@purchase_id", purchaseProduct.PurchaseId);
            command.Parameters.AddWithValue("@product_id", purchaseProduct.ProductId);
            command.Parameters.AddWithValue("@price", purchaseProduct.Price);
            command.Parameters.AddWithValue("@quantity", purchaseProduct.Quantity);
            command.Parameters.AddWithValue("@total", purchaseProduct.Total);
            Debug.WriteLine(command.CommandText.ToString());
            command.ExecuteNonQuery();
        }

        public List<OrderModel.PurchaseProduct> getPurchaseProductOf(int purchaseId)
        {
            var sql = $"SELECT * FROM PurchaseDetail join Product on PurchaseDetail.product_id = Product.product_id" +
                $" WHERE {PurchaseDetailTableField.PurchaseID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", purchaseId);
            var reader = command.ExecuteReader();

            List<OrderModel.PurchaseProduct> listPurchaseProduct = new List<OrderModel.PurchaseProduct>();

            while (reader.Read())
            {
                listPurchaseProduct.Add(new OrderModel.PurchaseProduct()
                {
                    PurchaseId = reader.GetInt32(1),
                    ProductId = reader.GetInt32(2),
                    Price = (float) reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Total = (float)reader.GetDouble(5),
                    Name = reader.GetString(7),
                });
            }

            return listPurchaseProduct;
        }

        public int insertCustomer(OrderModel.Customer customer)
        {
            OrderModel.Customer oldCustomer = getCustomerByTel(customer.Tel);
            if (oldCustomer.Tel == null)
            {
                var sql = $"INSERT INTO Customer ({CustomerTableField.Name}, {CustomerTableField.Address}, {CustomerTableField.Email}, {CustomerTableField.Tel}) " +
                                $"VALUES(@name, @address, @email, @tel); SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@name", customer.Name);
                command.Parameters.AddWithValue("@address", customer.Address);
                command.Parameters.AddWithValue("@email", customer.Email);
                command.Parameters.AddWithValue("@tel", customer.Tel);

                int id = Convert.ToInt32(command.ExecuteScalar());
                return id;
            }
            else
            {
                return oldCustomer.Id;
            }
        }

        public OrderModel.Customer getCustomerByTel(string tel)
        {
            var sql = $"SELECT * FROM Customer WHERE {CustomerTableField.Tel} LIKE @tel";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@tel", tel);
            var reader = command.ExecuteReader();

            var customer = new OrderModel.Customer();

            while (reader.Read())
            {
                customer.Id = reader.GetInt32(0);
                customer.Name = reader.GetString(1);
                customer.Address = reader.GetString(2);
                customer.Email = reader.GetString(3);
                customer.Tel = reader.GetString(4);
            }

            return customer;
        }

        public OrderModel.Customer getCustomerById(int customerId)
        {
            var sql = $"SELECT * FROM Customer WHERE {CustomerTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", customerId);
            var reader = command.ExecuteReader();

            var customer = new OrderModel.Customer();

            while (reader.Read())
            {
                customer.Id = reader.GetInt32(0);
                customer.Name = reader.GetString(1);
                customer.Address = reader.GetString(2);
                customer.Email = reader.GetString(3);
                customer.Tel = reader.GetString(4);
            }

            return customer;
        }

        internal List<OrderModel.PurchaseStatusEnum> getAllStatusEnum()
        {
            var sql = $"SELECT * FROM PurchaseStatusEnum";

            SqlCommand command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            List<OrderModel.PurchaseStatusEnum> statusEnum = new List<OrderModel.PurchaseStatusEnum>();

            while (reader.Read())
            {
                statusEnum.Add(new OrderModel.PurchaseStatusEnum()
                {
                    Key = reader.GetString(0),
                    Value = reader.GetInt32(1),
                    Description = reader.GetString(2),
                    DisplayText = reader.GetString(3)
                });
            }

            return statusEnum;
        }
    }
}
