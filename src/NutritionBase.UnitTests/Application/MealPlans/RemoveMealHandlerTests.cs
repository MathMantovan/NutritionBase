using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.MealPlans;

public class RemoveMealHandlerTests
{
    private readonly Mock<IMealPlanRepository> _repoMock = new();
    private readonly RemoveMealHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();

    public RemoveMealHandlerTests()
    {
        _handler = new RemoveMealHandler(_repoMock.Object);
    }

    private static MealPlan CreateValidMealPlan() =>
        MealPlan.Create(_patientId, "Plano Proteico", "Ganho de massa", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

    [Fact]
    public async Task Handle_MealPlanNotFound_ThrowsDomainException()
    {
        var mealPlanId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlanId)).ReturnsAsync((MealPlan?)null);

        var command = new RemoveMealCommand(mealPlanId, Guid.NewGuid());
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Plano alimentar*não encontrado*");
    }

    [Fact]
    public async Task Handle_MealNotFoundInPlan_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);

        var command = new RemoveMealCommand(mealPlan.Id, Guid.NewGuid());
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Refeição*não encontrada*");
    }

    [Fact]
    public async Task Handle_ValidCommand_RemovesMealAndCallsUpdateAsync()
    {
        var mealPlan = CreateValidMealPlan();
        var meal = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(8));
        mealPlan.AddMeal(meal);

        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<MealPlan>())).Returns(Task.CompletedTask);

        var command = new RemoveMealCommand(mealPlan.Id, meal.Id);
        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<MealPlan>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MealPlanNotFound_NeverCallsUpdateAsync()
    {
        var mealPlanId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlanId)).ReturnsAsync((MealPlan?)null);

        var command = new RemoveMealCommand(mealPlanId, Guid.NewGuid());
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<MealPlan>()), Times.Never);
    }
}
