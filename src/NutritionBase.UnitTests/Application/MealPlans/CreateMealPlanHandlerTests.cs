using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.MealPlans;

public class CreateMealPlanHandlerTests
{
    private readonly Mock<IMealPlanRepository> _mealPlanRepoMock = new();
    private readonly Mock<IPatientRepository> _patientRepoMock = new();
    private readonly CreateMealPlanHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();
    private static readonly Guid _nutritionistId = Guid.NewGuid();

    private static readonly CreateMealPlanCommand _validCommand = new(
        _patientId,
        "Plano Proteico",
        "Ganho de massa muscular",
        DateTime.UtcNow.AddDays(1),
        DateTime.UtcNow.AddDays(30));

    public CreateMealPlanHandlerTests()
    {
        _handler = new CreateMealPlanHandler(_mealPlanRepoMock.Object, _patientRepoMock.Object);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsDomainException()
    {
        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync((Patient?)null);

        Func<Task> act = async () => await _handler.Handle(_validCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsMealPlanCreateResponse()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _mealPlanRepoMock.Setup(r => r.AddAsync(It.IsAny<MealPlan>())).Returns(Task.CompletedTask);

        var response = await _handler.Handle(_validCommand, CancellationToken.None);

        response.Should().NotBeNull();
        response.PatientId.Should().Be(_patientId);
        response.Name.Should().Be("Plano Proteico");
        response.Objective.Should().Be("Ganho de massa muscular");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsync()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _mealPlanRepoMock.Setup(r => r.AddAsync(It.IsAny<MealPlan>())).Returns(Task.CompletedTask);

        await _handler.Handle(_validCommand, CancellationToken.None);

        _mealPlanRepoMock.Verify(r => r.AddAsync(It.IsAny<MealPlan>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EndDateBeforeStartDate_ThrowsDomainException()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _patientRepoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);

        var invalidCommand = _validCommand with
        {
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        Func<Task> act = async () => await _handler.Handle(invalidCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*data final*maior*");
    }
}
