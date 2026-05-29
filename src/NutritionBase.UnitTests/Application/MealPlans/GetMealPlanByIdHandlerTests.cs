using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.MealPlans;

public class GetMealPlanByIdHandlerTests
{
    private readonly Mock<IMealPlanRepository> _repoMock = new();
    private readonly GetMealPlanByIdHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();

    public GetMealPlanByIdHandlerTests()
    {
        _handler = new GetMealPlanByIdHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_MealPlanNotFound_ThrowsDomainException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(id)).ReturnsAsync((MealPlan?)null);

        var query = new GetMealPlanByIdQuery(id);
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Plano alimentar*não encontrado*");
    }

    [Fact]
    public async Task Handle_MealPlanFound_ReturnsMappedResponse()
    {
        var mealPlan = MealPlan.Create(_patientId, "Plano Proteico", "Ganho de massa",
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        _repoMock.Setup(r => r.GetByIdWithMealsAsync(mealPlan.Id)).ReturnsAsync(mealPlan);

        var query = new GetMealPlanByIdQuery(mealPlan.Id);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Id.Should().Be(mealPlan.Id);
        response.PatientId.Should().Be(_patientId);
        response.Name.Should().Be("Plano Proteico");
        response.Objective.Should().Be("Ganho de massa");
        response.Meals.Should().BeEmpty();
    }
}
