using FluentAssertions;
using NutritionBase.Domain.Entities;
using NutritionBase.Domain.Exceptions;

namespace NutritionBase.UnitTests.Domain.Entities;

public class PatientTests
{
    private static readonly Guid _nutritionistId = Guid.NewGuid();
    private static readonly DateTime _validBirthDate = new DateTime(1990, 6, 15);

    private static Patient CreateValidPatient(Guid? nutritionistId = null) =>
        Patient.Create(
            nutritionistId ?? _nutritionistId,
            "João Silva",
            "joao@example.com",
            "987654321",
            "11",
            _validBirthDate,
            75m,
            1.80m);

    [Fact]
    public void Create_ValidData_ReturnsPatientWithCorrectProperties()
    {
        var patient = CreateValidPatient();

        patient.Id.Should().NotBeEmpty();
        patient.NutritionistId.Should().Be(_nutritionistId);
        patient.Name.Should().Be("João Silva");
        patient.Email.Value.Should().Be("joao@example.com");
        patient.Phone.AreaCode.Should().Be("11");
        patient.Phone.Number.Should().Be("987654321");
        patient.BirthDate.Should().Be(_validBirthDate);
        patient.Weight.Should().Be(75m);
        patient.Height.Should().Be(1.80m);
        patient.MealPlans.Should().BeEmpty();
    }

    [Fact]
    public void Create_WeightZero_ThrowsDomainException()
    {
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", _validBirthDate, 0m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*Peso*maior que zero*");
    }

    [Fact]
    public void Create_WeightNegative_ThrowsDomainException()
    {
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", _validBirthDate, -5m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*Peso*maior que zero*");
    }

    [Fact]
    public void Create_HeightZero_ThrowsDomainException()
    {
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", _validBirthDate, 75m, 0m);
        act.Should().Throw<DomainException>().WithMessage("*Altura*maior que zero*");
    }

    [Fact]
    public void Create_HeightNegative_ThrowsDomainException()
    {
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", _validBirthDate, 75m, -1m);
        act.Should().Throw<DomainException>().WithMessage("*Altura*maior que zero*");
    }

    [Fact]
    public void Create_BirthDateInFuture_ThrowsDomainException()
    {
        var futureBirthDate = DateTime.UtcNow.AddDays(1);
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", futureBirthDate, 75m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*nascimento*");
    }

    [Fact]
    public void Create_BirthDateIsNow_ThrowsDomainException()
    {
        var now = DateTime.UtcNow;
        Action act = () => Patient.Create(_nutritionistId, "João", "joao@example.com", "987654321", "11", now, 75m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*nascimento*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_ThrowsDomainException(string name)
    {
        Action act = () => Patient.Create(_nutritionistId, name, "joao@example.com", "987654321", "11", _validBirthDate, 75m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*Nome*");
    }

    [Fact]
    public void Create_InvalidEmailFormat_ThrowsDomainException()
    {
        Action act = () => Patient.Create(_nutritionistId, "João", "invalid-email", "987654321", "11", _validBirthDate, 75m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*inválido*");
    }

    [Fact]
    public void Update_WeightZero_ThrowsDomainException()
    {
        var patient = CreateValidPatient();
        Action act = () => patient.Update("João", "joao@example.com", "987654321", "11", _validBirthDate, 0m, 1.80m);
        act.Should().Throw<DomainException>().WithMessage("*Peso*maior que zero*");
    }

    [Fact]
    public void Update_HeightZero_ThrowsDomainException()
    {
        var patient = CreateValidPatient();
        Action act = () => patient.Update("João", "joao@example.com", "987654321", "11", _validBirthDate, 75m, 0m);
        act.Should().Throw<DomainException>().WithMessage("*Altura*maior que zero*");
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var patient = CreateValidPatient();
        var newBirthDate = new DateTime(1985, 3, 20);

        patient.Update("Maria Silva", "maria@example.com", "98765432", "21", newBirthDate, 60m, 1.65m);

        patient.Name.Should().Be("Maria Silva");
        patient.Email.Value.Should().Be("maria@example.com");
        patient.Weight.Should().Be(60m);
        patient.Height.Should().Be(1.65m);
    }

    [Fact]
    public void AddMealPlan_ValidMealPlan_AddsToPlan()
    {
        var patient = CreateValidPatient();
        var mealPlan = MealPlan.Create(patient.Id, "Plano A", "Emagrecimento", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

        patient.AddMealPlan(mealPlan);

        patient.MealPlans.Should().HaveCount(1);
    }

    [Fact]
    public void AddMealPlan_DuplicateName_ThrowsDomainException()
    {
        var patient = CreateValidPatient();
        var mealPlan1 = MealPlan.Create(patient.Id, "Plano A", "Emagrecimento", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        var mealPlan2 = MealPlan.Create(patient.Id, "Plano A", "Ganho de massa", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        patient.AddMealPlan(mealPlan1);

        Action act = () => patient.AddMealPlan(mealPlan2);
        act.Should().Throw<DomainException>().WithMessage("*Plano A*");
    }

    [Fact]
    public void RemoveMealPlan_ExistingPlan_RemovesFromList()
    {
        var patient = CreateValidPatient();
        var mealPlan = MealPlan.Create(patient.Id, "Plano A", "Emagrecimento", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));
        patient.AddMealPlan(mealPlan);

        patient.RemoveMealPlan(mealPlan.Id);

        patient.MealPlans.Should().BeEmpty();
    }

    [Fact]
    public void RemoveMealPlan_NonExistingPlan_ThrowsDomainException()
    {
        var patient = CreateValidPatient();

        Action act = () => patient.RemoveMealPlan(Guid.NewGuid());
        act.Should().Throw<DomainException>().WithMessage("*Plano alimentar*não encontrado*");
    }
}
