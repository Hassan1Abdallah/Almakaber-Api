using Almakaber.BLL.DTOs.Account;
using FluentValidation;

namespace Almakaber.BLL.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("الاسم بالكامل مطلوب.")
                .MinimumLength(3).WithMessage("الاسم يجب أن يكون 3 أحرف على الأقل.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
                .MinimumLength(6).WithMessage("كلمة المرور يجب أن تكون 6 أحرف على الأقل.")
                .Matches("[A-Z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل.")
                .Matches("[a-z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل.")
                .Matches("[0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.")
                .Matches("[^a-zA-Z0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل.");
        }
    }
}