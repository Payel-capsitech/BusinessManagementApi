namespace BusinessManagementApi.DTOs
{
    public class AddInvoiceDto
    {
        public string BusinessId { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal VatPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}