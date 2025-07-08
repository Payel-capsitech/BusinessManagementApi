using BusinessManagementApi.Models;
using MongoDB.Driver;

namespace BusinessManagementApi.Repositories
{
    /// <summary>
    /// Repository for managing invoices in MongoDB.
    /// Inherits basic CRUD from the generic Repository.
    /// </summary>
    public class InvoiceRepository : Repository<Invoice>
    {
        private readonly IMongoCollection<Invoice> _collection;

        /// <summary>
        /// Initialize the Invoice repository.
        /// </summary>
        public InvoiceRepository(MongoDbContext context)
            : base(context, "Invoices")
        {
            _collection = context.GetCollection<Invoice>("Invoices");
        }

        /// <summary>
        /// Filter invoices by service, start date, and end date.
        /// </summary>
        public async Task<List<Invoice>> FilterAsync(string? service, DateTime? startDate, DateTime? endDate)
        {
            var filter = Builders<Invoice>.Filter.Empty;

            if (!string.IsNullOrEmpty(service))
                filter &= Builders<Invoice>.Filter.Eq(i => i.Service, service);

            if (startDate.HasValue)
                filter &= Builders<Invoice>.Filter.Gte(i => i.StartDate, startDate.Value);

            if (endDate.HasValue)
                filter &= Builders<Invoice>.Filter.Lte(i => i.DueDate, endDate.Value);

            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Get all invoices that belong to a specific business.
        /// </summary>
        public async Task<List<Invoice>> GetByBusinessIdAsync(string businessId)
        {
            return await _collection.Find(i => i.BusinessId == businessId).ToListAsync();
        }
    }
}