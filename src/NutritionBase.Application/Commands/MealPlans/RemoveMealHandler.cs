using MediatR;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.MealPlans;

public class RemoveMealHandler : IRequestHandler<RemoveMealCommand>
{
    private readonly IMealPlanRepository _repository;

    public RemoveMealHandler(IMealPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemoveMealCommand command, CancellationToken cancellationToken)
    {
        var mealPlan = await _repository.GetByIdWithMealsAsync(command.MealPlanId)
            ?? throw new DomainException("Plano alimentar não encontrado.");

        mealPlan.RemoveMeal(command.MealId);
        await _repository.UpdateAsync(mealPlan);
    }
}
