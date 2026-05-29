using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Enums;

namespace NutritionBase.Application.Commands.MealPlans;

public record AddMealCommand(
    Guid MealPlanId,
    string Name,
    TimeSpan MealTime,
    IReadOnlyList<FoodItemInput> FoodItems) : IRequest<MealPlanResponse>;

public record FoodItemInput(
    string Name,
    decimal Quantity,
    MeasurementUnit Unit,
    decimal Calories);
