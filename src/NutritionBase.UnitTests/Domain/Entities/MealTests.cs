using FluentAssertions;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Enums;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.UnitTests.Domain.Entities;

public class MealTests
{
    private static readonly Guid _mealPlanId = Guid.NewGuid();

    private static Meal CreateValidMeal() =>
        Meal.Create(_mealPlanId, "Café da Manhã", TimeSpan.FromHours(8));

    [Fact]
    public void Create_ValidData_ReturnsMealWithCorrectProperties()
    {
        var meal = CreateValidMeal();

        meal.Id.Should().NotBeEmpty();
        meal.MealPlanId.Should().Be(_mealPlanId);
        meal.Name.Should().Be("Café da Manhã");
        meal.MealTime.Should().Be(TimeSpan.FromHours(8));
        meal.FoodItems.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_ThrowsDomainException(string name)
    {
        Action act = () => Meal.Create(_mealPlanId, name, TimeSpan.FromHours(8));
        act.Should().Throw<DomainException>().WithMessage("*Nome*refeição*");
    }

    [Fact]
    public void Create_NegativeMealTime_ThrowsDomainException()
    {
        Action act = () => Meal.Create(_mealPlanId, "Almoço", TimeSpan.FromMinutes(-1));
        act.Should().Throw<DomainException>().WithMessage("*Horário*inválido*");
    }

    [Fact]
    public void Create_MealTimeExactly24Hours_ThrowsDomainException()
    {
        Action act = () => Meal.Create(_mealPlanId, "Almoço", TimeSpan.FromHours(24));
        act.Should().Throw<DomainException>().WithMessage("*Horário*inválido*");
    }

    [Fact]
    public void Create_MealTimeOver24Hours_ThrowsDomainException()
    {
        Action act = () => Meal.Create(_mealPlanId, "Almoço", TimeSpan.FromHours(25));
        act.Should().Throw<DomainException>().WithMessage("*Horário*inválido*");
    }

    [Theory]
    [MemberData(nameof(ValidMealTimes))]
    public void Create_ValidMealTimes_Succeeds(TimeSpan mealTime)
    {
        var meal = Meal.Create(_mealPlanId, "Refeição", mealTime);
        meal.MealTime.Should().Be(mealTime);
    }

    public static IEnumerable<object[]> ValidMealTimes()
    {
        yield return [TimeSpan.Zero];
        yield return [TimeSpan.FromHours(8)];
        yield return [TimeSpan.FromHours(12).Add(TimeSpan.FromMinutes(30))];
        yield return [new TimeSpan(23, 59, 0)];
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var meal = CreateValidMeal();
        meal.Update("Almoço", TimeSpan.FromHours(12));

        meal.Name.Should().Be("Almoço");
        meal.MealTime.Should().Be(TimeSpan.FromHours(12));
    }

    [Fact]
    public void AddFoodItem_ValidItem_AddsToList()
    {
        var meal = CreateValidMeal();
        var foodItem = FoodItem.Create(meal.Id, "Arroz", 100m, MeasurementUnit.Grams, 130m);

        meal.AddFoodItem(foodItem);

        meal.FoodItems.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveFoodItem_ExistingItem_RemovesFromList()
    {
        var meal = CreateValidMeal();
        var foodItem = FoodItem.Create(meal.Id, "Arroz", 100m, MeasurementUnit.Grams, 130m);
        meal.AddFoodItem(foodItem);

        meal.RemoveFoodItem(foodItem.Id);

        meal.FoodItems.Should().BeEmpty();
    }

    [Fact]
    public void RemoveFoodItem_NonExistingItem_ThrowsDomainException()
    {
        var meal = CreateValidMeal();

        Action act = () => meal.RemoveFoodItem(Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*Alimento*não encontrado*");
    }
}
