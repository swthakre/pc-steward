# Security and development status

This is an unsigned, read-only development alpha, not a production optimizer or antivirus. It never deletes files, stops processes/services, installs schedules, or requests elevation. Do not rely on it to establish that a PC is malware-free.

Scans enumerate metadata without opening file contents. Reparse/offline roots and children are skipped. They are not atomic filesystem snapshots; concurrent changes can affect results. The current bounded in-memory scanner stops at 100,000 entries and labels results Limited. Logical bytes count hard links by path; allocated space is not measured. Coverage and errors must accompany totals.

Reports contain local full paths. Review before sharing. No telemetry, inventory upload, advisory queries or automatic updates are present. Inventory publisher strings are unverified metadata.

Do not publish sensitive local reports or exploit details as public issues. A private security reporting channel and maintainer identity must be configured before public release.
