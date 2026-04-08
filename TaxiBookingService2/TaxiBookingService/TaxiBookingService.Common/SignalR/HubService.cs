using Microsoft.AspNetCore.SignalR;
using TaxiBookingService.Common.SignalR.Interfaces;


namespace TaxiBookingService.Common.SignalR
{
    public class HubService : IHubService
    {
        private readonly IHubContext<RideHub> _hubContext;
        private readonly IConnectionHandler _connectionHandler;


        public HubService(IHubContext<RideHub> hubContext, IConnectionHandler connectionHandler)
        {
            _hubContext = hubContext;
            _connectionHandler = connectionHandler;
        }

        public async Task NotifyDriverAsync(int driverId, string eventName, object data)
        {
            string? connectionId = _connectionHandler.GetConnectionId(driverId);

            if (connectionId is not null)
                await _hubContext.Clients
                    .Client(connectionId)
                    .SendAsync(eventName, data);
        }

        public async Task NotifyPassengerAsync(int passengerId, string eventName, object data)
        {
            string? connectionId = _connectionHandler.GetConnectionId(passengerId);

            if (connectionId is not null)
                await _hubContext.Clients
                    .Client(connectionId)
                    .SendAsync(eventName, data);
        }

        public async Task NotifyGroupAsync(string groupName, string eventName, object data)
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync(eventName, data);
        }
    }
}
