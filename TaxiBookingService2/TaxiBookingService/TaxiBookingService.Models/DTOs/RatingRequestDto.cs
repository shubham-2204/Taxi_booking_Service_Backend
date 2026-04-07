namespace TaxiBookingService.Models.DTOs
{
    public class CreateRatingRequestDto
    {
        public int RideId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }

    public class RatingResponseDto
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public double DriverNewAverageRating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
