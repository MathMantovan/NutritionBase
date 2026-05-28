using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Queries.MealPlans;

public class GetMealPlanByIdHandler : IRequestHandler<GetMealPlanByIdQuery, MealPlanResponse>
{
    private readonly IMealPlanRepository _repository;

    public GetMealPlanByIdHandler(IMealPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<MealPlanResponse> Handle(GetMealPlanByIdQuery query, CancellationToken cancellationToken)
    {
        var mealPlan = await _repository.GetByIdWithMealsAsync(query.Id)
            ?? throw new DomainException("Plano alimentar não encontrado.");

        return MapToResponse(mealPlan);
    }

    private static MealPlanResponse MapToResponse(MealPlan mealPlan) =>
        new(mealPlan.Id, mealPlan.PatientId, mealPlan.Name, mealPlan.Objective,
            mealPlan.StartDate, mealPlan.EndDate, mealPlan.IsActive,
            mealPlan.Meals.Select(MapMealToResponse).ToList().AsReadOnly());

    private static MealResponse MapMealToResponse(Meal meal) =>
        new(meal.Id, meal.Name, meal.MealTime,
            meal.FoodItems.Select(f => new FoodItemResponse(f.Id, f.Name, f.Quantity, f.Unit.ToString(), f.Calories)).ToList().AsReadOnly());
}
