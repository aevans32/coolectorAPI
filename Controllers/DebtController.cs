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
            Console.WriteLine(debtDto);

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
        public async Task<IActionResult> DeleteDebts([FromBody] CodeRequestDTO request)
        {
            if (request.Codes == null || !request.Codes.Any())
            { 
                return  BadRequest("No codes provided for deletion.");
            }

            await _debtRepository.DeleteDebtsAsync(request.Codes);
            return NoContent();

            // Modify the response of DeleteDebtsAsync to make the following return a result to the frontend
            //var deletedCodes = await _debtRepository.DeleteDebtsAsync(request.Codes);
            //return Ok(new { message = "Deletion successful", deletedCodes = deletedCodes });
        }
        // ==================== end of DeleteDebts DELETE call ====================




        /// <summary>
        /// Retrieves all pending debts for a specific client based on their name.
        /// </summary>
        /// <param name="clientNameDto">The JSON object containing the client's name.</param>
        /// <returns>
        /// A list of pending debts for the specified client, including the debt code, 
        /// client name, amount, issue date, and expiration date.
        /// </returns>
        /// <remarks>
        /// - The <paramref name="clientNameDto"/> must include a valid client name in the `ClientName` property.
        /// - This endpoint only retrieves debts with a status of "pending".
        /// - The client's name must match exactly with the values stored in the database.
        /// - Use this endpoint for filtered debt retrieval by client name; for retrieving all debts, use `/dashboard`.
        /// </remarks>
        /// <response code="200">Returns the list of pending debts for the specified client.</response>
        /// <response code="400">If the client name is null, empty, or invalid.</response>
        /// <response code="500">If an internal server error occurs while processing the request.</response>
        [HttpPost("filter-by-client")]
        public async Task<ActionResult<IEnumerable<DebtDashboardWCodeDTO>>> GetDebtsByClientName([FromBody] ClientNameDTO clientNameDto)
        {
            if (string.IsNullOrWhiteSpace(clientNameDto.ClientName))
            {
                return BadRequest("Client name is required.");
            }

            // Call the repository method
            var debts = await _debtRepository.GetPendingDebtsByClientNameAsync(clientNameDto.ClientName);

            // Return the result
            return Ok(debts);
        }



        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateDebtStatus([FromBody] UpdateDebtStatusDTO updateRequest)
        {
            if (updateRequest == null || updateRequest.Codes == null || !updateRequest.Codes.Any())
            {
                return BadRequest("Invalid request: No codes provided.");
            }

            if (string.IsNullOrEmpty(updateRequest.Status))
            {
                return BadRequest("Invalid request: Status is required.");
            }

            try
            {
                await _debtRepository.UpdateDebtStatusAsync(updateRequest.Codes, updateRequest.Status);
                return Ok(new { message = "Debt statuses updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating debt statuses:", ex);
                return StatusCode(500, new { error = "An error occurred while updating debts.", details = ex.Message } );
            }
        }




    }
}
