using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IMapService
    {
        Task<DistanceResultDto> GetDistanceAndDurationAsync(
            double originLat, double originLng,
            double destLat, double destLng);
    }
}
