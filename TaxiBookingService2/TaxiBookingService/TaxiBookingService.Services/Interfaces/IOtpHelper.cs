using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IOtpHelper
    {
        string GenerateOtp();
        bool IsExpired(DateTime? otpExpiresAt);
    }
}
