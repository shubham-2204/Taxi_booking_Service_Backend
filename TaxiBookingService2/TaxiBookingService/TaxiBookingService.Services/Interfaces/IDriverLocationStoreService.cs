using TaxiBookingService.Models.DTOs;
namespace TaxiBookingService.Services.Interfaces
{
    public interface IDriverLocationStoreService
    {
        void UpdateLocation(int driverId, double latitude, double longitude, int cabType, bool isAvailable);
        IEnumerable<DriverLocationDto> GetAllLocations();
        bool TryRemove(int driverId);
    }
}
