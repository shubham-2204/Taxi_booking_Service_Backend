namespace TaxiBookingService.Models.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponseDto<T> SuccessResponse(T data, string message, int statusCode = 200)
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ApiResponseDto<T> FailureResponse(string message, int statusCode, List<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }
    }
}