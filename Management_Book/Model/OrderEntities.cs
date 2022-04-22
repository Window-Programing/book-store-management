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

        private OrderEntities()
        {
            var server = AppConfig.getValue(AppConfig.Server);
            var database = AppConfig.getValue(AppConfig.Database);
            var username = AppConfig.getValue(AppConfig.Username);
            var password = AppConfig.getValue(AppConfig.Password);

            string connectionString =
                $"Server={server};Database={database};User Id={username};Password={password};MultipleActiveResultSets=true;";

            connection = new SqlConnection(connectionString);
        }
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
            static public string Profit = "profit";
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
            var sql = $"SELECT * FROM Purchase ORDER BY create_at DESC";

            SqlCommand command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            List<OrderModel.Purchase> listPurchase = new List<OrderModel.Purchase>();

            while (reader.Read())
            {
                listPurchase.Add(new OrderModel.Purchase()
                {
                    Id = reader.GetInt32(0),
                    Total = reader.GetDouble(1),
                    CreateDate = reader.GetDateTime(2),
                    CustomerId = reader.GetInt32(3),
                    Status = reader.GetInt32(4),
                });
            }

            return listPurchase;
        }

        public List<OrderModel.Purchase> getPurchasesFilterByDate(DateTime fromDate, DateTime toDate)
        {
            var sql = $"SELECT * FROM Purchase " +
                        $"WHERE {PurchaseTableField.CreateAt} >= @fromDate and {PurchaseTableField.CreateAt} <= @toDate " +
                        $"ORDER BY create_at DESC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fromDate", fromDate);
            command.Parameters.AddWithValue("@toDate", toDate);

            var reader = command.ExecuteReader();

            List<OrderModel.Purchase> listPurchase = new List<OrderModel.Purchase>();

            while (reader.Read())
            {
                listPurchase.Add(new OrderModel.Purchase()
                {
                    Id = reader.GetInt32(0),
                    Total = reader.GetDouble(1),
                    Profit = reader.GetDouble(5),
                    CreateDate = reader.GetDateTime(2),
                    CustomerId = reader.GetInt32(3),
                    Status = reader.GetInt32(4),
                });
            }

            return listPurchase;
        }
        public List<OrderModel.Purchase> getTotalProfitFilterByDate(DateTime fromDate, DateTime toDate)
        {
            var sql = $"select distinct DATEADD(dd, 0, DATEDIFF(dd, 0, pc.create_at)) as Date, SUM(pc.total) over (partition by DATEADD(dd, 0, DATEDIFF(dd, 0, pc.create_at))) as total, SUM(pc.profit) over (partition by convert(varchar(10), pc.create_at, 120)) as profit from Purchase pc WHERE {PurchaseTableField.CreateAt} >= @fromDate and {PurchaseTableField.CreateAt} <= @toDate ";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fromDate", fromDate);
            command.Parameters.AddWithValue("@toDate", toDate);

            var reader = command.ExecuteReader();

            List<OrderModel.Purchase> listPurchase = new List<OrderModel.Purchase>();

            while (reader.Read())
            {
                listPurchase.Add(new OrderModel.Purchase()
                {
                    Total = reader.GetDouble(1),
                    Profit = reader.GetDouble(2),
                    CreateDate = reader.GetDateTime(0),
                });
            }

            return listPurchase;
        }
        public List<OrderModel.ReportProduct> getReportProductFilterByDate(DateTime fromDate, DateTime toDate)
        {
            var sql = $"SELECT distinct pd.product_id, pd.product_name, SUM(pcd.quantity) over (partition by pcd.product_id) as quantity_sale " +
                $"FROM Product as pd, Purchase as pc, PurchaseDetail as pcd " +
                $"WHERE pc.create_at >= @fromDate and pc.create_at <= @toDate and pc.purchase_id = pcd.purchase_id and pd.product_id = pcd.product_id " +
                $"ORDER BY quantity_sale DESC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fromDate", fromDate);
            command.Parameters.AddWithValue("@toDate", toDate);

            var reader = command.ExecuteReader();

            List<OrderModel.ReportProduct> listProduct = new List<OrderModel.ReportProduct>();

            while (reader.Read())
            {
                listProduct.Add(new OrderModel.ReportProduct()
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Quantity = reader.GetInt32(2),
                });
            }

            return listProduct;
        }
        public List<OrderModel.ReportProduct> getSingleProductFilterByDate(int productId, DateTime fromDate, DateTime toDate)
        {
            var sql = $"SELECT distinct pd.product_id, pd.product_name, DATEADD(dd, 0, DATEDIFF(dd, 0, pc.create_at)) as Date, SUM(pcd.quantity) over (partition by pcd.product_id, DATEADD(dd, 0, DATEDIFF(dd, 0, pc.create_at))) as quantity_sale " +
                $"FROM Product as pd, Purchase as pc, PurchaseDetail as pcd " +
                $"WHERE pc.create_at >= @fromDate and pc.create_at <= @toDate and pc.purchase_id = pcd.purchase_id and pd.product_id = pcd.product_id and pd.product_id = @productId " +
                $"ORDER BY Date ASC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fromDate", fromDate);
            command.Parameters.AddWithValue("@toDate", toDate);
            command.Parameters.AddWithValue("@productId", productId);

            var reader = command.ExecuteReader();

            List<OrderModel.ReportProduct> listProduct = new List<OrderModel.ReportProduct>();

            while (reader.Read())
            {
                listProduct.Add(new OrderModel.ReportProduct()
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Quantity = reader.GetInt32(3),
                    CreateDate = reader.GetDateTime(2),
                });
            }

            return listProduct;
        }
        public List<OrderModel.Purchase> getPurchasesFilterByStatus(int statusValue)
        {
            var sql = $"SELECT * FROM Purchase " +
                        $"WHERE {PurchaseTableField.Status} = @status " +
                        $"ORDER BY create_at DESC";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@status", statusValue);

            var reader = command.ExecuteReader();

            List<OrderModel.Purchase> listPurchase = new List<OrderModel.Purchase>();

            while (reader.Read())
            {
                listPurchase.Add(new OrderModel.Purchase()
                {
                    Id = reader.GetInt32(0),
                    Total = reader.GetDouble(1),
                    CreateDate = reader.GetDateTime(2),
                    CustomerId = reader.GetInt32(3),
                    Status = reader.GetInt32(4),
                });
            }

            return listPurchase;
        }

        public void updatePurchase(OrderModel.Purchase targetpurchase)
        {
            var sql = $"UPDATE Purchase SET {PurchaseTableField.CreateAt} = @create_at, {PurchaseTableField.Total} = @total, " +
                $"{PurchaseTableField.CustomerTel} =  @tel, {PurchaseTableField.Status} = @stat " +
                $"WHERE {PurchaseTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", targetpurchase.Id);
            command.Parameters.AddWithValue("@create_at", targetpurchase.CreateDate);
            command.Parameters.AddWithValue("@total", targetpurchase.Total);
            command.Parameters.AddWithValue("@tel", targetpurchase.CustomerId);
            command.Parameters.AddWithValue("@stat", targetpurchase.Status);

            command.ExecuteNonQuery();
        }

        public void updatePurchaseStatus(int purchaseId, int statusValue)
        {
            var sql = $"UPDATE Purchase SET {PurchaseTableField.Status} = @statusValue " +
                $"WHERE {PurchaseTableField.ID} = @id";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", purchaseId);
            command.Parameters.AddWithValue("@statusValue", statusValue);

            command.ExecuteNonQuery();
        }

        public void deletePurchase(int id)
        {
            var sql = $"DELETE FROM Purchase WHERE {PurchaseTableField.ID} = @id;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
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
                    Price = reader.GetDouble(3),
                    Quantity = reader.GetInt32(4),
                    Total = reader.GetDouble(5),
                    Name = reader.GetString(7),
                });
            }

            return listPurchaseProduct;
        }

        public void deleteProductPurchaseDetail(int purchaseId)
        {
            var sql = $"DELETE FROM PurchaseDetail WHERE {PurchaseDetailTableField.PurchaseID} = @id;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", purchaseId);

            command.ExecuteNonQuery();
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

        public void updateCustomer(OrderModel.Customer customer)
        {
            var sql = $"UPDATE Customer SET {CustomerTableField.Name} = @name, {CustomerTableField.Address} = @address, " +
                    $"{CustomerTableField.Email} = @email " +
                    $"WHERE {CustomerTableField.Tel} like @tel";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", customer.Name);
            command.Parameters.AddWithValue("@address", customer.Address);
            command.Parameters.AddWithValue("@email", customer.Email);
            command.Parameters.AddWithValue("@tel", customer.Tel);

            command.ExecuteNonQuery();
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
