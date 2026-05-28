namespace NutritionBase.Application.DTOs.MealPlans;

public record MealResponse(
    Guid Id,
    string Name,
    TimeSpan MealTime,
    IReadOnlyList<FoodItemResponse> FoodItems);
