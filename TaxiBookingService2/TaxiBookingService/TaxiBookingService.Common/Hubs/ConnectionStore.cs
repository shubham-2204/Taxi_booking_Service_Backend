using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Common.Hubs
{
    public static class ConnectionStore
    {
        private static readonly ConcurrentDictionary<int, string> _connections = new();

        public static void Add(int userId, string connectionId)
            => _connections[userId] = connectionId;

        public static void Remove(int userId)
            => _connections.TryRemove(userId, out _);

        public static string? GetConnectionId(int userId)
            => _connections.TryGetValue(userId, out string? connId) ? connId : null;
    }
}
