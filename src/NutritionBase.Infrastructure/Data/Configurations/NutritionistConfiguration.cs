using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionBase.Domain.Entities;

namespace NutritionBase.Infrastructure.Data.Configurations;

public class NutritionistConfiguration : IEntityTypeConfiguration<Nutritionist>
{
    public void Configure(EntityTypeBuilder<Nutritionist> builder)
    {
        builder.ToTable("Nutritionists");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.OwnsOne(n => n.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);

            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(n => n.PasswordHash).IsRequired();
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.UpdatedAt).IsRequired();
        builder.Property(n => n.IsActive).IsRequired();

        builder.HasIndex(n => n.Name).IsUnique();

        builder.HasMany(n => n.Patients)
            .WithOne()
            .HasForeignKey(p => p.NutritionistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(n => n.Patients)
            .HasField("_patients")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
