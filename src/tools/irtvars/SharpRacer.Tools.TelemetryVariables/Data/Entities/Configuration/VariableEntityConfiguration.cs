using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpRacer.Interop;

namespace SharpRacer.Tools.TelemetryVariables.Data.Entities.Configuration;
internal static class VariableEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<VariableEntity> builder)
    {
        builder.ToTable("Variable");

        // Keys
        builder.HasKey(t => t.Id);

        builder.HasAlternateKey(t => t.NormalizedName)
            .HasName("AK_Variable_NormalizedName");

        // Properties
        builder.Property(t => t.Id)
           .HasColumnName("Id")
           .HasColumnOrder(0);

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasColumnOrder(1)
            .IsRequired()
            .HasMaxLength(DataFileConstants.MaxStringLength);

        builder.Property(t => t.NormalizedName)
            .HasColumnName("NormalizedName")
            .HasColumnOrder(2)
            .IsRequired()
            .HasMaxLength(DataFileConstants.MaxStringLength);

        builder.Property(t => t.ValueType)
            .HasColumnName("ValueType")
            .HasColumnOrder(3);

        builder.Property(t => t.ValueUnit)
            .HasColumnName("ValueUnit")
            .HasColumnOrder(4)
            .HasMaxLength(DataFileConstants.MaxStringLength);

        builder.Property(t => t.ValueCount)
            .HasColumnName("ValueCount")
            .HasColumnOrder(5);

        builder.Property(t => t.IsTimeSliceArray)
           .HasColumnName("IsTimeSliceArray")
           .HasColumnOrder(6);

        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasColumnOrder(7)
            .HasMaxLength(DataFileConstants.MaxDescriptionLength);

        builder.Property(t => t.IsDeprecated)
            .HasColumnName("IsDeprecated")
            .HasColumnOrder(8);

        builder.Property<int?>("DeprecatingVariableKey")
            .HasColumnName("DeprecatingVariableKey")
            .HasColumnOrder(9)
            .IsRequired(false);

        builder.Property(t => t.SimulatorVersion)
            .HasColumnName("SimulatorVersion")
            .HasColumnOrder(10)
            .HasConversion(
                v => v.ToString(),
                s => ContentVersion.Parse(s, null));

        // Relationships
        builder.HasOne(t => t.DeprecatingVariable)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey("DeprecatingVariableKey")
            .HasConstraintName("FK_Variable_DeprecatingVariable")
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(t => t.NormalizedName)
            .IsUnique()
            .HasDatabaseName("AK_Variable_NormalizedName");
    }
}
