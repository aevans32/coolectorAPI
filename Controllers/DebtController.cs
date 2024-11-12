using CoolectorAPI.DTO.Debt;
using CoolectorAPI.Models;
using CoolectorAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CoolectorAPI.Controllers
{
    [Route("api/debt")]
    [ApiController]
    public class DebtController : ControllerBase
    {

        private readonly DebtRepository _debtRepository;

        /// <summary>
        /// Constructor initializes a new instance of the <see cref="DebtController"/> class.
        /// </summary>
        /// <param name="repository">An instance of <see cref="DebtRepository"/> for handling SQL queries related to debts.</param>
        public DebtController(DebtRepository debtRepository)
        { 
            _debtRepository = debtRepository;
        }
        // ==================== end of constructor ====================


        /// <summary>
        /// Retrieves all debts for the dashboard including the code for each one.
        /// </summary>
        /// <returns>A list of debts formatted for the dashboard.</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<DebtDashboardWCodeDTO>>> GetAllDebtsForDashboard()
        { 
            var debts = await _debtRepository.GetAllDebtsForDashboardAsync();
            return Ok(debts);
        }
        // ==================== end of GetAllDebtsForDashboard GET call ====================

        
        /// <summary>
        /// Creates a new debt.
        /// </summary>
        /// <param name="debtDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<ActionResult<Int32>> AddDebt(DebtDashboardDTO debtDto)
        {
            var newDebt = new Debt
            {
                ClientName = debtDto.ClientName,
                Status = debtDto.Status,
                Amount = debtDto.Amount,
                IssueDate = debtDto.IssueDate,
                ExpDate = debtDto.ExpDate
            };
                                
            int newCode = await _debtRepository.AddDebtAsync(newDebt);
            return Ok(new { Code = newCode });
        }


        /// <summary>
        /// Placeholder for updating a debt. To be implemented.
        /// </summary>
        /// <returns>Status indicating the result of the update.</returns>
        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateDebt(int code, [FromBody] DebtDashboardDTO debtDto)
        {
            // Placeholder for updating debt by code.
            // Implementation will be added in the future.
            return Ok(new { message = $"Update functionality for debt with code {code} will be implemented." });
        }
        // ==================== end of UpdateDebt PUT call ====================


        /// <summary>
        /// Method to delete a list of debts.
        /// </summary>
        /// <returns> 204 No Content indicates successful deletion with no response body.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDebts([FromBody] List<int> codes)
        {
            if (codes == null || !codes.Any())
            { 
                return  BadRequest("No codes provided for deletion.");
            }

            await _debtRepository.DeleteDebtsAsync(codes);
            return NoContent();
        }
        // ==================== end of DeleteDebts DELETE call ====================

    }
}
