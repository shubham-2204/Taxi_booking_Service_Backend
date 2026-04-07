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
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [Authorize(Roles = AppConstants.PassengerRole)]
        public async Task<IActionResult> SubmitRating([FromBody] CreateRatingRequestDto dto)
        {
            int raterId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ApiResponseDto<RatingResponseDto> result =
                await _ratingService.SubmitRatingAsync(raterId, dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetDriverRatings(
            int driverId,
            [FromQuery] int page = AppConstants.DefaultPage,
            [FromQuery] int pageSize = AppConstants.DefaultPageSize)
        {
            ApiResponseDto<IEnumerable<RatingResponseDto>> result =
                await _ratingService.GetDriverRatingsAsync(driverId, page, pageSize);

            return StatusCode(result.StatusCode, result);
        }
    }
}
