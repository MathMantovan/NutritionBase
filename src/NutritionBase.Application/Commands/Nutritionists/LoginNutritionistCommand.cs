using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;

namespace NutritionBase.Application.Commands.Nutritionists;

public record LoginNutritionistCommand(string Email, string Password) : IRequest<AuthResponse>;
