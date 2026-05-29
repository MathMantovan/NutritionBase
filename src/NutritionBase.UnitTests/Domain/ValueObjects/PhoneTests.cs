using FluentAssertions;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.ValueObjects;

namespace NutritionBase.UnitTests.Domain.ValueObjects;

public class PhoneTests
{
    [Fact]
    public void Create_ValidPhone_ReturnsPhoneWithCorrectValues()
    {
        var phone = Phone.Create("11", "987654321");
        phone.AreaCode.Should().Be("11");
        phone.Number.Should().Be("987654321");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespaceAreaCode_ThrowsDomainException(string areaCode)
    {
        Action act = () => Phone.Create(areaCode, "987654321");
        act.Should().Throw<DomainException>().WithMessage("*código de área*vazio*");
    }

    [Fact]
    public void Create_NullAreaCode_ThrowsDomainException()
    {
        Action act = () => Phone.Create(null!, "987654321");
        act.Should().Throw<DomainException>().WithMessage("*código de área*vazio*");
    }

    [Theory]
    [InlineData("1")]     // 1 digit — too short
    [InlineData("111")]   // 3 digits — too long
    [InlineData("AB")]    // letters
    [InlineData("1A")]    // mixed
    public void Create_InvalidAreaCodeFormat_ThrowsDomainException(string areaCode)
    {
        Action act = () => Phone.Create(areaCode, "987654321");
        act.Should().Throw<DomainException>().WithMessage("*código de área*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespaceNumber_ThrowsDomainException(string number)
    {
        Action act = () => Phone.Create("11", number);
        act.Should().Throw<DomainException>().WithMessage("*número de telefone*vazio*");
    }

    [Fact]
    public void Create_NullNumber_ThrowsDomainException()
    {
        Action act = () => Phone.Create("11", null!);
        act.Should().Throw<DomainException>().WithMessage("*número de telefone*vazio*");
    }

    [Theory]
    [InlineData("1234567")]    // 7 digits — too short
    [InlineData("1234567890")] // 10 digits — too long
    [InlineData("ABCDEFGHI")]  // letters, 9 chars
    [InlineData("1234ABCD")]   // mixed digits and letters
    public void Create_InvalidNumberFormat_ThrowsDomainException(string number)
    {
        Action act = () => Phone.Create("11", number);
        act.Should().Throw<DomainException>().WithMessage("*número de telefone*");
    }

    [Theory]
    [InlineData("11", "98765432")]   // 8 digits
    [InlineData("21", "987654321")]  // 9 digits
    [InlineData("85", "12345678")]   // 8 digits different DDD
    public void Create_ValidFormats_ReturnsPhone(string areaCode, string number)
    {
        var phone = Phone.Create(areaCode, number);
        phone.AreaCode.Should().Be(areaCode);
        phone.Number.Should().Be(number);
    }
}
