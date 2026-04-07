namespace TaxiBookingService.Models.DTOs;

public class DriverLocationDto
{
    public int DriverId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CabType { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateLocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class NearbyDriverDto
{
    public int DriverId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public int CabType { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double DistanceKm { get; set; }
    public DateTime LastUpdated { get; set; }
}