using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharpRacer.Tools.TelemetryVariables.Data.Entities.Configuration;
internal static class SessionVariableEntityConfiguration
{
    public static void Configure(EntityTypeBuilder<SessionVariableEntity> builder)
    {
        builder.ToView("SessionVariables");

        builder.HasNoKey();

        builder.Property(t => t.VariableKey)
            .HasColumnName("VariableKey")
            .IsRequired();

        builder.HasOne(t => t.Variable)
            .WithOne()
            .HasForeignKey<SessionVariableEntity>(d => d.VariableKey)
            .IsRequired();

        builder.Navigation(t => t.Variable)
            .AutoInclude();
    }
}
