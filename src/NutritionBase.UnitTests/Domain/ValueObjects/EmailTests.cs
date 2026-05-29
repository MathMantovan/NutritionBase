using FluentAssertions;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.ValueObjects;

namespace NutritionBase.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ValidEmail_ReturnsEmailWithCorrectValue()
    {
        var email = Email.Create("user@example.com");
        email.Value.Should().Be("user@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyOrWhitespace_ThrowsDomainException(string value)
    {
        Action act = () => Email.Create(value);
        act.Should().Throw<DomainException>().WithMessage("*vazio*");
    }

    [Fact]
    public void Create_Null_ThrowsDomainException()
    {
        Action act = () => Email.Create(null!);
        act.Should().Throw<DomainException>().WithMessage("*vazio*");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@tld")]
    [InlineData("@nodomain.com")]
    public void Create_InvalidFormat_ThrowsDomainException(string value)
    {
        Action act = () => Email.Create(value);
        act.Should().Throw<DomainException>().WithMessage("*inválido*");
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("user.name@sub.domain.org")]
    [InlineData("USER@DOMAIN.COM")]
    public void Create_ValidFormats_ReturnsEmail(string value)
    {
        var email = Email.Create(value);
        email.Value.Should().Be(value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var email = Email.Create("test@example.com");
        email.ToString().Should().Be("test@example.com");
    }
}
