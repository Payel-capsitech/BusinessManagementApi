using BusinessManagementApi.DTOs;
using BusinessManagementApi.Helpers;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;
using MongoDB.Bson;

namespace BusinessManagementApi.Services
{
    public class BusinessService
    {
        private readonly BusinessRepository _businessRepository;
        private readonly Repository<User> _userRepository;
        private readonly HistoryHelper _historyHelper;

        public BusinessService(BusinessRepository businessRepository, Repository<User> userRepository, HistoryHelper historyHelper)
        {
            _businessRepository = businessRepository;
            _userRepository = userRepository;
            _historyHelper = historyHelper;
        }

        /// <summary>
        /// Add new business
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> AddBusinessAsync(AddBusinessDto dto, string userId)
        {
            // Get the total count of businesses in the database
            var totalCount = await _businessRepository.GetBusinessCountAsync();

            // Increment count to get the next business number (starting from 1)
            var nextNumber = totalCount + 1;

            // Generate the BusinessCode in the format BE-001, BE-002, ...
            var businessCode = $"BE-{nextNumber.ToString("D3")}";  // "D3" ensures it's zero-padded to 3 digits

            var user = await _userRepository.GetByIdAsync(userId);
            var username = user?.UserName ?? "-";
            var email = user?.Email ?? "-";
            if(user == null)
            {
                throw new Exception($"user with {userId} not found");
            }
            // Create business entity
            var business = new Business
            {
                BusinessId = ObjectId.GenerateNewId().ToString(),
                BusinessCode = businessCode,
                Name = dto.Name,
                Type = dto.Type.ToString(),
                PhoneNumber = dto.PhoneNumber,
                Address = new Address
                {
                    Country = dto.Address?.Country ?? "",
                    City = dto.Address?.City ?? "",
                    PostalCode = dto.Address?.PostalCode ?? "",
                    Building = dto.Address?.Building ?? "",
                    Street = dto.Address?.Street ?? ""
                },
                userId = userId,
                username=username,
                Email = email,

                CreatedBy = new CreatedBy
                {
                    CreatedByUserId = userId,
                    Name = username,
                    CreatedOn = DateTime.UtcNow
                },
                
                InvoiceIds = new List<string>()
            };

            // Save the new business to the database
            await _businessRepository.AddAsync(business);
            await _historyHelper.AddHistoryAsync(
                business.BusinessId,
                userId,
                username,
                HistoryType.Business,
                "Created",
                $"Business {dto.Name} created",
                business.BusinessId
            );

            return "Business created successfully";
        }

        /// <summary>
        /// Getting all businesses 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public async Task<List<Business>> GetUserBusinessesAsync(string userId, string userRole)
        {
            if (userRole.ToLower() == "admin")
            {
                return await _businessRepository.GetAllAsync();
            }

            var allBusinesses = await _businessRepository.GetAllAsync();
            return allBusinesses.Where(b => b.CreatedBy != null && b.CreatedBy.CreatedByUserId == userId).ToList();

        }
        /// <summary>
        /// Getting all business details by its id 
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<BusinessDetailsDto?> GetBusinessDetailsByIdAsync(string businessId)
        {
            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
                return null;

            var user = await _userRepository.GetByIdAsync(business.CreatedBy?.CreatedByUserId ?? "");


            return new BusinessDetailsDto
            {
                Name = business.Name,
                Type = Enum.TryParse<BusinessType>(business.Type, out var parsedType) ? parsedType : BusinessType.Unknown,
                PhoneNumber = business.PhoneNumber,
                CreatedOn = business.CreatedOn,
                Address = business.Address,
                Username = user?.UserName ?? "-",
                Email = user?.Email ?? "-",
                BusinessCode = business.BusinessCode
            };
        }

        /// <summary>
        /// getting businesses by business id 
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<Business?> GetBusinessByIdAsync(string businessId)
        {
            return await _businessRepository.GetByIdAsync(businessId);
        }

        
        /// <summary>
        /// Adding pagination based on businesses count
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<object> GetAllBusinessesPaginatedAsync(int page, int pageSize, string userId, string role)
        {
            List<Business> filteredBusinesses;

            if (role.ToLower() == "admin")
            {
                filteredBusinesses = await _businessRepository.GetAllAsync();
            }
            else
            {
                var allBusinesses = await _businessRepository.GetAllAsync();
                filteredBusinesses = allBusinesses
                    .Where(b => b.CreatedBy != null && b.CreatedBy.CreatedByUserId == userId)
                    .ToList();
            }

            var total = filteredBusinesses.Count;
            var pagedBusinesses = filteredBusinesses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new
            {
                data = pagedBusinesses,
                total = total
            };
        }

        /// <summary>
        /// Search businesses
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<List<Business>> SearchBusinessesAsync(string query, string userId, string role)
        {
            if (string.IsNullOrEmpty(query))
            {
                return await GetUserBusinessesAsync(userId, role);
            }

            if (role.ToLower() == "admin")
            {
                return await _businessRepository.GetAllAsync(b => b.Name.ToLower().Contains(query.ToLower()));
            }

            return await _businessRepository.GetAllAsync(
                b => b.CreatedBy != null && b.CreatedBy.CreatedByUserId == userId && b.Name.ToLower().Contains(query.ToLower()));

        }

        /// <summary>
        /// getting history of the business creation
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task<List<History>> GetBusinessHistoryAsync(string businessId)
        {
            var allHistory = await _historyHelper.GetAllHistoryAsync();
            return allHistory
                .Where(h => h.BusinessId == businessId)
                .OrderByDescending(h => h.Date)
                .ToList();
        }




    }
}

