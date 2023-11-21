using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;
using SharpRacer.Tools.TelemetryVariables.Data.Entities.Configuration;

namespace SharpRacer.Tools.TelemetryVariables.Data;
public class TelemetryVariablesDbContext : DbContext
{
    public TelemetryVariablesDbContext(DbContextOptions<TelemetryVariablesDbContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    public DbSet<CarEntity> Cars { get; set; }
    public DbSet<SessionVariableEntity> SessionVariables { get; set; }
    public DbSet<VariableEntity> Variables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CarEntityConfiguration.Configure(modelBuilder.Entity<CarEntity>());
        CarVariableEntityConfiguration.Configure(modelBuilder.Entity<CarVariableEntity>());
        SessionVariableEntityConfiguration.Configure(modelBuilder.Entity<SessionVariableEntity>());
        VariableEntityConfiguration.Configure(modelBuilder.Entity<VariableEntity>());
    }
}
