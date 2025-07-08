using BusinessManagementApi.DTOs;
using BusinessManagementApi.Helpers;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;

namespace BusinessManagementApi.Services
{
    /// <summary>
    /// Handles user registration and login operations.
    /// </summary>
    public class AuthService
    {
        private readonly Repository<User> _userRepository;
        private readonly JwtHelper _jwtHelper;

        /// <summary>
        /// Injects user repository and JWT helper.
        /// </summary>
        public AuthService(Repository<User> userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Registers a new user by saving their data into the database with a hashed password.
        /// </summary>
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            return "Registration successful";
        }

        /// <summary>
        /// Validates user login and returns a JWT token if the email and password match.
        /// </summary>
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var users = await _userRepository.GetAllAsync(u => u.Email == dto.Email);
            var user = users.FirstOrDefault();

            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return _jwtHelper.GenerateToken(user);
        }
    }
}