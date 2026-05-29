using MediatR;
using NutritionBase.Application.DTOs.MealPlans;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.MealPlans;

public class CreateMealPlanHandler : IRequestHandler<CreateMealPlanCommand, MealPlanCreateResponse>
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IPatientRepository _patientRepository;

    public CreateMealPlanHandler(IMealPlanRepository mealPlanRepository, IPatientRepository patientRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _patientRepository = patientRepository;
    }

    public async Task<MealPlanCreateResponse> Handle(CreateMealPlanCommand command, CancellationToken cancellationToken)
    {
        _ = await _patientRepository.GetByIdAsync(command.PatientId)
            ?? throw new DomainException("Paciente não encontrado.");

        var mealPlan = MealPlan.Create(command.PatientId, command.Name, command.Objective, command.StartDate, command.EndDate);
        await _mealPlanRepository.AddAsync(mealPlan);

        return MapToResponse(mealPlan);
    }

    private static MealPlanCreateResponse MapToResponse(MealPlan mealPlan) =>
        new(mealPlan.Id, mealPlan.PatientId, mealPlan.Name, mealPlan.Objective,
            mealPlan.StartDate, mealPlan.EndDate);

}
