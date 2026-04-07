using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IFareService
    {
        decimal CalculateFare(double distanceKm, CabType cabType);
    }
}
