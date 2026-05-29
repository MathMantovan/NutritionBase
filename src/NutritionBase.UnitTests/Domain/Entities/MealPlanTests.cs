using FluentAssertions;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.UnitTests.Domain.Entities;

public class MealPlanTests
{
    private static readonly Guid _patientId = Guid.NewGuid();
    private static readonly DateTime _start = DateTime.UtcNow.AddDays(1);
    private static readonly DateTime _end = DateTime.UtcNow.AddDays(30);

    private static MealPlan CreateValidMealPlan() =>
        MealPlan.Create(_patientId, "Plano Proteico", "Ganho de massa muscular", _start, _end);

    [Fact]
    public void Create_ValidData_ReturnsMealPlanWithCorrectProperties()
    {
        var mealPlan = CreateValidMealPlan();

        mealPlan.Id.Should().NotBeEmpty();
        mealPlan.PatientId.Should().Be(_patientId);
        mealPlan.Name.Should().Be("Plano Proteico");
        mealPlan.Objective.Should().Be("Ganho de massa muscular");
        mealPlan.StartDate.Should().Be(_start);
        mealPlan.EndDate.Should().Be(_end);
        mealPlan.Meals.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_ThrowsDomainException(string name)
    {
        Action act = () => MealPlan.Create(_patientId, name, "Objetivo válido", _start, _end);
        act.Should().Throw<DomainException>().WithMessage("*Nome*plano alimentar*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyObjective_ThrowsDomainException(string objective)
    {
        Action act = () => MealPlan.Create(_patientId, "Plano A", objective, _start, _end);
        act.Should().Throw<DomainException>().WithMessage("*Objetivo*");
    }

    [Fact]
    public void Create_EndDateEqualToStartDate_ThrowsDomainException()
    {
        var sameDate = DateTime.UtcNow.AddDays(5);
        Action act = () => MealPlan.Create(_patientId, "Plano A", "Objetivo", sameDate, sameDate);
        act.Should().Throw<DomainException>().WithMessage("*data final*maior*");
    }

    [Fact]
    public void Create_EndDateBeforeStartDate_ThrowsDomainException()
    {
        var start = DateTime.UtcNow.AddDays(10);
        var end = DateTime.UtcNow.AddDays(5);
        Action act = () => MealPlan.Create(_patientId, "Plano A", "Objetivo", start, end);
        act.Should().Throw<DomainException>().WithMessage("*data final*maior*");
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var mealPlan = CreateValidMealPlan();
        var newStart = DateTime.UtcNow.AddDays(2);
        var newEnd = DateTime.UtcNow.AddDays(60);

        mealPlan.Update("Novo Plano", "Emagrecimento", newStart, newEnd);

        mealPlan.Name.Should().Be("Novo Plano");
        mealPlan.Objective.Should().Be("Emagrecimento");
        mealPlan.StartDate.Should().Be(newStart);
        mealPlan.EndDate.Should().Be(newEnd);
    }

    [Fact]
    public void Update_EndDateBeforeStartDate_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();
        var start = DateTime.UtcNow.AddDays(10);
        var end = DateTime.UtcNow.AddDays(1);

        Action act = () => mealPlan.Update("Plano A", "Objetivo", start, end);
        act.Should().Throw<DomainException>().WithMessage("*data final*maior*");
    }

    [Fact]
    public void AddMeal_ValidMeal_AddsToList()
    {
        var mealPlan = CreateValidMealPlan();
        var meal = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(8));

        mealPlan.AddMeal(meal);

        mealPlan.Meals.Should().HaveCount(1);
    }

    [Fact]
    public void AddMeal_DuplicateName_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();
        var meal1 = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(8));
        var meal2 = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(9));
        mealPlan.AddMeal(meal1);

        Action act = () => mealPlan.AddMeal(meal2);
        act.Should().Throw<DomainException>().WithMessage("*Café da Manhã*");
    }

    [Fact]
    public void AddMeal_DuplicateNameCaseInsensitive_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();
        var meal1 = Meal.Create(mealPlan.Id, "Almoço", TimeSpan.FromHours(12));
        var meal2 = Meal.Create(mealPlan.Id, "ALMOÇO", TimeSpan.FromHours(13));
        mealPlan.AddMeal(meal1);

        Action act = () => mealPlan.AddMeal(meal2);
        act.Should().Throw<DomainException>().WithMessage("*ALMOÇO*");
    }

    [Fact]
    public void RemoveMeal_ExistingMeal_RemovesFromList()
    {
        var mealPlan = CreateValidMealPlan();
        var meal = Meal.Create(mealPlan.Id, "Café da Manhã", TimeSpan.FromHours(8));
        mealPlan.AddMeal(meal);

        mealPlan.RemoveMeal(meal.Id);

        mealPlan.Meals.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMeal_NonExistingMeal_ThrowsDomainException()
    {
        var mealPlan = CreateValidMealPlan();

        Action act = () => mealPlan.RemoveMeal(Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*Refeição*não encontrada*");
    }
}
