using BusinessManagementApi.DTOs;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagementApi.Controllers
{
    /// <summary>
    /// Manages businesses. Requires JWT authentication.
    /// </summary>
    [ApiController]
    [Route("api/business")]
    [Authorize] // All endpoints require JWT token
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? "";

            var role = User.FindFirstValue(ClaimTypes.Role)
                     ?? User.FindFirst("role")?.Value
                     ?? "";
            var result = await _businessService.AddBusinessAsync(dto, userId);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Gets all businesses linked to the logged-in user. Admins get all businesses.
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBusinesses()
        {
            // Try to get userId from multiple places
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirst("sub")?.Value
                      ?? "";

            var role = User.FindFirstValue(ClaimTypes.Role)
                    ?? User.FindFirst("role")?.Value
                    ?? "";

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
    }
}