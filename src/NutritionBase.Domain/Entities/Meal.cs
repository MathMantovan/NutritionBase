using NutritionBase.Domain.Enums;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.Domain.Entities;

public class Meal
{
    public Guid Id { get; private set; }
    public Guid MealPlanId { get; private set; }
    public string Name { get; private set; }
    public TimeSpan MealTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<FoodItem> _foodItems = new();
    public IReadOnlyList<FoodItem> FoodItems => _foodItems.AsReadOnly();

    protected Meal() { }

    private Meal(Guid mealPlanId, string name, TimeSpan mealTime)
    {
        Id = Guid.NewGuid();
        MealPlanId = mealPlanId;
        Name = name;
        MealTime = mealTime;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Meal Create(Guid mealPlanId, string name, TimeSpan mealTime)
    {
        ValidateName(name);
        ValidateMealTime(mealTime);

        return new Meal(mealPlanId, name, mealTime);
    }

    public void Update(string name, TimeSpan mealTime)
    {
        ValidateName(name);
        ValidateMealTime(mealTime);

        Name = name;
        MealTime = mealTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddFoodItem(FoodItem foodItem)
    {
        _foodItems.Add(foodItem);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFoodItem(Guid foodItemId)
    {
        var foodItem = _foodItems.FirstOrDefault(f => f.Id == foodItemId);

        if (foodItem == null)
            throw new DomainException("Alimento não encontrado.");

        _foodItems.Remove(foodItem);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFoodItem(Guid foodItemId, string name, decimal quantity, MeasurementUnit unit, decimal calories)
    {
        var foodItem = _foodItems.FirstOrDefault(f => f.Id == foodItemId);

        if (foodItem == null)
            throw new DomainException("Alimento não encontrado.");

        if (_foodItems.Any(f => f.Id != foodItemId && f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Já existe um alimento com o nome '{name}' nesta refeição.");

        foodItem.Update(name, quantity, unit, calories);
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da refeição não pode ser vazio.");
    }

    private static void ValidateMealTime(TimeSpan mealTime)
    {
        if (mealTime < TimeSpan.Zero || mealTime >= TimeSpan.FromHours(24))
            throw new DomainException("Horário da refeição inválido.");
    }
}