namespace BusinessManagementApi.DTOs
{
    /// <summary>
    /// Data model for user registration input.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// User's display name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address (used for login).
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password in plain text (will be hashed before saving).
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user (examples: admin, manager, staff).
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Unknown;
    }

    public enum UserRole
    {
        Unknown,
        Admin,
        Manager,
        Staff
    }

}