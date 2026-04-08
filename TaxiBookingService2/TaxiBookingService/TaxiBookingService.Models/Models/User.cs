using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Models.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsAvailable { get; set; } = false;
        public double AverageRating { get; set; } = 0.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Vehicle? Vehicle { get; set; }
        public ICollection<Ride> PassengerRides { get; set; } = new List<Ride>();
        public ICollection<Ride> DriverRides { get; set; } = new List<Ride>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Rating> RatingsGiven { get; set; } = new List<Rating>();
        public ICollection<Rating> RatingsReceived { get; set; } = new List<Rating>();
    }
}