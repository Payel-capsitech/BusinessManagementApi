using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessManagementApi.DTOs
{
    public class GetInvoiceDto
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        ///<summary>
        ///Id for the business
        /// </summary>
        public string? Id { get; set; }

        public IdNameModel? Business { get; set; }

        public string InvoiceId { get; set; } = string.Empty;
        /// <summary>
        /// service array which has its own id, name, description, amount
        /// </summary>
        [BsonElement("service")]
        public List<ServiceModel>? Service { get; set; }
        /// <summary>
        /// Amount of Invoice(Total of services)
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The total amount of the invoice
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// vat percentage to calculate the total amount
        /// </summary>
        public decimal VatPercentage { get; set; }
        /// <summary>
        /// start date of the invoice
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Due date of the invoice
        /// </summary>
        public DateTime DueDate { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Active;
    }

    
}
