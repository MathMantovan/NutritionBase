namespace NutritionBase.Application.DTOs.Patients;

public record PatientResponse(
    Guid Id,
    Guid NutritionistId,
    string Name,
    string Email,
    string AreaCode,
    string PhoneNumber,
    DateTime BirthDate,
    decimal Weight,
    decimal Height,
    bool IsActive);
