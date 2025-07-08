namespace BusinessManagementApi.Helpers
{
    /// <summary>
    /// This class stores the MongoDB connection details from appsettings.json.
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}