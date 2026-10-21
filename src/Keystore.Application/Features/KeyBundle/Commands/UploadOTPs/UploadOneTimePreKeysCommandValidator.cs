using FluentValidation;
using Keystore.Application.Dtos;

namespace Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;

public sealed class UploadOneTimePreKeysCommandValidator : AbstractValidator<UploadOneTimePreKeysCommand>
{
    public UploadOneTimePreKeysCommandValidator()
    {
        RuleFor(c => c.PreKeys)
            .NotNull().WithMessage("PreKeys collection is missing.")
            .NotEmpty().WithMessage("At least one pre‑key is required.")
            .Must(pks => pks.Count <= PreKeyDtoValidator.MaxPreKeysPerRequest)
            .WithMessage($"At most {PreKeyDtoValidator.MaxPreKeysPerRequest} pre-keys can be uploaded per request.")
            .Must(pks => pks.Select(pk => pk.KeyId).Distinct().Count() == pks.Count)
            .WithMessage("KeyId values must be unique within the batch.");

        RuleForEach(c => c.PreKeys).SetValidator(new PreKeyDtoValidator());
    }
}
