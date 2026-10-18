# Implementation ledger — docs/implementation-plan.md

User approved Aurora Light and implementation on 2026-10-18. Branch: codex/aurora-alpha.

First increment: native WPF app, read-only scanner, CLI/report export and inventories. No claim of milestone A completion until every A acceptance gate is met.

- Ruling: use the existing empty repository on a new branch; no committed base exists for a worktree. Preserve planning files. Git author identity remains unset; do not invent one.
- Ruling: initial scanner caps metadata at 100,000 entries and reports Limited. This is a development increment, not the planned million-file/indexed release. SQLite indexing and large-corpus qualification remain required.
- Interfaces: GUI and CLI consume the same scan results; report serialization omits parent cycles. Inventory adapters are read-only and explicitly report errors. No broker in this increment.
- Toolchain: .NET SDK 10.0.402, official Microsoft ZIP, SHA512 verified by bootstrap command.
- Original code: GPL-3.0-or-later. No competitor code/assets or REA runtime dependency.

## Delivered increment

- Native WPF Aurora Light shell with ten sections, real volume overview, no automatic scans.
- Bounded cancellable scanner, directory/file drilldown, treemap, file-type totals, JSON/CSV reports and CLI.
- Aged TEMP metadata preview; no mutation. App/startup/process/service/task/basic hardware inventories. Defender provider status only.
- Self-contained unsigned win-x64 package in `artifacts/PCSteward-win-x64`; source plus license/build scripts in repository. Package uses .NET 10.0.13 runtime; SDK 10.0.402.

## Verification, 2026-10-18

- Initial seven fixture tests failed against stubs, then passed. Review added stale-response and incomplete-directory tests; both failed before their fixes. Final 9/9 core tests pass.
- Release solution build: zero warnings/errors. Native WPF harness renders all ten sections and tests tree navigation/expansion persistence; passes. Images in `artifacts/ui-check` were inspected, and button text contrast/background rendering corrected.
- Independent review found three P2 issues: tree expansion loss, stale inventory results, and unvisited folders marked Read. Fixed in this increment; tree tested in native harness, other two covered by core regressions.
- CLI integration: generated fixture, two entries, 14 bytes, Complete, local JSON export.
- Packaged native application launched and reported responding with the expected window title. An initial sandbox-only process lookup falsely appeared to indicate exit; elevated lookup confirmed running. Duplicate diagnostic windows closed; one retained.
- `git diff --check` passes. No commit/push: Git author identity was not supplied. No production/user-data mutation tests were performed.

## Next work; no milestone-completion claim

Persistent indexed scanning, allocated/unique accounting, more inventory evidence, million-file/performance and adversarial filesystem coverage, DPI/screen-reader/high-contrast checks and full native interaction tests remain Phase 1 gates. Phase 2 cleanup/recovery and later CVE matching, scans, automation and broker are pending. Full premium feature parity is not delivered.

Runtime license/notices copied into the local package. Signing, SBOM, distribution audit and public release are pending. No additional scope decision is needed to continue the already-approved local roadmap; service-dependent milestone E retains its separate scope/budget review.
