using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Models.Models
{
    public class RideCancellation
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public int CancelledByUserId { get; set; }
        public int? ReasonId { get; set; }
        public decimal CancellationFee { get; set; } = 0;
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

        public Ride Ride { get; set; } = null!;
        public User CancelledBy { get; set; } = null!;
        public CancellationReason? Reason { get; set; }
    }
}