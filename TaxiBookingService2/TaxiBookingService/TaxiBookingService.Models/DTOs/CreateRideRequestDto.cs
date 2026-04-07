namespace TaxiBookingService.Models.DTOs
{
    public class CreateRideRequestDto
    {
        public string PickupAddress { get; set; } = string.Empty;
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public string DropOffAddress { get; set; } = string.Empty;
        public double DropOffLatitude { get; set; }
        public double DropOffLongitude { get; set; }
        public int CabType { get; set; }
    }
}