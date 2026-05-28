using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;

namespace NutritionBase.Application.Commands.Nutritionists;

public record CreateNutritionistCommand(string Name, string Email, string PasswordHash) : IRequest<NutritionistResponse>;
