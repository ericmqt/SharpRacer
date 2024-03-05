using System.CommandLine;
using Microsoft.EntityFrameworkCore;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal interface IConfigureDbContextCommand
{
    void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider);
}
