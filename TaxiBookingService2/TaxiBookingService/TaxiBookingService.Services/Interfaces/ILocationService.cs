using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<NearbyDriverDto>> GetNearbyDriversAsync(
        double latitude, double longitude,
        int cabType, double radiusKm = 5.0);

    void UpdateDriverLocation(
        int driverId, double latitude, double longitude,
        int cabType, bool isAvailable);

    Task<List<int>> GetTopDriversForRideAsync(
        double pickupLat, double pickupLng, int cabType);
}
