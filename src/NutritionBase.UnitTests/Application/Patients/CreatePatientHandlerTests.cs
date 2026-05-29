using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class CreatePatientHandlerTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly Mock<INutritionistRepository> _nutritionistRepoMock = new();
    private readonly CreatePatientHandler _handler;

    private static readonly Guid _nutritionistId = Guid.NewGuid();

    private static readonly CreatePatientCommand _validCommand = new(
        _nutritionistId,
        "João Silva",
        "joao@example.com",
        "11",
        "987654321",
        new DateTime(1990, 1, 1),
        75m,
        1.80m);

    public CreatePatientHandlerTests()
    {
        _handler = new CreatePatientHandler(_patientRepoMock.Object, _nutritionistRepoMock.Object);
    }

    [Fact]
    public async Task Handle_NutritionistNotFound_ThrowsDomainException()
    {
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync((Nutritionist?)null);

        Func<Task> act = async () => await _handler.Handle(_validCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Nutricionista*não encontrado*");
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ThrowsDomainException()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana", "ana@clinica.com", "hashvalido");
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync(nutritionist);
        _patientRepoMock.Setup(r => r.ExistsByEmailAsync("joao@example.com", _nutritionistId)).ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(_validCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*joao@example.com*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPatientResponse()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana", "ana@clinica.com", "hashvalido");
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync(nutritionist);
        _patientRepoMock.Setup(r => r.ExistsByEmailAsync("joao@example.com", _nutritionistId)).ReturnsAsync(false);
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        var response = await _handler.Handle(_validCommand, CancellationToken.None);

        response.Should().NotBeNull();
        response.NutritionistId.Should().Be(_nutritionistId);
        response.Name.Should().Be("João Silva");
        response.Email.Should().Be("joao@example.com");
        response.Weight.Should().Be(75m);
        response.Height.Should().Be(1.80m);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsync()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana", "ana@clinica.com", "hashvalido");
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync(nutritionist);
        _patientRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);
        _patientRepoMock.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        await _handler.Handle(_validCommand, CancellationToken.None);

        _patientRepoMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WeightZero_ThrowsDomainException()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana", "ana@clinica.com", "hashvalido");
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync(nutritionist);
        _patientRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var command = _validCommand with { Weight = 0m };
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Peso*maior que zero*");
    }

    [Fact]
    public async Task Handle_HeightZero_ThrowsDomainException()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana", "ana@clinica.com", "hashvalido");
        _nutritionistRepoMock.Setup(r => r.GetByIdAsync(_nutritionistId)).ReturnsAsync(nutritionist);
        _patientRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var command = _validCommand with { Height = 0m };
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Altura*maior que zero*");
    }
}
