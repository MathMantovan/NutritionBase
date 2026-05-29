using MediatR;
using NutritionBase.Application.DTOs.MealPlans;

namespace NutritionBase.Application.Commands.MealPlans;

public record CreateMealPlanCommand(
    Guid PatientId,
    string Name,
    string Objective,
    DateTime StartDate,
    DateTime EndDate) : IRequest<MealPlanCreateResponse>;
