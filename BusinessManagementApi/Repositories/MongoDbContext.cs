using BusinessManagementApi.Helpers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BusinessManagementApi.Repositories
{
    // This context class creates MongoDB client & gives access to collections
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName) =>
            _database.GetCollection<T>(collectionName);
    }
}