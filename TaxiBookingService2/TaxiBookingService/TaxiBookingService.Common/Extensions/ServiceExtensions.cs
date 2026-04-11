using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TaxiBookingService.Common.Helpers;
using TaxiBookingService.Common.SignalR;
using TaxiBookingService.Common.SignalR.Interfaces;
using TaxiBookingService.Repositories;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Repositories.Persistence;
using TaxiBookingService.Repositories.Repositories;
using TaxiBookingService.Services.Implementations;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRideRepository, RideRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRideService, RideService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IFareService, FareService>();
            services.AddScoped<IHaversineHelper,HaversineHelper>();
            services.AddScoped<IOtpHelper, OtpHelper>();
            services.AddScoped<JwtService>();
            services.AddHttpClient<IMapService, OsrmMapService>();
            services.AddSingleton<IDriverLocationStoreService, DriverLocationStoreService>();
            services.AddSingleton<IConnectionHandler, ConnectionHandler>();
            services.AddSingleton<IHubService, HubService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string secretKey = configuration["JwtSettings:SecretKey"]!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string? accessToken = context.Request.Query["access_token"];
                        bool isHubPath = context.HttpContext.Request.Path
                            .Value?.StartsWith("/hubs") ?? false;

                        if (!string.IsNullOrEmpty(accessToken) && isHubPath)
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Taxi Booking API",
                    Version = "v1",
                    Description = "Production-grade Taxi Booking Service API"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            return services;
        }
    }
}
