# base-transport

A small .NET library providing a simple transportation layer abstraction and credential support.

Status: Work in progress.

Prerequisites
- .NET SDK 9.0 or later (verify with `dotnet --version`).
- A supported IDE such as JetBrains Rider or Visual Studio / VS Code with the C# extension.

Quickstart

1. Restore and build the solution:

```bash
dotnet restore
dotnet build base-transport.sln --configuration Debug
```

2. Run the tests:

```bash
dotnet test
```

Notes
- `base-transport` is implemented as a class library. There is no standalone executable; run the tests or reference the project from an application to exercise the library.
- Key source files:
  - `base-transport/TransportationLayer.cs`
  - `base-transport/TransportationLayerCredentials.cs`
  - `base-transport-tests/TEST_TransportationLayer.cs`

Project layout
- `base-transport/` — main library project
- `base-transport-tests/` — unit tests
- `base-transport.sln` — solution file

Opening the project
- Rider: open `base-transport.sln`.
- VS Code: open the workspace folder and install the C# extension.

Contributing
1. Fork the repository.
2. Create a feature branch.
3. Run tests locally (`dotnet test`).
4. Open a pull request with a clear description.

License
Add a `LICENSE` file to the repository to indicate the project license. If no license is present, assume "All rights reserved".

Contact
For questions or issues, open an issue on the repository.

