using MongoDB.Bson;
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

        public Repository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            return filter == null
                ? await _collection.Find(_ => true).ToListAsync()
                : await _collection.Find(filter).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", BsonValue.Create(id));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", BsonValue.Create(id)), entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", BsonValue.Create(id)));
        }
    }

}