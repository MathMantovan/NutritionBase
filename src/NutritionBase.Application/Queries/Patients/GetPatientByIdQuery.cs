using MediatR;
using NutritionBase.Application.DTOs.Patients;

namespace NutritionBase.Application.Queries.Patients;

public record GetPatientByIdQuery(Guid Id, Guid NutritionistId) : IRequest<PatientResponse>;
