using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;
using SecureIdentity.Password;

namespace NutritionBase.Application.Commands.Nutritionists;

public class CreateNutritionistHandler : IRequestHandler<CreateNutritionistCommand, NutritionistResponse>
{
    private readonly INutritionistRepository _repository;

    public CreateNutritionistHandler(INutritionistRepository repository)
    {
        _repository = repository;
    }

    public async Task<NutritionistResponse> Handle(CreateNutritionistCommand command, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByEmailAsync(command.Email))
            throw new DomainException($"Já existe um nutricionista com o e-mail '{command.Email}'.");

        var hash = PasswordHasher.Hash(command.Password);
        var nutritionist = Nutritionist.Create(command.Name, command.Email, hash);
        await _repository.AddAsync(nutritionist);

        return new NutritionistResponse(nutritionist.Id, nutritionist.Name, nutritionist.Email.Value, nutritionist.IsActive);
    }
}
