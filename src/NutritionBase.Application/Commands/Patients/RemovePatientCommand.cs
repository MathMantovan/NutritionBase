using MediatR;

namespace NutritionBase.Application.Commands.Patients;

public record RemovePatientCommand(Guid PatientId, Guid NutritionistId) : IRequest;
