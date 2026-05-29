using FluentAssertions;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.UnitTests.Domain.Entities;

public class NutritionistTests
{
    private static Nutritionist CreateValidNutritionist() =>
        Nutritionist.Create("Dr. Ana Lima", "ana@clinica.com", "hash_seguro_abc123");

    private static Patient CreateValidPatient(Guid nutritionistId) =>
        Patient.Create(nutritionistId, "João Silva", "joao@example.com", "987654321", "11",
            new DateTime(1990, 1, 1), 75m, 1.80m);

    [Fact]
    public void Create_ValidData_ReturnsNutritionistWithCorrectProperties()
    {
        var nutritionist = CreateValidNutritionist();

        nutritionist.Id.Should().NotBeEmpty();
        nutritionist.Name.Should().Be("Dr. Ana Lima");
        nutritionist.Email.Value.Should().Be("ana@clinica.com");
        nutritionist.PasswordHash.Should().Be("hash_seguro_abc123");
        nutritionist.IsActive.Should().BeTrue();
        nutritionist.Patients.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_ThrowsDomainException(string name)
    {
        Action act = () => Nutritionist.Create(name, "ana@clinica.com", "hash_seguro_abc123");
        act.Should().Throw<DomainException>().WithMessage("*Nome*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyPasswordHash_ThrowsDomainException(string passwordHash)
    {
        Action act = () => Nutritionist.Create("Dr. Ana Lima", "ana@clinica.com", passwordHash);
        act.Should().Throw<DomainException>().WithMessage("*PasswordHash*");
    }

    [Fact]
    public void Create_InvalidEmail_ThrowsDomainException()
    {
        Action act = () => Nutritionist.Create("Dr. Ana Lima", "notanemail", "hash_seguro_abc123");
        act.Should().Throw<DomainException>().WithMessage("*inválido*");
    }

    [Fact]
    public void UpdateName_ValidName_UpdatesName()
    {
        var nutritionist = CreateValidNutritionist();
        nutritionist.UpdateName("Dr. Carlos Souza");
        nutritionist.Name.Should().Be("Dr. Carlos Souza");
    }

    [Fact]
    public void UpdateName_EmptyName_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();
        Action act = () => nutritionist.UpdateName("");
        act.Should().Throw<DomainException>().WithMessage("*Nome*");
    }

    [Fact]
    public void UpdateEmail_ValidEmail_UpdatesEmail()
    {
        var nutritionist = CreateValidNutritionist();
        nutritionist.UpdateEmail("novoemail@clinica.com");
        nutritionist.Email.Value.Should().Be("novoemail@clinica.com");
    }

    [Fact]
    public void UpdateEmail_InvalidEmail_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();
        Action act = () => nutritionist.UpdateEmail("invalidemail");
        act.Should().Throw<DomainException>().WithMessage("*inválido*");
    }

    [Fact]
    public void AddPatient_ValidPatient_AddsToList()
    {
        var nutritionist = CreateValidNutritionist();
        var patient = CreateValidPatient(nutritionist.Id);

        nutritionist.AddPatient(patient);

        nutritionist.Patients.Should().HaveCount(1);
    }

    [Fact]
    public void AddPatient_DuplicateEmail_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();
        var patient1 = CreateValidPatient(nutritionist.Id);
        var patient2 = Patient.Create(nutritionist.Id, "Outro João", "joao@example.com", "98765432", "21",
            new DateTime(1985, 5, 10), 80m, 1.75m);
        nutritionist.AddPatient(patient1);

        Action act = () => nutritionist.AddPatient(patient2);
        act.Should().Throw<DomainException>().WithMessage("*joao@example.com*");
    }

    [Fact]
    public void RemovePatient_ExistingPatient_RemovesFromList()
    {
        var nutritionist = CreateValidNutritionist();
        var patient = CreateValidPatient(nutritionist.Id);
        nutritionist.AddPatient(patient);

        nutritionist.RemovePatient(patient.Id);

        nutritionist.Patients.Should().BeEmpty();
    }

    [Fact]
    public void RemovePatient_NonExistingPatient_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();

        Action act = () => nutritionist.RemovePatient(Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*Paciente*não encontrado*");
    }

    [Fact]
    public void Deactivate_ActiveNutritionist_SetsIsActiveFalse()
    {
        var nutritionist = CreateValidNutritionist();

        nutritionist.Deactivate();

        nutritionist.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_AlreadyInactive_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();
        nutritionist.Deactivate();

        Action act = () => nutritionist.Deactivate();
        act.Should().Throw<DomainException>().WithMessage("*inativo*");
    }

    [Fact]
    public void Activate_InactiveNutritionist_SetsIsActiveTrue()
    {
        var nutritionist = CreateValidNutritionist();
        nutritionist.Deactivate();

        nutritionist.Activate();

        nutritionist.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_AlreadyActive_ThrowsDomainException()
    {
        var nutritionist = CreateValidNutritionist();

        Action act = () => nutritionist.Activate();
        act.Should().Throw<DomainException>().WithMessage("*ativo*");
    }
}
