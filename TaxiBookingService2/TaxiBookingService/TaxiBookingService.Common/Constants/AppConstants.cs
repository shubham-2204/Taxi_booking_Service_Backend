using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Common.Constants
{
    public static class AppConstants
    {
        public const string PassengerRole = "Passenger";
        public const string DriverRole = "Driver";
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 10;
        public const int OtpExpiryMinutes = 10;
        public const int DriverResponseTimeoutSeconds = 60;
        public const int MaxDriverRetries = 3;
    }
    public static class MessageConstants
    {
        public const string UnauthorizedAccess = "You are not authorized to perform this action";
        public const string ServerError = "An unexpected error occurred. Please try again later";
        public const string ValidationFailed = "One or more validation errors occurred";
        public const string NotFound = "The requested resource was not found";
        public const string RideNotFound = "Ride not found";
        public const string UserNotFound = "User not found";
        public const string DriverNotFound = "Driver not found";
        public const string InvalidCredentials = "Invalid email or password";
        public const string EmailAlreadyExists = "Email is already registered";
        public const string RideAlreadyRated = "This ride has already been rated";
        public const string OtpExpired = "OTP has expired";
        public const string InvalidOtp = "Invalid OTP";
        public const string RideCancelled = "Ride cancelled successfully";
        public const string RideCompleted = "Ride completed successfully";
        public const string LoginSuccess = "Login successful";
        public const string RegisterSuccess = "Registration successful";
        public const string LogoutSuccess = "Logged out successfully";
        public const string ProfileUpdated = "Profile updated successfully";
        public const string AvailabilityUpdated = "Availability updated successfully";
        public const string RatingSubmitted = "Rating submitted successfully";
        public const string LocationUpdated = "Location updated successfully";
    }

    public static class SignalRConstants
    {
        public const string RideHubUrl = "/hubs/ride";
        public const string LocationHubUrl = "/hubs/location";

        public const string NewRideRequest = "NewRideRequest";
        public const string DriverAssigned = "DriverAssigned";
        public const string OtpGenerated = "OtpGenerated";
        public const string RideStatusChanged = "RideStatusChanged";
        public const string RideCancelled = "RideCancelled";
        public const string DriverLocationUpdated = "DriverLocationUpdated";

        public static string RideGroup(int rideId) => $"ride-{rideId}";
    }
}
