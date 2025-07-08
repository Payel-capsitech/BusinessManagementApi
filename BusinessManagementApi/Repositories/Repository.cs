using MongoDB.Driver;
using System.Linq.Expressions;

namespace BusinessManagementApi.Repositories
{
    /// <summary>
    /// Generic repository for basic CRUD operations on a MongoDB collection.
    /// </summary>
    public class Repository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        /// <summary>
        /// Initialize the repository with the collection name.
        /// </summary>
        public Repository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Get all documents with optional filtering.
        /// </summary>
        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            return filter == null
                ? await _collection.Find(_ => true).ToListAsync()
                : await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Get a single document by Id.
        /// </summary>
        public async Task<T?> GetByIdAsync(string id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Add a new document to the collection.
        /// </summary>
        public async Task AddAsync(T entity) =>
            await _collection.InsertOneAsync(entity);

        /// <summary>
        /// Update a document by Id.
        /// </summary>
        public async Task UpdateAsync(string id, T entity) =>
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), entity);

        /// <summary>
        /// Delete a document by Id.
        /// </summary>
        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
    }
}