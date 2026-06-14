using Almakaber.BLL.DTOs.Graves;
using FluentValidation;

namespace Almakaber.BLL.Validators
{
    public class CreateGraveDtoValidator : AbstractValidator<CreateGraveDto>
    {
        public CreateGraveDtoValidator()
        {
            RuleFor(x => x.StreetNumber)
                .GreaterThan(0).WithMessage("رقم الشارع مطلوب ويجب أن يكون أكبر من الصفر.");

            RuleFor(x => x.GraveNumber)
                .NotEmpty().WithMessage("رقم المقبرة مطلوب.");

            RuleFor(x => x.GenderType)
                .NotEmpty().WithMessage("نوع المقبرة غير صالح. (اكتب رجال أو نساء ).");
        }
    }
}