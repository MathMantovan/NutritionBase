using Microsoft.EntityFrameworkCore;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;
using NutritionBase.Infrastructure.Data;
using NutritionBase.Infrastructure.Exceptions;

namespace NutritionBase.Infrastructure.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly NutritionBaseDbContext _context;

    public MealPlanRepository(NutritionBaseDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan?> GetByIdWithMealsAsync(Guid id)
        => await _context.MealPlans
            .Include(mp => mp.Meals)
                .ThenInclude(m => m.FoodItems)
            .AsSplitQuery()
            .FirstOrDefaultAsync(mp => mp.Id == id);

    public async Task<IReadOnlyList<MealPlan>> GetAllByPatientIdAsync(Guid patientId)
        => await _context.MealPlans
            .AsNoTracking()
            .Include(mp => mp.Meals)
                .ThenInclude(m => m.FoodItems)
            .AsSplitQuery()
            .Where(mp => mp.PatientId == patientId)
            .ToListAsync();

    public async Task AddAsync(MealPlan mealPlan)
    {
        try
        {
            await _context.MealPlans.AddAsync(mealPlan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao salvar plano alimentar.", ex);
        }
    }

    public async Task UpdateAsync(MealPlan mealPlan)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao atualizar plano alimentar.", ex);
        }
    }
}
