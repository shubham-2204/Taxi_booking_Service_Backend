using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Models.Models
{
    public class Ride
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int? DriverId { get; set; }
        public string PickupAddress { get; set; } = string.Empty;
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public string DropOffAddress { get; set; } = string.Empty;
        public double DropOffLatitude { get; set; }
        public double DropOffLongitude { get; set; }
        public CabType CabType { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Requested;
        public decimal EstimatedFare { get; set; }
        public decimal? FinalFare { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public string? RideOtp { get; set; }
        public DateTime? OtpExpiresAt { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PickedUpAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public User Passenger { get; set; } = null!;
        public User? Driver { get; set; }
        public RideCancellation? RideCancellation { get; set; }
        public Rating? Rating { get; set; }
    }
}