using MediatR;
using NutritionBase.Application.DTOs.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Queries.Patients;

public class GetPatientsByNameHandler : IRequestHandler<GetPatientsByNameQuery, PatientResponse?>
{
    private readonly IPatientRepository _repository;

    public GetPatientsByNameHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<PatientResponse?> Handle(GetPatientsByNameQuery query, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByNameAsync(query.Name, query.NutritionistId)
                        ?? throw new DomainException("Paciente não encontrado.");

        return MapToResponse(patient);
    }

    private static PatientResponse MapToResponse(Patient patient) =>
        new(patient.Id, patient.NutritionistId, patient.Name, patient.Email.Value,
            patient.Phone.AreaCode, patient.Phone.Number, patient.BirthDate,
            patient.Weight, patient.Height);
}
