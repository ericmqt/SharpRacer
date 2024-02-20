using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;

namespace SharpRacer.Tools.TelemetryVariables.Data;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTelemetryVariablesDbContext(
        this IServiceCollection services,
        Action<SqliteConnectionStringBuilder> configureConnectionString,
        Action<SqliteDbContextOptionsBuilder>? configureSqlite = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureConnectionString);

        return services.AddTelemetryVariablesDbContext(
            (svc, csb) => configureConnectionString(csb),
            configureSqlite);
    }

    public static IServiceCollection AddTelemetryVariablesDbContext(
        this IServiceCollection services,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder>? configureSqlite = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddDbContext<TelemetryVariablesDbContext>(
            (svc, db) =>
            {
                db.UseSqlite(connectionString,
                    sqlite =>
                    {
                        sqlite.UseSharpRacerMigrationsAssembly();

                        configureSqlite?.Invoke(sqlite);
                    });
            });
    }

    public static IServiceCollection AddTelemetryVariablesDbContext(
        this IServiceCollection services,
        Action<IServiceProvider, SqliteConnectionStringBuilder> configureConnectionString,
        Action<SqliteDbContextOptionsBuilder>? configureSqlite = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddDbContext<TelemetryVariablesDbContext>(
            (svc, db) =>
            {
                var csb = new SqliteConnectionStringBuilder();

                configureConnectionString(svc, csb);

                db.UseSqlite(csb.ConnectionString,
                    sqlite =>
                    {
                        sqlite.UseSharpRacerMigrationsAssembly();

                        configureSqlite?.Invoke(sqlite);
                    });
            });
    }

    public static IServiceCollection AddTelemetryVariablesDataServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddScoped<ICarStore, CarStore>()
            .AddScoped<ICarVariableStore, CarVariableStore>()
            .AddScoped<IVariableStore, VariableStore>()
            .AddScoped<ICarManager, CarManager>()
            .AddScoped<IVariableManager, VariableManager>();
    }

    public static SqliteDbContextOptionsBuilder UseSharpRacerMigrationsAssembly(this SqliteDbContextOptionsBuilder builder)
    {
        return builder.MigrationsAssembly("SharpRacer.Tools.TelemetryVariables.Data");
    }
}
