using CHM.MODELS.Auth;
using FluentValidation;

namespace CHM.API.Validators;

// Kullanıcının Login (Giriş) yaparken gönderdiği bilgilerin basit geçerlilik kontrolünü yapan sınıf.
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(200);
    }
}

