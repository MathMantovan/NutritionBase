using FluentAssertions;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Enums;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.UnitTests.Domain.Entities;

public class FoodItemTests
{
    private static readonly Guid _mealId = Guid.NewGuid();

    private static FoodItem CreateValidFoodItem() =>
        FoodItem.Create(_mealId, "Arroz Integral", 100m, MeasurementUnit.Grams, 130m);

    [Fact]
    public void Create_ValidData_ReturnsFoodItemWithCorrectProperties()
    {
        var item = CreateValidFoodItem();

        item.Id.Should().NotBeEmpty();
        item.MealId.Should().Be(_mealId);
        item.Name.Should().Be("Arroz Integral");
        item.Quantity.Should().Be(100m);
        item.Unit.Should().Be(MeasurementUnit.Grams);
        item.Calories.Should().Be(130m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_ThrowsDomainException(string name)
    {
        Action act = () => FoodItem.Create(_mealId, name, 100m, MeasurementUnit.Grams, 130m);
        act.Should().Throw<DomainException>().WithMessage("*Nome*alimento*");
    }

    [Fact]
    public void Create_QuantityZero_ThrowsDomainException()
    {
        Action act = () => FoodItem.Create(_mealId, "Arroz", 0m, MeasurementUnit.Grams, 130m);
        act.Should().Throw<DomainException>().WithMessage("*Quantidade*maior que zero*");
    }

    [Fact]
    public void Create_QuantityNegative_ThrowsDomainException()
    {
        Action act = () => FoodItem.Create(_mealId, "Arroz", -1m, MeasurementUnit.Grams, 130m);
        act.Should().Throw<DomainException>().WithMessage("*Quantidade*maior que zero*");
    }

    [Fact]
    public void Create_CaloriesZero_ThrowsDomainException()
    {
        Action act = () => FoodItem.Create(_mealId, "Arroz", 100m, MeasurementUnit.Grams, 0m);
        act.Should().Throw<DomainException>().WithMessage("*Calorias*maiores que zero*");
    }

    [Fact]
    public void Create_CaloriesNegative_ThrowsDomainException()
    {
        Action act = () => FoodItem.Create(_mealId, "Arroz", 100m, MeasurementUnit.Grams, -10m);
        act.Should().Throw<DomainException>().WithMessage("*Calorias*maiores que zero*");
    }

    [Fact]
    public void Create_InvalidMeasurementUnit_ThrowsDomainException()
    {
        Action act = () => FoodItem.Create(_mealId, "Arroz", 100m, (MeasurementUnit)999, 130m);
        act.Should().Throw<DomainException>().WithMessage("*Unidade de medida*inválida*");
    }

    [Theory]
    [InlineData(MeasurementUnit.Grams)]
    [InlineData(MeasurementUnit.Milliliters)]
    [InlineData(MeasurementUnit.Units)]
    public void Create_AllValidUnits_Succeeds(MeasurementUnit unit)
    {
        var item = FoodItem.Create(_mealId, "Alimento", 100m, unit, 50m);
        item.Unit.Should().Be(unit);
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var item = CreateValidFoodItem();

        item.Update("Feijão", 150m, MeasurementUnit.Milliliters, 200m);

        item.Name.Should().Be("Feijão");
        item.Quantity.Should().Be(150m);
        item.Unit.Should().Be(MeasurementUnit.Milliliters);
        item.Calories.Should().Be(200m);
    }

    [Fact]
    public void Update_QuantityZero_ThrowsDomainException()
    {
        var item = CreateValidFoodItem();
        Action act = () => item.Update("Arroz", 0m, MeasurementUnit.Grams, 130m);
        act.Should().Throw<DomainException>().WithMessage("*Quantidade*maior que zero*");
    }

    [Fact]
    public void Update_CaloriesZero_ThrowsDomainException()
    {
        var item = CreateValidFoodItem();
        Action act = () => item.Update("Arroz", 100m, MeasurementUnit.Grams, 0m);
        act.Should().Throw<DomainException>().WithMessage("*Calorias*maiores que zero*");
    }
}
