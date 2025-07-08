/// <summary>
/// Invoice model to store invoice information.
/// </summary>
namespace BusinessManagementApi.Models
{
    public class Invoice
    {
        public string Id { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}