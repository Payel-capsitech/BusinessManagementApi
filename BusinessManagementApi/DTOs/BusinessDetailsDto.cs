using BusinessManagementApi.Models;

namespace BusinessManagementApi.DTOs
{
    public class BusinessDetailsDto
    {
        /// <summary>
        /// Name of the business
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of the business - LLP, Limited, Partnership
        /// </summary>
        public BusinessType Type { get; set; } = BusinessType.Limited;
        /// <summary>
        /// PhoneNumber to store in business
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// BusinessCode for Business uniquely identify(Example - BE-001, ..)
        /// </summary>
        public string BusinessCode { get; set; } = string.Empty;
        /// <summary>
        /// new address from address object 
        /// </summary>
        public Address Address { get; set; } = new Address();
        /// <summary>
        /// The business is created by the user
        /// </summary>
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }

    }

}