using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Models.Enums;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Services.Interfaces;


namespace TaxiBookingService.Services.Implementations
{
    public class RideService : IRideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFareService _fareService;
        private readonly IMapService _mapService;
        private readonly ILocationService _locationService;

        public RideService(
            IUnitOfWork unitOfWork,
            IFareService fareService,
            IMapService mapService,
            ILocationService locationService
            )
        {
            _unitOfWork = unitOfWork;
            _fareService = fareService;
            _mapService = mapService;
            _locationService = locationService;
        }

        public async Task<ApiResponseDto<RideEstimateResponseDto>> GetEstimateAsync(
            CreateRideRequestDto dto)
        {
            CabType cabType = (CabType)dto.CabType;

            DistanceResultDto distanceResult = await _mapService.GetDistanceAndDurationAsync(
                dto.PickupLatitude, dto.PickupLongitude,
                dto.DropOffLatitude, dto.DropOffLongitude);

            decimal estimatedFare = _fareService.CalculateFare(
                distanceResult.DistanceKm, cabType);

            RideEstimateResponseDto response = new RideEstimateResponseDto
            {
                EstimatedFare = estimatedFare,
                EstimatedDurationMinutes = distanceResult.DurationMinutes,
                DistanceKm = distanceResult.DistanceKm,
                CabType = cabType.ToString()
            };

            return ApiResponseDto<RideEstimateResponseDto>.SuccessResponse(
                response, "Estimate calculated");
        }

        public async Task<ApiResponseDto<RideResponseDto>> CreateRideAsync(
            int passengerId, CreateRideRequestDto dto)
        {
            CabType cabType = (CabType)dto.CabType;

            DistanceResultDto distanceResult = await _mapService.GetDistanceAndDurationAsync(
                dto.PickupLatitude, dto.PickupLongitude,
                dto.DropOffLatitude, dto.DropOffLongitude);

            decimal estimatedFare = _fareService.CalculateFare(
                distanceResult.DistanceKm, cabType);

            Ride ride = new Ride
            {
                PassengerId = passengerId,
                PickupAddress = dto.PickupAddress,
                PickupLatitude = dto.PickupLatitude,
                PickupLongitude = dto.PickupLongitude,
                DropOffAddress = dto.DropOffAddress,
                DropOffLatitude = dto.DropOffLatitude,
                DropOffLongitude = dto.DropOffLongitude,
                CabType = cabType,
                Status = RideStatus.Requested,
                EstimatedFare = estimatedFare,
                EstimatedDurationMinutes = distanceResult.DurationMinutes,
                RequestedAt = DateTime.UtcNow
            };

            await _unitOfWork.Rides.AddAsync(ride);
            await _unitOfWork.SaveChangesAsync();

            List<int> matchedDriverIds = await _locationService
    .GetTopDriversForRideAsync(
        dto.PickupLatitude, dto.PickupLongitude, (int)cabType);

            if (matchedDriverIds.Any())
            {
                ride.DriverId = matchedDriverIds.First();
                ride.Status = RideStatus.DriverAssigned;
                _unitOfWork.Rides.Update(ride);
                await _unitOfWork.SaveChangesAsync();
            }

            Ride? rideWithDetails = await _unitOfWork.Rides.GetByIdWithDetailsAsync(ride.Id);

            RideResponseDto response = MapToRideResponseDto(rideWithDetails ?? ride);

            return ApiResponseDto<RideResponseDto>.SuccessResponse(
                response, "Ride booked successfully", 201);
        }

        public async Task<ApiResponseDto<RideResponseDto>> GetRideByIdAsync(
            int rideId, int requestingUserId)
        {
            Ride? ride = await _unitOfWork.Rides.GetByIdWithDetailsAsync(rideId);

            if (ride == null)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "Ride not found", 404);

            bool isPassenger = ride.PassengerId == requestingUserId;
            bool isDriver = ride.DriverId == requestingUserId;

