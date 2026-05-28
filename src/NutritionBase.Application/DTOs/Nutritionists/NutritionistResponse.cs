namespace NutritionBase.Application.DTOs.Nutritionists;

public record NutritionistResponse(Guid Id, string Name, string Email, bool IsActive);
