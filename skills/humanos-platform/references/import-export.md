# Import / export, order, verification, backup

Docs: `Manuals/RestApi/Entity` (Import, Direct Import), `Manuals/SnapshotManagement`, `Manuals/ProductPackage`, `Setup/DataMigration`. JSON shapes: `import-shapes.md`.

## Envelope rules (`directimport`, UI Import)
- PascalCase keys exactly as the platform export writes them (server also reads lower-case).
- One file = one collection. Children (model `Fields`/`Actions`/`Views`, stream model `Fields`/`NotificationSources`, snapshot config `Segments`) nest inside the parent's `Fields`.
- **Ids decide create vs update.** Same `Id` = update in place (children survive). Generate each Id once, keep in git forever. Re-import is idempotent, also after failure. Never regenerate Ids, never delete+recreate.
- Always `"LifeCycleState": 1`. Strip audit fields (`CreatedAt, CreatedBy, LastChangedAt, LastChangedBy, Revision`); keep `TimeStamp: null`.
- Enum-typed model fields by name, custom enums integers, `LifeCycleState` number. Empty GUID `00000000-0000-0000-0000-000000000000`.
- **UTF-8 without BOM.** PS 5.1: read `Get-Content -Raw -Encoding UTF8`, write `[System.IO.File]::WriteAllText($p,$s,(New-Object System.Text.UTF8Encoding($false)))`, send `[System.Text.Encoding]::UTF8.GetBytes($json)`.

## Ways to import
| Way | Caveat |
|---|---|
| REST `directimport` | push one change; read `state`, then verify |
| REST `import` (multipart) | `bDeleteMissingObjects=true` **permanently deletes** non-built-in entities of the collection missing from the file |
| Web UI list > Import | SystemAdmin |
| Snapshot restore | "Delete Missing Objects" deletes the same way; DB dump first |

## Dependency order
Curated files (source of truth): `src/Platform/NN-<topic>.json`. Export (diff/backup; never hand-edited; commit only without customer data): `src/Platform/Export/<EntityName>/<Name>.json`.
`00-datatypes` -> `10-models` (abstract first, then concrete, with fields/actions/views) -> `20-streammodels` -> `30-reportschemas` -> `40-settings`/tasks/notification sources -> `50-navmenu` -> `60-languagetext`. Forward references fail (`ReferencedCollectionId`, model ids, `StateModelId`); mutual model references: import twice. New field on existing model/stream creates its column without restart. Changed *generated* views do not auto-update.

## Verify (never trust `Succeeded`)
1. `queryentitylist` with `WithChildren: true`: fields/actions/views present.
2. Query the new model's own collection: `totalCount: 0` without `reason` proves the table exists. `Table '...' doesn't exist` = earlier schema init failed: `gotchas.md` (third import).
3. `ReadOnlyView` models: always query (master model first).
4. Check `LifeCycleState` of everything imported (Draft inert).
5. Create one entity per role; check list, details, menu.

## Full instance export (diff/backup)
No export-all button; REST `export` throws NullReferenceException on tested builds. Build from queries:
1. Per configuration collection: `queryentitylist`, `MainFilter: entity.getValue<bool>("BuiltIn") == false`, `WithChildren: true`, page `Count`/`Offset` to `totalCount`.
2. Collections: `Model`, `DataType`, `StreamModel`, `Setting` (not `PlatformVersion`/`ProductVersion`), `ReportSchema`, `Task`, `PipelineTemplate`, `NotificationSource`, `SnapshotConfig`, `Language`, `LanguageText`, plus `NavMenu`/`Page` if customised.
3. Convert REST entity (camelCase `collectionId, id, fields`) to export shape: PascalCase, nested children, audit fields dropped, `LifeCycleState` name to number, `ExportedBy/ExportedAt: null`.
4. Layout `src/Platform/Export/<EntityName>/<SafeName>.json`, one entity per file, `LanguageText.json` grouped; sorted by `Name` then `Id`. Invalid file-name chars -> `_`; duplicates disambiguated with first 8 id chars.
5. Restore one entity: post its file to `directimport`. Files of entities deleted on the instance are not removed: check `git status`.
6. Keep **out** of git: licenses, certificates, secrets, tokens, customer data, notification **rules** and **channels** (installation specific; ship only events), instance-local test objects.
7. `Setting` values holding JSON (dashboards, 2.2 menu): patch as **text**, only validate; re-serialising rewrites the whole line.
Diff two exports before/after a deploy (no diff = in sync). Curated `src/Platform/` files stay the source of truth: never overwrite from an export.

## Snapshots, packages
- Snapshot (`SnapshotConfig` segments with `ModelId`, `FilterQuery`, `SegmentOrder`) is **not a full backup**: DB dump, document store, configuration folder (`appsettings.json`, `Secrets`) first.
- Restore matches stream fields strictly by `Id`. Settings segment must exclude `PlatformVersion`/`ProductVersion`.
- Product packages are vendor zips; update order: read ChangeLog, backup, platform installer, **then** product package. Neither reversible: backup is the way back.
- **Update 2.1 to 2.2/2.3:** users log in again; stream retention (>0) deletes data after 2.2, review first; **2.3:** earlier API tokens invalid, menu moves to `NavMenu`, dynamic columns NOT NULL.
