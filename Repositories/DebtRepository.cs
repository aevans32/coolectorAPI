using CoolectorAPI.DTO.Debt;
using CoolectorAPI.Models;
using Microsoft.Data.SqlClient;

namespace CoolectorAPI.Repositories
{
    public class DebtRepository
    {
        private readonly string _connectionString;
        private const string TableName = "dbo.Debt";

        public DebtRepository(string connectionString)
        { 
            _connectionString = connectionString;
        }

        /// <summary>
        /// GET ALL DEBTS FOR DASHBOARD VIEW WITH DTO
        /// </summary>
        /// <returns></returns>
        public async Task<List<DebtDashboardWCodeDTO>> GetAllDebtsForDashboardAsync() 
        {
            var debts = new List<DebtDashboardWCodeDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                string sql = $@"SELECT Code, ClientName, Status, Amount, IssueDate, ExpDate FROM {TableName}";

                SqlCommand command = new SqlCommand(sql, connection);

                await connection.OpenAsync();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync()) 
                {
                    var debt = new DebtDashboardWCodeDTO
                    {
                        Code = reader.GetInt32(0),
                        ClientName = reader.GetString(1),
                        Status = reader.GetString(2),
                        Amount = reader.GetDecimal(3),
                        IssueDate = reader.GetDateTime(4),
                        ExpDate = reader.GetDateTime(5)
                    };
                    debts.Add(debt);
                }
                // End of while loop

                await reader.CloseAsync();
            }

            return debts;
        }
        // ---------------------------- END OF GetAllDebtsForDashboardAsync-----------------------------------



        /// <summary>
        /// Adds a new debt entry to the database.
        /// </summary>
        /// <param name="newDebt">The debt data to be added.</param>
        /// <returns>The added debt, including any auto-generated fields like ID if applicable.</returns>
        public async Task<Int32> AddDebtAsync(Debt newDebt)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $@"
            INSERT INTO dbo.Debt (ClientName, Amount, IssueDate, ExpDate, Status)
            VALUES (@ClientName, @Amount, @IssueDate, @ExpDate, @Status);
            SELECT SCOPE_IDENTITY();"; // Get the new code

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@ClientName", newDebt.ClientName);
                command.Parameters.AddWithValue("@Amount", newDebt.Amount);
                command.Parameters.AddWithValue("@IssueDate", newDebt.IssueDate);
                command.Parameters.AddWithValue("@ExpDate", newDebt.ExpDate);
                command.Parameters.AddWithValue("@Status", newDebt.Status);

                await connection.OpenAsync();

                var newCode = await command.ExecuteScalarAsync();
                if (newCode != null && int.TryParse(newCode.ToString(), out int code))
                {
                    newDebt.Code = code; // Update the DTO with the new code
                }

                return newDebt.Code; // Return the new debt code
            }
        }


        public async Task DeleteDebtsAsync(List<int> codes)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $@"
                    DELETE FROM {TableName}
                    WHERE Code IN ({string.Join(",", codes.Select((_, i) =>  $"@Code{i}"))})";

                SqlCommand command = new SqlCommand(sql, connection);

                // Add each code as a parameter
                for (int i = 0; i < codes.Count; i++)
                {
                    command.Parameters.AddWithValue($"@Code{i}", codes[i]);
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }


    }
}
