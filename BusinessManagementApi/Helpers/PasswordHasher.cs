using BCrypt.Net;

namespace BusinessManagementApi.Helpers
{
    /// <summary>
    /// Handles password hashing and verifying using BCrypt.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Converts a plain text password into a hashed password using BCrypt.
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Checks if the plain text password matches the hashed password.
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}