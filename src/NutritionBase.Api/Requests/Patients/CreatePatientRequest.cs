namespace NutritionBase.Api.Requests.Patients;

public record CreatePatientRequest(
    string Name,
    string Email,
    string AreaCode,
    string PhoneNumber,
    DateTime BirthDate,
    decimal Weight,
    decimal Height);
