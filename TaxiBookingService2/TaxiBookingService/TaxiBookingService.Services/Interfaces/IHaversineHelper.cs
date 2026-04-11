using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IHaversineHelper
    {
        double CalculateDistance(
            double lat1, double lon1,
            double lat2, double lon2);
    }
}
