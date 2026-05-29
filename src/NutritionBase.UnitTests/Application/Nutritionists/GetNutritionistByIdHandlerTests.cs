using FluentAssertions;
using Moq;
using NutritionBase.Application.Queries.Nutritionists;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;
using NutritionBase.Domain.Interfaces;

namespace NutritionBase.UnitTests.Application.Nutritionists;

public class GetNutritionistByIdHandlerTests
{
    private readonly Mock<INutritionistRepository> _repoMock = new();
    private readonly GetNutritionistByIdHandler _handler;

    public GetNutritionistByIdHandlerTests()
    {
        _handler = new GetNutritionistByIdHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_NutritionistNotFound_ThrowsDomainException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Nutritionist?)null);

        var query = new GetNutritionistByIdQuery(id);
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        var assertion = await act.Should().ThrowAsync<DomainException>();
        assertion.WithMessage("*Nutricionista*não encontrado*");
    }

    [Fact]
    public async Task Handle_NutritionistFound_ReturnsMappedResponse()
    {
        var nutritionist = Nutritionist.Create("Dr. Ana Lima", "ana@clinica.com", "hashvalido");
        _repoMock.Setup(r => r.GetByIdAsync(nutritionist.Id)).ReturnsAsync(nutritionist);

        var query = new GetNutritionistByIdQuery(nutritionist.Id);
        var response = await _handler.Handle(query, CancellationToken.None);

        response.Id.Should().Be(nutritionist.Id);
        response.Name.Should().Be("Dr. Ana Lima");
        response.Email.Should().Be("ana@clinica.com");
        response.IsActive.Should().BeTrue();
    }
}
