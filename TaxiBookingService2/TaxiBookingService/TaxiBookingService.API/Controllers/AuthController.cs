using Microsoft.AspNetCore.Mvc;
using TaxiBookingService.Models.DTOs;
using TaxiBookingService.Services.Interfaces;

namespace TaxiBookingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/passenger")]
        public async Task<IActionResult> RegisterPassenger(
            [FromBody] RegisterPassengerRequestDto dto)
        {
            ApiResponseDto<AuthResponseDto> result =
                await _authService.RegisterPassengerAsync(dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("register/driver")]
        public async Task<IActionResult> RegisterDriver(
            [FromBody] RegisterDriverRequestDto dto) 
        {
            ApiResponseDto<AuthResponseDto> result =
                await _authService.RegisterDriverAsync(dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            ApiResponseDto<AuthResponseDto> result =
                await _authService.LoginAsync(dto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            ApiResponseDto<AuthResponseDto> result =
                await _authService.RefreshTokenAsync(refreshToken);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            ApiResponseDto<string> result =
                await _authService.LogoutAsync(refreshToken);

            return StatusCode(result.StatusCode, result);
        }
    }
}
