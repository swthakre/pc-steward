# Implementation plan

## Primary goal

Deliver an installable public open-source Windows application that safely cleans chosen disposable data, explains and visualizes disk usage, diagnoses performance issues, and performs specifically approved maintenance with verifiable outcomes and recovery.

This is a pre-build proposal. Read [the design](superpowers/specs/2026-10-18-pc-steward-design.md), [feature matrix](feature-matrix.md) and [research](research.md) before implementation. Phase scopes remain resumable independently; each phase produces a demonstration, tests and a short known-limitations record. No phase may label an unimplemented feature as working.

The feature matrix IDs and milestone assignments are normative acceptance scope. A phase summary does not silently remove a matrix commitment. Before declaring A/B/C/D/E complete, resolve every feature slice assigned to that milestone as implemented and tested, or obtain an explicit scope change and update both documents. “D feasibility study” rows require a documented feasibility result, not a claim that the eventual feature is implemented.

## Phase 0 — Evidence, APIs and release scope

**How this serves the primary goal:** establishes what will actually work on Windows and prevents copying unsupported product claims into the implementation.

Research baseline and initial REA artifact inspection are complete; deeper native investigation remains scoped follow-up. Pin product versions, dependency licenses and source provenance. Adopt the design after user review, settle the first supported Windows 11 build/test matrix, and record the initial release exclusions. Use REA for a specific shipped-artifact question when source/docs cannot answer it; use ordinary source inspection for open-source architecture.

Allowed API patterns to read before coding:

