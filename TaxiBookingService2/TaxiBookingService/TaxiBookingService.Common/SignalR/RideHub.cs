using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TaxiBookingService.Common.Constants;
using TaxiBookingService.Common.SignalR.Interfaces;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Common.SignalR
{
    [Authorize]
    public class RideHub : Hub
    {
        private readonly ILocationService _locationService;
        private readonly IConnectionHandler _connectionHandler;
         

        public RideHub(ILocationService locationService, IConnectionHandler connectionHandler)
        {
            _locationService = locationService;
            _connectionHandler = connectionHandler;
        }

        public override async Task OnConnectedAsync()
        {
            int userId = int.Parse(
                Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

            string? role = Context.User!.FindFirstValue(ClaimTypes.Role);

            _connectionHandler.Add(userId, Context.ConnectionId);


            if (role == AppConstants.DriverRole)
                await Groups.AddToGroupAsync(Context.ConnectionId, "Drivers");
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, "Passengers");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId = int.Parse(
                Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

            _connectionHandler.Remove(userId);

            await base.OnDisconnectedAsync(exception);
        }



        public async Task JoinRideGroup(int rideId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                SignalRConstants.RideGroup(rideId));
        }

        public async Task LeaveRideGroup(int rideId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                SignalRConstants.RideGroup(rideId));
        }



        public async Task UpdateLocation(
            int rideId, double latitude, double longitude)
        {
            int driverId = int.Parse(
                Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);


            _locationService.UpdateDriverLocation(
                driverId, latitude, longitude,
                cabType: 0,
                isAvailable: true);


            await Clients
                .Group(SignalRConstants.RideGroup(rideId))
                .SendAsync(SignalRConstants.DriverLocationUpdated, new
                {
                    latitude,
                    longitude,
                    updatedAt = DateTime.UtcNow
                });
        }
    }

}