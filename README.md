# SharpRacer

![GitHub License](https://img.shields.io/github/license/ericmqt/SharpRacer) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ericmqt/SharpRacer/build.yml?style=flat) ![SharpRacer All Libraries Code Coverage](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2Fericmqt%2FSharpRacer%2Frefs%2Fheads%2Fcode-coverage%2Fmain%2Fjson%2FSharpRacer.all.json&query=%24.summary.linecoverage&suffix=%25&label=code%20coverage)

SharpRacer is a modern, high-performance .NET library for interacting with the [iRacing racing simulator](https://www.iracing.com/).

This project is under active development and is currently in a pre-release state.

## Getting Started

*TODO*

## Testing

SharpRacer includes an automated test suite for the SharpRacer library and the source generator. The tests can be run using the `dotnet test` command from the repository directory.

### Running the Test Suite

```
dotnet restore
dotnet test
```

### Generate Code Coverage Reports

This repository uses [ReportGenerator](https://reportgenerator.io/) as a dotnet local tool for generating code coverage reports. Before generating code coverage reports, you'll need to ensure it is installed for your local copy of the repository:

```
dotnet tool restore
```

Generate the code coverage reports using the `eng\GenerateCodeCoverageReport.ps1` PowerShell script:

```powershell
.\eng\GenerateCodeCoverageReport.ps1 -OutputDirectory .\artifacts\test -SharpRacer -SourceGenerator -Html
```

## Contributing

This project is not accepting contributions at the moment. I will begin taking contributions once the first release is completed and published.

## License

This project is licensed under the terms of the MIT license. See the [LICENSE](LICENSE) file for details.

## Support

If you like this project, found it useful, and want to support my work, you can [buy me a coffee](https://buymeacoffee.com/ericmqt). You don't have to, though. I promise I won't be mad if you don't.