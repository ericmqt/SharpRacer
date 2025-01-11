namespace SharpRacer.Internal.Connections;
internal interface IConnectionAcquisitionWorker
{
    TimeSpan DataReadyWaitTimeout { get; set; }
    Thread Thread { get; }

    void Start();
}
