namespace TaxiBookingService.Models.Models
{
    public class CancellationReason
    {
        public int Id { get; set; }
        public string ReasonText { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}