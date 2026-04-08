using Microsoft.EntityFrameworkCore;
using TaxiBookingService.Models.Models;

namespace TaxiBookingService.Repositories.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Ride> Rides => Set<Ride>();
        public DbSet<RideCancellation> RideCancellations => Set<RideCancellation>();
        public DbSet<CancellationReason> CancellationReasons => Set<CancellationReason>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.AverageRating).HasColumnType("decimal(3,2)");

                entity.HasOne(u => u.Vehicle)
                    .WithOne(v => v.Driver)
                    .HasForeignKey<Vehicle>(v => v.DriverId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.PassengerRides)
                    .WithOne(r => r.Passenger)
                    .HasForeignKey(r => r.PassengerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.DriverRides)
                    .WithOne(r => r.Driver)
                    .HasForeignKey(r => r.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(t => t.User)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.RatingsGiven)
                    .WithOne(r => r.Rater)
                    .HasForeignKey(r => r.RaterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.RatingsReceived)
                    .WithOne(r => r.RatedDriver)
                    .HasForeignKey(r => r.RatedDriverId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasIndex(v => v.DriverId).IsUnique();
                entity.HasIndex(v => v.PlateNumber).IsUnique();
                entity.Property(v => v.PlateNumber).IsRequired().HasMaxLength(20);
                entity.Property(v => v.Model).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Color).IsRequired().HasMaxLength(30);
                entity.Property(v => v.CabType).IsRequired();
            });

            modelBuilder.Entity<Ride>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.PickupAddress).IsRequired().HasMaxLength(255);
                entity.Property(r => r.DropOffAddress).IsRequired().HasMaxLength(255);
                entity.Property(r => r.EstimatedFare).HasColumnType("decimal(10,2)");
                entity.Property(r => r.FinalFare).HasColumnType("decimal(10,2)");
                entity.Property(r => r.RideOtp).HasMaxLength(6);
                entity.Property(r => r.Status).IsRequired();
                entity.Property(r => r.CabType).IsRequired();

                entity.HasOne(r => r.RideCancellation)
                    .WithOne(rc => rc.Ride)
                    .HasForeignKey<RideCancellation>(rc => rc.RideId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Rating)
                    .WithOne(rt => rt.Ride)
                    .HasForeignKey<Rating>(rt => rt.RideId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RideCancellation>(entity =>
            {
                entity.HasKey(rc => rc.Id);
                entity.HasIndex(rc => rc.RideId).IsUnique();
                entity.Property(rc => rc.CancellationFee).HasColumnType("decimal(10,2)");

                entity.HasOne(rc => rc.Reason)
                    .WithMany()
                    .HasForeignKey(rc => rc.ReasonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rc => rc.CancelledBy)
                    .WithMany()
                    .HasForeignKey(rc => rc.CancelledByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(r => r.RideId).IsUnique();
                entity.Property(r => r.Stars).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(500);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<CancellationReason>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.ReasonText).IsRequired().HasMaxLength(150);
            });

            DataSeeder.Seed(modelBuilder);
        }
    }
}