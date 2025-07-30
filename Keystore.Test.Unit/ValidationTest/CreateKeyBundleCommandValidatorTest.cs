using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using Keystore.Application.Features.KeyBundle.Commands.Create;

namespace Keystore.Test.Unit.ValidationTest;

public class CreateKeyBundleCommandValidatorTest
{
    private readonly CreateKeyBundleCommandValidator _validator = new CreateKeyBundleCommandValidator();
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        // Arrange
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.AccessKey, Guid.NewGuid)
            .With(x => x.IdentityKey, [1, 2, 3])
            .With(x => x.RegistrationId, 1)
            .With(x => x.SignedPrekeyId, 1)
            .With(x => x.SignedPreKeyPublic, [4, 5, 6])
            .With(x => x.SignedPreKeySignature, [7, 8, 9])
            .With(x => x.SignedPreKeyTimestamp, DateTimeOffset.UtcNow)
            .Create();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_UserId_Is_Empty()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.UserId, Guid.Empty)
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Fail_When_AccessKey_Is_Empty()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.AccessKey, Guid.Empty)
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AccessKey);
    }

    [Fact]
    public void Should_Fail_When_IdentityKey_Is_Empty()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.IdentityKey, Array.Empty<byte>())
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.IdentityKey);
    }

    [Fact]
    public void Should_Fail_When_RegistrationId_Is_Zero()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.RegistrationId, 0)
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RegistrationId);
    }

    [Fact]
    public void Should_Fail_When_SignedPrekeyId_Is_Not_Positive()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.SignedPrekeyId, 0)
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignedPrekeyId);
    }

    [Fact]
    public void Should_Fail_When_SignedPreKeyPublic_Is_Empty()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.SignedPreKeyPublic, [])
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignedPreKeyPublic);
    }


    [Fact]
    public void Should_Fail_When_SignedPreKeyTimestamp_Is_In_The_Future()
    {
        var command = _fixture.Build<CreateKeyBundleCommand>()
            .With(x => x.SignedPreKeyTimestamp, DateTimeOffset.UtcNow.AddMinutes(6))
            .Create();

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SignedPreKeyTimestamp);
    }
}
