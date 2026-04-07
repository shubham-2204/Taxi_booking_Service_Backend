using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IDriverService
    {
        Task<ApiResponseDto<RideResponseDto>> RespondToRideAsync(int rideId, int driverId, bool accept);
        Task<ApiResponseDto<string>> UpdateAvailabilityAsync(int driverId, bool isAvailable);
    }
}
