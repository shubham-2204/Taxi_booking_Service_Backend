using TaxiBookingService.Models.Enums;

namespace TaxiBookingService.Models.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public CabType CabType { get; set; }

        public User Driver { get; set; } = null!;
    }
}