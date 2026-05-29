using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class GetPatientByIdHandlerTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly GetPatientByIdHandler _handler;

    private static readonly Guid _nutritionistId = Guid.NewGuid();

    public GetPatientByIdHandlerTests()
    {
        _handler = new GetPatientByIdHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsDomainException()
    {
        var patientId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

        var query = new GetPatientByIdQuery(patientId);
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_PatientFound_ReturnsMappedResponse()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _repoMock.Setup(r => r.GetByIdAsync(patient.Id)).ReturnsAsync(patient);

        var query = new GetPatientByIdQuery(patient.Id);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Id.Should().Be(patient.Id);
        response.Name.Should().Be("João Silva");
        response.Email.Should().Be("joao@example.com");
        response.AreaCode.Should().Be("11");
        response.PhoneNumber.Should().Be("987654321");
        response.NutritionistId.Should().Be(_nutritionistId);
    }
}
