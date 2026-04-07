using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TaxiBookingService.Services.Interfaces;
using TaxiBookingService;


namespace TaxiBookingService.Services.Implementations
{
    public class HubService : IHubService
    {
        private readonly IHubContext<RideHub> _hubContext;

        public HubService(IHubContext<RideHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyDriverAsync(int driverId, string eventName, object data)
        {
            string? connectionId = _hubContext.ConnectionStore.GetConnectionId(driverId);

            if (connectionId is not null)
                await _hubContext.Clients
                    .Client(connectionId)
                    .SendAsync(eventName, data);
        }

        public async Task NotifyPassengerAsync(int passengerId, string eventName, object data)
        {
            string? connectionId = ConnectionStore.GetConnectionId(passengerId);

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
