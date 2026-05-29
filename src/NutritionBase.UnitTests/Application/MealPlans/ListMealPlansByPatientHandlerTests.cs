using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.MealPlans;

public class ListMealPlansByPatientHandlerTests
{
    private readonly Mock<IMealPlanRepository> _repoMock = new();
    private readonly ListMealPlansByPatientHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();

    public ListMealPlansByPatientHandlerTests()
    {
        _handler = new ListMealPlansByPatientHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_NoMealPlans_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllByPatientIdAsync(_patientId))
            .ReturnsAsync(new List<MealPlan>().AsReadOnly());

        var query = new ListMealPlansByPatientQuery(_patientId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MultipleMealPlans_ReturnsMappedList()
    {
        var mealPlan1 = MealPlan.Create(_patientId, "Plano A", "Emagrecimento",
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        var mealPlan2 = MealPlan.Create(_patientId, "Plano B", "Ganho de massa",
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(60));
        var plans = new List<MealPlan> { mealPlan1, mealPlan2 }.AsReadOnly();

        _repoMock.Setup(r => r.GetAllByPatientIdAsync(_patientId)).ReturnsAsync(plans);

        var query = new ListMealPlansByPatientQuery(_patientId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Should().HaveCount(2);
        response.Should().Contain(p => p.Name == "Plano A");
        response.Should().Contain(p => p.Name == "Plano B");
    }

    [Fact]
    public async Task Handle_MealPlanWithMeals_MapsCorrectly()
    {
        var mealPlan = MealPlan.Create(_patientId, "Plano A", "Emagrecimento",
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        var meal = Meal.Create(mealPlan.Id, "Almoço", TimeSpan.FromHours(12));
        mealPlan.AddMeal(meal);

        _repoMock.Setup(r => r.GetAllByPatientIdAsync(_patientId))
            .ReturnsAsync(new List<MealPlan> { mealPlan }.AsReadOnly());

        var query = new ListMealPlansByPatientQuery(_patientId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Single().Meals.Should().HaveCount(1);
        response.Single().Meals[0].Name.Should().Be("Almoço");
    }
}
