# PC Steward — Aurora Light

[![Windows build and tests](https://github.com/swthakre/pc-steward/actions/workflows/ci.yml/badge.svg)](https://github.com/swthakre/pc-steward/actions/workflows/ci.yml)
[![License: GPL v3 or later](https://img.shields.io/badge/license-GPL--3.0--or--later-blue)](LICENSE)

Working name for a public, open-source Windows maintenance application. The name is provisional and has not been cleared for release.

PC Steward brings disk analysis and PC maintenance into an original Aurora Light desktop interface. Built with C#/.NET 10 and WPF, it starts with evidence-based, read-only discovery before adding reviewed cleanup and recovery features.

**Status: native Windows read-only development alpha.** Aurora Light was approved for implementation. This is the first increment, not the full premium-feature suite or completed milestone A. No PC cleaning, service changes, application removal, or scheduled maintenance is performed.

## Run

```powershell
git clone https://github.com/swthakre/pc-steward.git
cd pc-steward
powershell -File scripts/build.ps1 -Publish
.\artifacts\PCSteward-win-x64\Steward.App.exe
```

Prerequisites: Windows 11 x64 and the .NET SDK pinned in `global.json`. Generated packages are not committed to Git. No signed production release is currently available.

Local self-contained build: `artifacts/PCSteward-win-x64/Steward.App.exe`. Keep the entire folder together. It is unsigned and intended for local evaluation, not a signed production release. No separate .NET installation is required for that package.

Build from source with the pinned .NET SDK 10.0.402 on Windows: `powershell -File scripts/build.ps1 -Publish`. A project-local SDK at `.tools/dotnet` is used if present; otherwise install the pinned SDK from Microsoft. Application dependencies are Windows/.NET only. CLI: `dotnet run --project src/Steward.Cli -- scan "C:\selected-folder" report.json` (or `.csv`). CLI exports refuse to overwrite existing files.

## Working now

- Native Aurora Light dashboard and navigation, actual local volume capacity.
- Cancellable folder/drive metadata scans, nested directory tree, sortable file sizes/types, treemap, per-subtree extension totals and file details.
- JSON/CSV exports with scan coverage. Reports contain full paths; review before sharing.
- Read-only aged TEMP-file preview; no removal.
- Installed-app registry, startup, process, service, scheduled-task and basic hardware inventories with explicit coverage limits.
- User-triggered Microsoft Defender status query; no malware scan or vulnerability verdict.

## Known limits and next gates

- Metadata retained in memory, capped at 100,000 entries. Limited/cancelled/partial coverage is explicit. Indexed million-file scanning remains pending.
- Logical sizes only; hard links counted by path. Allocated-byte accounting and sparse-file qualification remain pending. Reparse/offline items are skipped; concurrently changing paths are not an atomic snapshot.
- Treemap shows up to 100 largest direct children plus an aggregate; the table contains all discovered children. Keyboard Enter opens table items. Tiny rectangles have table alternatives.
- Fixed Aurora Light theme; high contrast, full screen-reader and 150/200% DPI qualification remain pending.
- Cleanup/recovery, CVE matching, malware scans, updater, automatic schedules, privileged broker and advanced modules are not implemented. Their pages state this explicitly.
- No snapshot import is implemented. No third-party cloud services or telemetry.

Tests: `Steward.Tests` is a dependency-free fixture runner; `Steward.UiTests` constructs/renders the actual WPF views and checks tree navigation. Full VM, denied-path, race, sparse/hard-link, million-file, screen-reader and native end-to-end qualification remain release gates.

- [Design, architecture, security and licensing](docs/superpowers/specs/2026-10-18-pc-steward-design.md)
- [Feature coverage and parity roadmap](docs/feature-matrix.md)
- [Phased implementation and acceptance plan](docs/implementation-plan.md)
- [Research evidence and REA investigation](docs/research.md)
- [Security tab, UI direction and public distribution](docs/security-and-ui-addendum.md)

## Repository structure

```text
.github/                   Windows CI, issue and PR templates
src/Steward.App/            Native Aurora Light WPF interface
src/Steward.Core/           Scanner, coverage and report contracts
src/Steward.Windows/        Read-only Windows inventory adapters
src/Steward.Cli/            Command-line scanning and export
tests/Steward.Tests/        Core regression fixture runner
tests/Steward.UiTests/      Native WPF rendering/navigation checks
scripts/                   Reproducible build and local packaging
docs/                      Architecture, evidence and phased roadmap
```

See [CONTRIBUTING.md](CONTRIBUTING.md), [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md), and [CHANGELOG.md](CHANGELOG.md) before contributing. Use GitHub issues for non-sensitive bugs and feature requests.

Original source is GPL-3.0-or-later; see [LICENSE](LICENSE), [third-party notices](THIRD-PARTY-NOTICES.md), [security status](SECURITY.md) and [implementation ledger](docs/build-progress.md). No competitor source/assets were copied. Production binary release still requires signing/provenance/SBOM and completion of the applicable acceptance gates.
