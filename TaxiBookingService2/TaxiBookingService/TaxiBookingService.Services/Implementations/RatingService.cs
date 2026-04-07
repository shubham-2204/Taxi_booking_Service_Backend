using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<RatingResponseDto>> SubmitRatingAsync(
            int raterId, CreateRatingRequestDto dto)
        {
            Ride? ride = await _unitOfWork.Rides.GetByIdWithDetailsAsync(dto.RideId);

            if (ride == null)
                return ApiResponseDto<RatingResponseDto>.FailureResponse(
                    "Ride not found", 404);

            if (ride.PassengerId != raterId)
                return ApiResponseDto<RatingResponseDto>.FailureResponse(
                    "You are not authorized to rate this ride", 403);

            if (ride.Status != Models.Enums.RideStatus.RideCompleted)
                return ApiResponseDto<RatingResponseDto>.FailureResponse(
                    "You can only rate a completed ride", 400);

            bool alreadyRated = await _unitOfWork.Ratings.RideAlreadyRatedAsync(dto.RideId);

            if (alreadyRated)
                return ApiResponseDto<RatingResponseDto>.FailureResponse(
                    "This ride has already been rated", 409);

            Rating rating = new Rating
            {
                RideId = dto.RideId,
                RaterId = raterId,
                RatedDriverId = ride.DriverId!.Value,
                Stars = dto.Stars,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Ratings.AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();

            double newAverage = await _unitOfWork.Ratings
                .GetDriverAverageRatingAsync(ride.DriverId.Value);

            User? driver = await _unitOfWork.Users.GetByIdAsync(ride.DriverId.Value);

            if (driver != null)
            {
                driver.AverageRating = newAverage;
                _unitOfWork.Users.Update(driver);
                await _unitOfWork.SaveChangesAsync();
            }

            RatingResponseDto response = new RatingResponseDto
            {
                Id = rating.Id,
                RideId = rating.RideId,
                Stars = rating.Stars,
                Comment = rating.Comment,
                DriverNewAverageRating = newAverage,
                CreatedAt = rating.CreatedAt
            };

            return ApiResponseDto<RatingResponseDto>.SuccessResponse(
                response, "Rating submitted", 201);
        }

        public async Task<ApiResponseDto<IEnumerable<RatingResponseDto>>> GetDriverRatingsAsync(
            int driverId, int page, int pageSize)
        {
            IEnumerable<Rating> ratings = await _unitOfWork.Ratings
                .GetDriverRatingsAsync(driverId, page, pageSize);

            IEnumerable<RatingResponseDto> result = ratings.Select(r => new RatingResponseDto
            {
                Id = r.Id,
                RideId = r.RideId,
                Stars = r.Stars,
                Comment = r.Comment,
                DriverNewAverageRating = 0,
                CreatedAt = r.CreatedAt
            });

            return ApiResponseDto<IEnumerable<RatingResponseDto>>
                .SuccessResponse(result, "Ratings fetched");
        }
    }
}
