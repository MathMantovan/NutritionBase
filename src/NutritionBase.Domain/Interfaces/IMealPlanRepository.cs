using NutritionBase.Domain.Entities;

namespace NutritionBase.Domain.Interfaces;

public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdWithMealsAsync(Guid id);
    Task<IReadOnlyList<MealPlan>> GetAllByPatientIdAsync(Guid patientId);
    Task AddAsync(MealPlan mealPlan);
    Task UpdateAsync(MealPlan mealPlan);
}
