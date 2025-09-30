using System;
using System.Security.Cryptography;
using System.Text;

namespace QLKDPhongTro.DataLayer.Utils
{
    /// <summary>
    /// Helper class để xử lý mật khẩu
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash mật khẩu
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Verify mật khẩu
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
