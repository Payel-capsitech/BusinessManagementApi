/// <summary>
/// Address model to hold the address part of a business.
/// </summary>
namespace BusinessManagementApi.Models
{
    public class Address
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
    }
}