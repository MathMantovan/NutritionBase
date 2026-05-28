using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.MealPlans;

public class CreateMealPlanHandler : IRequestHandler<CreateMealPlanCommand, MealPlanResponse>
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IPatientRepository _patientRepository;

    public CreateMealPlanHandler(IMealPlanRepository mealPlanRepository, IPatientRepository patientRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _patientRepository = patientRepository;
    }

    public async Task<MealPlanResponse> Handle(CreateMealPlanCommand command, CancellationToken cancellationToken)
    {
        _ = await _patientRepository.GetByIdAsync(command.PatientId)
            ?? throw new DomainException("Paciente não encontrado.");

        var mealPlan = MealPlan.Create(command.PatientId, command.Name, command.Objective, command.StartDate, command.EndDate);
        await _mealPlanRepository.AddAsync(mealPlan);

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
