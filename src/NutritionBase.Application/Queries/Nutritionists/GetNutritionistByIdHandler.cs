using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Queries.Nutritionists;

public class GetNutritionistByIdHandler : IRequestHandler<GetNutritionistByIdQuery, NutritionistResponse>
{
    private readonly INutritionistRepository _repository;

    public GetNutritionistByIdHandler(INutritionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<NutritionistResponse> Handle(GetNutritionistByIdQuery query, CancellationToken cancellationToken)
    {
        var nutritionist = await _repository.GetByIdAsync(query.Id)
            ?? throw new DomainException("Nutricionista não encontrado.");

        return new NutritionistResponse(nutritionist.Id, nutritionist.Name, nutritionist.Email.Value, nutritionist.IsActive);
    }
}
