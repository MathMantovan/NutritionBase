using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Queries.MealPlans;

public class ListMealPlansByPatientHandler : IRequestHandler<ListMealPlansByPatientQuery, IReadOnlyList<MealPlanResponse>>
{
    private readonly IMealPlanRepository _repository;

    public ListMealPlansByPatientHandler(IMealPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MealPlanResponse>> Handle(ListMealPlansByPatientQuery query, CancellationToken cancellationToken)
    {
        var mealPlans = await _repository.GetAllByPatientIdAsync(query.PatientId);
        return mealPlans.Select(MapToResponse).ToList().AsReadOnly();
    }

    private static MealPlanResponse MapToResponse(MealPlan mealPlan) =>
        new(mealPlan.Id, mealPlan.PatientId, mealPlan.Name, mealPlan.Objective,
            mealPlan.StartDate, mealPlan.EndDate,
            mealPlan.Meals.Select(MapMealToResponse).ToList().AsReadOnly());

    private static MealResponse MapMealToResponse(Meal meal) =>
        new(meal.Id, meal.Name, meal.MealTime,
            meal.FoodItems.Select(f => new FoodItemResponse(f.Id, f.Name, f.Quantity, f.Unit.ToString(), f.Calories)).ToList().AsReadOnly());
}
