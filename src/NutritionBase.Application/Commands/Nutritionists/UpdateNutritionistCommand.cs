using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;

namespace NutritionBase.Application.Commands.Nutritionists;

public record UpdateNutritionistCommand(Guid Id, string Name, string Email) : IRequest<NutritionistResponse>;
