using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IRideService
    {
        Task<ApiResponseDto<RideEstimateResponseDto>> GetEstimateAsync(CreateRideRequestDto dto);
        Task<ApiResponseDto<RideResponseDto>> CreateRideAsync(int passengerId, CreateRideRequestDto dto);
        Task<ApiResponseDto<RideResponseDto>> GetRideByIdAsync(int rideId, int requestingUserId);
        Task<ApiResponseDto<IEnumerable<RideResponseDto>>> GetPassengerHistoryAsync(int passengerId, int page, int pageSize);
        Task<ApiResponseDto<IEnumerable<RideResponseDto>>> GetDriverHistoryAsync(int driverId, int page, int pageSize);
        Task<ApiResponseDto<CancelRideResponseDto>> CancelRideAsync(int rideId, int passengerId, CancelRideRequestDto dto);
        Task<ApiResponseDto<RideResponseDto>> VerifyOtpAsync(int rideId, int driverId, string otp);
        Task<ApiResponseDto<RideResponseDto>> CompleteRideAsync(int rideId, int driverId);
        Task<ApiResponseDto<IEnumerable<CancellationReasonResponseDto>>> GetCancellationReasonsAsync();
    }
}
