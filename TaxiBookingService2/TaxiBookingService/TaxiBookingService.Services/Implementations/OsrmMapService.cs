using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Services.Implementations
{
    public class OsrmMapService : IMapService
    {
        private readonly HttpClient _httpClient;

        public OsrmMapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DistanceResultDto> GetDistanceAndDurationAsync(
            double originLat, double originLng,
            double destLat, double destLng)
        {
            string url = $"http://router.project-osrm.org/route/v1/driving/" +
                         $"{originLng},{originLat};{destLng},{destLat}" +
                         $"?overview=false&steps=false";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return GetFallbackResult(originLat, originLng, destLat, destLng);

            string content = await response.Content.ReadAsStringAsync();
            JsonDocument json = JsonDocument.Parse(content);

            JsonElement routes = json.RootElement.GetProperty("routes");

            if (routes.GetArrayLength() == 0)
                return GetFallbackResult(originLat, originLng, destLat, destLng);

            double distanceMeters = routes[0].GetProperty("distance").GetDouble();
            double durationSeconds = routes[0].GetProperty("duration").GetDouble();

            return new DistanceResultDto
            {
                DistanceKm = Math.Round(distanceMeters / 1000.0, 2),
                DurationMinutes = (int)Math.Ceiling(durationSeconds / 60.0)
            };
        }

        private DistanceResultDto GetFallbackResult(
            double originLat, double originLng,
            double destLat, double destLng)
        {
            double dLat = ToRadians(destLat - originLat);
            double dLon = ToRadians(destLng - originLng);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(originLat)) * Math.Cos(ToRadians(destLat)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distanceKm = Math.Round(6371.0 * c * 1.3, 2);

            return new DistanceResultDto
            {
                DistanceKm = distanceKm,
                DurationMinutes = (int)Math.Ceiling(distanceKm / 30.0 * 60)
            };
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
