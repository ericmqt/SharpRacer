using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharpRacer.Tools.TelemetryVariables.Data.Entities.Configuration;
internal static class CarEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<CarEntity> builder)
    {
        builder.ToTable("Car");

        // Keys
        builder.HasKey(t => t.Id);

        builder.HasAlternateKey(t => t.NormalizedPath)
            .HasName("AK_Car_NormalizedPath");

        // Properties
        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .HasColumnOrder(0);

        builder.Property(t => t.Path)
            .HasColumnName("Path")
            .HasColumnOrder(1)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasColumnOrder(2)
            .IsRequired();

        builder.Property(t => t.ShortName)
            .HasColumnName("ShortName")
            .HasColumnOrder(3)
            .IsRequired();

        builder.Property(t => t.NormalizedPath)
            .HasColumnName("NormalizedPath")
            .HasColumnOrder(4)
            .IsRequired();

        builder.Property(t => t.ContentVersion)
            .HasColumnName("ContentVersion")
            .HasColumnOrder(5)
            .HasConversion(
                v => v.ToString(),
                s => ContentVersion.Parse(s, null));

        // Relationships
        builder.HasMany(t => t.Variables)
            .WithMany()
            .UsingEntity<CarVariableEntity>(
                configureRight: j => j
                    .HasOne(d => d.Variable)
                    .WithMany()
                    .HasForeignKey(d => d.VariableKey)
                    .HasConstraintName("FK_CarVariable_Variable")
                    .OnDelete(DeleteBehavior.Cascade),
                configureLeft: j => j
                    .HasOne(d => d.Car)
                    .WithMany()
                    .HasForeignKey(d => d.CarKey)
                    .HasConstraintName("FK_CarVariable_Car")
                    .OnDelete(DeleteBehavior.Cascade));

        // Indexes
        builder.HasIndex(t => t.NormalizedPath)
            .IsUnique()
            .HasDatabaseName("AK_Car_NormalizedPath");
    }
}
