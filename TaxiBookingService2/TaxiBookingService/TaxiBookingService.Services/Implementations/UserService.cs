using System;
using System.Collections.Generic;
using System.Text;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Repositories.Interfaces;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<UserDto>> GetProfileAsync(int userId)
        {
            User? user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return ApiResponseDto<UserDto>.FailureResponse("User not found", 404);

            UserDto dto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                AverageRating = user.AverageRating,
                IsAvailable = user.IsAvailable
            };

            return ApiResponseDto<UserDto>.SuccessResponse(dto, "Profile fetched");
        }

        public async Task<ApiResponseDto<UserDto>> UpdateProfileAsync(
            int userId, UpdateProfileRequestDto dto)
        {
            User? user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return ApiResponseDto<UserDto>.FailureResponse("User not found", 404);

            user.FullName = dto.FullName.Trim();
            user.PhoneNumber = dto.PhoneNumber.Trim();

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            UserDto result = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                AverageRating = user.AverageRating,
                IsAvailable = user.IsAvailable
            };

            return ApiResponseDto<UserDto>.SuccessResponse(result, "Profile updated");
        }
    }
}
