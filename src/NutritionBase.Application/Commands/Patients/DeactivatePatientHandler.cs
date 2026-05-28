using MediatR;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Patients;

public class DeactivatePatientHandler : IRequestHandler<DeactivatePatientCommand>
{
    private readonly IPatientRepository _repository;

    public DeactivatePatientHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeactivatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await _repository.GetByIdAsync(command.Id)
            ?? throw new DomainException("Paciente não encontrado.");

        patient.Deactivate();
        await _repository.UpdateAsync(patient);
    }
}
