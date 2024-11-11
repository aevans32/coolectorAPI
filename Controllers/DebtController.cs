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
        /// GET: api/debt/dashboard
        /// Retrieves all debts for the dashboard.
        /// </summary>
        /// <returns>A list of debts formatted for the dashboard.</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<DebtDashboardDTO>>> GetAllDebtsForDashboard()
        { 
            var debts = await _debtRepository.GetAllDebtsForDashboardAsync();
            return Ok(debts);
        }
        // ==================== end of GetAllDebtsForDashboard GET call ====================

        
        /// <summary>
        /// 
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
        /// Placeholder for deleting a debt. To be implemented.
        /// </summary>
        /// <param name="code">The unique code of the debt to be deleted.</param>
        /// <returns>Status indicating the result of the deletion.</returns>
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteDebt(int code)
        {
            // Placeholder for deleting debt by code.
            // Implementation will be added in the future.
            return Ok(new { message = $"Delete functionality for debt with code {code} will be implemented." });
        }
        // ==================== end of DeleteDebt DELETE call ====================

    }
}
