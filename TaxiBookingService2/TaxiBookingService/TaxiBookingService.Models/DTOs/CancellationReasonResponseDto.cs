namespace TaxiBookingService.Models.DTOs
{
    public class CancellationReasonResponseDto
    {
        public int Id { get; set; }
        public string ReasonText { get; set; } = string.Empty;
    }
}
