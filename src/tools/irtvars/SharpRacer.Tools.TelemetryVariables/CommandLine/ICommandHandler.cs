namespace SharpRacer.Tools.TelemetryVariables.CommandLine;

internal interface ICommandHandler
{
    Task<int> ExecuteAsync(CancellationToken cancellationToken);
}

internal interface ICommandHandler<TOptions> : ICommandHandler
    where TOptions : class
{
    TOptions Options { get; }
}
