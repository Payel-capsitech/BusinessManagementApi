//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;

///// <summary>
///// Business model to store business details and related invoices.
///// </summary>
//namespace BusinessManagementApi.Models
//{
//    public class Business
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        [BsonElement("_id")]
//        public string BusinessId { get; set; } = string.Empty;

//        public string BusinessCode { get; set; } = string.Empty;
//        public int SerialNumber { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public string Type { get; set; } = string.Empty;
//        public string userId { get; set; } = string.Empty;
//        public string username { get; set; } = string.Empty;
//        public string Email { get; set; } = string.Empty;
//        public Address Address { get; set; } = new Address();
//        public string CreatedByUserId { get; set; } = string.Empty;
//        public string PhoneNumber { get; set; } = string.Empty;
//        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
//        public List<string> InvoiceIds { get; set; } = new List<string>();
//    }
//}


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

/// <summary>
/// Business model to store business details and related invoices.
/// </summary>
namespace BusinessManagementApi.Models
{
    public class Business
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string BusinessId { get; set; } = string.Empty;

        public string BusinessCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
        public CreatedBy? CreatedBy { get; set; } = new CreatedBy();
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public List<string> InvoiceIds { get; set; } = new List<string>();
    }
}