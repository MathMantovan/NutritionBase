using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.ValueObjects;

namespace NutritionBase.Domain.Entities;

public class Nutritionist
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Patient> _patients = new();
    public IReadOnlyList<Patient> Patients => _patients.AsReadOnly();

    protected Nutritionist() { } 

    private Nutritionist(string name, string email, string passwordHash)
    {
        ValidateName(name);
        ValidatePasswordHash(passwordHash);

        Id = Guid.NewGuid();
        Name = name;
        Email =  Email.Create(email);
        _patients = [];
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Nutritionist Create(string name, string email, string passwordHash)
    {
        return new Nutritionist(name, email, passwordHash);
    }

    public void UpdateName(string name)
    {
        ValidateName(name);
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        var _email = Email.Create(email);
        Email = _email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AddPatient(Patient patient)
    {
        if (_patients.Any(p => p.Email.Value.Equals(patient.Email.Value, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Já existe um paciente com o e-mail '{patient.Email.Value}' para este nutricionista.");

        _patients.Add(patient);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePatient(Guid patientId)
    {
        var patientToRemove = _patients.FirstOrDefault(p => p.Id == patientId);

        if (patientToRemove == null)
            throw new DomainException($"Paciente não encontrado.");

        _patients.Remove(patientToRemove);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePatient(Guid patientId, string name, string email, string areaCode, string number, DateTime birthDate, decimal weight, decimal height)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == patientId);

        if (patient == null)
            throw new DomainException("Paciente não encontrado.");

        if (_patients.Any(p => p.Id != patientId && p.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Já existe um paciente com o e-mail '{email}' para este nutricionista.");

        patient.Update(name, email, areaCode, number, birthDate, weight, height);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Nutricionista já está inativo.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Nutricionista já está ativo.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do nutricionista não pode ser vazio.");
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PasswordHash não pode ser vazio.");
    }
}