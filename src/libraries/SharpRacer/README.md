# SharpRacer

### SharpRacer and iRacing SDK Types

#### Structures

| iRacing SDK | SharpRacer |
| ----------- | ---------- |
| irsdk_diskSubHeader | SharpRacer.Interop.DiskSubHeader |
| irsdk_header | SharpRacer.Interop.DataFileHeader |
| irsdk_varBuf | SharpRacer.Interop.TelemetryBufferHeader |
| irsdk_varHeader | SharpRacer.Interop.TelemetryVariableHeader |

#### Enumerations

| iRacing SDK | SharpRacer |
| ----------- | ---------- |
| irsdk_CameraState | SharpRacer.CameraState |
| irsdk_CarLeftRight | SharpRacer.CarLeftRight |
| irsdk_EngineWarnings | SharpRacer.EngineWarnings |
| irsdk_Flags | SharpRacer.RacingFlags |
| irsdk_PaceFlags | SharpRacer.PaceRacingFlags |
| irsdk_PaceMode | SharpRacer.PaceMode |
| irsdk_PitSvFlags | SharpRacer.PitServiceOptions |
| irsdk_PitSvStatus | SharpRacer.PitServiceStatus |
| irsdk_SessionState | SharpRacer.SessionState |
| irsdk_TrkLoc | SharpRacer.TrackLocationType |
| irsdk_TrkSurf | SharpRacer.TrackSurfaceType |
| **Commands** | |
| irsdk_BroadcastMsg | SharpRacer.Commands.SimulatorCommandId |
| irsdk_ChatCommandMode | SharpRacer.Commands.ChatCommandType |
| irsdk_csMode | SharpRacer.Commands.CameraTargetMode |
| irsdk_PitCommandMode | SharpRacer.Commands.PitCommandType |
| irsdk_RpyPosMode | SharpRacer.Commands.ReplaySeekOrigin |
| irsdk_RpySrchMode | SharpRacer.Commands.ReplaySearchMode |
| irsdk_TelemCommandMode | SharpRacer.Commands.TelemetryCommandType |
| irsdk_VideoCaptureMode | SharpRacer.Commands.VideoCaptureCommandType |