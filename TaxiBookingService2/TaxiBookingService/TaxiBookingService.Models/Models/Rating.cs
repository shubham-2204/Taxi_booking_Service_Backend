using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Models.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public int RaterId { get; set; }
        public int RatedDriverId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Ride Ride { get; set; } = null!;
        public User Rater { get; set; } = null!;
        public User RatedDriver { get; set; } = null!;
    }
}