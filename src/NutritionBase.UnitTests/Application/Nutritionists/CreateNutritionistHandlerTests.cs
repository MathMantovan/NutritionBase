using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Nutritionists;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Nutritionists;

public class CreateNutritionistHandlerTests
{
    private readonly Mock<INutritionistRepository> _repoMock = new();
    private readonly CreateNutritionistHandler _handler;

    private static readonly CreateNutritionistCommand _validCommand =
        new("Dr. Ana Lima", "ana@clinica.com", "SenhaSegura@123");

    public CreateNutritionistHandlerTests()
    {
        _handler = new CreateNutritionistHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ThrowsDomainException()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync("ana@clinica.com")).ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(_validCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*ana@clinica.com*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNutritionistResponse()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync("ana@clinica.com")).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Nutritionist>())).Returns(Task.CompletedTask);

        var response = await _handler.Handle(_validCommand, CancellationToken.None);

        response.Should().NotBeNull();
        response.Name.Should().Be("Dr. Ana Lima");
        response.Email.Should().Be("ana@clinica.com");
        response.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsync()
    {
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Nutritionist>())).Returns(Task.CompletedTask);

        await _handler.Handle(_validCommand, CancellationToken.None);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Nutritionist>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_StoresHashedPassword_NotPlainText()
    {
        Nutritionist? captured = null;
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Nutritionist>()))
            .Callback<Nutritionist>(n => captured = n)
            .Returns(Task.CompletedTask);

        await _handler.Handle(_validCommand, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.PasswordHash.Should().NotBe("SenhaSegura@123");
        captured.PasswordHash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ThrowsDomainException()
    {
        var commandWithInvalidEmail = _validCommand with { Email = "invalidemail" };
        _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(commandWithInvalidEmail, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>();
    }
}
