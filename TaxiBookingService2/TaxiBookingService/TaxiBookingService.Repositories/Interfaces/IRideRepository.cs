using TaxiBookingService.Models.Models;

namespace TaxiBookingService.Repositories.Interfaces
{
    public interface IRideRepository
    {
        Task<Ride?> GetByIdAsync(int id);
        Task<Ride?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Ride>> GetPassengerHistoryAsync(int passengerId, int page, int pageSize);
        Task<IEnumerable<Ride>> GetDriverHistoryAsync(int driverId, int page, int pageSize);
        Task<int> GetPassengerHistoryCountAsync(int passengerId);
        Task<int> GetDriverHistoryCountAsync(int driverId);
        Task<IEnumerable<CancellationReason>> GetCancellationReasonsAsync();
        Task AddAsync(Ride ride);
        Task AddCancellationAsync(RideCancellation cancellation);
        void Update(Ride ride);
    }
}
