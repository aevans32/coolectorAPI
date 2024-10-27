namespace CoolectorAPI.Models
{
    public class Debt
    {
        public int Code { get; set; }          // Primary Key
        public int ClientID { get; set; }      // Foreign Key or reference to the client
        public string ClientName { get; set; } // Name of the client
        public decimal Amount { get; set; }    // Amount of debt
        public DateTime IssueDate { get; set; } // Date the debt was issued
        public DateTime ExpDate { get; set; }   // Expiration date of the debt
        public string Status { get; set; }      // Status of the debt (e.g., 'pending')
        public int CompanyCode { get; set; }    // Foreign Key to the company
        public int ServiceCode { get; set; }    // Foreign Key to the service
    }
}
