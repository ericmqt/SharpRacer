using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharpRacer.Tools.TelemetryVariables.Data.Entities.Configuration;
internal static class CarVariableEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<CarVariableEntity> builder)
    {
        builder.ToTable("CarVariable");

        builder.HasKey(t => new { t.CarKey, t.VariableKey });

        builder.Property(t => t.CarKey)
            .HasColumnName("CarKey")
            .HasColumnOrder(0);

        builder.Property(t => t.VariableKey)
            .HasColumnName("VariableKey")
            .HasColumnOrder(1);
    }
}
