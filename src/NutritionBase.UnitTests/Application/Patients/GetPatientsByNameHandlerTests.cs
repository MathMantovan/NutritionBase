using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.Patients;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Patients;

public class GetPatientsByNameHandlerTests
{
    private readonly Mock<IPatientRepository> _repoMock = new();
    private readonly GetPatientsByNameHandler _handler;

    private static readonly Guid _nutritionistId = Guid.NewGuid();

    public GetPatientsByNameHandlerTests()
    {
        _handler = new GetPatientsByNameHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsDomainException()
    {
        _repoMock.Setup(r => r.GetByNameAsync("Inexistente", _nutritionistId)).ReturnsAsync((Patient?)null);

        var query = new GetPatientsByNameQuery("Inexistente", _nutritionistId);
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public async Task Handle_PatientFound_ReturnsMappedResponse()
    {
        var patient = Patient.Create(_nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);
        _repoMock.Setup(r => r.GetByNameAsync("João Silva", _nutritionistId)).ReturnsAsync(patient);

        var query = new GetPatientsByNameQuery("João Silva", _nutritionistId);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Should().NotBeNull();
        response!.Name.Should().Be("João Silva");
        response.Email.Should().Be("joao@example.com");
    }
}
