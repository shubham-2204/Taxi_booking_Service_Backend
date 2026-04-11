using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Repositories.Persistence;


namespace TaxiBookingService.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; }
        public IRideRepository Rides { get; }
        public IRatingRepository Ratings { get; }

        public UnitOfWork(
            AppDbContext context,
            IUserRepository users,
            IRideRepository rides,
            IRatingRepository ratings)
        {
            _context = context;
            Users = users;
            Rides = rides;
            Ratings = ratings;
        }

        public async Task<int> SaveChangesAsync()
        {
               return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        

        
    }
}

