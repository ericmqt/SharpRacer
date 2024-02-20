namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal interface ICliCommand<TSelf, THandler, TOptions>
    where TSelf : ICliCommand<TSelf, THandler, TOptions>
    where THandler : class
    where TOptions : class
{
}
