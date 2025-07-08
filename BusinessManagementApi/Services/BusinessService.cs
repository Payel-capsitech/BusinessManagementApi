using BusinessManagementApi.DTOs;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;

namespace BusinessManagementApi.Services
{
    public class BusinessService
    {
        private readonly BusinessRepository _businessRepository;

        public BusinessService(BusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        public async Task<string> AddBusinessAsync(AddBusinessDto dto, string userId)
        {
            var business = new Business
            {
                Id = Guid.NewGuid().ToString(),
                SerialNumber = new Random().Next(1000, 9999),
                Name = dto.Name,
                Type = dto.Type,
                PhoneNumber = dto.PhoneNumber,
                Address = new Address
                {
                    Country = dto.Address.Country,
                    City = dto.Address.City,
                    PostalCode = dto.Address.PostalCode,
                    Building = dto.Address.Building,
                    Street = dto.Address.Street
                },
                CreatedByUserId = userId,
                CreatedOn = DateTime.UtcNow,
                InvoiceIds = new List<string>()
            };

            await _businessRepository.AddAsync(business);
            return "Business created successfully";
        }

        public async Task<List<Business>> GetUserBusinessesAsync(string userId, string userRole)
        {
            if (userRole.ToLower() == "admin")
            {
                return await _businessRepository.GetAllAsync();
            }

            return await _businessRepository.GetAllAsync(b => b.CreatedByUserId == userId);
        }

        public async Task<Business?> GetBusinessByIdAsync(string businessId)
        {
            return await _businessRepository.GetByIdAsync(businessId);
        }
    }
}