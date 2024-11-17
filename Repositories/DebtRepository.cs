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


        /// <summary>
        /// Retrieves a list of pending debts for a specific client by their name.
        /// </summary>
        /// <param name="clientName">The exact name of the client to filter debts for.</param>
        /// <returns>
        /// A list of <see cref="DebtDashboardWCodeDTO"/> objects representing the pending debts
        /// for the specified client. Each object includes the debt code, client name, amount,
        /// issue date, and expiration date.
        /// </returns>
        /// <remarks>
        /// - The query filters debts where the <c>Status</c> is "pending".
        /// - The <c>Status</c> column is not returned in the results, as it is assumed to be constant.
        /// - Ensure that <paramref name="clientName"/> is a valid and non-null string to avoid errors.
        /// </remarks>
        /// <exception cref="SqlException">
        /// Thrown if there is an issue with the database connection or query execution.
        /// </exception>
        /// <example>
        /// Example usage:
        /// <code>
        /// var debts = await _debtRepository.GetPendingDebtsByClientNameAsync("Robert");
        /// </code>
        /// </example>
        public async Task<List<DebtDashboardWCodeDTO>> GetPendingDebtsByClientNameAsync(string clientName)
        {
            var debts = new List<DebtDashboardWCodeDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $@"
                    SELECT Code, ClientName, Status, Amount, IssueDate, ExpDate
                    FROM {TableName} 
                    WHERE ClientName = @ClientName AND Status = @Status";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClientName", clientName); // Filter by ClientName
                command.Parameters.AddWithValue("@Status", "pending");     // Filter by Status

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
                        ExpDate = reader.GetDateTime(5),
                    };
                    debts.Add(debt);
                }

                await reader.CloseAsync();
            }

            return debts;
        }



        public async Task UpdateDebtStatusAsync(List<int> codes, string status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = $@"
            UPDATE {TableName} 
            SET Status = @Status
            WHERE Code IN ({string.Join(",", codes)})";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Status", status);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }




    }
}
