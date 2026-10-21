using FluentValidation;
using Keystore.Application.Dtos;

namespace Keystore.Application.Features.KeyBundle.Validators
{
    public sealed class PreKeyDtoValidator : AbstractValidator<PreKeyDto>
    {
        public const int MaxPreKeysPerRequest = 100;
        private const int MaxKeyId = 0xFFFFFF;
        private const int PublicKeyLength = 33;
        private const byte DjbKeyType = 0x05;

        public PreKeyDtoValidator()
        {
            RuleFor(p => p.KeyId)
                .GreaterThan(0).WithMessage("KeyId must be greater than zero.")
                .LessThanOrEqualTo(MaxKeyId)
                .WithMessage($"KeyId must be ≤ {MaxKeyId} (fits the 24‑bit protocol field).");

            RuleFor(p => p.PublicKey)
                .NotNull().WithMessage("PublicKey is required.")
                .Must(k => k.Length == PublicKeyLength && k[0] == DjbKeyType)
                .WithMessage($"PublicKey must be a {PublicKeyLength}-byte serialized Curve25519 key.");
        }
    }
}
