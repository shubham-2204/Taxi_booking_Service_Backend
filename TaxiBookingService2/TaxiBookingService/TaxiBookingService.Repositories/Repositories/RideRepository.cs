using Microsoft.EntityFrameworkCore;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Repositories.Persistence;

namespace TaxiBookingService.Repositories.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly AppDbContext _context;

        public RideRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ride?> GetByIdAsync(int id)
        {
            return await _context.Rides
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Ride?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Rides
                .Include(r => r.Passenger)
                .Include(r => r.Driver)
                    .ThenInclude(d => d != null ? d.Vehicle : null)
                .Include(r => r.RideCancellation)
                    .ThenInclude(rc => rc != null ? rc.Reason : null)
                .Include(r => r.Rating)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Ride>> GetPassengerHistoryAsync(
            int passengerId, int page, int pageSize)
        {
            return await _context.Rides
                .Where(r => r.PassengerId == passengerId)
                .OrderByDescending(r => r.RequestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ride>> GetDriverHistoryAsync(
            int driverId, int page, int pageSize)
        {
            return await _context.Rides
                .Where(r => r.DriverId == driverId)
                .OrderByDescending(r => r.RequestedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetPassengerHistoryCountAsync(int passengerId)
        {
            return await _context.Rides
                .CountAsync(r => r.PassengerId == passengerId);
        }

        public async Task<int> GetDriverHistoryCountAsync(int driverId)
        {
            return await _context.Rides
                .CountAsync(r => r.DriverId == driverId);
        }

        public async Task<IEnumerable<CancellationReason>> GetCancellationReasonsAsync()
        {
            return await _context.CancellationReasons
                .Where(cr => cr.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(Ride ride)
        {
            await _context.Rides.AddAsync(ride);
        }

        public async Task AddCancellationAsync(RideCancellation cancellation)
        {
            await _context.RideCancellations.AddAsync(cancellation);
        }

        public void Update(Ride ride)
        {
            _context.Rides.Update(ride);
        }
    }
}