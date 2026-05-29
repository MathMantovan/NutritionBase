using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data.Configurations;

public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.ToTable("FoodItems");

        builder.HasKey(fi => fi.Id);

        builder.Property(fi => fi.MealId).IsRequired();

        builder.Property(fi => fi.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(fi => fi.Quantity)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(fi => fi.Unit)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(fi => fi.Calories)
            .IsRequired()
            .HasPrecision(8, 2);

        builder.Property(fi => fi.CreatedAt).IsRequired();
        builder.Property(fi => fi.UpdatedAt).IsRequired();
    }
}
