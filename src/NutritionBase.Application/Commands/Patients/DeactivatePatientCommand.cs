using MediatR;

namespace NutritionBase.Application.Commands.Patients;

public record DeactivatePatientCommand(Guid Id) : IRequest;
