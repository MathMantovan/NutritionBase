using Microsoft.EntityFrameworkCore;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data;

public class NutritionBaseDbContext : DbContext
{
    public NutritionBaseDbContext(DbContextOptions<NutritionBaseDbContext> options) : base(options) { }

    public DbSet<Nutritionist> Nutritionists => Set<Nutritionist>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutritionBaseDbContext).Assembly);
    }
}
