# Research record and evidence limitations

Prepared 18 October 2026 (Asia/Calcutta). Scope: public open-source Windows maintenance suite with paid-product feature coverage and a dedicated disk map. Research used official product pages, documentation, repositories, Microsoft API references and an actual REA run. Vendor marketing establishes advertised capability, not measured efficacy.

## Findings that change the project

1. The comparison is multiple products and services, not a single cleaner. VPN infrastructure, recovery engines, driver catalogs and human support cannot be supplied merely by recreating screens.
2. BleachBit has no paid feature tier; its code and rules have explicit copyleft licensing.
3. Modern WinDirStat extends beyond a treemap. The roadmap retains its advanced feature families while starting with reliable local analysis.
4. Supported Windows facilities already cover many maintenance tasks. The proposed suite coordinates these facilities with better evidence, scope and recovery.
5. Neither a service's name nor its inactivity establishes it is safe to stop. The proposed automation is explicit and bounded.
6. REA can provide shipped-artifact evidence, but it is not a cleaner framework or permission to copy third-party proprietary code.

## Claim ledger

All web observations are as of the research date; pages may change. “Reported” means the publisher documents it; “observed” means a local/tool fact; “proposed” means our engineering judgment.

| Question | Finding / status | Evidence and limit |
|---|---|---|
| Current CCleaner baseline | Reported: 7.12.1526, 29 September 2026 | [Release history](https://www.ccleaner.com/ccleaner/version-history/); direct page corrected an older search result. Feature availability still varies by tier/architecture. |
| Paid suite boundary | Reported: core paid features plus Recuva, Speccy, Kamo and human support across bundles | [Plans](https://www.ccleaner.com/ccleaner/plans); not all capabilities belong to the main executable. |
| Sensitive cleaning scope | Reported: Custom Clean includes AI-related data categories | [Custom Clean 7](https://support.ccleaner.com/articles/en_US/Master_Article/using-custom-clean-in-ccleaner-7); proposed design treats histories/models as valuable opt-in data. |
| Driver parity | Reported: maintained driver catalog and matching/recovery workflow | [Driver Updater](https://support.ccleaner.com/articles/en_US/Master_Article/what-is-driver-updater); no independent access rights or equivalent catalog established. |
| Privacy bundle | Reported: tracking/fingerprint controls and private connection | [Kamo](https://www.ccleaner.com/kamo); infrastructure/platform parity not tested. |
| Recovery bundle | Reported: deleted-file recovery with professional extras | [Recuva](https://www.ccleaner.com/recuva); distinct from restoring our own quarantine. |
| BleachBit baseline | Reported: Windows stable 6.0.4; no paid Pro tier | [Download](https://www.bleachbit.org/download/windows), [cost](https://www.bleachbit.org/features/cost); published version-display caveat says it can show 6.0.3. |
| BleachBit configuration | Reported: extensible cleaners, CLI, exclusions and cookie preservation | [Features](https://www.bleachbit.org/features), [preferences](https://docs.bleachbit.org/doc/preferences/), [cookie manager](https://docs.bleachbit.org/doc/cookie-manager/); keeping cookies does not protect every localStorage/IndexedDB store. |
| Rule execution boundary | Reported: CleanerML supports powerful structured and executable actions | [CleanerML](https://docs.bleachbit.org/cml/cleanerml/); proposed importer deliberately supports less than the complete action vocabulary. |
| Ashampoo baseline | Reported: WinOptimizer Pro 29 | [Overview](https://www.ashampoo.com/en-us/winoptimizer), [details](https://www.ashampoo.com/en-us/winoptimizer/detail); current pages supersede older version-28 search results. |
| Ashampoo legacy modules | Reported in older manual; current parity partly unresolved | [Official manual PDF](https://support.ashampoo.com/hc/en-us/article_attachments/26072752572178); do not claim every older utility exists unchanged in Pro 29. |
| WinDirStat baseline | Reported: 2.9.3, released 11 October 2026 | [Official site](https://windirstat.net/); Firecrawl response was cached 17 October 2026, not a fresh origin fetch on the 18th. |
| WinDirStat workflows | Reported: expanded analysis, reports, monitoring and recovery | [Reading results](https://github.com/windirstat/windirstat/wiki/Reading-Results), [scanning](https://github.com/windirstat/windirstat/wiki/Scanning); these linked guides are implementation follow-ups, not hands-on validation. |
| Local REA use | Observed: successful resource inspection of installed WinDirStat | Evidence summary below. No scanner algorithm, runtime behavior or premium feature was established by that operation. |
| License compatibility | Reported upstream licenses; proposed GPL-3.0-or-later application | [BleachBit README](https://github.com/bleachbit/bleachbit/blob/master/README.md), [WinDirStat license](https://github.com/windirstat/windirstat/blob/master/LICENSE.md), [REA license](https://github.com/morluto/rea/blob/main/LICENSE). Review imported files individually. |
| Patent concern | Vendor asserts patent 8,463,758 covers specified technology | [CCleaner product page](https://www.ccleaner.com/ccleaner?source=cpc). Search-index text supported the statement; a direct fetch returned a JavaScript gate. No claims analysis or jurisdiction-specific conclusion performed. |
| Native stack | Observed SDK 9.0.316 locally; proposed .NET 10 LTS | [.NET support](https://dotnet.microsoft.com/en-us/platform/support/policy), [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/). SDK acquisition remains implementation work. |

## Microsoft evidence and design implications

| Source | Evidence / proposed consequence |
|---|---|
| [Storage Sense](https://learn.microsoft.com/windows/configuration/storage/storage-sense) | Supported retention and schedules; edition-dependent policy controls. Our scope defaults preserve personal/cloud data. |
| [Startup configuration](https://support.microsoft.com/en-gb/windows/experience/startup-boot/configure-startup-applications-in-windows) | Supported user management surfaces; use these before undocumented registry manipulations. |
| [Service enumeration](https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-enumservicesstatusexw) | Restricted access can affect completeness. Display permission/coverage limits. |
| [Service security](https://learn.microsoft.com/en-us/windows/win32/services/service-security-and-access-rights) | Configuration permissions are sensitive. Do not loosen ACLs to enable an optimization. |
| [Working-set information](https://github.com/MicrosoftDocs/win32/blob/docs/desktop-src/psapi/working-set-information.md) | Working-set manipulation is not evidence of user-visible speed improvement. No routine RAM purge feature. |
| [WinSxS cleanup](https://learn.microsoft.com/windows-hardware/manufacture/desktop/clean-up-the-winsxs-folder) | Use Windows servicing; exclude ResetBase from ordinary maintenance because of rollback consequences. |
| [Optimize-Volume](https://learn.microsoft.com/powershell/module/storage/optimize-volume) | Operation varies by media; don't force the same defrag operation onto every device. |
| [Storage reliability](https://learn.microsoft.com/en-us/powershell/module/storage/get-storagereliabilitycounter?view=windowsserver2025-ps) | Some counters are absent; show unknown instead of a fabricated healthy value. |
| [Defender](https://learn.microsoft.com/en-us/defender-endpoint/use-powershell-cmdlets-microsoft-defender-antivirus) | Delegate malware scanning/status to a security engine. Do not infer malware from optimization heuristics. |
| [WinGet](https://learn.microsoft.com/en-us/windows/package-manager/winget/upgrade) | Update coverage, pins and source semantics must be explicit. |
| [Broker model](https://learn.microsoft.com/en-us/windows/win32/secauthz/administrator-broker-model) | Separate standard-user UI from administrative helper. Authentication and scoped plan validation are additional design requirements. |

The old English Microsoft registry-cleaner policy URL redirected during research. We do not cite that redirect as a current policy statement. Excluding generic registry speed claims is our evidence/risk decision.

## Actual REA investigation

Read the official README, license, CLI guide and REA skill from `morluto/rea`. The skill says ordinary source-repository analysis should use normal source tools; binary/runtime questions are the appropriate REA use case. [REA workflow](https://github.com/morluto/rea/blob/main/.agents/skills/reverse-engineer-anything/SKILL.md), [CLI](https://github.com/morluto/rea/blob/main/docs/cli.md).

The official npm package resolved to **rea-agents 6.5.0**. Acquired that pinned version in a workspace-specific npm cache with installation scripts disabled. Used the CLI directly; did not register an MCP server, alter Codex configuration or install a native decompiler. Node 24.18.0 was present. A restricted-network lookup stalled; it was cancelled and the approved network-enabled lookup succeeded.

Ran `rea inspect-pe-resources` against the installed WinDirStat executable, using the command's inspected help contract and JSON output. Target version reported by Windows file metadata: **2.9.3.2253**.

| Field | Observed value |
|---|---|
| REA version | 6.5.0 |
| Operation | `inspect_pe_resources` |
| Result | `ok: true` |
| Artifact length | 2,910,384 bytes |
| Format | PE32+; machine value 34404 (0x8664) |
| SHA-256 | `c6925752fe688aa72115723f23dbb95a7e1c55f094a7d7f30acb5f509a38045b` |
| Resource coverage | complete for this operation; 75 examined entries, 32 resources |
| Evidence ID | `ev_6eb6a0fe79f2b2e57d2895ceee2657f5956c7e3dc6882816bfa6610e91afe786` |

Full local evidence is in `research/private/windirstat-pe-resources.json`, intentionally ignored by Git because it includes local paths. No target binary or extracted artwork was copied into the application. PE resources are mostly opaque to this operation; resource relationships do not establish actual UI behavior. No native scanning function was decompiled. No CCleaner or Ashampoo paid feature was unlocked, executed or reverse engineered during this research.

Next useful REA investigation, if needed: a bounded question about a specific scanning or allocation-accounting behavior, with pinned artifact digest, selected native provider and source comparison. Do not install a decompiler without need or represent metadata inspection as full reverse engineering. Available WinDirStat source may answer the question more directly.

## Research completeness and cost

Coverage includes the requested products' major advertised feature families, licensing, platform APIs, additional maintenance candidates and safety boundaries. This is not a claim to have discovered every possible PC optimization. Product internals, all cleaner definitions, hosted-service contracts, every hardware adapter and legal freedom to operate remain unverified.

Firecrawl reported one credit for the WinDirStat page scrape. Other web/GitHub tool costs were not reported; total research cost is unknown. No paid autonomous research job or data-provider agreement was started.

Collection stopped at a concrete, reviewable local-product design with explicit advanced/service gaps. Product code, builds, cleanup rules, installed tasks and public releases remain pending design approval.
