using MediatR;
using NutritionBase.Application.DTOs.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.Application.Commands.Patients;

public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, PatientResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly INutritionistRepository _nutritionistRepository;

    public CreatePatientHandler(IPatientRepository patientRepository, INutritionistRepository nutritionistRepository)
    {
        _patientRepository = patientRepository;
        _nutritionistRepository = nutritionistRepository;
    }

    public async Task<PatientResponse> Handle(CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var nutritionist = await _nutritionistRepository.GetByIdAsync(command.NutritionistId)
            ?? throw new DomainException("Nutricionista não encontrado.");

        if (await _patientRepository.ExistsByEmailAsync(command.Email, command.NutritionistId))
            throw new DomainException($"Já existe um paciente com o e-mail '{command.Email}' para este nutricionista.");

        var patient = Patient.Create(command.NutritionistId, command.Name, command.Email, command.PhoneNumber, command.AreaCode, command.BirthDate, command.Weight, command.Height);
        await _patientRepository.AddAsync(patient);

        return MapToResponse(patient);
    }

    private static PatientResponse MapToResponse(Patient patient) =>
        new(patient.Id, patient.NutritionistId, patient.Name, patient.Email.Value,
            patient.Phone.AreaCode, patient.Phone.Number, patient.BirthDate,
            patient.Weight, patient.Height);
}
