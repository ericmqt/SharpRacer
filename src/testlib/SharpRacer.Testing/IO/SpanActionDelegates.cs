namespace SharpRacer.Testing.IO;

public delegate void DataFileHeaderSpanAction(ref DataFileHeaderSpan spanAction);
public delegate void DiskSubHeaderSpanAction(ref DiskSubHeaderSpan spanAction);
public delegate void TelemetryBufferHeaderSpanAction(ref TelemetryBufferHeaderSpan spanAction);
public delegate void TelemetryBufferHeaderArraySpanAction(ref TelemetryBufferHeaderArraySpan arraySpanAction);
public delegate void TelemetryBufferSpanAction(ref TelemetryBufferSpan spanAction);
