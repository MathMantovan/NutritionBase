using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data.Configurations;

public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("MealPlans");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.PatientId).IsRequired();

        builder.Property(mp => mp.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(mp => mp.Objective)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(mp => mp.StartDate).IsRequired();
        builder.Property(mp => mp.EndDate).IsRequired();
        builder.Property(mp => mp.CreatedAt).IsRequired();
        builder.Property(mp => mp.UpdatedAt).IsRequired();

        builder.HasMany(mp => mp.Meals)
            .WithOne()
            .HasForeignKey(m => m.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(mp => mp.Meals)
            .HasField("_meals")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
