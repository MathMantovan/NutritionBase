using MediatR;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Nutritionists;

public class DeactivateNutritionistHandler : IRequestHandler<DeactivateNutritionistCommand>
{
    private readonly INutritionistRepository _repository;

    public DeactivateNutritionistHandler(INutritionistRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeactivateNutritionistCommand command, CancellationToken cancellationToken)
    {
        var nutritionist = await _repository.GetByIdAsync(command.Id)
            ?? throw new DomainException("Nutricionista não encontrado.");

        nutritionist.Deactivate();
        await _repository.UpdateAsync(nutritionist);
    }
}
