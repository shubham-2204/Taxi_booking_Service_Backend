using TaxiBookingService.Models.Models;

namespace TaxiBookingService.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<bool> RideAlreadyRatedAsync(int rideId);
        Task<IEnumerable<Rating>> GetDriverRatingsAsync(int driverId, int page, int pageSize);
        Task<int> GetDriverRatingsCountAsync(int driverId);
        Task<double> GetDriverAverageRatingAsync(int driverId);
        Task AddAsync(Rating rating);
    }
}