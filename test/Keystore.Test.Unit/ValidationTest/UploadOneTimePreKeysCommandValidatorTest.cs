using FluentAssertions;
using FluentValidation.TestHelper;
using Keystore.Application.Dtos;
using Keystore.Application.Features.KeyBundle.Commands.UploadOTPs;
using Keystore.Application.Features.KeyBundle.Validators;

namespace Keystore.Test.Unit.ValidationTest;

public class UploadOneTimePreKeysCommandValidatorTest
{
    private readonly UploadOneTimePreKeysCommandValidator _validator = new();

    private static PreKeyDto PreKey(int keyId, byte[]? publicKey = null) =>
        new() { KeyId = keyId, PublicKey = publicKey ?? [0x05, .. Enumerable.Repeat((byte)keyId, 32)] };

    private static UploadOneTimePreKeysCommand Command(params PreKeyDto[] preKeys) => new() { Id = Guid.NewGuid(), PreKeys = preKeys };

    private static UploadOneTimePreKeysCommand Command(int count) =>
        Command([.. Enumerable.Range(1, count).Select(i => PreKey(i))]);

    [Theory]
    [InlineData(1)]
    [InlineData(PreKeyDtoValidator.MaxPreKeysPerRequest)]
    public void Should_Pass_With_Up_To_The_Maximum_PreKeys(int count)
    {
        var result = _validator.TestValidate(Command(count));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Empty()
    {
        var result = _validator.TestValidate(Command(0));

        result.ShouldHaveValidationErrorFor(x => x.PreKeys);
    }

    [Fact]
    public void Should_Fail_When_Too_Many_PreKeys()
    {
        var result = _validator.TestValidate(Command(PreKeyDtoValidator.MaxPreKeysPerRequest + 1));

        result.ShouldHaveValidationErrorFor(x => x.PreKeys);
    }

    [Fact]
    public void Should_Fail_When_PreKey_Ids_Repeat()
    {
        var result = _validator.TestValidate(Command(PreKey(7), PreKey(7)));

        result.ShouldHaveValidationErrorFor(x => x.PreKeys);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0x1000000)]
    public void Should_Fail_When_KeyId_Is_Out_Of_Range(int keyId)
    {
        var result = _validator.TestValidate(Command(PreKey(keyId)));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_Public_Key_Is_Raw_32_Bytes()
    {
        var result = _validator.TestValidate(Command(PreKey(1, new byte[32])));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_Public_Key_Has_Wrong_Type_Byte()
    {
        var result = _validator.TestValidate(Command(PreKey(1, [0x04, .. new byte[32]])));

        result.IsValid.Should().BeFalse();
    }
}
