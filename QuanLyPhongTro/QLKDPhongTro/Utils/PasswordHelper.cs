using System;
using System.Security.Cryptography;
using System.Text;

namespace QLKDPhongTro.Utils
{
    /// <summary>
    /// Helper class xử lý mật khẩu
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash mật khẩu
        /// </summary>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verify mật khẩu
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validate mật khẩu
        /// </summary>
        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;
                if (char.IsDigit(c))
                    hasDigit = true;
            }

            return hasUpper || hasDigit;
        }
    }
}
