using NutritionBase.Domain.Entities;

namespace NutritionBase.Domain.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Patient>> GetAllByNutritionistIdAsync(Guid nutritionistId);
    Task<IReadOnlyList<Patient>> GetByNameAsync(string name, Guid nutritionistId);
    Task<bool> ExistsByEmailAsync(string email, Guid nutritionistId);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
}
