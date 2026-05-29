namespace NutritionBase.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid nutritionistId);
}
