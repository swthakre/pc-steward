# Security tab, UI direction and public distribution

18 October 2026. User-requested design extension. This document specifies proposed functionality; no security scan or production implementation has run.

## Interface direction

A native Windows sidebar with Overview, Cleaner, Disk Map, Apps & Services, Security, Health & Updates, Privacy, Automation, Recovery & History, and Advanced Tools. Apps & Services groups the formerly separate startup/process pages into subviews. Security owns malware and vulnerability reporting; Health & Updates owns hardware and update execution, with cross-links between them.

Main pages use a restrained light/dark theme, readable tables, small evidence summaries and a visible review action. Selected findings expose details before mutation. The Disk Map combines a folder list and selectable treemap; keyboard/table alternatives remain mandatory. Security has Vulnerabilities, Malware, Protection and report export. The inline concept shows illustrative data only; it is not a scan of the user's computer and does not establish application readiness.

## Security scope

### Malware

Use Windows Defender's supported interfaces for quick, full and selected-path scanning, protection/signature status, detection history and remediation status. `Start-MpScan` initiates scans; `Get-MpThreatDetection` exposes active and historical detections. Attribute findings to the engine and distinguish active, remediated, historical, failed, cancelled, inaccessible and not scanned. Do not count every historical detection as a current infection. [Scan API](https://learn.microsoft.com/en-us/powershell/module/defender/start-mpscan?view=windowsserver2025-ps), [detection API](https://learn.microsoft.com/en-us/powershell/module/defender/get-mpthreatdetection?view=windowsserver2025-ps).

Defender remains Windows-managed proprietary software; the application integrates supported interfaces and does not redistribute its engine/signatures under GPL. Capability-test each PC. If Defender is unavailable or another antivirus is active, report the limitation and route to that provider's UI unless an authorized supported adapter exists. Do not disable another antivirus or tamper protection to run our scanner.

Starting an antivirus scan can trigger that antivirus's existing automatic remediation policy. Disclose this before scan initiation; our normal cleanup preview cannot override Windows Defender's independent behavior. Do not remove antivirus quarantine files with the general cleaner. Releasing a quarantined threat is an explicit security action, never ordinary file recovery.

### Vulnerabilities (CVE, not CVS)

Inventory installed software using source-specific read-only adapters: publisher, product/package ID, version, architecture, install channel and reliable evidence location. Portable applications and bundled components may be missed; disclose scope. Optional user-selected repository/SBOM scanning can inspect lockfiles and dependency manifests without executing build/install scripts. Unknown inventory remains unknown, not safe.

Normalize identity to ecosystem/package identifiers for open-source dependencies and carefully validated CPE/vendor identities for desktop applications. Match affected ranges with ecosystem-specific version comparators, then evaluate platform/configuration constraints and vendor fixes/backports. Windows patch assessment must account for build/revision and superseding updates; missing one historical KB alone is not proof of vulnerability.

