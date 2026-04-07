namespace TaxiBookingService.Models.DTOs
{
    public class RideEstimateResponseDto
    {
        public decimal EstimatedFare { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public double DistanceKm { get; set; }
        public string CabType { get; set; } = string.Empty;
    }
}