using BusinessManagementApi.DTOs;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessManagementApi.Controllers
{
    /// <summary>
    /// Manages businesses. Requires JWT authentication.
    /// </summary>
    [ApiController]
    [Route("api/business")]
    [Authorize] 
    public class BusinessController : ControllerBase
    {
        private readonly BusinessService _businessService;

        /// <summary>
        /// Injects the BusinessService.
        /// </summary>
        public BusinessController(BusinessService businessService)
        {
            _businessService = businessService;
        }

        /// <summary>
        /// Adds a new business linked to the logged-in user.
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddBusiness([FromBody] AddBusinessDto dto)
        {
            var userId = HttpContext.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID missing in token." });
            var result = await _businessService.AddBusinessAsync(dto, userId);
            return Ok(new { message = result });
        }


        /// <summary>
        /// Retrieves all businesses created by the logged-in user.
        /// If the user is an admin, retrieves all businesses in the system.
        /// </summary>
        [HttpGet("user-businesses")]
        public async Task<IActionResult> GetUserBusinesses()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var businesses = await _businessService.GetUserBusinessesAsync(userId, role);

            return Ok(businesses);
        }


        /// <summary>
        /// Gets a business by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusinessById(string id)
        {
            var business = await _businessService.GetBusinessByIdAsync(id);

            if (business == null)
            {
                return NotFound(new { message = "Business not found" });
            }

            return Ok(business);
        }

        ///<summary>
        /// Search by business name 
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchBusinesses([FromQuery] string query="")
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var businesses = await _businessService.SearchBusinessesAsync(query, userId, role);

            return Ok(businesses);
        }

        /// <summary>
        /// Gets history items for a specific business by its ID.
        /// </summary>
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetBusinessHistory(string id)
        {
            var business = await _businessService.GetBusinessByIdAsync(id);
            if (business == null)
            {
                return NotFound(new { message = "Business not found" });
            }

            var history = await _businessService.GetBusinessHistoryAsync(id);
            return Ok(history);
        }

        /// <summary>
        /// Adding Pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedBusinesses([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

            var result = await _businessService.GetAllBusinessesPaginatedAsync(page, pageSize, userId, role);
            return Ok(result);
        }



    }
}