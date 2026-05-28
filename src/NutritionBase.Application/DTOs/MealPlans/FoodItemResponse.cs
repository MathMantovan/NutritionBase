namespace NutritionBase.Application.DTOs.MealPlans;

public record FoodItemResponse(
    Guid Id,
    string Name,
    decimal Quantity,
    string Unit,
    decimal Calories);
