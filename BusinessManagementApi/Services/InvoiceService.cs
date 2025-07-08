using BusinessManagementApi.DTOs;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;

namespace BusinessManagementApi.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<string> AddInvoiceAsync(AddInvoiceDto dto)
        {
            var invoice = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                BusinessId = dto.BusinessId,
                Service = dto.Service,
                Description = dto.Description,
                Amount = dto.Amount,
                VatPercentage = dto.VatPercentage,
                TotalAmount = dto.Amount + (dto.Amount * dto.VatPercentage / 100),
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            await _invoiceRepository.AddAsync(invoice);
            return "Invoice added successfully";
        }

        public async Task<string> UpdateInvoiceAsync(string invoiceId, UpdateInvoiceDto dto)
        {
            var existing = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (existing == null) return "Invoice not found";

            existing.Service = dto.Service;
            existing.Description = dto.Description;
            existing.Amount = dto.Amount;
            existing.VatPercentage = dto.VatPercentage;
            existing.TotalAmount = dto.Amount + (dto.Amount * dto.VatPercentage / 100);
            existing.StartDate = dto.StartDate;
            existing.DueDate = dto.DueDate;

            await _invoiceRepository.UpdateAsync(invoiceId, existing);
            return "Invoice updated successfully";
        }

        public async Task<string> DeleteInvoiceAsync(string invoiceId)
        {
            var existing = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (existing == null) return "Invoice not found";

            await _invoiceRepository.DeleteAsync(invoiceId);
            return "Invoice deleted successfully";
        }

        public async Task<List<Invoice>> GetInvoicesAsync(string? service = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _invoiceRepository.FilterAsync(service, startDate, endDate);
        }

        public async Task<List<Invoice>> GetInvoicesByBusinessIdAsync(string businessId)
        {
            return await _invoiceRepository.GetByBusinessIdAsync(businessId);
        }
    }
}