| Source | Use | Boundary |
|---|---|---|
| [NVD CVE API](https://nvd.nist.gov/developers/vulnerabilities) | CVE descriptions, severity and CPE applicability | `.../rest/json/cves/2.0`, pagination, CPE matching; text search is discovery only. Feed enrichment/coverage may be incomplete. |
| [OSV API](https://google.github.io/osv.dev/api/) | Open-source package/version or commit advisories | `POST /v1/query` and `/v1/querybatch`; not a universal Windows-app malware database. |
| [GitHub Advisory Database](https://github.com/github/advisory-database) | Ecosystem advisories and GHSA/CVE aliases | Deduplicate shared records; preserve attribution and review status. |
| [CISA KEV official mirror](https://github.com/cisagov/kev-data) | Known exploitation in the wild | Priority signal, not evidence that this particular PC was attacked. |
| [FIRST EPSS](https://www.first.org/epss/) | Optional exploitation-probability prioritization | Separate probability from severity, applicability and local exposure; missing score stays unknown. |
| [Microsoft Security Update Guide](https://msrc.microsoft.com/update-guide) and vendor advisories | Platform-specific applicability and fixes | Initial research establishes source location; exact automated API/patch-matching contract requires implementation discovery. |

The NVD documentation was read via Firecrawl after a regular page fetch returned no content. CISA's main page returned 403; its official GitHub mirror was readable. OSV query documentation permits package name/ecosystem or PURL; use one identity route and do not send a version twice. Follow pagination and preserve withdrawn/rejected advisory state. [OSV query contract](https://google.github.io/osv.dev/post-v1-query/).

Finding classes: **affected-version match**, **potential match requiring review**, **not affected with evidence**, and **coverage unknown**. “Affected-version match” does not establish exploit execution or full reachability. Fuzzy display-name similarity never becomes a confirmed vulnerability automatically. Match confidence, CVSS severity, known exploitation, exposure and EPSS are separate fields, not an unexplained composite health score.

Prefer supported vendor fixes and exact package-manager updates. Updates remain reviewed actions; an advisory cannot authorize arbitrary downloads, shell commands, service termination or patch execution. No exploit proof-of-concept is run against the user's PC. Rescan after remediation and record whether the original finding actually resolves. Suppressions need a reason, scope and expiry; they must not delete the evidence.

### Protection posture

Read-only checks for antivirus availability, firewall profile state, supported security-update status and later supported encryption/Secure Boot checks. Distinguish unavailable, managed and disabled states; read no recovery keys. Configuration findings remain separate from CVEs and malware. A setting deviation is not proof of compromise.

## Data flow and privacy

Local inventory → identity normalization → advisory matching → applicability/confidence → prioritization → report → selected remediation → verification.

Cache bounded, versioned advisory data with source provenance, retrieval time, source modification date, schema version and integrity hash. Trust configured official endpoints over TLS; validate schemas and limits, verify publisher signatures when actually provided, and do not confuse a content hash with source authentication. Treat all advisory prose and URLs as untrusted data. Escape HTML output, neutralize CSV formula injection, and allow only validated web links.

Default offline scan uses the existing cache and clearly labels stale/missing intelligence. Enabling feed refresh downloads public data; it does not automatically permit uploading software inventory. Remote package queries are a separate opt-in, with disclosure that package names/versions reveal inventory. Do not upload file contents, executable samples, paths, secrets or private repositories. Hash reputation lookups and third-party multi-engine services would require another explicit privacy/terms decision. Never ship shared API secrets in a public repository.

Preserve per-source licensing for cached/exported data. The GitHub advisory dataset is CC-BY-4.0; CISA's mirror identifies KEV data as CC0. The application's GPL license does not relicense those datasets. Verify OSV's underlying source licenses, NVD notices and EPSS terms before redistribution or bundling. Query access alone is not a blanket data redistribution license. [GitHub data license](https://github.com/github/advisory-database/blob/main/LICENSE.md), [KEV licensing](https://github.com/cisagov/kev-data#licensing).

## Report contract

Exports: local HTML, JSON, CSV, and PDF via print initially. Each report contains run ID/time, consented scope, machine identifier redacted by default, inventory coverage, engine/signature state, advisory freshness, partial failures and the limitations of a no-findings result.

Each finding includes category, affected component, installed version, canonical identifiers/CVE/GHSA aliases, affected/fixed range, evidence, matching rule/version, confidence, severity with scoring version/source, KEV/EPSS when available, remediation source, reboot requirements, suppression state and post-fix outcome. Preserve separate records for the same CVE affecting different installed components, while grouping aliases of the same advisory within each component.

Malware records include engine, threat ID, detection timestamp, current remediation status and resource location with export redaction. CVE reports never imply a forensic determination of compromise. “No findings in this scope using these sources” is the strongest default clean-result language; it is not a certificate that the PC is safe.

## Delivery and verification

Milestone C / implementation Phase 4: Security page, Defender integration, selected installed-package vulnerability matching with NVD/OSV and KEV enrichment, read-only protection status, redacted local reports, freshness and uncertainty indicators. Baseline advisory access is part of core maintenance, not deferred with commercial external services. Offline operation remains useful.

Milestone D: selected-project/SBOM dependency scans, wider portable/embedded component coverage, richer vendor patch applicability, optional EPSS prioritization and adapters for additional security engines. No custom antivirus engine or kernel driver is proposed.

Required tests: similar names/different publishers, missing version, prerelease and inclusive/exclusive ranges, vendor backports, patched Windows build with old KB absent, conditional CPE configurations, duplicate aliases, rejected/withdrawn advisory, stale feed, pagination, throttling, offline refresh, disabled/passive antivirus, historical remediated threat, scan cancellation and failed remediation. Test report escaping/CSV safety and privacy exports. Use synthetic fixtures and controlled security test environments, not live malicious samples on the user's workstation.

## Public repository answer

Yes, public distribution is compatible with the proposed license strategy if the shipped content is ours or properly licensed. Use GPL-3.0-or-later for original application code, preserve upstream copyright/license notices, mark modifications, and provide the complete corresponding source/build material for distributed GPL binaries. A public repo is useful, but it must contain the source corresponding to each released binary, not just the newest code. [GPLv3](https://www.gnu.org/licenses/gpl-3.0.html), [GPL FAQ](https://www.gnu.org/licenses/gpl-faq.en.html).

REA-authored code is MIT; retain its notice if copying covered code. Merely using REA as a research tool does not force the output application to use MIT. BleachBit's source and cleaner rules are GPL-3.0-or-later; WinDirStat's project advertises GPL v3 or later. Review file-level exceptions and dependency licenses before integration. [REA license](https://github.com/morluto/rea/blob/main/LICENSE), [BleachBit licensing](https://github.com/bleachbit/bleachbit#licenses), [WinDirStat](https://windirstat.net/).

GPL does not grant permission to redistribute CCleaner/Ashampoo code, artwork, trademarks, proprietary catalogs or hosted services. Use original branding and authorized dependencies. Patent and contractual questions remain separate from copyright licensing; REA does not eliminate them. This is the intended compliance approach, not a legal clearance of an as-yet-unbuilt release. Before publication, perform a dependency/provenance and relevant legal review. No public repository has been created or published by this design update.
