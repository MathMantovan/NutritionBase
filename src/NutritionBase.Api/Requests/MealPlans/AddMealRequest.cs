using NutritionBase.Application.Commands.MealPlans;

namespace NutritionBase.Api.Requests.MealPlans;

public record AddMealRequest(
    string Name,
    TimeSpan MealTime,
    IReadOnlyList<FoodItemInput> FoodItems);
