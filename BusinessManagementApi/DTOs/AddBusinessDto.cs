using BusinessManagementApi.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessManagementApi.DTOs
{
    public class AddBusinessDto
    {
        /// <summary>
        /// Business Name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Business Type
        /// </summary>
        public BusinessType Type { get; set; } = BusinessType.Limited;
        /// <summary>
        /// Phone number of the business
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        [BsonIgnoreIfDefault, BsonIgnoreIfNull]
        public Address? Address { get; set; }
    }
    /// <summary>
    /// Types of Businesses 
    /// </summary>
    public enum BusinessType
    {
        Unknown,
        Limited,
        LLP,
        Individual
    }
}