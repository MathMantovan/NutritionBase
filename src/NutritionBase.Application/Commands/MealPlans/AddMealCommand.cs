using MediatR;
using NutritionBase.Application.DTOs.MealPlans;

namespace NutritionBase.Application.Commands.MealPlans;

public record AddMealCommand(Guid MealPlanId, string Name, TimeSpan MealTime) : IRequest<MealPlanResponse>;
