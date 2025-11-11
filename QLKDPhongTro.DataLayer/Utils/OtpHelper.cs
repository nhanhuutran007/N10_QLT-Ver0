using System;
using System.Security.Cryptography;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class OtpHelper
    {
        private static string _currentOtp = string.Empty;
        private static DateTime _expiryTime = DateTime.MinValue;
        private static readonly object _lock = new object(); // đảm bảo thread-safe

        /// <summary>
        /// Sinh OTP 6 chữ số và lưu tạm với thời hạn 5 phút
        /// </summary>
        /// <returns>OTP</returns>
        public static string GenerateOtp()
        {
            lock (_lock)
            {
                byte[] randomNumber = new byte[4];
                RandomNumberGenerator.Fill(randomNumber);
                int value = Math.Abs(BitConverter.ToInt32(randomNumber, 0) % 1000000);
                _currentOtp = value.ToString("D6");
                _expiryTime = DateTime.Now.AddMinutes(5);

                Log($"Generated OTP: {_currentOtp}, Expiry: {_expiryTime:yyyy-MM-dd HH:mm:ss}");
                return _currentOtp;
            }
        }

        /// <summary>
        /// Xác thực OTP
        /// </summary>
        /// <param name="otp">OTP người dùng nhập</param>
        /// <returns>true nếu hợp lệ</returns>
        public static bool VerifyOtp(string otp)
        {
            lock (_lock)
            {
                bool isValid = !string.IsNullOrEmpty(_currentOtp) &&
                               otp == _currentOtp &&
                               DateTime.Now <= _expiryTime;

                Log($"Verifying OTP: Input={otp}, Stored={_currentOtp}, IsValid={isValid}, CurrentTime={DateTime.Now:yyyy-MM-dd HH:mm:ss}, Expiry={_expiryTime:yyyy-MM-dd HH:mm:ss}");
                return isValid;
            }
        }

        /// <summary>
        /// Lưu OTP thủ công (nếu cần quản lý nhiều OTP)
        /// </summary>
        public static void SaveOtp(string otp)
        {
            lock (_lock)
            {
                _currentOtp = otp;
                _expiryTime = DateTime.Now.AddMinutes(5);
                Log($"Saved OTP manually: {_currentOtp}, Expiry: {_expiryTime:yyyy-MM-dd HH:mm:ss}");
            }
        }

        /// <summary>
        /// Lấy OTP hiện tại (chỉ dùng trong dev/debug)
        /// </summary>
        public static string GetOtp()
        {
            lock (_lock)
            {
                return _currentOtp;
            }
        }

        /// <summary>
        /// Ghi log OTP ra console và file
        /// </summary>
        private static void Log(string message)
        {
            Console.WriteLine($"[DEV] {message}");
            System.IO.File.AppendAllText("otp_log.txt", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
    }
}
