using MediatR;
using NutritionBase.Application.DTOs.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Queries.Patients;

public class ListPatientsHandler : IRequestHandler<ListPatientsQuery, IReadOnlyList<PatientResponse>>
{
    private readonly IPatientRepository _repository;

    public ListPatientsHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PatientResponse>> Handle(ListPatientsQuery query, CancellationToken cancellationToken)
    {
        var patients = await _repository.GetAllByNutritionistIdAsync(query.NutritionistId);
        return patients.Select(MapToResponse).ToList().AsReadOnly();
    }

    private static PatientResponse MapToResponse(Patient patient) =>
        new(patient.Id, patient.NutritionistId, patient.Name, patient.Email.Value,
            patient.Phone.AreaCode, patient.Phone.Number, patient.BirthDate,
            patient.Weight, patient.Height);
}
