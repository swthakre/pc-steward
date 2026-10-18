# Contributing

PC Steward is an early Windows-only development project. Please describe proposed changes in an issue before starting a large feature. Current priorities and safety requirements are in [the implementation plan](docs/implementation-plan.md).

## Development setup

1. Install Git and the .NET SDK version in `global.json` on Windows 11 x64.
2. Clone the repository and create a feature branch.
3. Run `powershell -File scripts/build.ps1` to restore, build and run both test harnesses.
4. Run `powershell -File scripts/build.ps1 -Publish` to make a local unsigned portable build.

The tests are executable fixture harnesses, not `dotnet test` projects. Core tests use an isolated temporary directory and a junction fixture. Native UI tests render WPF controls offscreen. Never run destructive tests against personal files or a production system.

## Pull requests

- Use focused changes and Conventional Commit subjects, such as `feat(storage): add scan coverage filtering` or `fix(ui): preserve folder selection`.
- Explain the user-visible problem, behavior, verification and remaining limitations.
- Add meaningful regression tests for scanner, concurrency, security or recovery changes.
- Keep a normal-user process boundary. Never add automatic deletion, service termination, elevation, telemetry or inventory uploads without an explicit reviewed design.
- Preserve incomplete/unavailable states. Do not replace missing measurements with healthy/zero results.
- Do not commit local reports, binaries, SDKs, credentials or private filesystem paths.
- Update licensing/provenance records for any imported source, rules, icons or dependencies. Competitor feature descriptions do not authorize copying their code or assets.

Contributions must be your own work or supplied with compatible permission. By submitting a contribution, you agree to license it under GPL-3.0-or-later. No contributor license agreement is currently required.
