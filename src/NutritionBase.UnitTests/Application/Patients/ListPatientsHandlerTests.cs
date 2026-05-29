using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class ListPatientsHandlerTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly ListPatientsHandler _handler;

    private static readonly Guid _nutritionistId = Guid.NewGuid();

    public ListPatientsHandlerTests()
    {
        _handler = new ListPatientsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_NoPatients_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetAllByNutritionistIdAsync(_nutritionistId))
            .ReturnsAsync(new List<Patient>().AsReadOnly());

        var query = new ListPatientsQuery(_nutritionistId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MultiplePatients_ReturnsMappedList()
    {
        var patient1 = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        var patient2 = Patient.Create(_nutritionistId, "Maria Souza", "maria@example.com", "98765432", "21",
            new DateTime(1985, 6, 15), 60m, 1.65m);
        var patients = new List<Patient> { patient1, patient2 }.AsReadOnly();

        _repoMock.Setup(r => r.GetAllByNutritionistIdAsync(_nutritionistId)).ReturnsAsync(patients);

        var query = new ListPatientsQuery(_nutritionistId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Should().HaveCount(2);
        response.Should().Contain(p => p.Name == "João Silva");
        response.Should().Contain(p => p.Name == "Maria Souza");
    }

    [Fact]
    public async Task Handle_SinglePatient_MapsAllFieldsCorrectly()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _repoMock.Setup(r => r.GetAllByNutritionistIdAsync(_nutritionistId))
            .ReturnsAsync(new List<Patient> { patient }.AsReadOnly());

        var query = new ListPatientsQuery(_nutritionistId);
        var response = await _handler.Handle(query, CancellationToken.None);

        var dto = response.Single();
        dto.Id.Should().Be(patient.Id);
        dto.NutritionistId.Should().Be(_nutritionistId);
        dto.Email.Should().Be("joao@example.com");
        dto.AreaCode.Should().Be("11");
        dto.PhoneNumber.Should().Be("987654321");
    }
}
