using MediatR;
using NutritionBase.Application.DTOs.MealPlans;

namespace NutritionBase.Application.Queries.MealPlans;

public record ListMealPlansByPatientQuery(Guid PatientId) : IRequest<IReadOnlyList<MealPlanResponse>>;
