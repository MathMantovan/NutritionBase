using Microsoft.EntityFrameworkCore;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;
using NutritionBase.Infrastructure.Data;
using NutritionBase.Infrastructure.Exceptions;

namespace NutritionBase.Infrastructure.Repositories;

public class NutritionistRepository : INutritionistRepository
{
    private readonly NutritionBaseDbContext _context;

    public NutritionistRepository(NutritionBaseDbContext context)
    {
        _context = context;
    }

    public async Task<Nutritionist?> GetByIdAsync(Guid id)
        => await _context.Nutritionists
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task<Nutritionist?> GetByIdWithPatientsAsync(Guid id)
        => await _context.Nutritionists
            .Include(n => n.Patients)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task<Nutritionist?> GetByEmailAsync(string email)
        => await _context.Nutritionists
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Email.Value == email);

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _context.Nutritionists
            .AnyAsync(n => n.Email.Value == email);

    public async Task AddAsync(Nutritionist nutritionist)
    {
        try
        {
            await _context.Nutritionists.AddAsync(nutritionist);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao salvar nutricionista.", ex);
        }
    }

    public async Task UpdateAsync(Nutritionist nutritionist)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao atualizar nutricionista.", ex);
        }
    }
}
