using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaxiBookingService.Common.Constants;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RidesController : ControllerBase
    {
        private readonly IRideService _rideService;

        public RidesController(IRideService rideService)
        {
            _rideService = rideService;
        }

        [HttpPost("estimate")]
        [Authorize(Roles = AppConstants.PassengerRole)]
        public async Task<IActionResult> GetEstimate([FromBody] CreateRideRequestDto dto)
        {
            ApiResponseDto<RideEstimateResponseDto> result =
                await _rideService.GetEstimateAsync(dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = AppConstants.PassengerRole)]
        public async Task<IActionResult> CreateRide([FromBody] CreateRideRequestDto dto)
        {
            int passengerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RideResponseDto> result =
                await _rideService.CreateRideAsync(passengerId, dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRide(int id)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RideResponseDto> result =
                await _rideService.GetRideByIdAsync(id, userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] int page = AppConstants.DefaultPage,
            [FromQuery] int pageSize = AppConstants.DefaultPageSize)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            string role = User.FindFirstValue(ClaimTypes.Role)!;

            if (role == AppConstants.DriverRole)
            {
                ApiResponseDto<IEnumerable<RideResponseDto>> driverResult =
                    await _rideService.GetDriverHistoryAsync(userId, page, pageSize);
                return StatusCode(driverResult.StatusCode, driverResult);
            }

            ApiResponseDto<IEnumerable<RideResponseDto>> passengerResult =
                await _rideService.GetPassengerHistoryAsync(userId, page, pageSize);

            return StatusCode(passengerResult.StatusCode, passengerResult);
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = AppConstants.PassengerRole)]
        public async Task<IActionResult> CancelRide(
            int id, [FromBody] CancelRideRequestDto dto)
        {
            int passengerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<CancelRideResponseDto> result =
                await _rideService.CancelRideAsync(id, passengerId, dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/verify-otp")]
        [Authorize(Roles = AppConstants.DriverRole)]
        public async Task<IActionResult> VerifyOtp(int id, [FromBody] string otp)
        {
            int driverId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RideResponseDto> result =
                await _rideService.VerifyOtpAsync(id, driverId, otp);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/complete")]
        [Authorize(Roles = AppConstants.DriverRole)]
        public async Task<IActionResult> CompleteRide(int id)
        {
            int driverId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RideResponseDto> result =
                await _rideService.CompleteRideAsync(id, driverId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("cancellation-reasons")]
        [Authorize(Roles = AppConstants.PassengerRole)]
        public async Task<IActionResult> GetCancellationReasons()
        {
            ApiResponseDto<IEnumerable<CancellationReasonResponseDto>> result =
                await _rideService.GetCancellationReasonsAsync();

            return StatusCode(result.StatusCode, result);
        }
    } 
}
