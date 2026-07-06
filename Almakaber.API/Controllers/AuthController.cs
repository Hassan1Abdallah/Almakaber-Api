using Almakaber.BLL.DTOs.Account;
using Almakaber.BLL.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _accountService.RegisterAsync(dto);

            if (!result.IsAuthenticated && !result.Message.Contains("بنجاح"))
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _accountService.VerifyEmailOtpAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            
            var result = await _accountService.LoginAsync(dto);

            if (!result.IsAuthenticated)
                return Unauthorized(new { message = result.Message }); 

            return Ok(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            var result = await _accountService.ResendOtpAsync(dto.Email);
            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _accountService.ForgotPasswordAsync(dto);

            return Ok(new { message = result.Message });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _accountService.ResetPasswordWithOtpAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _accountService.RefreshTokenAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }
    }
}