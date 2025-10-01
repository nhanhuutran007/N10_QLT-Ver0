using System;
using System.Security.Cryptography;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class OtpHelper
    {
        private static string _currentOtp;
        private static DateTime _expiryTime;

        public static string GenerateOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);
                int value = BitConverter.ToInt32(randomNumber, 0);
                value = Math.Abs(value % 1000000);
                _currentOtp = value.ToString("D6");
                _expiryTime = DateTime.Now.AddMinutes(5);
                return _currentOtp;
            }
        }

        public static bool VerifyOtp(string otp)
        {
            return otp == _currentOtp && DateTime.Now <= _expiryTime;
        }
    }
}
