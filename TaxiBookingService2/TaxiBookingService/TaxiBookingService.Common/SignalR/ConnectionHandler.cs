using System.Collections.Concurrent;
using TaxiBookingService.Common.SignalR.Interfaces;

namespace TaxiBookingService.Common.SignalR
{
    public class ConnectionHandler : IConnectionHandler
    {
        private readonly ConcurrentDictionary<int, string> _connections = new();

        public void Add(int userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public void Remove(int userId)
        {
            _connections.TryRemove(userId, out _);
        }

        public string? GetConnectionId(int userId)
        {
            return _connections.TryGetValue(userId, out string? connId)
                ? connId
                : null;
        }
    }
}