using Microsoft.EntityFrameworkCore;
using TaxiBookingService.Models.Models;

namespace TaxiBookingService.Repositories.Persistence
{
    public static class DataSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CancellationReason>().HasData(
                new CancellationReason { Id = 1, ReasonText = "Driver is taking too long", IsActive = true },
                new CancellationReason { Id = 2, ReasonText = "Booked by mistake", IsActive = true },
                new CancellationReason { Id = 3, ReasonText = "Change of plans", IsActive = true },
                new CancellationReason { Id = 4, ReasonText = "Found alternative transport", IsActive = true },
                new CancellationReason { Id = 5, ReasonText = "Personal emergency", IsActive = true }
            );
        }
    }
}
