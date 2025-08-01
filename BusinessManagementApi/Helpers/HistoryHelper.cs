using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;
using MongoDB.Bson;

namespace BusinessManagementApi.Helpers
{
    public class HistoryHelper
    {
        private readonly Repository<History> _historyRepository;
        private readonly Repository<User> _userRepository;

        // Inject both History and User repositories
        public HistoryHelper(Repository<History> historyRepository, Repository<User> userRepository)
        {
            _historyRepository = historyRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Adding new history
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="createdById"></param>
        /// <param name="createdByName"></param>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <param name="description"></param>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public async Task AddHistoryAsync(string targetId, string createdById, string createdByName, HistoryType type, string action, string? description = null, string businessId = "")
        {
            if (string.IsNullOrEmpty(createdByName) || createdByName == "Unknown")
            {
                var user = await _userRepository.GetByIdAsync(createdById);
                createdByName = user?.UserName ?? "Unknown";
            }

            var history = new History
            {
                Id = ObjectId.GenerateNewId().ToString(),
                BusinessId = businessId,
                Description = description ?? $"Entity {type} {action}",
                Target = new Target
                {
                    Id = targetId,
                    Name = type.ToString()
                },

                CreatedBy = new CreatedBy
                {
                    CreatedByUserId = createdById,
                    Name = createdByName,
                    CreatedOn = DateTime.UtcNow
                },
                Date = DateTime.UtcNow,
                Type = type
            };

            await _historyRepository.AddAsync(history);
        }
        /// <summary>
        /// Get all history 
        /// </summary>
        /// <returns></returns>
        public async Task<List<History>> GetAllHistoryAsync()
        {
            return await _historyRepository.GetAllAsync();
        }
    }
}
