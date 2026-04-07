
using TaxiBookingService.Common.Extensions;
using TaxiBookingService.Repositories;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Repositories.Repositories;
using TaxiBookingService.Services.Interfaces;

namespace TBS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            

            var repositoryMappings = new (Type, Type)[]
            {
                (typeof(IUnitOfWork), typeof(UnitOfWork)),
                (typeof(IDriverLocationStoreService), typeof(DriverLocationStore)),
            };


            ServiceExtensions.AddRepositories(builder.Services,repositoryMappings);


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
