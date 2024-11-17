namespace CoolectorAPI.DTO.Debt
{
    public class UpdateDebtStatusDTO
    {
        public List<int> Codes { get; set; } // List of debt codes
        public string Status { get; set; }  // New status, e.g., "paid"
    }
}
