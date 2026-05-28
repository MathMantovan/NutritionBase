using MediatR;

namespace NutritionBase.Application.Commands.Nutritionists;

public record DeactivateNutritionistCommand(Guid Id) : IRequest;
