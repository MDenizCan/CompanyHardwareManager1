using CHM.MODELS.Auth;
using FluentValidation;

namespace CHM.API.Validators;

// Şifre değiştirme isteği sırasında eski ve yeni şifrelerin kurallara uygunluğunu kontrol eden sınıf.
public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200)
            .NotEqual(x => x.CurrentPassword);
    }
}

