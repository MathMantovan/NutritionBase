using Microsoft.EntityFrameworkCore;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;
using NutritionBase.Infrastructure.Data;
using NutritionBase.Infrastructure.Exceptions;

namespace NutritionBase.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly NutritionBaseDbContext _context;

    public PatientRepository(NutritionBaseDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
        => await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IReadOnlyList<Patient>> GetAllByNutritionistIdAsync(Guid nutritionistId)
        => await _context.Patients
            .AsNoTracking()
            .Where(p => p.NutritionistId == nutritionistId)
            .ToListAsync();

    public async Task<Patient?> GetByNameAsync(string name, Guid nutritionistId)
        => await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name && p.NutritionistId == nutritionistId);

    public async Task<IReadOnlyList<Patient>> GetAllByAsync(Guid nutritionistId)
        => await _context.Patients
            .AsNoTracking()
            .Where(p => p.NutritionistId == nutritionistId)
            .ToListAsync();

    public async Task<bool> ExistsByEmailAsync(string email, Guid nutritionistId)
        => await _context.Patients
            .AnyAsync(p => p.Email.Value == email);

    public async Task AddAsync(Patient patient)
    {
        try
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao salvar paciente.", ex);
        }
    }

    public async Task UpdateAsync(Patient patient)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao atualizar paciente.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient is null) return;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InfrastructureException("Erro ao remover paciente.", ex);
        }
    }
}
