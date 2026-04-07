using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.Enums;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Services.Implementations
{
    public class FareService : IFareService
    {
        private static readonly Dictionary<CabType, (decimal BaseFare, decimal PerKmRate)> FareConfig =
    new Dictionary<CabType, (decimal BaseFare, decimal PerKmRate)>
    {
        [CabType.Mini] = (BaseFare: 30m, PerKmRate: 10m),
        [CabType.Sedan] = (BaseFare: 50m, PerKmRate: 15m),
        [CabType.SUV] = (BaseFare: 80m, PerKmRate: 20m)
    };

        public decimal CalculateFare(double distanceKm, CabType cabType)
        {
            (decimal baseFare, decimal perKmRate) = FareConfig[cabType];
            return baseFare + (decimal)distanceKm * perKmRate;
        }
    }
}
