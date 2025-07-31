using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Address model to hold the address of a business.
/// </summary>
namespace BusinessManagementApi.Models
{
    public class Address
    {
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public string Building { get; set; } = string.Empty;
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public string Street { get; set; } = string.Empty;
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public string City { get; set; } = string.Empty;
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public string PostalCode { get; set; } = string.Empty;
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public string Country { get; set; } = string.Empty;
    }
}