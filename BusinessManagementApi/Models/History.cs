using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BusinessManagementApi.Models
{
    public class History
    {
        /// <summary>
        /// History Id 
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        /// <summary>
        /// Business code (BE-001,....)
        /// </summary>

        public string BusinessCode { get; set; } = string.Empty;
        /// <summary>
        /// The description of the invoice
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        public Target Target { get; set; } = new Target();

        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Type of the History-unknown, business, invoice
        /// </summary>
        [BsonRepresentation(BsonType.String)] 
        public HistoryType Type { get; set; } = HistoryType.Unknown;
        public CreatedBy? CreatedBy { get; set; }
    }
    /// <summary>
    /// Target of the history
    /// </summary>
    public class Target
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreatedBy
    {
        public string CreatedByUserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        
    }

    /// <summary>
    /// Type of the history -unknown, Business, Invoice
    /// </summary>
    public enum HistoryType
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        [Display(Name = "Business")]

        Business = 1,
        [Display(Name = "Invoice")]

        Invoice = 2
    }
    /// <summary>
    /// Display the name of the history type
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString()).FirstOrDefault();

            return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
        }
    }

}
