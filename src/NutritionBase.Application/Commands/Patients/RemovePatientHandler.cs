using MediatR;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Patients;

public class RemovePatientHandler : IRequestHandler<RemovePatientCommand>
{
    private readonly IPatientRepository _repository;

    public RemovePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemovePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(command.PatientId)
            ?? throw new DomainException("Paciente não encontrado.");

        await _repository.DeleteAsync(patient.Id);
    }
}
