using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IHubService
    {
        Task NotifyDriverAsync(int driverId, string eventName, object data);
        Task NotifyPassengerAsync(int passengerId, string eventName, object data);
        Task NotifyGroupAsync(string groupName, string eventName, object data);
    }
}
