namespace NutritionBase.Application.DTOs.MealPlans;

public record MealPlanCreateResponse(
    Guid Id,
    Guid PatientId,
    string Name,
    string Objective,
    DateTime StartDate,
    DateTime EndDate);
