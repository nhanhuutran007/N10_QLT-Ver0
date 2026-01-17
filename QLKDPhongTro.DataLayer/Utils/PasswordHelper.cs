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
            // 1. Check if it matches the hash
            var hashedInput = HashPassword(password);
            if (hashedInput == hashedPassword) return true;

            // 2. Fallback: Check if it matches plain text (for legacy data)
            if (password.Trim() == hashedPassword.Trim()) return true;

            return false;
        }
    }
}
