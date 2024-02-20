using System.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal interface IDatabaseFileNameProviderCommand
{
    string GetDatabaseFileName(ParseResult parseResult);
}
