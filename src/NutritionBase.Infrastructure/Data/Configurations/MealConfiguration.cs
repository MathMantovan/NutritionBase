using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data.Configurations;

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MealPlanId).IsRequired();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.MealTime).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.UpdatedAt).IsRequired();

        builder.HasMany(m => m.FoodItems)
            .WithOne()
            .HasForeignKey(fi => fi.MealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(m => m.FoodItems)
            .HasField("_foodItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
