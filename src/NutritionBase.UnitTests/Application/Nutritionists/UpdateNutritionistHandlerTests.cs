using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Nutritionists;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Nutritionists;

public class UpdateNutritionistHandlerTests
{
    private readonly Mock<INutritionistRepository> _repoMock = new();
    private readonly UpdateNutritionistHandler _handler;

    public UpdateNutritionistHandlerTests()
    {
        _handler = new UpdateNutritionistHandler(_repoMock.Object);
    }

    private static Nutritionist CreateValidNutritionist() =>
        Nutritionist.Create("Dr. Ana Lima", "ana@clinica.com", "hashvalido");

    [Fact]
    public async Task Handle_NutritionistNotFound_ThrowsDomainException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Nutritionist?)null);

        var command = new UpdateNutritionistCommand(id, "Novo Nome", "novo@clinica.com");
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Nutricionista*não encontrado*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedResponse()
    {
        var nutritionist = CreateValidNutritionist();
        _repoMock.Setup(r => r.GetByIdAsync(nutritionist.Id)).ReturnsAsync(nutritionist);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Nutritionist>())).Returns(Task.CompletedTask);

        var command = new UpdateNutritionistCommand(nutritionist.Id, "Dr. Carlos Souza", "carlos@clinica.com");
        var response = await _handler.Handle(command, CancellationToken.None);

        response.Name.Should().Be("Dr. Carlos Souza");
        response.Email.Should().Be("carlos@clinica.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var nutritionist = CreateValidNutritionist();
        _repoMock.Setup(r => r.GetByIdAsync(nutritionist.Id)).ReturnsAsync(nutritionist);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Nutritionist>())).Returns(Task.CompletedTask);

        var command = new UpdateNutritionistCommand(nutritionist.Id, "Dr. Carlos Souza", "carlos@clinica.com");
        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Nutritionist>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();
        _repoMock.Setup(r => r.GetByIdAsync(nutritionist.Id)).ReturnsAsync(nutritionist);

        var command = new UpdateNutritionistCommand(nutritionist.Id, "Dr. Carlos", "invalidemail");
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*inválido*");
    }
}
