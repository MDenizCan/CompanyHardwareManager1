using CHM.MODELS.Asset;
using FluentValidation;

namespace CHM.API.Validators;

// API'ye gelen CreateAssetRequest (Yeni Cihaz Kaydı) verilerinin doğruluğunu (Boş olmaması, uzunluğu vb.) kontrol eden sınıf.
// İlgili Controller metoduna girmeden önce çalışır ve kurallara uymayan istekleri 400 Bad Request olarak reddeder.
public sealed class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SerialNumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
