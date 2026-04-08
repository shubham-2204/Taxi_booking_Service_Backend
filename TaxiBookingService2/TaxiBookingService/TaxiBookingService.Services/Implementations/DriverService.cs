    using TaxiBookingService.Models.DTOs;
    using TaxiBookingService.Models.Enums;
    using TaxiBookingService.Models.Models;
    using TaxiBookingService.Repositories.Interfaces;
    using TaxiBookingService.Services.Interfaces;

    namespace TaxiBookingService.Services.Implementations
    {
        public class DriverService : IDriverService
        {
            private readonly IUnitOfWork _unitOfWork;
        

            public DriverService(
                IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<ApiResponseDto<RideResponseDto>> RespondToRideAsync(
                int rideId, int driverId, bool accept)
            {
                Ride? ride = await _unitOfWork.Rides.GetByIdWithDetailsAsync(rideId);

                if (ride == null)
                    return ApiResponseDto<RideResponseDto>.FailureResponse(
                        "Ride not found", 404);

                if (ride.Status != RideStatus.DriverAssigned || ride.DriverId != driverId)
                    return ApiResponseDto<RideResponseDto>.FailureResponse(
                        "This ride cannot be responded to", 400);

                if (!accept)
                {
                    ride.Status = RideStatus.Requested;
                    ride.DriverId = null;
                    _unitOfWork.Rides.Update(ride);
                    await _unitOfWork.SaveChangesAsync();

                    return ApiResponseDto<RideResponseDto>.FailureResponse(
                        "Ride declined", 200);
                }

                ride.Status = RideStatus.DriverArriving;
                _unitOfWork.Rides.Update(ride);
                await _unitOfWork.SaveChangesAsync();

                RideResponseDto dto = MapToRideResponseDto(ride);

                return ApiResponseDto<RideResponseDto>.SuccessResponse(dto, "Ride accepted");
            }

            public async Task<ApiResponseDto<string>> UpdateAvailabilityAsync(
                int driverId, bool isAvailable)
            {
                User? driver = await _unitOfWork.Users.GetByIdAsync(driverId);

                if (driver == null)
                    return ApiResponseDto<string>.FailureResponse("Driver not found", 404);

                driver.IsAvailable = isAvailable;
                _unitOfWork.Users.Update(driver);
                await _unitOfWork.SaveChangesAsync();

                

                string message = isAvailable ? "You are now online" : "You are now offline";

                return ApiResponseDto<string>.SuccessResponse(
                    isAvailable.ToString(), message);
            }

            private RideResponseDto MapToRideResponseDto(Ride ride)
            {
                return new RideResponseDto
                {
                    Id = ride.Id,
                    Status = ride.Status.ToString(),
                    PickupAddress = ride.PickupAddress,
                    DropOffAddress = ride.DropOffAddress,
                    CabType = ride.CabType.ToString(),
                    EstimatedFare = ride.EstimatedFare,
                    FinalFare = ride.FinalFare,
                    EstimatedDurationMinutes = ride.EstimatedDurationMinutes,
                    RequestedAt = ride.RequestedAt,
                    CompletedAt = ride.CompletedAt,
                    Passenger = ride.Passenger == null ? null : new PassengerDto
                    {
                        Id = ride.Passenger.Id,
                        FullName = ride.Passenger.FullName,
                        PhoneNumber = ride.Passenger.PhoneNumber
                    },
                    Driver = ride.Driver == null ? null : new AssignedDriverDto
                    {
                        Id = ride.Driver.Id,
                        FullName = ride.Driver.FullName,
                        PhoneNumber = ride.Driver.PhoneNumber,
                        AverageRating = ride.Driver.AverageRating,
                        VehicleModel = ride.Driver.Vehicle?.Model ?? string.Empty,
                        VehiclePlate = ride.Driver.Vehicle?.PlateNumber ?? string.Empty,
                        VehicleColor = ride.Driver.Vehicle?.Color ?? string.Empty
                    }
                };
            }
        }
    }
