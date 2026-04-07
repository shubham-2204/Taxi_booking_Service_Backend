using System.Collections.Concurrent;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Repositories;

public class DriverLocationStoreService : IDriverLocationStoreService
{
    private readonly ConcurrentDictionary<int, DriverLocationDto> _locations = new();

    public void UpdateLocation(
        int driverId, double latitude, double longitude,
        int cabType, bool isAvailable)
    {
        _locations.AddOrUpdate(
            key: driverId,
            addValue: new DriverLocationDto
            {
                DriverId = driverId,
                Latitude = latitude,
                Longitude = longitude,
                CabType = cabType,
                IsAvailable = isAvailable,
                UpdatedAt = DateTime.UtcNow
            },
            updateValueFactory: (key, old) => new DriverLocationDto
            {
                DriverId = driverId,
                Latitude = latitude,
                Longitude = longitude,
                CabType = cabType,
                IsAvailable = isAvailable,
                UpdatedAt = DateTime.UtcNow
            });
    }

    public IEnumerable<DriverLocationDto> GetAllLocations()
    {
        return _locations.Values;
    }

    public bool TryRemove(int driverId)
    {
        return _locations.TryRemove(driverId, out _);
    }
}