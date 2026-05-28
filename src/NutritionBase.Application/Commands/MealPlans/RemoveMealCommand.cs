using MediatR;

namespace NutritionBase.Application.Commands.MealPlans;

public record RemoveMealCommand(Guid MealPlanId, Guid MealId) : IRequest;
