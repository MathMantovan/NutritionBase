namespace NutritionBase.Api.Requests.MealPlans;

public record CreateMealPlanRequest(
    Guid PatientId,
    string Name,
    string Objective,
    DateTime StartDate,
    DateTime EndDate);
