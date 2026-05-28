using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.MealPlans;

public class AddMealHandler : IRequestHandler<AddMealCommand, MealPlanResponse>
{
    private readonly IMealPlanRepository _repository;

    public AddMealHandler(IMealPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<MealPlanResponse> Handle(AddMealCommand command, CancellationToken cancellationToken)
    {
        var mealPlan = await _repository.GetByIdWithMealsAsync(command.MealPlanId)
            ?? throw new DomainException("Plano alimentar não encontrado.");

        var meal = Meal.Create(command.MealPlanId, command.Name, command.MealTime);
        mealPlan.AddMeal(meal);
        await _repository.UpdateAsync(mealPlan);

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
