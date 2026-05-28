// NutritionBase.Domain/Entities/MealPlan.cs
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.Domain.Entities;

public class MealPlan
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string Name { get; private set; }
    public string Objective { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Meal> _meals = new();
    public IReadOnlyList<Meal> Meals => _meals.AsReadOnly();

    protected MealPlan() { } 

    private MealPlan(Guid patientId, string name, string objective, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        PatientId = patientId;
        Name = name;
        Objective = objective;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static MealPlan Create(Guid patientId, string name, string objective, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);
        ValidateObjective(objective);
        ValidateDates(startDate, endDate);

        return new MealPlan(patientId, name, objective, startDate, endDate);
    }

    public void Update(string name, string objective, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);
        ValidateObjective(objective);
        ValidateDates(startDate, endDate);

        Name = name;
        Objective = objective;
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMeal(Meal meal)
    {
        if (_meals.Any(m => m.Name.Equals(meal.Name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Já existe uma refeição com o nome '{meal.Name}' neste plano alimentar.");

        _meals.Add(meal);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMeal(Guid mealId)
    {
        var meal = _meals.FirstOrDefault(m => m.Id == mealId);

        if (meal == null)
            throw new DomainException("Refeição não encontrada.");

        _meals.Remove(meal);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMeal(Guid mealId, string name, TimeSpan mealTime)
    {
        var meal = _meals.FirstOrDefault(m => m.Id == mealId);

        if (meal == null)
            throw new DomainException("Refeição não encontrada.");

        if (_meals.Any(m => m.Id != mealId && m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Já existe uma refeição com o nome '{name}' neste plano alimentar.");

        meal.Update(name, mealTime);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Plano alimentar já está inativo.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Plano alimentar já está ativo.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do plano alimentar não pode ser vazio.");
    }

    private static void ValidateObjective(string objective)
    {
        if (string.IsNullOrWhiteSpace(objective))
            throw new DomainException("Objetivo do plano alimentar não pode ser vazio.");
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new DomainException("A data final deve ser maior que a data de início.");
    }
}