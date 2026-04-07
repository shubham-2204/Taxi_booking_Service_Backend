using Microsoft.EntityFrameworkCore;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Repositories.Persistence;

namespace TaxiBookingService.Repositories.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _context;

        public RatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RideAlreadyRatedAsync(int rideId)
        {
            return await _context.Ratings
                .AnyAsync(r => r.RideId == rideId);
        }

        public async Task<IEnumerable<Rating>> GetDriverRatingsAsync(
            int driverId, int page, int pageSize)
        {
            return await _context.Ratings
                .Where(r => r.RatedDriverId == driverId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetDriverRatingsCountAsync(int driverId)
        {
            return await _context.Ratings
                .CountAsync(r => r.RatedDriverId == driverId);
        }

        public async Task<double> GetDriverAverageRatingAsync(int driverId)
        {
            bool hasRatings = await _context.Ratings
                .AnyAsync(r => r.RatedDriverId == driverId);

            if (!hasRatings)
                return 0.0;

            return await _context.Ratings
                .Where(r => r.RatedDriverId == driverId)
                .AverageAsync(r => r.Stars);
        }

        public async Task AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
        }
    }
}