using Almakaber.BLL.DTOs.Account;
using Almakaber.BLL.Helpers.Email;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Almakaber.BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,IEmailSender emailSender = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "البريد الإلكتروني مسجل بالفعل." };

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                OtpCode = GenerateRandomOtp(),
                OtpExpiryTime = DateTime.UtcNow.AddMinutes(5) 
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { IsAuthenticated = false, Message = errors };
            }

            var emailBody = _emailSender.GetOtpTemplate(user.FullName, user.OtpCode, "تأكيد الحساب");
            await _emailSender.SendEmailAsync(user.Email, "تأكيد البريد الإلكتروني - منصة المقابر", emailBody);

            return new AuthResponseDto
            {
                IsAuthenticated = false,
                Message = "تم إنشاء الحساب بنجاح. برجاء مراجعة بريدك الإلكتروني لإدخال كود التحقق (OTP)."
            };
        }

        public async Task<AuthResponseDto> VerifyEmailOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return new AuthResponseDto { IsAuthenticated = false, Message = "المستخدم غير موجود." };

            if (user.OtpCode != dto.OtpCode || user.OtpExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { IsAuthenticated = false, Message = "الكود غير صحيح أو منتهي الصلاحية." };

            user.EmailConfirmed = true;
            user.OtpCode = null;
            user.OtpExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user),
                IsAuthenticated = true,
                Message = "تم تأكيد الحساب بنجاح."
            };
        }

        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return new AuthResponseDto { IsAuthenticated = false, Message = "إذا كان البريد مسجلاً، سيتم إرسال كود التحقق." };

            user.OtpCode = GenerateRandomOtp();
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            var emailBody = _emailSender.GetOtpTemplate(user.FullName, user.OtpCode, "إعادة تعيين كلمة المرور");
            await _emailSender.SendEmailAsync(user.Email, "إعادة تعيين كلمة المرور", emailBody);

            return new AuthResponseDto { IsAuthenticated = false, Message = "تم إرسال كود التحقق إلى بريدك الإلكتروني." };
        }

        public async Task<AuthResponseDto> ResetPasswordWithOtpAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || user.OtpCode != dto.OtpCode || user.OtpExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { IsAuthenticated = false, Message = "الكود غير صحيح أو منتهي الصلاحية." };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded) return new AuthResponseDto { IsAuthenticated = false, Message = "حدث خطأ أثناء تغيير كلمة المرور." };

            user.OtpCode = null;
            user.OtpExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto { IsAuthenticated = true, Message = "تم تغيير كلمة المرور بنجاح. يمكنك تسجيل الدخول الآن." };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return new AuthResponseDto { IsAuthenticated = false, Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة." };

            var jwtToken = await GenerateJwtTokenAsync(user, dto.RememberMe);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            
            user.RefreshTokenExpiryTime = dto.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(1);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = jwtToken,
                RefreshToken = refreshToken, 
                IsAuthenticated = true,
                Message = "تم تسجيل الدخول بنجاح."
            };
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, bool rememberMe = false)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName", user.FullName)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expiryDuration = rememberMe
                ? DateTime.UtcNow.AddDays(double.Parse(jwtSettings["DurationInDays"]))
                : DateTime.UtcNow.AddHours(12);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryDuration,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == dto.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponseDto { IsAuthenticated = false, Message = "انتهت صلاحية الجلسة، برجاء تسجيل الدخول مجدداً." };
            }

            var newJwtToken = await GenerateJwtTokenAsync(user, true);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                IsAuthenticated = true
            };
        }
        private string GenerateRandomOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}