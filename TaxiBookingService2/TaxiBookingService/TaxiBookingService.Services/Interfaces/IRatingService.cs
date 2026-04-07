using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IRatingService
    {
        Task<ApiResponseDto<RatingResponseDto>> SubmitRatingAsync(int raterId, CreateRatingRequestDto dto);
        Task<ApiResponseDto<IEnumerable<RatingResponseDto>>> GetDriverRatingsAsync(int driverId, int page, int pageSize);
    }
}
