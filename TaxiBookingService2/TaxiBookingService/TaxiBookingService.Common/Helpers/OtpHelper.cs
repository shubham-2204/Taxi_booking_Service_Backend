using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Common.Helpers;

public class OtpHelper : IOtpHelper
{
    public string GenerateOtp()
    {
        Random random = new Random();
        return random.Next(1000, 10000).ToString();
    }

    public bool IsExpired(DateTime? otpExpiresAt)
    {
        if (otpExpiresAt == null)
            return true;

        return DateTime.UtcNow > otpExpiresAt;
    }
}