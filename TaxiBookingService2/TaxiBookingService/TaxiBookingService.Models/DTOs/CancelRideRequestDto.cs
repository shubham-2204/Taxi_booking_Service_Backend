namespace TaxiBookingService.Models.DTOs
{
    public class CancelRideRequestDto
    {
        public int? ReasonId { get; set; }
    }

    public class CancelRideResponseDto
    {
        public int RideId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal CancellationFee { get; set; }
    }
}