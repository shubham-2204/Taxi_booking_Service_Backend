using TaxiBookingService.Models.DTOs;

namespace TaxiBookingService.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponseDto<UserDto>> GetProfileAsync(int userId);
        Task<ApiResponseDto<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
    }
}
