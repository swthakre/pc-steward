# Feature coverage and release roadmap

18 October 2026. **Every row is planned, not implemented.** This is a broad feature-family inventory based on public documentation; it is not a verified reproduction of every proprietary cleaner rule or algorithm.

Evidence baseline: CCleaner 7.12.1526; BleachBit 6.0.4; WinOptimizer Pro 29; WinDirStat 2.9.3. See [research evidence](research.md) for versions, source links, gaps and REA results.

Milestones: **A** = read-only alpha; **B** = safe-cleaning beta; **C** = maintenance 1.0; **D** = advanced local tools; **E** = external-service projects. These are release gates, not dates. Risk: **L** read-only; **M** reversible or disposable data with scope; **H** destructive/security-sensitive.

Reference key: C = [CCleaner plans](https://www.ccleaner.com/ccleaner/plans); CH = [CCleaner history](https://www.ccleaner.com/ccleaner/version-history/); B = [BleachBit features](https://www.bleachbit.org/features); BP = [BleachBit preferences](https://docs.bleachbit.org/doc/preferences/); A = [Ashampoo details](https://www.ashampoo.com/en-us/winoptimizer/detail); AO = [Ashampoo overview](https://www.ashampoo.com/en-us/winoptimizer); W = [WinDirStat](https://windirstat.net/). A reference identifies a feature family; our implementation and safety rules are proposed independently.

## Cleaning and privacy

| ID | Capability / reference | Delivery and scope | Risk / automation |
|---|---|---|---|
| F01 | System/app temporary files and caches — C, B, A | A preview; B tested categories, minimum age, exclusions and exact target review | M; schedule only approved rules |
| F02 | Browser traces/history — C, B, A | B cache; C history and session-aware rules | H for history; browsers closed; explicit opt-in |
| F03 | Cookies and keep-list — BP, A | C per-browser/profile domain selection | H; explain that cookies do not cover all login storage |
| F04 | Custom cleaning rules — B | B constrained declarative rules, validation and fixtures | M/H; no arbitrary executable rules |
| F05 | CleanerML/winapp2 coverage — B | D audited import of a restricted subset; license review per corpus | H; never auto-trust downloaded definitions |
| F06 | Multi-user cleanup — A | D administrator-selected users, per-profile exclusions | H; no silent scan of other users' private content |
| F07 | Windows component/update leftovers — A | C supported Windows cleanup handoff; D bounded integration | H; disclose lost rollback ability; never delete WinSxS manually |
| F08 | AI histories, agent caches, local models — CH | D separate categories with measured size and app ownership | H; conversations/models off by default |
| F09 | Database compaction/structured cleanup — BleachBit CleanerML | D offline transactional per-app adapters | H; backups, schema versions and app-closed checks |
| F10 | Shredding/free-space wiping — B | D expert-only limited capability after media analysis | H; no guaranteed SSD/flash erasure claim |
| F11 | Privacy settings/traces — A | C read-only review; D reversible documented settings | H; preserve security/update policy |
| F12 | Cloud storage/mail cleaning — CH | E OAuth integrations for Drive, OneDrive, Dropbox and Gmail | H; provider scopes, quotas, trash and audit; separate consent |

## Disk Map: a dedicated tab

| ID | Capability / reference | Delivery and scope | Risk / automation |
|---|---|---|---|
| F13 | Folder tree, treemap, file types and largest files — W | A synchronized views, size/color legends, navigation, export | L; no cleanup on scan |
| F14 | Multiple roots, drives and network paths — W | A local folders/drives; D explicit remote/MTP support | L; disclose inaccessible paths and disconnects |
| F15 | Logical versus allocated sizes — W | A distinguish accounting; D advanced filesystem fidelity | L; hard-link and sparse-file test corpus |
| F16 | Search/filter by name, size, type, owner — W | A name/size/type; D owner/ACL analysis | L; privacy-preserving local index |
| F17 | Duplicate content — W, CH, AO | C full-content proof and keep-original workflow | H deletion; never schedule deduplication |
| F18 | Snapshots, comparison, CSV/JSON and monitoring — W | A export; C snapshots/diffs; D bounded change monitoring | L; sensitive paths redacted from shared reports |
| F19 | Sunburst/flame views; storage age/cost analysis — W | D additional views and user-supplied cost assumptions | L; estimates labelled |
| F20 | Accelerated NTFS scanning — W | D after safe portable enumerator; benchmark and native isolation | L/elevated read; same coverage semantics |
| F21 | Recycle/quarantine and restore | B application recovery, C mature retention | M; actual freed space distinguished from moved files |

## Performance, maintenance and diagnostics

| ID | Capability / reference | Delivery and scope | Risk / automation |
|---|---|---|---|
| F22 | Health overview — C, A | A measured storage/resource findings; C trends | L; no fabricated health score |
| F23 | Startup apps/tasks — CH, A | A inventory; C reversible source-specific controls | H; preserve original settings/dependencies |
| F24 | Service/process manager — CH, A | A state/publisher/resource view; C approved graceful operations | H; protected categories and shared hosts guarded |
| F25 | Background/performance profiles — C, AO | D explicit chosen apps/services, restoration and benchmark | H; no auto-kill from reputation or inactivity |
| F26 | Application uninstall/leftovers — CH, A | C official uninstaller; D audited leftovers/batch workflow | H; publisher identity; leftover deletion separately approved |
| F27 | Temporary app removal/restoration — CH | D feasibility study; no generic rollback guarantee | H; installer/data availability and licensing constrain restore |
| F28 | Software updates — C, A | C WinGet-capable inventory/update previews, selected installs | H; respect pins, agreements, managed policies and reboot |
| F29 | Driver status/updates — C | C Windows/OEM guidance; D constrained update adapters | H; proprietary catalog parity remains E dependency |
| F30 | HDD/SSD optimization — A | C media-aware Windows analysis/maintenance | H; use Windows choices, avoid duplicate schedules |
| F31 | Hardware and drive health — A, Speccy bundle C | A basic inventory; C supported reliability counters; D sensors | L; unsupported readings shown as unknown |
| F32 | Boot/crash/resource analysis — A | C events and trends; D opt-in advanced traces | L; clocks/workload differences documented |
| F33 | System/disk benchmarks — A | D bounded local reference tests | M; writes and heat disclosed; no default online upload |
| F34 | Firewall rules/network review — A | D inventory, conflict explanations, reversible selected edits | H; no automatic security/network weakening |
| F35 | Browser extensions/context menus — CH, A | C inventory/official settings; D supported controls | H; extension permissions and provenance, no remote ratings verdict |
| F36 | Fonts, associations, policies, desktop tweaks — A, legacy manual | D modular supported features; reconcile current parity | H; avoid undocumented unsupported tweaks |
| F37 | Automation, app-close triggers and notifications — C, B, AO | C task schedules; D bounded event triggers | M/H; specific opt-in, no overlapping work |
| F38 | Recovery from deleted/formatted media — Recuva, W, AO | D separate read-only imaging/recovery project | H; write recovered data elsewhere; success never guaranteed |
| F39 | Split/join/encrypt/decrypt, broken-link/ADS tools — AO/manual | D utilities; use reviewed cryptography, original formats | M/H; no custom cryptography or blanket ADS deletion |
| F40 | Backups, restore points and history — A | B per-item journal; C settings backups; D system integration | M; restore point is not full backup |

## Additional worthwhile capabilities

These are proposed additions, not claims about competitor parity.

| ID | Proposal | Why it belongs / constraints |
|---|---|---|
| F41 | Defender status and supported scans — C milestone | Distinguish security findings from optimization guesses; respect third-party AV and tamper protection |
| F42 | Windows Update status and supported repairs — C/D | Diagnose failed/pending maintenance; SFC/DISM repairs only on request with logs and restart disclosure |
| F43 | Storage-growth alerts and capacity forecasts — C | Identify the responsible folders from snapshots; use uncertainty rather than exact exhaustion predictions |
| F44 | Developer caches, containers, WSL and VM disk review — D | Potentially large storage gains; use owner tools, never delete container volumes or VHDX blindly |
| F45 | OneDrive/cloud-placeholder awareness — A/C | Avoid downloading cloud-only content during scan; dehydration is separate from cloud deletion |
| F46 | Battery/power and thermal-aware maintenance — C/D | Adapt work to device capability; no overclocking, undervolting or proprietary sensor driver initially |
| F47 | Backup posture and encryption/security status — D | Report backup age/BitLocker/security settings without collecting recovery keys or disabling protections |
| F48 | Network diagnostics — D | Identify connectivity/DNS/proxy problems using bounded tests; no generic “internet speed booster” |
| F49 | Rule health and regression reporting — B/C | Disable rules on incompatible app versions; opt-in redacted issue bundles and safe rollback of rulesets |
| F50 | CLI, portable mode and accessibility — A–C | Reproducible scans and broad usability; portable mode has explicit privilege limits |
| F51 | Security tab: installed software CVE checks — C | NVD/OSV identities and ranges, uncertainty, KEV priority and stale-source reporting; see security addendum |
| F52 | Evidence-based security reports — C | HTML/JSON/CSV, PDF via print; malware/vulnerability/posture separated, redaction and coverage included |
| F53 | Selected repository/SBOM security scans — D | Dependency identity/range matching without executing repository code; optional EPSS enrichment |

Security requirements are detailed in [Security and UI addendum](security-and-ui-addendum.md). F41 malware integration now appears in the Security page. Basic public advisory integration in C is not deferred to milestone E's commercial-service workstreams.

## Features that cannot be silently promised

| Requirement | Decision and reason |
|---|---|
| Kamo-like VPN and fingerprint protection | E: independent browser/privacy project plus VPN infrastructure. Not fulfilled by deleting cookies. [Kamo](https://www.ccleaner.com/kamo) |
| Huge driver catalog, community reputation, online benchmark comparisons | E: data rights, maintenance, abuse prevention and hosting required. Use independent sources or agreements. [Driver updater](https://support.ccleaner.com/articles/en_US/Master_Article/what-is-driver-updater) |
| 24/7 priority human support | Operating commitment requiring people and funding, not a software feature. Community support initially. |
| Generic registry cleanup/defrag, RAM booster, blanket service killing | Deliberately not adopted as performance features. No demonstrated benefit in this project and disproportionate failure potential. Narrow diagnosed repairs remain possible. |
| Reliable secure erase on every SSD, RAID, flash and backup | Cannot promise universal results from a file-level overwrite. Device-specific sanitization belongs in a separate verified workflow. |
| All premium behavior immediately | Not a truthful deliverable. Completion requires per-feature tests, compatible data services and documented exceptions. The initial release is not advertised as complete parity. |

## Parity accounting

Each implemented feature will track source/version, implementation status, dependency, supported OS/architecture, safety mode, license provenance, test ID and a demonstrable user journey. A setting that merely opens Windows Settings is recorded as a handoff, not an independently implemented engine. A mocked UI or an unsupported action does not count as delivered.

Remaining research gaps: complete cleaner/app coverage; precise current Speccy sensors/export behavior; old versus current Ashampoo utilities; architecture-specific differences; proprietary optimization internals and backend coverage. These do not block the safe local core, but they block any “100% premium parity” claim.
