namespace TaxiBookingService.Models.Enums
{
    public enum RideStatus
    {
        Requested = 0,
        DriverAssigned = 1,
        DriverArriving = 2,
        RideStarted = 3,
        RideCompleted = 4,
        Cancelled = 5
    }
}
