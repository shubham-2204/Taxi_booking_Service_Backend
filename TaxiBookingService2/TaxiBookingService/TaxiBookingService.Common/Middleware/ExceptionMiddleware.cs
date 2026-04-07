using System.Net;
using System.Text.Json;
using TaxiBookingService.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace TaxiBookingService.Common.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ApiResponseDto<object> response = ApiResponseDto<object>.FailureResponse(
                "An unexpected error occurred. Please try again later",
                (int)HttpStatusCode.InternalServerError,
                new List<string> { exception.Message });

            string json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
