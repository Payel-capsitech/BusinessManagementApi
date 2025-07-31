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
            await _collection.Find(b => b.BusinessId == id).FirstOrDefaultAsync();

        ///<summary>
        ///Adding Pagination in List 
        ///</summary>
        public async Task<(List<Business> Businesses, long TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<Business>.Empty);

            var businesses = await _collection.Find(FilterDefinition<Business>.Empty)
                                              .Skip((page - 1) * pageSize)
                                              .Limit(pageSize)
                                              .ToListAsync();

            return (businesses, totalCount);
        }

        public async Task<Business?> GetLastBusinessAsync()
        {
            return await _collection
                .Find(_ => true)
                .SortByDescending(b => b.BusinessCode) 
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// gets all businesses
        /// </summary>

        public async Task<long> GetBusinessCountAsync()
        {
            return await _collection.CountDocumentsAsync(FilterDefinition<Business>.Empty);
        }

        public async Task UpdateAsync(string businessId, UpdateDefinition<Business> update)
        {
            var filter = Builders<Business>.Filter.Eq(b => b.BusinessId, businessId);
            await _collection.UpdateOneAsync(filter, update);
        }



    }
}