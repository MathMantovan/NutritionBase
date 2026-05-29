using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Nutritionists;
using NutritionBase.Application.Interfaces;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;
using SecureIdentity.Password;

namespace NutritionBase.UnitTests.Application.Nutritionists;

public class LoginNutritionistHandlerTests
{
    private readonly Mock<INutritionistRepository> _repoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly LoginNutritionistHandler _handler;

    private const string _plainPassword = "SenhaSegura@123";
    private static readonly string _passwordHash = PasswordHasher.Hash(_plainPassword);

    public LoginNutritionistHandlerTests()
    {
        _handler = new LoginNutritionistHandler(_repoMock.Object, _tokenServiceMock.Object);
    }

    private static Nutritionist CreateNutritionistWithHash(string hash) =>
        Nutritionist.Create("Dr. Ana Lima", "ana@clinica.com", hash);

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsDomainException()
    {
        _repoMock.Setup(r => r.GetByEmailAsync("ana@clinica.com")).ReturnsAsync((Nutritionist?)null);

        var command = new LoginNutritionistCommand("ana@clinica.com", _plainPassword);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Credenciais*inválidas*");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsDomainException()
    {
        var nutritionist = CreateNutritionistWithHash(_passwordHash);
        _repoMock.Setup(r => r.GetByEmailAsync("ana@clinica.com")).ReturnsAsync(nutritionist);

        var command = new LoginNutritionistCommand("ana@clinica.com", "SenhaErrada@123");
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Credenciais*inválidas*");
    }

    [Fact]
    public async Task Handle_CorrectCredentials_ReturnsAuthResponseWithToken()
    {
        var nutritionist = CreateNutritionistWithHash(_passwordHash);
        _repoMock.Setup(r => r.GetByEmailAsync("ana@clinica.com")).ReturnsAsync(nutritionist);
        _tokenServiceMock.Setup(t => t.GenerateToken(nutritionist.Id)).Returns("jwt.token.here");

        var command = new LoginNutritionistCommand("ana@clinica.com", _plainPassword);
        var response = await _handler.Handle(command, CancellationToken.None);

        response.Token.Should().Be("jwt.token.here");
        response.NutritionistId.Should().Be(nutritionist.Id);
    }

    [Fact]
    public async Task Handle_CorrectCredentials_CallsGenerateToken()
    {
        var nutritionist = CreateNutritionistWithHash(_passwordHash);
        _repoMock.Setup(r => r.GetByEmailAsync("ana@clinica.com")).ReturnsAsync(nutritionist);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>())).Returns("jwt.token.here");

        var command = new LoginNutritionistCommand("ana@clinica.com", _plainPassword);
        await _handler.Handle(command, CancellationToken.None);

        _tokenServiceMock.Verify(t => t.GenerateToken(nutritionist.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailNotFound_NeverCallsGenerateToken()
    {
        _repoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Nutritionist?)null);

        var command = new LoginNutritionistCommand("ana@clinica.com", _plainPassword);
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<Guid>()), Times.Never);
    }
}
