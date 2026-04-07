using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Models.Enums;
using TaxiBookingService.Models.Models;
using TaxiBookingService.Services.Interfaces;
using TaxiBookingService.Repositories.Interfaces;

namespace TaxiBookingService.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterPassengerAsync(
            RegisterPassengerRequestDto dto)
        {
            bool emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);

            if (emailExists)
                return ApiResponseDto<AuthResponseDto>.FailureResponse(
                    "Email is already registered", 409);

            User user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower().Trim(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Passenger,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await GenerateAuthResponseAsync(user, "Registration successful", 201);
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterDriverAsync(
            RegisterDriverRequestDto dto)
        {
            bool emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);

            if (emailExists)
                return ApiResponseDto<AuthResponseDto>.FailureResponse(
                    "Email is already registered", 409);

            User user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower().Trim(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Driver,
                IsAvailable = false,
                CreatedAt = DateTime.UtcNow,
                Vehicle = new Vehicle
                {
                    PlateNumber = dto.PlateNumber.ToUpper().Trim(),
                    Model = dto.Model,
                    Color = dto.Color,
                    CabType = (CabType)dto.CabType
                }
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return await GenerateAuthResponseAsync(user, "Driver registration successful", 201);
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            User? user = await _unitOfWork.Users.GetByEmailAsync(dto.Email.ToLower().Trim());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ApiResponseDto<AuthResponseDto>.FailureResponse(
                    "Invalid email or password", 401);

            return await GenerateAuthResponseAsync(user, "Login successful", 200);
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            RefreshToken? storedToken = await _unitOfWork.Users.GetRefreshTokenAsync(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                return ApiResponseDto<AuthResponseDto>.FailureResponse(
                    "Invalid or expired refresh token", 401);

            storedToken.IsRevoked = true;
            

            return await GenerateAuthResponseAsync(storedToken.User, "Token refreshed", 200);
        }

        public async Task<ApiResponseDto<string>> LogoutAsync(string refreshToken)
        {
            RefreshToken? storedToken = await _unitOfWork.Users.GetRefreshTokenAsync(refreshToken);

            if (storedToken == null)
                return ApiResponseDto<string>.FailureResponse("Invalid token", 400);

            storedToken.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<string>.SuccessResponse("Logged out", "Logout successful");
        }

        private async Task<ApiResponseDto<AuthResponseDto>> GenerateAuthResponseAsync(
            User user, string message, int statusCode)
        {
            string accessToken = _jwtService.GenerateAccessToken(user);
            string refreshTokenValue = _jwtService.GenerateRefreshToken();

            RefreshToken refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddRefreshTokenAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            AuthResponseDto response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString(),
                    AverageRating = user.AverageRating,
                    IsAvailable = user.IsAvailable
                }
            };

            return ApiResponseDto<AuthResponseDto>.SuccessResponse(response, message, statusCode);
        }
    }
}
