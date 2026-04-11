using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("nearby-drivers")]
    public async Task<IActionResult> GetNearbyDrivers(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int cabType,
        [FromQuery] double radiusKm = 5.0)
    {
        IEnumerable<NearbyDriverDto> drivers =
            await _locationService.GetNearbyDriversAsync(
                latitude, longitude, cabType, radiusKm);

        //if (!drivers.Any())
        //    return NotFound(new { message = "No drivers found nearby." });

        //return Ok(new { drivers });

        return Ok(new
        {
            drivers = drivers ?? Enumerable.Empty<NearbyDriverDto>()
        });

    }

    [HttpPut("update")]
    public IActionResult UpdateDriverLocation(
        [FromBody] UpdateLocationDto dto)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int driverId = int.Parse(userIdClaim);

        string? cabTypeStr = User.FindFirstValue("CabType");
        int cabType = cabTypeStr != null ? int.Parse(cabTypeStr) : 0;

        _locationService.UpdateDriverLocation(
            driverId, dto.Latitude, dto.Longitude, cabType, true);

        return Ok(new { message = "Driver location updated successfully." });
    }
}