using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Nutritionists;

public class UpdateNutritionistHandler : IRequestHandler<UpdateNutritionistCommand, NutritionistResponse>
{
    private readonly INutritionistRepository _repository;

    public UpdateNutritionistHandler(INutritionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<NutritionistResponse> Handle(UpdateNutritionistCommand command, CancellationToken cancellationToken)
    {
        var nutritionist = await _repository.GetByIdAsync(command.Id)
            ?? throw new DomainException("Nutricionista não encontrado.");

        nutritionist.UpdateName(command.Name);
        nutritionist.UpdateEmail(command.Email);
        await _repository.UpdateAsync(nutritionist);

        return new NutritionistResponse(nutritionist.Id, nutritionist.Name, nutritionist.Email.Value, nutritionist.IsActive);
    }
}
