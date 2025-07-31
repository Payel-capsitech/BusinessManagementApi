namespace BusinessManagementApi.DTOs
{
    public class LoginDto
    {
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Password for login 
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}