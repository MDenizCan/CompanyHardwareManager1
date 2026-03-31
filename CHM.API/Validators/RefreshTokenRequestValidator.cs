using CHM.MODELS.Auth;
using FluentValidation;

namespace CHM.API.Validators;

// Token yenileme isteği sırasında gönderilen Refresh Token'ın geçerliliğini (boş olmamasını) kontrol eden sınıf.
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(500);
    }
}

