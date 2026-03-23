using CHM.ENTITIES.Entities;
using CHM.MODELS.Asset;
using FluentValidation;

namespace CHM.API.Validators;

public sealed class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
{
    public UpdateAssetRequestValidator()
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

        RuleFor(x => x.Status)
            .Must(s => Enum.IsDefined(typeof(AssetStatus), s))
            .WithMessage("Invalid status value. Valid values: 0=Available, 1=InUse, 2=Faulty, 3=UnderMaintenance, 4=Retired");
    }
}
