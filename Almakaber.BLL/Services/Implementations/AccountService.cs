using Almakaber.BLL.DTOs.Account;
using Almakaber.BLL.Helpers.Email;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<(IEnumerable<UserDto> Data, int TotalCount)> GetAllUsersAsync(int pageNumber, int pageSize, string? searchName = null, string? sortField = null, int sortOrder = 1)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(u => u.FullName.Contains(searchName) || u.Email.Contains(searchName));

            if (!string.IsNullOrEmpty(sortField))
            {
                bool isAscending = sortOrder == 1;

                query = sortField switch
                {
                    "fullName" => isAscending ? query.OrderBy(u => u.FullName) : query.OrderByDescending(u => u.FullName),
                    "createdAt" => isAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
                    "isBlocked" => isAscending ? query.OrderBy(u => u.IsBlocked) : query.OrderByDescending(u => u.IsBlocked),
                    _ => query.OrderByDescending(u => u.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt); 
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userList = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsBlocked = user.IsBlocked,
                    CreatedAt = user.CreatedAt,
                    Roles = roles
                });
            }

            return (userList, totalCount);
        }

        public async Task<AuthResponseDto> ToggleBlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "المستخدم غير موجود." };

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return new AuthResponseDto { IsAuthenticated = false, Message = "لا يمكن حظر مدير نظام آخر." };

            user.IsBlocked = !user.IsBlocked;
            await _userManager.UpdateAsync(user);

            var statusMsg = user.IsBlocked ? "تم حظر المستخدم بنجاح." : "تم إلغاء حظر المستخدم بنجاح.";
            return new AuthResponseDto { IsAuthenticated = true, Message = statusMsg };
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

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new AuthResponseDto { IsAuthenticated = false, Message = "يرجى تأكيد البريد الإلكتروني أولاً." };

            if (user.IsBlocked)
                return new AuthResponseDto { IsAuthenticated = false, Message = "تم حظر هذا الحساب من قبل الإدارة." };

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

        public async Task<AuthResponseDto> ResendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResponseDto { IsAuthenticated = false, Message = "المستخدم غير موجود." };

            if (await _userManager.IsEmailConfirmedAsync(user))
                return new AuthResponseDto { IsAuthenticated = false, Message = "هذا الحساب مؤكد بالفعل." };

            var newOtp = GenerateRandomOtp();
            user.OtpCode = newOtp;

            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { IsAuthenticated = false, Message = $"حدث خطأ أثناء تحديث البيانات: {errors}" };
            }

            var emailBody = _emailSender.GetOtpTemplate(user.FullName, user.OtpCode, "تأكيد الحساب (كود جديد)");
            await _emailSender.SendEmailAsync(user.Email, "كود تفعيل جديد - منصة المقابر", emailBody);

            return new AuthResponseDto { IsAuthenticated = true, Message = "تم إرسال كود جديد بنجاح." };
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