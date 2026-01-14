namespace SharpRacer.Internal.Connections;
public class ConnectionCancellationTokenSourceTests
{
    [Fact]
    public void Ctor_Test()
    {
        var cts = new ConnectionCancellationTokenSource();

        Assert.NotEqual(default, cts.Token);
    }

    [Fact]
    public void Cancel_Test()
    {
        var cts = new ConnectionCancellationTokenSource();

        Assert.False(cts.Token.IsCancellationRequested);

        cts.Cancel();

        Assert.True(cts.Token.IsCancellationRequested);
    }

    [Fact]
    public void CreateLinkedTokenSource_Test()
    {
        using var externalCancellationTokenSource = new CancellationTokenSource();

        var cts = new ConnectionCancellationTokenSource();

        using var cancelSourceResult = cts.CreateLinkedTokenSource(externalCancellationTokenSource.Token);

        Assert.NotEqual(cts.Token, cancelSourceResult.Token);
        Assert.NotEqual(externalCancellationTokenSource.Token, cancelSourceResult.Token);
    }
}
