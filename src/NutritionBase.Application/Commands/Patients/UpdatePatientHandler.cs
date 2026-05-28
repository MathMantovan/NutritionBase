using MediatR;
using NutritionBase.Application.DTOs.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Patients;

public class UpdatePatientHandler : IRequestHandler<UpdatePatientCommand, PatientResponse>
{
    private readonly IPatientRepository _repository;

    public UpdatePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<PatientResponse> Handle(UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(command.Id)
            ?? throw new DomainException("Paciente não encontrado.");

        patient.Update(command.Name, command.Email, command.PhoneNumber, command.AreaCode, command.BirthDate, command.Weight, command.Height);
        await _repository.UpdateAsync(patient);

        return MapToResponse(patient);
    }

    private static PatientResponse MapToResponse(Patient patient) =>
        new(patient.Id, patient.NutritionistId, patient.Name, patient.Email.Value,
            patient.Phone.AreaCode, patient.Phone.Number, patient.BirthDate,
            patient.Weight, patient.Height, patient.IsActive);
}
