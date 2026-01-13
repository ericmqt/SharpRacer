using SharpRacer.Interop;

namespace SharpRacer.Commands;

public interface ISimulatorNotifyMessageSinkObserver
{
    void OnMessageSent(SimulatorNotifyMessageData message);
}
