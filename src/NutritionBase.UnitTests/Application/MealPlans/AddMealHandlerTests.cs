using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Enums;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.MealPlans;

public class AddMealHandlerTests
{
    private readonly Mock<IMealPlanRepository> _repoMock = new();
    private readonly AddMealHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();

    public AddMealHandlerTests()
    {
        _handler = new AddMealHandler(_repoMock.Object);
    }

    private static MealPlan CreateValidMealPlan() =>
        MealPlan.Create(_patientId, "Plano Proteico", "Ganho de massa", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

    private static AddMealCommand BuildCommand(Guid mealPlanId) => new(
        mealPlanId,
        "Café da Manhã",
        TimeSpan.FromHours(8),
        new List<FoodItemInput>
        {
            new("Ovos", 200m, MeasurementUnit.Grams, 280m),
            new("Pão Integral", 50m, MeasurementUnit.Grams, 120m)
        }.AsReadOnly());

    [Fact]
    public async Task Handle_MealPlanNotFound_ThrowsDomainException()
    {
        var mealPlanId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlanId)).ReturnsAsync((MealPlan?)null);

        var command = BuildCommand(mealPlanId);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Plano alimentar*não encontrado*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsMealPlanResponseWithMeal()
    {
        var mealPlan = CreateValidMealPlan();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<MealPlan>())).Returns(Task.CompletedTask);

        var command = BuildCommand(mealPlan.Id);
        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.Meals.Should().HaveCount(1);
        response.Meals[0].Name.Should().Be("Café da Manhã");
        response.Meals[0].FoodItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var mealPlan = CreateValidMealPlan();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<MealPlan>())).Returns(Task.CompletedTask);

        await _handler.Handle(BuildCommand(mealPlan.Id), CancellationToken.None);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<MealPlan>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateMealName_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();
        var existingMeal = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(8));
        mealPlan.AddMeal(existingMeal);

        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);

        var command = BuildCommand(mealPlan.Id);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Café da Manhã*");
    }
}