| Area | Documentation and concrete contract |
|---|---|
| GUI | [WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/): XAML `Window`, `DataGrid`, virtualized items, binding and commands |
| Enumeration | [Directory.EnumerateFileSystemEntries](https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.enumeratefilesystementries): use the overload with `EnumerationOptions`; wrap errors per directory and report coverage |
| File identity | [GetFileInformationByHandleEx](https://learn.microsoft.com/en-us/windows/desktop/api/winbase/nf-winbase-getfileinformationbyhandleex), [GetFinalPathNameByHandleW](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfinalpathnamebyhandlew): handles, final paths and identity checks; verify structs/flags from docs before interop |
| Recoverable shell operation | [IFileOperation::SetOperationFlags](https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifileoperation-setoperationflags): `FOFX_RECYCLEONDELETE`; verify outcome rather than assume success |
| Service discovery/control | [EnumServicesStatusExW](https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-enumservicesstatusexw), [ControlService](https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-controlservice): minimal rights, dependency/state handling |
| Signature evidence | [WinVerifyTrust](https://learn.microsoft.com/en-us/windows/win32/api/wintrust/nf-wintrust-winverifytrust): only return value zero indicates success |
| Privilege split | [Administrator broker model](https://learn.microsoft.com/en-us/windows/win32/secauthz/administrator-broker-model): `asInvoker` main app, separate `requireAdministrator` helper |
| Scheduled execution | [Task security contexts](https://learn.microsoft.com/en-us/windows/win32/taskschd/security-contexts-for-running-tasks): principal/run level/ACL and protected command location |
| App updates | [WinGet upgrade](https://learn.microsoft.com/en-us/windows/package-manager/winget/upgrade): preview with `winget upgrade`, target exact selected ID; inspect exit/results and source |
| Windows updates | [IUpdateSearcher](https://learn.microsoft.com/en-us/windows/win32/api/wuapi/nn-wuapi-iupdatesearcher): search/history; begin with status and Windows Settings handoff |
| Security | [Defender PowerShell](https://learn.microsoft.com/en-us/defender-endpoint/use-powershell-cmdlets-microsoft-defender-antivirus): `Get-MpComputerStatus`, `Start-MpScan`, capability-aware errors |
| Storage maintenance | [Optimize-Volume](https://learn.microsoft.com/powershell/module/storage/optimize-volume), [reliability counters](https://learn.microsoft.com/en-us/powershell/module/storage/get-storagereliabilitycounter?view=windowsserver2025-ps): analyze first, media-aware optimization, missing readings preserved |

Verification: confirm live signatures, current framework support and the actual local capability before adopting each adapter. Source links are not substitutes for reading the relevant reference sections. Anti-patterns: invented APIs, undocumented `StartupApproved` manipulation or `UsoClient` commands, automatic blanket update/install switches.

## Phase 1 — Native shell and read-only alpha

**How this serves the primary goal:** gives the user a real desktop application that can report useful facts without modifying the PC.

Acquire a pinned .NET 10 SDK, create the solution boundaries from the design, add GPL/provenance files and native accessible navigation. Implement domain findings, capability reporting, cancellation and an initial CLI. Read the WPF examples above and reproduce supported patterns with original UI.

Deliver Overview, Cleaner preview and Disk Map using real folder metadata; folder/file-type tables, largest items, original treemap, logical/allocated distinction, scan coverage and JSON/CSV export. The treemap must have a usable table alternative. Portable user-mode package first; no privileged helper required yet.

Also deliver the matrix's A inventory slices: installed-app/startup/task listing (F23), accessible service/process/publisher/resource information (F24), and basic hardware/storage inventory (F31). Access-denied and missing capabilities are explicit. Later phases add privileged coverage and mutation; initial inventories never need to stop processes or alter settings.

Verification: known fixture tree, Unicode/long paths, denied directory, changing/locked files, sparse files, hard links, junction loop, multiple roots, cancellation, malformed imported snapshots and bounded memory on a million-file synthetic corpus. Visually verify at 100/150/200% DPI, keyboard-only and screen reader. Anti-patterns: recursive link following, one UI object per file, storing every file body, fabricated scan results or dormant buttons presented as features.

Milestone **A**: installable read-only alpha. No cleaning claims.

## Phase 2 — Scoped cleanup, preview and recovery

**How this serves the primary goal:** turns identified disposable data into controlled, testable maintenance instead of risky bulk deletion.

Implement the declarative rule schema, built-in minimum-age temp/cache rules, exclusions, immutable plans, changed-target rejection, journal and recycle/quarantine adapters. Start in the current user's scope. Add restore conflicts, retention controls and actual reclaimed-space reporting. Adopt only audited rule definitions with provenance; independent rules are acceptable.

Documentation: file identity/shell APIs from Phase 0, [reparse points](https://learn.microsoft.com/en-us/windows/win32/fileio/reparse-points), [CleanerML concepts](https://docs.bleachbit.org/cml/cleanerml/) for compatibility boundaries, not unrestricted execution.

Verification: test protected roots, sibling-prefix paths, path swapping, reparse ancestors, rename between scan/delete, full quarantine volume, unsupported recycle target, failed/partial restore, app-running rule, crash at each journal transition, cancellation between items and retention purge bounds. The adversarial fixture must attempt to delete a protected sentinel and prove it remains unchanged. No mutation test runs against the developer's real user data.

Anti-patterns: permanent-delete fallback, claiming same-volume moves free space, arbitrary XML commands, unreviewed downloaded rules, assuming System Restore backs up files. Milestone **B**: usable safe-cleaning beta with a narrow, declared rule list.

## Phase 3 — Privileged broker and guarded system maintenance

**How this serves the primary goal:** enables administrative maintenance while limiting the damage possible from a compromised UI, rule or local file.

Create the authenticated typed IPC protocol, independent broker policy, plan binding/expiry, administrative installer location and audit records. Implement capability-gated service/startup inventory, selected reversible changes and supported Windows maintenance handoffs. Add dependency graph, protected service policy and graceful state transitions. Do not offer administrator operations from an unprotected portable install.

The broker renders its own independently validated action review and records consent in protected storage. UAC alone and UI-generated tokens are insufficient. Refuse differing requester/elevated SIDs in the first release. Register elevated schedules only through the broker's policy review; later tasks consume protected exact-scope policies. Extend Phase 1 inventories only where administrative reads are actually necessary.

Documentation: administrator broker model, Task Scheduler security, service security/control APIs. Read exact service/query/config APIs before adding each adapter. Approved operations must be enumerated in a versioned contract; command execution is not a generic broker endpoint.

Verification: standard-user attacker, forged/replayed plan, wrong SID/session, oversized IPC, editable policy file, substituted helper, link swap, unavailable UAC, interrupted broker, protected/critical/shared service, dependent services and outside modification before restore. VM snapshot rollback is mandatory for test scenarios. Anti-patterns: LocalSystem resident optimizer, broad ACL grants, arbitrary scripts, trusting UI approval flags, killing shared `svchost`.

## Phase 4 — Health, updates, automation and maintenance release

**How this serves the primary goal:** keeps the PC's condition visible over time and runs only the maintenance the user explicitly configured.

Include the user-requested Security page and F51/F52: installed-software vulnerability matching, NVD/OSV/KEV source adapters, explicit confidence/freshness and redacted reports. Follow [Security and UI addendum](security-and-ui-addendum.md) for consent, licensing, matching and adversarial tests. Malware detection and vulnerability matching are separate pipelines. Phase 5 adds F53 selected-project/SBOM coverage. These core public advisory sources are not postponed to the commercial services phase.

Add history/snapshot differences, storage-growth findings, app inventory, WinGet previews and selected updates, Windows Update status, Defender status/scans, drive counters, event-based diagnostics and media-aware maintenance. Add Task Scheduler registration, idle/AC/quiet-hour gates, byte/time limits, missed-run reporting, single-instance locks and meaningful notifications. Implement duplicates with final full comparison and manual deletion.

Complete remaining C slices explicitly: selected browser history and per-profile cookie rules (F02/F03), privacy review (F11), official application uninstall (F26), extension/context-menu inventory and supported settings handoffs (F35), settings backups (F40), plus cloud-placeholder handling (F45), maintenance power gates (F46), and rule regression safeguards (F49). These are all required for C even if another feature is more visually prominent.

Documentation: Phase 0 adapters plus [Storage Sense](https://learn.microsoft.com/windows/configuration/storage/storage-sense), [component cleanup](https://learn.microsoft.com/windows-hardware/manufacture/desktop/clean-up-the-winsxs-folder). Prefer existing Windows maintenance facilities; never manually clean Windows component directories.

Verification: absent WinGet, managed machine, passive/third-party antivirus, offline/metered network, unsupported sensors, sleep/resume, missed task, battery transition, concurrent task, locked browser, package pin/source ambiguity, failed update and reboot-required state. Capture actual CPU/I/O overhead and before/after workload results. Tests must not auto-accept vendor agreements or restart the test host.

Milestone **C**: maintenance 1.0 after the final release gate. This includes the safe local core, not every external premium bundle service.

## Phase 5 — Advanced local parity

**How this serves the primary goal:** expands useful capabilities without weakening the proven cleanup and privilege boundaries.

Implement independently reviewed modules from matrix milestone D: broader browser/rule compatibility, multi-user scope, NTFS acceleration, additional disk views, supported app/service profiles, crash/boot traces, selected tweaks, developer storage, firewall review and specialist file utilities. Separate recovery-from-media and secure-sanitization designs from ordinary undo; assess audited existing projects and their licenses before writing engines.

For each module, first reconcile the latest upstream feature documentation and compatibility constraints; write its acceptance contract, then build and demonstrate it. Arbitrary driver collection, arbitrary process injection and new kernel drivers are outside the approved core design.

Verification: per-module VM/hardware matrix, permissions and failure injection, behavior under managed policies and external concurrent changes, benchmark against declared fixtures. Recovery writes to another destination; SSD sanitization claims require device-specific evidence. Anti-patterns: broad hidden-tweak packs, malware verdicts from community ratings, unconditional benchmark uploads, home-grown cryptography.

Milestone **D**: advanced local suite; completion reported per feature, never by count of tabs.

## Phase 6 — External services and bundle parity

**How this serves the primary goal:** makes service-dependent requirements explicit so they cannot be mistaken for delivered local features.

Plan separate workstreams for cloud cleanup, optional reputation/catalog sources, privacy browser integration/VPN and hosted support. Requirements: provider terms/data licenses, sustainable cost, OAuth scope minimization, token storage/revocation, API quotas, privacy disclosures, operational monitoring and abuse prevention. Human support needs staffing; a source repository cannot fulfill a 24/7 support contract.

Documentation: actual provider APIs and terms must be selected and read during this phase; they have not been verified in this research. Current product descriptions establish demand, not an available implementation API. Never imitate or call undocumented proprietary vendor backends.

Verification: provider sandbox accounts, revoked/expired tokens, rate limits, trash/restore behavior, multi-account identity, interrupted operation, audit export and data deletion. No real cloud account mutation without explicitly selected targets. Milestone **E** requires separate scope/budget review. Local milestones remain fully usable without it.

## Phase 7 — Verify and publish each milestone

**How this serves the primary goal:** ensures users receive trustworthy binaries with source, measurable limitations and a tested recovery path.

Run this gate before publishing A, B, C or later releases, not just once at the end. Compare every claimed feature to its acceptance test and documented API; review all privileged/dangerous diffs; check source/dependency licenses and vulnerability reports; produce SBOM, source archive and reproducible build instructions. Pin toolchains, run unit/VM/UI tests, verify installer upgrade/uninstall and signing/update verification. Test new rules against golden fixtures before distribution.

Record OS/build/architecture/hardware, selected dataset, test results, performance measurements, failed/unsupported capabilities and manual checks. Required destructive-operation gates: zero writes outside approved roots, no unauthorized privileged operation, no silent permanent fallback, recoverable crash states and no sensitive default telemetry.

Public packaging: signed installer with protected broker plus standard-user portable variant. Signing identity and release location are user/maintainer decisions; until supplied, deliver clearly marked local test builds and checksums without claiming publisher trust. Do not publish an unsigned build as a signed production release.

Anti-patterns: treating compilation as end-to-end proof, skipping native UI checks, asserting all PCs become faster, claiming full parity while dependency/gap rows remain open.

## Completion ledger and practical scale

This is a multi-release desktop product, not a one-turn wrapper around Windows commands. Estimate effort after Phase 1 benchmarks and Phase 2 safety tests; external services and compatibility maintenance dominate uncertainty. Each session should leave a build/demo, focused verification, updated matrix and next-step handoff. Do not compress all phases into an unverified first release merely to claim completion.

Current deliverables: research, approved Aurora Light design, feature matrix and phased plan, plus the first native read-only development increment. See [build progress](build-progress.md) for exact delivered scope and verification. Milestone A is not complete: full indexed scans/accounting, inventory slices and qualification gates remain. Later phases, signing and public release remain pending.
