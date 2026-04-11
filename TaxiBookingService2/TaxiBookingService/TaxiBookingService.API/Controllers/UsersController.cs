using Microsoft.AspNetCore.Authorization;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDriverService _driverService;

        public UsersController(IUserService userService, IDriverService driverService)
        {
            _userService = userService;
            _driverService = driverService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<UserDto> result =
                await _userService.GetProfileAsync(userId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequestDto dto)
        {
            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<UserDto> result =
                await _userService.UpdateProfileAsync(userId, dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("/availability")]
        [Authorize(Roles = AppConstants.DriverRole)]
        public async Task<IActionResult> UpdateAvailability(
            [FromBody] bool isAvailable)
        {
            int driverId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<string> result =
                await _driverService.UpdateAvailabilityAsync(driverId, isAvailable);

            return StatusCode(result.StatusCode, result);
        }
    } 
}
