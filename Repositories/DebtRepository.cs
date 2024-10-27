using CoolectorAPI.DTO.Debt;
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
        public async Task<List<DebtDashboardDTO>> GetAllDebtsForDashboardAsync() 
        {
            var debts = new List<DebtDashboardDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                string sql = @"SELECT ClientName, Status, Amount, IssueDate, ExpDate FROM {TableName}";

                SqlCommand command = new SqlCommand(sql, connection);

                await connection.OpenAsync();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync()) 
                {
                    var debt = new DebtDashboardDTO
                    {
                        ClientName = reader.GetString(0),
                        Status = reader.GetString(1),
                        Amount = reader.GetDecimal(2),
                        IssueDate = reader.GetDateTime(3),
                        ExpDate = reader.GetDateTime(4)
                    };
                    debts.Add(debt);
                }
                // End of while loop

                await reader.CloseAsync();
            }

            return debts;
        }
        // ---------------------------- END OF GetAllDebtsForDashboardAsync-----------------------------------

    }
}
