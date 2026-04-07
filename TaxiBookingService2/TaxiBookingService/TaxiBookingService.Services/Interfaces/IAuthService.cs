using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterPassengerAsync(RegisterPassengerRequestDto dto);
        Task<ApiResponseDto<AuthResponseDto>> RegisterDriverAsync(RegisterDriverRequestDto dto);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponseDto<string>> LogoutAsync(string refreshToken);
    }
}
