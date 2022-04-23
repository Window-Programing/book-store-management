using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Management_Book.Model
{
    internal class UserEntities
    {
        public static string UserTable = "UserInformation";
        static SqlConnection connection;

        private static UserEntities Instance;
        private static readonly object _lock = new object();

        private UserEntities()
        {
            var server = AppConfig.getValue(AppConfig.Server);
            var database = AppConfig.getValue(AppConfig.Database);

            string connectionString =
                $"Server={server};Database={database};Trusted_Connection=True;MultipleActiveResultSets=true;";

            connection = new SqlConnection(connectionString);
        }
        public static UserEntities getInstance()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new UserEntities();
                    }
                }
            }
            return Instance;
        }

        public class UserTableField
        {
            static public string ID = "UserId";
            static public string Username = "Username";
            static public string Password = "Password";
            static public string Entropy = "Entropy";
        }
        public SqlConnection openConnection()
        {
            try
            {
                connection.Open();
                return connection;
            }

            catch (Exception) { }
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
        public void insertUser(string username, string password, string entropy)
        {
            var sql = $"INSERT INTO UserInformation ({UserTableField.Username}, {UserTableField.Password},{UserTableField.Entropy} ) VALUES(@username, @password, @entropy);";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@entropy", entropy);

            command.ExecuteNonQuery();
        }

        public UserModel.Account getUser(string username)
        {
            var sql = $"SELECT * FROM UserInformation WHERE {UserTableField.Username} LIKE @username;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);

            var reader = command.ExecuteReader();
            UserModel.Account acc = new UserModel.Account();
            while (reader.Read())
            {
                acc.UserName = reader.GetString(1);
                acc.Password = reader.GetString(2);
                acc.Entropy = reader.GetString(3);
            }

            return acc;
        }

        public void updatePassword(string username, string password, string entropy)
        {
            var sql = $"UPDATE UserInformation SET {UserTableField.Password} = @password, {UserTableField.Entropy} = @entropy WHERE {UserTableField.Username} = @username;";

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@entropy", entropy);

            command.ExecuteNonQuery();
        }
    }
}
