using BusinessManagementApi.Models;
using System.Net;

namespace BusinessManagementApi.DTOs
{
    public class AddBusinessDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Address Address { get; set; } = new Address();
    }
}