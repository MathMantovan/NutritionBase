using MediatR;
using NutritionBase.Application.DTOs.Patients;

namespace NutritionBase.Application.Queries.Patients;

public record GetPatientsByNameQuery(string Name, Guid NutritionistId) : IRequest<PatientResponse?>;
