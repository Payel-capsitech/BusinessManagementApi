using BusinessManagementApi.DTOs;
using BusinessManagementApi.Helpers;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using HistoryType = BusinessManagementApi.Models.HistoryType;

namespace BusinessManagementApi.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly BusinessRepository _businessRepository;
        private readonly HistoryHelper _historyHelper;

        /// <summary>
        /// Constructor to initialize repositories and helper
        /// </summary>
        public InvoiceService(
            InvoiceRepository invoiceRepository,
            BusinessRepository businessRepository,
            HistoryHelper historyHelper)
        {
            _invoiceRepository = invoiceRepository;
            _businessRepository = businessRepository;
            _historyHelper = historyHelper;
            
        }

        /// <summary>
        /// Add Invoice and show the created by user details
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="createdById"></param>
        /// <param name="createdByName"></param>
        /// <returns></returns>
        public async Task<Invoice> AddInvoiceAsync(AddInvoiceDto dto, string createdById, string createdByName)
        {
            if (dto.Service != null)
            {
                foreach (var service in dto.Service)
                {
                    if (string.IsNullOrEmpty(service.Id) || !ObjectId.TryParse(service.Id, out _))
                    {
                        service.Id = ObjectId.GenerateNewId().ToString();
                    }
                }
            }

            var totalCount = (await _invoiceRepository.GetAllAsync()).Count;
            var nextNumber = totalCount + 1;
            var newInvoiceCode = $"INV-{nextNumber:D3}";
            var business = await _businessRepository.GetByIdAsync(dto.Business?.Id ?? "");
            var businessCode = business?.BusinessCode ?? "UNKNOWN";
            var businessName = business?.Name ?? "UNKNOWN";

            if (string.IsNullOrEmpty(businessName))
            {
                Console.WriteLine($"Business {dto.Business?.Id} has no name.");
                businessName = "UNKNOWN";
            }

            // Create new invoice object
            var invoice = new Invoice
            {
                Id = ObjectId.GenerateNewId().ToString(),
                InvoiceId = newInvoiceCode,
                BusinessCode = businessCode,
                BusinessId = dto.Business?.Id ?? string.Empty,
                BusinessName = businessName,
                Service = dto.Service ?? new List<ServiceModel>(),
                Amount = dto.Amount,
                VatPercentage = dto.VatPercentage,
                TotalAmount = dto.Amount + (dto.Amount * dto.VatPercentage / 100),
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Active
            };

            // Save to database
            await _invoiceRepository.AddAsync(invoice);

            if (!string.IsNullOrEmpty(invoice.BusinessId))
            {
                await _businessRepository.UpdateAsync(
                    invoice.BusinessId,
                    Builders<Business>.Update.Push(b => b.InvoiceIds, invoice.Id)
                );
            }

            string description = $"Invoice {newInvoiceCode} created on {invoice.CreatedAt:dd-MM-yyyy} with total amount ₹{invoice.TotalAmount}.<br/> " +
                                 $"Start Date: {invoice.StartDate:dd-MM-yyyy}, Due Date: {invoice.DueDate:dd-MM-yyyy}.";

            // Save to history
            await _historyHelper.AddHistoryAsync(
                invoice.Id,
                createdById,
                createdByName,
                HistoryType.Invoice,
                "created",
                description,
                invoice.BusinessId
            );

            return invoice;
        }


        /// <summary>
        /// Updates an existing invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="dto"></param>
        /// <param name="createdById"></param>
        /// <param name="createdByName"></param>
        /// <returns></returns>
        public async Task<string> UpdateInvoiceAsync(string invoiceId, AddInvoiceDto dto, string createdById, string createdByName)
        {
            // Get the invoice to update
            Invoice existing = await _invoiceRepository.GetInvoiceByInvoiceIdAsync(invoiceId);
            if (existing == null) return "Invoice not found";

            var changes = new List<string>();
            var oldServiceNames = string.Join(", ", existing.Service.Select(s => s.Name));
            var newServiceNames = string.Join(", ", dto.Service?.Select(s => s.Name) ?? new List<string>());
            // Track the changes in invoice fields
            if (oldServiceNames != newServiceNames)
                changes.Add($"Service changed from {oldServiceNames} to {newServiceNames}<br/>");

            //if (existing.Description != dto.Description)
            //    changes.Add($"Description changed");

            if (existing.Amount != dto.Amount)
                changes.Add($"Amount changed from ₹{existing.Amount} to ₹{dto.Amount}");

            if (existing.VatPercentage != dto.VatPercentage)
                changes.Add($"VAT changed from {existing.VatPercentage}% to {dto.VatPercentage}%");

            if (existing.StartDate != dto.StartDate)
                changes.Add($"StartDate changed from {existing.StartDate:dd-MM-yyyy} to {dto.StartDate:dd-MM-yyyy}");

            if (existing.DueDate != dto.DueDate)
                changes.Add($"DueDate changed from {existing.DueDate:dd-MM-yyyy} to {dto.DueDate:dd-MM-yyyy}");
            // Update invoice values
            existing.Service = dto.Service ?? new List<ServiceModel>();
            //existing.Description = dto.Description ?? string.Empty;
            existing.Amount = dto.Amount;
            existing.VatPercentage = dto.VatPercentage;
            existing.TotalAmount = dto.Amount + (dto.Amount * dto.VatPercentage / 100);
            existing.StartDate = dto.StartDate;
            existing.DueDate = dto.DueDate;
            existing.Status = InvoiceStatus.Active;

            
            await _invoiceRepository.UpdateInvoiceByInvoiceId(invoiceId, existing);

            string description = changes.Count > 0
                ? $"Invoice {existing.InvoiceId} updated: {string.Join(", ", changes)}."
                : $"Invoice {existing.InvoiceId} updated with no field changes.";

            await _historyHelper.AddHistoryAsync(
                invoiceId,
                createdById,
                createdByName,
                HistoryType.Invoice,
                "updated",
                description,
                existing.BusinessId
            );

            return "Invoice updated successfully";
        }

        /// <summary>
        /// Deletes an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="createdById"></param>
        /// <param name="createdByName"></param>
        /// <returns></returns>
        public async Task<string> DeleteInvoiceAsync(string invoiceId, string createdById, string createdByName)
        {
            //Get the existing invoice and check for the invoice
            var existing = await _invoiceRepository.GetInvoiceByInvoiceIdAsync(invoiceId);
            if (existing == null || existing.Id == null)
                return "Invoice not found";

            await _invoiceRepository.DeleteInvoiceByIdAsync(invoiceId);

            var description = $"Invoice {existing.InvoiceId} deleted at {DateTime.Now:dd-MM-yyyy hh:mm tt}";
            await _historyHelper.AddHistoryAsync(
                invoiceId,
                createdById,
                createdByName,
                HistoryType.Invoice,
                "deleted",
                description,
                existing.BusinessId
            );

            return "Invoice deleted successfully";
        }


        /// <summary>
        /// Get invoices by their service, start date, due date
        /// </summary>
        /// <param name="service"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<Invoice>> GetInvoicesAsync(string? service = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var all = await _invoiceRepository.FilterAsync(service, startDate, endDate);
            return all.Where(i => i.Status == InvoiceStatus.Active).ToList();
        }

        /// <summary>
        /// Get invoice by their business id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<List<GetInvoiceDto>> GetInvoicesByBusinessIdAsync(string businessId)
        {
            var all = await _invoiceRepository.GetByBusinessIdAsync(businessId);
            return all;

        }


        /// <summary>
        /// Fetch all history of the corresponding businessId
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<List<History>> GetInvoiceHistoryAsync(string businessId)
        {
            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null || string.IsNullOrEmpty(business.BusinessCode))
                return new List<History>();
            var allHistory = await _historyHelper.GetAllHistoryAsync();
            return allHistory
                .Where(h => h.Type == HistoryType.Invoice && h.BusinessCode == business.BusinessCode)
                .OrderByDescending(h => h.Date)
                .ToList();
        }

        /// <summary>
        /// Getting all invoices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }

        /// <summary>
        /// Adding pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="service"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<(List<Invoice> Invoices, long TotalCount)> GetInvoicesPaginatedAsync(
            int page, 
            int pageSize,
            string? search = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null
        )
        {
            var (invoices, totalCount) = await _invoiceRepository.FilterPaginatedAsync(page, pageSize, search, startDate, endDate);
            return (invoices.Where(i => i.Status == InvoiceStatus.Active).ToList(), totalCount);
        }

    }
}