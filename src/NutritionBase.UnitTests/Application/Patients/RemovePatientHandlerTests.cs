using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class RemovePatientHandlerTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly RemovePatientHandler _handler;

    private static readonly Guid _nutritionistId = Guid.NewGuid();

    public RemovePatientHandlerTests()
    {
        _handler = new RemovePatientHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsDomainException()
    {
        var patientId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

        var command = new RemovePatientCommand(patientId, _nutritionistId);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_PatientBelongsToDifferentNutritionist_ThrowsDomainException()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _repoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient);

        var otherNutritionistId = Guid.NewGuid();
        var command = new RemovePatientCommand(patient.Id, otherNutritionistId);
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsDeleteAsync()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _repoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient);
        _repoMock.Setup(r => r.DeleteAsync(patient.Id)).Returns(Task.CompletedTask);

        var command = new RemovePatientCommand(patient.Id, _nutritionistId);
        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.DeleteAsync(patient.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_PatientNotFound_NeverCallsDeleteAsync()
    {
        var patientId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

        var command = new RemovePatientCommand(patientId, _nutritionistId);
        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
