# Telemetry Variable Manager Tool

`irtvars` is a command-line application for collecting iRacing telemetry variables in a SQLite database, primarily for exporting variable data to be used with the source generator for generating strongly-typed telemetry variable instances.

## Background

The iRacing simulator exposes telemetry data represented by telemetry variables (e.g. the number of laps remaining, throttle position, or fuel level) that may be accessed by other applications for analysis, updating in-game overlays, etc. 

There is no documented listing of every telemetry variable the simulator might expose--it is expected that you will query the simulator for a list of the variables that are available in the session and go from there. As a developer this inevitably leads to dumping those variables for examination, then doing lookups at run-time by the variable name. To complicate things, the simulator does not expose all possible variables in every session (e.g. some variables may be exclusive to a particular car or set of cars), so dumping them all can be a little laborious.

This tool was created to maintain a definitive database of telemetry variables exposed by the simulator with the primary purpose of exporting the collected telemetry variable information for use by the source generator to create strongly-typed variables, facilitating significantly faster lookups, type-safe reads, and protection from reading a variable that isn't available in a particular session or telemetry file.