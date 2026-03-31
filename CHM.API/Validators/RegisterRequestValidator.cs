using CHM.MODELS.Auth;
using FluentValidation;

namespace CHM.API.Validators;

// Sisteme yeni kayıt olan kullanıcının seçtiği E-posta, Şifre ve Kullanıcı Adı kurallarını kontrol eden sınıf.
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);
    }
}

