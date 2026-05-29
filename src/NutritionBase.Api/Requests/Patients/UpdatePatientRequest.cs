namespace NutritionBase.Api.Requests.Patients;

public record UpdatePatientRequest(
    string Name,
    string Email,
    string AreaCode,
    string PhoneNumber,
    DateTime BirthDate,
    decimal Weight,
    decimal Height);
