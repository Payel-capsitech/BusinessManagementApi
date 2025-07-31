using BusinessManagementApi.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
/// <summary>
/// Status of invoice
/// </summary>
public enum InvoiceStatus
{
    Unknown = 0,
    Active = 1,
    Deleted = 2
}

public class Invoice
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string InvoiceId { get; set; } = string.Empty;
    /// <summary>
    /// Business code
    /// </summary>
    public string BusinessCode { get; set; } = string.Empty;
    /// <summary>
    /// Business Id --objectId
    /// </summary>
    public string BusinessId { get; set; } = string.Empty;
    /// <summary>
    /// Business Name
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;
    /// <summary>
    /// Service list of array with its id, description, amount
    /// </summary>
    public List<ServiceModel> Service { get; set; } = new();
    /// <summary>
    /// Global description of invoice
    /// </summary>
    //public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Amount calculated by service amount 
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Vat percentage to apply on the amount of services
    /// </summary>
    public decimal VatPercentage { get; set; }
    /// <summary>
    /// The total amount of the invoice
    /// </summary>
    public decimal TotalAmount { get; set; }
    /// <summary>
    /// the startdate of the invoice
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// DueDate of the invoice
    /// </summary>
    public DateTime DueDate { get; set; }
    /// <summary>
    /// the created at time of the invoice
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// The enum type of the Status--Active, delete, unknown
    /// </summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Active;
}
