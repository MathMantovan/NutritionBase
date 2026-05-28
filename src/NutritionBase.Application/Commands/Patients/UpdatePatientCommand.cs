using MediatR;
using NutritionBase.Application.DTOs.Patients;

namespace NutritionBase.Application.Commands.Patients;

public record UpdatePatientCommand(
    Guid Id,
    string Name,
    string Email,
    string AreaCode,
    string PhoneNumber,
    DateTime BirthDate,
    decimal Weight,
    decimal Height) : IRequest<PatientResponse>;
