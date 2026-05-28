namespace NutritionBase.Application.DTOs.MealPlans;

public record MealPlanResponse(
    Guid Id,
    Guid PatientId,
    string Name,
    string Objective,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    IReadOnlyList<MealResponse> Meals);
