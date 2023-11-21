﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Configuration;
internal class TelemetryVariablesDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TelemetryVariablesDbContext>
{
    public TelemetryVariablesDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TelemetryVariablesDbContext>();

        // For generating migrations, the path to the database doesn't matter
        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = "test.db"
        };

        builder.UseSqlite(csb.ConnectionString);

        return new TelemetryVariablesDbContext(builder.Options);
    }
}
