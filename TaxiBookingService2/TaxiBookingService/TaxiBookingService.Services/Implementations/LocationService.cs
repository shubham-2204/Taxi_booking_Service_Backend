using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Services.Implementations;

public class LocationService : ILocationService
{
    private readonly IDriverLocationStoreService _driverLocationStore;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapService _mapService;

    public LocationService(
        IDriverLocationStoreService driverLocationStore,
        IUnitOfWork unitOfWork,
        IMapService mapService)
    {
        _driverLocationStore = driverLocationStore;
        _unitOfWork = unitOfWork;
        _mapService = mapService;
    }

    public void UpdateDriverLocation(
        int driverId, double latitude, double longitude,
        int cabType, bool isAvailable)
    {
        _driverLocationStore.UpdateLocation(
            driverId, latitude, longitude, cabType, isAvailable);
    }

    public async Task<IEnumerable<NearbyDriverDto>> GetNearbyDriversAsync(
        double latitude, double longitude,
        int cabType, double radiusKm = 5.0)
    {
        IEnumerable<DriverLocationDto> allLocations =
            _driverLocationStore.GetAllLocations();

        List<DriverLocationDto> nearbyLocations = allLocations
            .Where(d =>
                d.IsAvailable &&
                d.CabType == cabType &&
                HaversineHelper.CalculateDistance(
                    latitude, longitude,
                    d.Latitude, d.Longitude) <= radiusKm)
            .ToList();

        if (!nearbyLocations.Any())
            return Enumerable.Empty<NearbyDriverDto>();

        List<NearbyDriverDto> result = new();

        foreach (DriverLocationDto driverLocation in nearbyLocations)
        {
            var driver = await _unitOfWork.Users
                .GetByIdAsync(driverLocation.DriverId);

            if (driver == null) continue;

            result.Add(new NearbyDriverDto
            {
                DriverId = driverLocation.DriverId,
                FullName = driver.FullName,
                VehicleModel = driver.Vehicle?.Model ?? string.Empty,
                PlateNumber = driver.Vehicle?.PlateNumber ?? string.Empty,
                CabType = driverLocation.CabType,
                Latitude = driverLocation.Latitude,
                Longitude = driverLocation.Longitude,
                DistanceKm = Math.Round(
                    HaversineHelper.CalculateDistance(
                        latitude, longitude,
                        driverLocation.Latitude,
                        driverLocation.Longitude), 2),
                LastUpdated = driverLocation.UpdatedAt
            });
        }

        return result.OrderBy(d => d.DistanceKm);
    }

    public async Task<List<int>> GetTopDriversForRideAsync(
        double pickupLat, double pickupLng, int cabType)
    {
        IEnumerable<DriverLocationDto> allDrivers =
            _driverLocationStore.GetAllLocations();

        List<DriverLocationDto> filtered = allDrivers
            .Where(d => d.IsAvailable && d.CabType == cabType)
            .ToList();

        if (!filtered.Any())
            return new List<int>();

        List<(DriverLocationDto Driver, double StraightKm)> top7 = filtered
            .Select(d => (Driver: d, StraightKm: HaversineHelper.CalculateDistance(
                pickupLat, pickupLng, d.Latitude, d.Longitude)))
            .OrderBy(x => x.StraightKm)
            .Take(7)
            .ToList();

        IEnumerable<Task<(int DriverId, double RoadKm)>> osrmTasks =
            top7.Select(async candidate =>
            {
                DistanceResultDto result = await _mapService
                    .GetDistanceAndDurationAsync(
                        pickupLat, pickupLng,
                        candidate.Driver.Latitude,
                        candidate.Driver.Longitude);

                return (candidate.Driver.DriverId, result.DistanceKm);
            });

        (int DriverId, double RoadKm)[] osrmResults =
            await Task.WhenAll(osrmTasks);

        return osrmResults
            .OrderBy(x => x.RoadKm)
            .Select(x => x.DriverId)
            .ToList();
    }
}