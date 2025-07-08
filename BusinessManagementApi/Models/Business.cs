/// <summary>
/// Business model to store business details and related invoices.
/// </summary>
namespace BusinessManagementApi.Models
{
    public class Business
    {
        public string Id { get; set; } = string.Empty;
        public int SerialNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public List<string> InvoiceIds { get; set; } = new List<string>();
    }
}