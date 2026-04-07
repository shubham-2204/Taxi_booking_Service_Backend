using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Common.Helpers
{
    public static class OtpHelper
    {
        public static string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(1000, 10000).ToString();
        }

        public static bool IsExpired(DateTime? otpExpiresAt)
        {
            if (otpExpiresAt == null)
                return true;

            return DateTime.UtcNow > otpExpiresAt;
        }
    }
}
