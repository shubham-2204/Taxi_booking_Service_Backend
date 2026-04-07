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
    [Authorize(Roles = AppConstants.DriverRole)]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriversController(
            IDriverService driverService,
            ILocationService locationService)
        {
            _driverService = driverService;
        }

        [HttpPost("rides/{id}/respond")]
        public async Task<IActionResult> RespondToRide(int id, [FromBody] bool accept)
        {
            int driverId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RideResponseDto> result =
                await _driverService.RespondToRideAsync(id, driverId, accept);

            return StatusCode(result.StatusCode, result);
        }

        
    }
}
