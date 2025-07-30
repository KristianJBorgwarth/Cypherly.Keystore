using FluentValidation;

namespace Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;

public sealed class UploadOneTimePreKeysCommandValidator : AbstractValidator<UploadOneTimePreKeysCommand>
{
    private const int MaxKeyId = 0xFFFFFF; // 16 777 215 (24‑bit field)
    private const int Curve25519Length = 32; // bytes

    public UploadOneTimePreKeysCommandValidator()
    {
        RuleFor(c => c.PreKeys)
            .NotNull().WithMessage("PreKeys collection is missing.")
            .NotEmpty().WithMessage("At least one pre‑key is required.")
            .Must(pks => pks.Select(pk => pk.KeyId).Distinct().Count() == pks.Count)
            .WithMessage("KeyId values must be unique within the batch.");

        RuleForEach(c => c.PreKeys).ChildRules(pk =>
        {
            pk.RuleFor(p => p.KeyId)
                .GreaterThan(0).WithMessage("KeyId must be greater than zero.")
                .LessThanOrEqualTo(MaxKeyId)
                .WithMessage($"KeyId must be ≤ {MaxKeyId} (fits the 24‑bit protocol field).");

            pk.RuleFor(p => p.PublicKey)
                .NotNull().WithMessage("PublicKey is required.")
                .Must(k => k.Length == Curve25519Length)
                .WithMessage($"PublicKey must be exactly {Curve25519Length} bytes.");
        });
    }
}