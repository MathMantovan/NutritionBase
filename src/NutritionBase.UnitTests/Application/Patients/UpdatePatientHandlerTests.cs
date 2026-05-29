using FluentAssertions;
using Moq;
using NutritionBase.Application.Commands.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class UpdatePatientHandlerTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly UpdatePatientHandler _handler;

    private static readonly Guid _patientId = Guid.NewGuid();
    private static readonly Guid _nutritionistId = Guid.NewGuid();

    private static Patient CreateValidPatient() =>
        Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);

    private static readonly UpdatePatientCommand _validCommand = new(
        _patientId,
        _nutritionistId,
        "João Atualizado",
        "joao@example.com",
        "11",
        "987654321",
        new DateTime(1990, 1, 1),
        80m,
        1.82m);

    public UpdatePatientHandlerTests()
    {
        _handler = new UpdatePatientHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsDomainException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync((Patient?)null);

        Func<Task> act = async () => await _handler.Handle(_validCommand, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_EmailChangedAndAlreadyExists_ThrowsDomainException()
    {
        var patient = CreateValidPatient();
        _repoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);

        var commandWithNewEmail = _validCommand with { Email = "outro@example.com" };
        _repoMock.Setup(r => r.ExistsByEmailAsync("outro@example.com", _nutritionistId)).ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(commandWithNewEmail, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*email*");
    }

    [Fact]
    public async Task Handle_SameEmail_SkipsEmailDuplicateCheck()
    {
        var patient = CreateValidPatient();
        _repoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        // email in command matches patient's existing email — no duplicate check expected
        await _handler.Handle(_validCommand, CancellationToken.None);

        _repoMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedPatientResponse()
    {
        var patient = CreateValidPatient();
        _repoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        var response = await _handler.Handle(_validCommand, CancellationToken.None);

        response.Name.Should().Be("João Atualizado");
        response.Weight.Should().Be(80m);
        response.Height.Should().Be(1.82m);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
    {
        var patient = CreateValidPatient();
        _repoMock.Setup(r => r.GetByIdAsync(_patientId)).ReturnsAsync(patient);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        await _handler.Handle(_validCommand, CancellationToken.None);

        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
    }
}
