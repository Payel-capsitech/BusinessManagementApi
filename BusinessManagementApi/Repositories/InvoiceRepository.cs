using BusinessManagementApi.DTOs;
using BusinessManagementApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace BusinessManagementApi.Repositories
{
    /// <summary>
    /// Repository for managing invoices in MongoDB.
    /// Inherits basic CRUD from the generic Repository.
    /// </summary>
    public class InvoiceRepository : Repository<Invoice>
    {
        private readonly IMongoCollection<Invoice> _collection;
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Invoice> _invoices;

        /// <summary>
        /// Initialize the Invoice repository.
        /// </summary>
        public InvoiceRepository(MongoDbContext context)
            : base(context, "Invoices")
        {
            _collection = context.GetCollection<Invoice>("Invoices");
            _context = context;
            _invoices = _context.GetCollection<Invoice>("Invoices");
        }

        /// <summary>
        /// Filter invoices by service, start date, and end date.
        /// </summary>
        public async Task<List<Invoice>> FilterAsync(string? service, DateTime? startDate, DateTime? endDate)
        {
            var filter = Builders<Invoice>.Filter.Empty;

            if (!string.IsNullOrEmpty(service))
            {
                filter &= Builders<Invoice>.Filter.ElemMatch(i => i.Service,
                    s => s.Name == service);
            }

            if (startDate.HasValue)
                filter &= Builders<Invoice>.Filter.Gte(i => i.StartDate, startDate.Value);

            if (endDate.HasValue)
                filter &= Builders<Invoice>.Filter.Lte(i => i.DueDate, endDate.Value);

            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Get all invoices(special field to show in network) that belong to a specific business.
        /// </summary>
        public async Task<List<GetInvoiceDto>> GetByBusinessIdAsync(string businessId)
        {
            try
            {
                var invoice = await _collection.Find(i => i.BusinessId == businessId).Project(i => new GetInvoiceDto
                {
                    Id = i.Id,
                    Business = new IdNameModel { 
                        Id = i.BusinessId,
                        Name = i.BusinessName
                    },
                    Service = i.Service.Select(s => new ServiceModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description=s.Description,
                        Amount=s.Amount
                    }).ToList(),
                    InvoiceId= i.InvoiceId,
                    Amount = i.Amount,
                    TotalAmount=i.TotalAmount,
                    StartDate = i.StartDate,
                    DueDate = i.DueDate,
                    VatPercentage = i.VatPercentage

                }).ToListAsync();

                return invoice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching invoices by businessId: {ex.Message}");
                return new List<GetInvoiceDto>();
            }
        }

        /// <summary>
        /// Get all invoice details thta belong to a specific business .
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<List<Invoice>> GetBusinessIdAsyncAll(string businessId)
        {
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }catch(Exception ex)
            {
                Console.WriteLine($"Error fetching invoices by businessId: {ex.Message}");
                return new List<Invoice>();
            }
        }
        /// <summary>
        /// Get invoice by invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>

        public async Task<Invoice> GetInvoiceByInvoiceIdAsync(string invoiceId)
        {
            try
            {
                return await _collection.Find(i => i.Id == invoiceId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching invoices by businessId: {ex.Message}");

                return new Invoice();
            }
        }
        /// <summary>
        /// Update Invoice by its Id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="updatedInvoice"></param>
        /// <returns></returns>
        public async Task UpdateInvoiceByInvoiceId(string invoiceId, Invoice updatedInvoice)
        {
            var filter = Builders<Invoice>.Filter.Eq(i => i.Id, invoiceId);

            var update = Builders<Invoice>.Update
                .Set(i => i.InvoiceId, updatedInvoice.InvoiceId)
                .Set(i => i.BusinessCode, updatedInvoice.BusinessCode)
                .Set(i => i.BusinessId, updatedInvoice.BusinessId)
                .Set(i => i.Service, updatedInvoice.Service)
                .Set(i => i.Amount, updatedInvoice.Amount)
                .Set(i => i.VatPercentage, updatedInvoice.VatPercentage)
                .Set(i => i.TotalAmount, updatedInvoice.TotalAmount)
                .Set(i => i.StartDate, updatedInvoice.StartDate)
                .Set(i => i.DueDate, updatedInvoice.DueDate)
                .Set(i => i.CreatedAt, updatedInvoice.CreatedAt)
                .Set(i => i.Status, updatedInvoice.Status);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                Console.WriteLine($"Invoice with ID {invoiceId} not found.");
            }
        }
        /// <summary>
        /// Deletes invoice by its id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public async Task DeleteInvoiceByIdAsync(string invoiceId)
        {
            var filter = Builders<Invoice>.Filter.Eq(i => i.Id, invoiceId);
            var update = Builders<Invoice>.Update.Set(i => i.Status, InvoiceStatus.Deleted);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                Console.WriteLine($"Invoice with ID {invoiceId} not found for deletion.");
            }
        }

        /// <summary>
        /// Getting all invoices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Invoice>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();


        /// <summary>
        /// Adding pagination
        /// </summary>
        /// <param name="service"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public async Task<(List<Invoice> Invoices, long TotalCount)> FilterPaginatedAsync(
        int page,
        int pageSize,
        string? search = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
       {
            var filter = Builders<Invoice>.Filter.Empty;

            if (!string.IsNullOrEmpty(search))
            {
                //filter &= Builders<Invoice>.Filter.ElemMatch(i => i.Service, s => s.Name == search);
                var regex = new BsonRegularExpression(search, "i");
                var regexFilter = Builders<Invoice>.Filter.Or(
                    Builders<Invoice>.Filter.Regex("Service.Name", regex),
                    Builders<Invoice>.Filter.Regex(i => i.InvoiceId, regex),
                    Builders<Invoice>.Filter.Regex("BusinessName", regex)
                );

                filter &= regexFilter;
            }

            if (startDate.HasValue)
            {
                filter &= Builders<Invoice>.Filter.Gte(i => i.StartDate, startDate.Value);
            }

            if (endDate.HasValue)
            {
                filter &= Builders<Invoice>.Filter.Lte(i => i.DueDate, endDate.Value);
            }

            // Only active invoices
            filter &= Builders<Invoice>.Filter.Eq(i => i.Status, InvoiceStatus.Active);

            var totalCount = await _collection.CountDocumentsAsync(filter);

            var invoices = await _collection
                .Find(filter)
                .SortBy(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (invoices, totalCount);
        }

    }

}