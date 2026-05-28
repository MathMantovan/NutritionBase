// NutritionBase.Domain/Entities/FoodItem.cs
using NutritionBase.Domain.Enums;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.Domain.Entities;

public class FoodItem
{
    public Guid Id { get; private set; }
    public Guid MealId { get; private set; }
    public string Name { get; private set; }
    public decimal Quantity { get; private set; }
    public MeasurementUnit Unit { get; private set; }
    public decimal Calories { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected FoodItem() { } 

    private FoodItem(Guid mealId, string name, decimal quantity, MeasurementUnit unit, decimal calories)
    {
        Id = Guid.NewGuid();
        MealId = mealId;
        Name = name;
        Quantity = quantity;
        Unit = unit;
        Calories = calories;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static FoodItem Create(Guid mealId, string name, decimal quantity, MeasurementUnit unit, decimal calories)
    {
        ValidateName(name);
        ValidateQuantity(quantity);
        ValidateCalories(calories);
        ValidateUnit(unit);

        return new FoodItem(mealId, name, quantity, unit, calories);
    }

    public void Update(string name, decimal quantity, MeasurementUnit unit, decimal calories)
    {
        ValidateName(name);
        ValidateQuantity(quantity);
        ValidateCalories(calories);
        ValidateUnit(unit);

        Name = name;
        Quantity = quantity;
        Unit = unit;
        Calories = calories;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do alimento não pode ser vazio.");
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantidade do alimento deve ser maior que zero.");
    }

    private static void ValidateCalories(decimal calories)
    {
        if (calories <= 0)
            throw new DomainException("Calorias do alimento devem ser maiores que zero.");
    }

    private static void ValidateUnit(MeasurementUnit unit)
    {
        if (!Enum.IsDefined(typeof(MeasurementUnit), unit))
            throw new DomainException("Unidade de medida inválida.");
    }
}