namespace TaxiBookingService.Models.DTOs
{
    public class RideResponseDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string DropOffAddress { get; set; } = string.Empty;
        public string CabType { get; set; } = string.Empty;
        public decimal EstimatedFare { get; set; }
        public decimal? FinalFare { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public PassengerDto? Passenger { get; set; }
        public AssignedDriverDto? Driver { get; set; }
    }

    public class PassengerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AssignedDriverDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public string VehicleColor { get; set; } = string.Empty;
    }
}