using CoolectorAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CoolectorAPI.Repositories
{
    public class UserRepository
    {

        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /*
            GET ALL USERS COMMAND
        */
        public async Task<List<User>> GetAllUsersAsync()
        { 
            var users = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                //Specify the query
                string sql = "SELECT * FROM dbo.users";

                SqlCommand command = new SqlCommand(sql, connection);

                await connection.OpenAsync();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync()) 
                {
                    var user = new User
                    {
                        FirstName = reader.GetString(0),
                        LastName = reader.GetString(1),
                        Relation = reader.GetString(2),
                        Email = reader.GetString(3),
                        Password = reader.GetString(4),
                        CompanyCode = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        Id = reader.GetInt32(6)
                    };
                    users.Add(user);
                }
                await reader.CloseAsync();
            }

            return users;
        }

        // Async method to ADD User
        public async Task AddUserAsync(User user) 
        {
            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO dbo.users (Email, Password, FirstName, LastName, Relation, CompanyCode)
                                VALUES (@Email, @Password, @FirstName, @LastName, @Relation, NULL)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Relation", user.Relation);

                await command.ExecuteNonQueryAsync();

            }
        }

        // GET ONE USER for SIGN IN
        public async Task<int?> GetUserByCredentialsAsync(string email, string password) 
        {
            int? userCode = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Query to check if a user with matching email and password exists
                string sql = @"SELECT code FROM dbo.users
                                WHERE email = @Email 
                                AND password = @Password";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                await connection.OpenAsync();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync()) 
                {
                    userCode = reader.GetInt32(0);
                }
                await reader.CloseAsync();
            }
            return userCode;
        }


        // ADD MORE SQL QUERIES


    }
}
