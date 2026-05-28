// NutritionBase.Domain/Entities/Patient.cs
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.ValueObjects;

namespace NutritionBase.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public Guid NutritionistId { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public DateTime BirthDate { get; private set; }
    public decimal Weight { get; private set; }
    public decimal Height { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<MealPlan> _mealPlans = new();
    public IReadOnlyList<MealPlan> MealPlans => _mealPlans.AsReadOnly();

    protected Patient() { }

    private Patient(Guid nutritionistId, string name, Email email, Phone phone, DateTime birthDate, decimal weight, decimal height)
    {
        Id = Guid.NewGuid();
        NutritionistId = nutritionistId;
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        Weight = weight;
        Height = height;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Patient Create(Guid nutritionistId, string name, string email, string number, string areaCode, DateTime birthDate, decimal weight, decimal height)
    {
        ValidateName(name);
        ValidateWeight(weight);
        ValidateHeight(height);
        ValidateBirthDate(birthDate);
        var _email = Email.Create(email);
        var _phone = Phone.Create(areaCode, number);
        return new Patient(nutritionistId, name, _email, _phone, birthDate, weight, height);
    }

    public void Update(string name, string email, string number, string areaCode, DateTime birthDate, decimal weight, decimal height)
    {
        ValidateName(name);
        ValidateWeight(weight);
        ValidateHeight(height);
        ValidateBirthDate(birthDate);

        Name = name;
        Email =  Email.Create(email);
        Phone = Phone.Create(areaCode, number);
        BirthDate = birthDate;
        Weight = weight;
        Height = height;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AddMealPlan(MealPlan mealPlan)
    {
        if (_mealPlans.Any(mp => mp.Name.Equals(mealPlan.Name, StringComparison.OrdinalIgnoreCase) && mp.IsActive))
            throw new DomainException($"Já existe um plano alimentar ativo com o nome '{mealPlan.Name}' para este paciente.");

        _mealPlans.Add(mealPlan);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMealPlan(Guid mealPlanId)
    {
        var mealPlan = _mealPlans.FirstOrDefault(mp => mp.Id == mealPlanId);

        if (mealPlan == null)
            throw new DomainException("Plano alimentar não encontrado.");

        _mealPlans.Remove(mealPlan);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMealPlan(Guid mealPlanId, string name, string objective, DateTime startDate, DateTime endDate)
    {
        var mealPlan = _mealPlans.FirstOrDefault(mp => mp.Id == mealPlanId);

        if (mealPlan == null)
            throw new DomainException("Plano alimentar não encontrado.");

        if (_mealPlans.Any(mp => mp.Id != mealPlanId && mp.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && mp.IsActive))
            throw new DomainException($"Já existe um plano alimentar ativo com o nome '{name}' para este paciente.");

        mealPlan.Update(name, objective, startDate, endDate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Paciente já está inativo.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Paciente já está ativo.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do paciente não pode ser vazio.");
    }



    private static void ValidateWeight(decimal weight)
    {
        if (weight <= 0)
            throw new DomainException("Peso deve ser maior que zero.");
    }

    private static void ValidateHeight(decimal height)
    {
        if (height <= 0)
            throw new DomainException("Altura deve ser maior que zero.");
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate >= DateTime.UtcNow)
            throw new DomainException("Data de nascimento inválida.");
    }
}