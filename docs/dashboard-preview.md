# Dashboard preview, 18 October 2026

Status: interactive design only. Application implementation awaits user confirmation.

The linked Claude artifact requires sign-in and could not be inspected. The installed UI/UX Pro Max skill informed this independent preview; it is not claimed to reproduce the linked artifact. Its search results included unrelated marketing layouts, which were discarded in favor of a desktop application shell, native Windows typography, readable tables, restrained blue selection, and paired light/dark colors.

## Navigable preview

Overview, Cleaner, Disk Map, Apps & services, Security, Health & updates, Privacy, Automation, Recovery & history, and Advanced tools. All metrics, file inventories, and security findings are synthetic. Secondary maintenance controls explain intended workflows; they do not implement operating-system actions.

## Disk Map requirements

- Enumerate each supported local volume separately, with used/free/capacity and scan completeness.
- Navigate an arbitrary-depth directory hierarchy. Show every accessible directory, subdirectory, and file, not merely category summaries.
- Synchronize directory tree, current-folder size table, breadcrumbs, and file treemap. Provide explicit Back and Up actions.
- Aggregate directory sizes from the inventory. Display individual sizes, extensions, file categories, file counts, and share of the current folder. Sort by size or name.
- Select treemap files to inspect their full path, type, and size; provide an open-containing-folder action.
- Separate logical size from allocated bytes. Explain hard-link accounting, sparse/compressed files, inaccessible items, metadata, and discrepancies with volume usage.
- Do not traverse reparse points or hydrate cloud placeholders by default. Allow cancellation and progressive scan results.
- Real implementation must virtualize large trees/tables, keep traversal off the UI thread, and provide keyboard equivalents for visual selection.

The preview demonstrates C:, D:, E:, five-level nesting, empty folders, size/name sorting, breadcrumbs, a recursive proportional treemap, file selection, and category totals. It does not scan disks, calculate allocated bytes, or delete files.

## Approval boundary

User reviews the interactive design before application implementation. Confirmation applies to this direction and the phased implementation plan, not permission to clean the current machine or disable services.
