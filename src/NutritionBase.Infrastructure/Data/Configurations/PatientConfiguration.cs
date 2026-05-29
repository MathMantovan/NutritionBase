using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.NutritionistId).IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.OwnsOne(p => p.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);
        });

        builder.OwnsOne(p => p.Phone, phone =>
        {
            phone.Property(ph => ph.AreaCode)
                .HasColumnName("PhoneAreaCode")
                .IsRequired()
                .HasMaxLength(2);

            phone.Property(ph => ph.Number)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(9);
        });

        builder.Property(p => p.BirthDate).IsRequired();
        builder.Property(p => p.Weight).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.Height).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.HasIndex(p => new { p.NutritionistId, p.Name }).IsUnique();

        builder.HasMany(p => p.MealPlans)
            .WithOne()
            .HasForeignKey(mp => mp.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.MealPlans)
            .HasField("_mealPlans")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
