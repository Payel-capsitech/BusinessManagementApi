using BusinessManagementApi.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace BusinessManagementApi.Repositories
{
    /// <summary>
    /// Repository for managing businesses in MongoDB.
    /// </summary>
    public class BusinessRepository
    {
        private readonly IMongoCollection<Business> _collection;

        /// <summary>
        /// Initialize the collection.
        /// </summary>
        public BusinessRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Business>("Businesses");
        }

        /// <summary>
        /// Add a new business.
        /// </summary>
        public async Task AddAsync(Business business) =>
            await _collection.InsertOneAsync(business);

        /// <summary>
        /// Get all businesses.
        /// </summary>
        public async Task<List<Business>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        /// <summary>
        /// Get all businesses filtered by a condition.
        /// </summary>
        public async Task<List<Business>> GetAllAsync(Expression<Func<Business, bool>> filter) =>
            await _collection.Find(filter).ToListAsync();

        /// <summary>
        /// Get a business by Id.
        /// </summary>
        public async Task<Business?> GetByIdAsync(string id) =>
            await _collection.Find(b => b.Id == id).FirstOrDefaultAsync();
    }
}