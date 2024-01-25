namespace SharpRacer.Commands;
public enum ReplaySearchMode : ushort
{
    Start = 0,
    End = 1,
    PreviousSession = 2,
    NextSession = 3,
    PreviousLap = 4,
    NextLap = 5,
    PreviousFrame = 6,
    NextFrame = 7,
    PreviousIncident = 8,
    NextIncident = 9
}
