using BusinessManagementApi.DTOs;
using BusinessManagementApi.Models;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _service.AddInvoiceAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing invoice by its ID.
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateInvoice(string id, [FromBody] UpdateInvoiceDto dto)
        {
            var result = await _service.UpdateInvoiceAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an invoice by its ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInvoice(string id)
        {
            var result = await _service.DeleteInvoiceAsync(id);
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
        [HttpGet("by-business/{businessId}")]
        public async Task<IActionResult> GetInvoicesByBusiness(string businessId)
        {
            var invoices = await _service.GetInvoicesByBusinessIdAsync(businessId);
            return Ok(invoices);
        }
    }
}