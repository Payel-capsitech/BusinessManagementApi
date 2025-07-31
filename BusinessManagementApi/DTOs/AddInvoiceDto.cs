using MongoDB.Bson.Serialization.Attributes;

namespace BusinessManagementApi.DTOs
{
    public class AddInvoiceDto
    {
        /// <summary>
        /// Business id and name which invoice is created
        /// </summary>
        public IdNameModel? Business { get; set; }
        /// <summary>
        /// service array which has its own id, name, description, amount
        /// </summary>
        [BsonElement("service")]
        public List<ServiceModel>? Service { get; set; }
        /// <summary>
        /// Deascription of the service
        /// </summary>
        //public string? Description { get; set; }
        /// <summary>
        /// Amount of Invoice(Total of services)
        /// </summary>
        public decimal Amount { get; set; }
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

    public class IdNameModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        ///<summary>
        ///Id for the business
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Name of the business
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
    public class ServiceModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        ///<summary>
        ///Id of the service
        ///</summary>
        public string? Id { get; set; }
        /// <summary>
        /// Name of the service
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the each services
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Amount for each service
        /// </summary>
        public decimal Amount { get; set; }
    }
}