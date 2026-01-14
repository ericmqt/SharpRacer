using SharpRacer.Interop;

namespace SharpRacer.Commands;

public class ObservableMessageSink : ISimulatorNotifyMessageSink
{
    private readonly ISimulatorNotifyMessageSinkObserver _observer;

    public ObservableMessageSink(ISimulatorNotifyMessageSinkObserver observer)
    {
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
    }

    public void Send(SimulatorNotifyMessageData message)
    {
        _observer.OnMessageSent(message);
    }
}
