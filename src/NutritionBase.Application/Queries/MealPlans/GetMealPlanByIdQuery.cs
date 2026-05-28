using MediatR;
using NutritionBase.Application.DTOs.MealPlans;

namespace NutritionBase.Application.Queries.MealPlans;

public record GetMealPlanByIdQuery(Guid Id) : IRequest<MealPlanResponse>;
