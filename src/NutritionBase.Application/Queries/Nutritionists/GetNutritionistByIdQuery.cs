using MediatR;
using NutritionBase.Application.DTOs.Nutritionists;

namespace NutritionBase.Application.Queries.Nutritionists;

public record GetNutritionistByIdQuery(Guid Id) : IRequest<NutritionistResponse>;
