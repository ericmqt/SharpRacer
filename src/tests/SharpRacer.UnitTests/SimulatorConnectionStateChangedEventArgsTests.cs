namespace SharpRacer;
public class SimulatorConnectionStateChangedEventArgsTests
{
    [Fact]
    public void Ctor_Test()
    {
        var newState = SimulatorConnectionState.Open;
        var oldState = SimulatorConnectionState.Connecting;

        var args = new SimulatorConnectionStateChangedEventArgs(newState, oldState);

        Assert.Equal(newState, args.NewState);
        Assert.Equal(oldState, args.OldState);
    }
}
