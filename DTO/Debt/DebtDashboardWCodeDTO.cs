namespace CoolectorAPI.DTO.Debt
{
    public class DebtDashboardWCodeDTO
    {
        public int Code { get; set; }
        public string ClientName { get; set; }  // Name of the client
        public string Status { get; set; }      // Status of the debt (e.g., 'pending')
        public decimal Amount { get; set; }     // Amount of the debt
        public DateTime IssueDate { get; set; } // Date the debt was issued
        public DateTime ExpDate { get; set; }   // Expiration date of the debt
    }
}
