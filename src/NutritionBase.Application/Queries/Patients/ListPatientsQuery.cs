using MediatR;
using NutritionBase.Application.DTOs.Patients;

namespace NutritionBase.Application.Queries.Patients;

public record ListPatientsQuery(Guid NutritionistId) : IRequest<IReadOnlyList<PatientResponse>>;
