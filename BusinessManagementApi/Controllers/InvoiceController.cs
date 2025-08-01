using BusinessManagementApi.DTOs;
using BusinessManagementApi.Models;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessManagementApi.Controllers
{
    /// <summary>
    /// Manages invoice-related APIs. Requires JWT authentication.
    /// </summary>
    [Route("api/invoice")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _service;

        /// <summary>
        /// Injects the InvoiceService.
        /// </summary>
        public InvoiceController(InvoiceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Adds a new invoice.
        /// </summary>
        
        [HttpPost("add")]
        public async Task<IActionResult> AddInvoice([FromBody] AddInvoiceDto dto)
        {
            try
            {
                var createdById = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "Unknown";
                var createdByName = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "Unknown";

                var newInvoice = await _service.AddInvoiceAsync(dto, createdById, createdByName);
                return Ok(new { data = newInvoice });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error occurred", error = ex.Message }); 
            }
        }


        /// <summary>
        /// Updates an existing invoice by its ID.
        /// </summary>
        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateInvoice(string id, [FromBody] AddInvoiceDto dto)
        {
            var createdById = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "Unknown";
            var createdByName = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "Unknown";

            var result = await _service.UpdateInvoiceAsync(id, dto, createdById, createdByName);
            return Ok(result);
        }


        /// <summary>
        /// Deletes an invoice by its ID.
        /// </summary>

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteInvoice(string id)
        {
            var createdById = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "Unknown";
            var createdByName = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "Unknown";


            var result = await _service.DeleteInvoiceAsync(id, createdById, createdByName);

            return Ok(result);
        }


        /// <summary>
        /// Gets all invoices. Can filter by service name and date range.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetInvoices([FromQuery] string? service, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var invoices = await _service.GetInvoicesAsync(service, startDate, endDate);
            return Ok(invoices);
        }

        /// <summary>
        /// Gets all invoices for a specific business by its ID.
        /// </summary>
        [HttpGet("byBusiness/{businessId}")]
        public async Task<IActionResult> GetInvoicesByBusiness(string businessId)
        {
            if (string.IsNullOrEmpty(businessId))
                return BadRequest(new { message = "Business ID is required." });

                
            try
            {
                var invoices = await _service.GetInvoicesByBusinessIdAsync(businessId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching invoices: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching invoices", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets history for a specific business by its Id
        /// </summary>
        [HttpGet("history/{businessId}")]
        public async Task<IActionResult> GetInvoiceHistory(string businessId)
        {
            var history = await _service.GetInvoiceHistoryAsync(businessId);
            return Ok(history);
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns></returns>
        [HttpGet("getInvoice")]
        public async Task<IActionResult> GetInvoiceDetails()
        {
            var invoices = await _service.GetAllInvoicesAsync();

            return Ok(invoices);
        }

         /// <summary>
         /// adding Pagination
         /// </summary>
         /// <param name="page"></param>
         /// <param name="pageSize"></param>
         /// <param name="search"></param>
         /// <param name="startDate"></param>
         /// <param name="endDate"></param>
         /// <returns></returns>
        
        [HttpGet("PaginatedInvoice")]
        public async Task<IActionResult> GetInvoices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? search=null,
            [FromQuery] DateTime? startDate=null, 
            [FromQuery] DateTime? endDate = null
        )
        {
            var (invoices, totalCount) = await _service.GetInvoicesPaginatedAsync(page, pageSize, search, startDate, endDate);

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data = invoices,
            });
        }

    }
}