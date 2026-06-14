using Almakaber.BLL.DTOs.Account;
using Almakaber.BLL.Services.Interfaces;
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

            // لو مسجش بنجاح ومفيش رسالة نجاح الـ OTP، بنرجع 400 Bad Request
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
                return Unauthorized(new { message = result.Message }); // 401 Unauthorized

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _accountService.ForgotPasswordAsync(dto);

            // في حالات نسيان الباسورد، دايماً بنرجع 200 OK لأسباب أمنية 
            // عشان الهاكرز ميعرفوش هل الإيميل ده متسجل في الداتا بيز فعلاً ولا لأ
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