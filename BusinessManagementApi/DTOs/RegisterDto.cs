namespace BusinessManagementApi.DTOs
{
    /// <summary>
    /// Data model for user registration input.
    /// </summary>
    public class RegisterDto
    {
        ///// <summary>
        ///// User's display name.
        ///// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// User's email address 
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password in plain text 
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user : admin, staff, manager
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}