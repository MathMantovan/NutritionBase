using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;
using NutritionBase.Application.Interfaces;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;
using SecureIdentity.Password;

namespace NutritionBase.Application.Commands.Nutritionists;

public class LoginNutritionistHandler : IRequestHandler<LoginNutritionistCommand, AuthResponse>
{
    private readonly INutritionistRepository _repository;
    private readonly ITokenService _tokenService;

    public LoginNutritionistHandler(INutritionistRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginNutritionistCommand command, CancellationToken cancellationToken)
    {
        var nutritionist = await _repository.GetByEmailAsync(command.Email)
            ?? throw new DomainException("Credenciais inválidas.");

        if (!PasswordHasher.Verify(nutritionist.PasswordHash, command.Password))
            throw new DomainException("Credenciais inválidas.");

        var token = _tokenService.GenerateToken(nutritionist.Id);

        return new AuthResponse(token, nutritionist.Id);
    }
}
