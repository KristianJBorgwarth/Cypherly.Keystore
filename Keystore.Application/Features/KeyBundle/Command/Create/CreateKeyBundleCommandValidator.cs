using FluentValidation;

namespace Keystore.Application.Features.KeyBundle.Command.Create;

public sealed class CreateKeyBundleCommandValidator : AbstractValidator<CreateKeyBundleCommand>
{
    public CreateKeyBundleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID cannot be empty.");

        RuleFor(x => x.AccessKey)
            .NotEmpty().WithMessage("Access key cannot be empty.");

        RuleFor(x => x.IdentityKey)
            .NotNull().WithMessage("Identity key must not be null.")
            .Must(x => x.Length > 0).WithMessage("Identity key must not be empty.");

        RuleFor(x => x.RegistrationId)
            .Must(x => x > 0).WithMessage("Registration ID must be greater than 0.");

        RuleFor(x => x.SignedPrekeyId)
            .GreaterThan(0).WithMessage("Signed Prekey ID must be greater than 0.");

        RuleFor(x => x.SignedPreKeyPublic)
            .NotNull().WithMessage("Signed PreKey Public must not be null.")
            .Must(x => x.Length > 0).WithMessage("Signed PreKey Public must not be empty.");

        RuleFor(x => x.SignedPreKeySignature)
            .NotNull().WithMessage("Signed PreKey Signature must not be null.")
            .Must(x => x.Length > 0).WithMessage("Signed PreKey Signature must not be empty.");

        RuleFor(x => x.SignedPreKeyTimestamp)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("SignedPreKeyTimestamp must be less than or equal to the current time plus 5 minutes. This is to prevent replay attacks.");
    }
}