            if (!isPassenger && !isDriver)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "You are not authorized to view this ride", 403);

            return ApiResponseDto<RideResponseDto>.SuccessResponse(
                MapToRideResponseDto(ride), "Ride fetched");
        }

        public async Task<ApiResponseDto<IEnumerable<RideResponseDto>>> GetPassengerHistoryAsync(
            int passengerId, int page, int pageSize)
        {
            IEnumerable<Ride> rides = await _unitOfWork.Rides
                .GetPassengerHistoryAsync(passengerId, page, pageSize);

            IEnumerable<RideResponseDto> result = rides.Select(MapToRideResponseDto);

            return ApiResponseDto<IEnumerable<RideResponseDto>>
                .SuccessResponse(result, "History fetched");
        }

        public async Task<ApiResponseDto<IEnumerable<RideResponseDto>>> GetDriverHistoryAsync(
            int driverId, int page, int pageSize)
        {
            IEnumerable<Ride> rides = await _unitOfWork.Rides
                .GetDriverHistoryAsync(driverId, page, pageSize);

            IEnumerable<RideResponseDto> result = rides.Select(MapToRideResponseDto);

            return ApiResponseDto<IEnumerable<RideResponseDto>>
                .SuccessResponse(result, "History fetched");
        }

        public async Task<ApiResponseDto<CancelRideResponseDto>> CancelRideAsync(
            int rideId, int passengerId, CancelRideRequestDto dto)
        {
            Ride? ride = await _unitOfWork.Rides.GetByIdAsync(rideId);

            if (ride == null)
                return ApiResponseDto<CancelRideResponseDto>.FailureResponse(
                    "Ride not found", 404);

            if (ride.PassengerId != passengerId)
                return ApiResponseDto<CancelRideResponseDto>.FailureResponse(
                    "You are not authorized to cancel this ride", 403);

            if (ride.Status == RideStatus.RideStarted ||
                ride.Status == RideStatus.RideCompleted ||
                ride.Status == RideStatus.Cancelled)
                return ApiResponseDto<CancelRideResponseDto>.FailureResponse(
                    "This ride cannot be cancelled", 400);

            bool isPostAssignment = ride.Status == RideStatus.DriverAssigned ||
                                    ride.Status == RideStatus.DriverArriving;

            decimal cancellationFee = 0;

            if (isPostAssignment)
            {
                if (dto.ReasonId == null)
                    return ApiResponseDto<CancelRideResponseDto>.FailureResponse(
                        "Cancellation reason is required", 400);

                cancellationFee = Math.Round(ride.EstimatedFare * 0.05m, 2);
            }

            ride.Status = RideStatus.Cancelled;
            _unitOfWork.Rides.Update(ride);

            RideCancellation cancellation = new RideCancellation
            {
                RideId = rideId,
                CancelledByUserId = passengerId,
                ReasonId = dto.ReasonId,
                CancellationFee = cancellationFee,
                CancelledAt = DateTime.UtcNow
            };

            await _unitOfWork.Rides.AddCancellationAsync(cancellation);
            await _unitOfWork.SaveChangesAsync();

            CancelRideResponseDto response = new CancelRideResponseDto
            {
                RideId = rideId,
                Status = ride.Status.ToString(),
                CancellationFee = cancellationFee
            };

            return ApiResponseDto<CancelRideResponseDto>.SuccessResponse(
                response, "Ride cancelled");
        }

        public async Task<ApiResponseDto<RideResponseDto>> VerifyOtpAsync(
            int rideId, int driverId, string otp)
        {
            Ride? ride = await _unitOfWork.Rides.GetByIdWithDetailsAsync(rideId);

            if (ride == null)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "Ride not found", 404);

            if (ride.DriverId != driverId)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "You are not the assigned driver", 403);

            if (ride.Status != RideStatus.DriverArriving)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "OTP verification is not valid at this stage", 400);

            if (OtpHelper.IsExpired(ride.OtpExpiresAt))
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "OTP has expired", 410);

            if (ride.RideOtp != otp)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "Invalid OTP", 400);

            ride.Status = RideStatus.RideStarted;
            ride.RideOtp = null;
            ride.OtpExpiresAt = null;
            ride.PickedUpAt = DateTime.UtcNow;

            _unitOfWork.Rides.Update(ride);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<RideResponseDto>.SuccessResponse(
                MapToRideResponseDto(ride), "OTP verified. Ride started.");
        }

        public async Task<ApiResponseDto<RideResponseDto>> CompleteRideAsync(
            int rideId, int driverId)
        {
            Ride? ride = await _unitOfWork.Rides.GetByIdWithDetailsAsync(rideId);

            if (ride == null)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "Ride not found", 404);

            if (ride.DriverId != driverId)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "You are not the assigned driver", 403);

            if (ride.Status != RideStatus.RideStarted)
                return ApiResponseDto<RideResponseDto>.FailureResponse(
                    "Ride cannot be completed at this stage", 400);

            ride.Status = RideStatus.RideCompleted;
            ride.FinalFare = ride.EstimatedFare;
            ride.CompletedAt = DateTime.UtcNow;

            _unitOfWork.Rides.Update(ride);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<RideResponseDto>.SuccessResponse(
                MapToRideResponseDto(ride), "Ride completed");
        }

        public async Task<ApiResponseDto<IEnumerable<CancellationReasonResponseDto>>>
            GetCancellationReasonsAsync()
        {
            IEnumerable<CancellationReason> reasons = await _unitOfWork.Rides
                .GetCancellationReasonsAsync();

            IEnumerable<CancellationReasonResponseDto> result = reasons.Select(r =>
                new CancellationReasonResponseDto
                {
                    Id = r.Id,
                    ReasonText = r.ReasonText
                });

            return ApiResponseDto<IEnumerable<CancellationReasonResponseDto>>
                .SuccessResponse(result, "Reasons fetched");
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
