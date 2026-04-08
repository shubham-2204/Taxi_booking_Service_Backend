namespace TaxiBookingService.Common.SignalR.Interfaces
{
    public interface IConnectionHandler
    {
        void Add(int userId, string connectionId);

        void Remove(int userId);

        string? GetConnectionId(int userId);
    }
}