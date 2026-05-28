using NutritionBase.Domain.Entities;

namespace NutritionBase.Domain.Interfaces;

public interface INutritionistRepository
{
    Task<Nutritionist?> GetByIdAsync(Guid id);
    Task<Nutritionist?> GetByIdWithPatientsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(Nutritionist nutritionist);
    Task UpdateAsync(Nutritionist nutritionist);
}
