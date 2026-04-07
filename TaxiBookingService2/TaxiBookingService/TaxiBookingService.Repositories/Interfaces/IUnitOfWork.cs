namespace TaxiBookingService.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRideRepository Rides { get; }
        IRatingRepository Ratings { get; }
        Task<int> SaveChangesAsync();
    }
}