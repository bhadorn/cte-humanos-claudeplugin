# Gotchas: symptom -> cause -> fix

Owner of every symptom/error-message item; topic files keep design rules only. Long form of deployment items: `Manuals/RestApi/DynamicModelDeploymentPitfalls`.

## Deployment and lifecycle
| Symptom | Cause | Fix |
|---|---|---|
| New entity fails `Could not find object [<ModelId>] in object space.` / `Object reference not set...` / `Sequence contains no matching element` | Model/ModelField/ModelAction/DataType still Draft; or field uses a built-in enum with no `DataType` row | `LifeCycleState: 1`, or `update {LifeCycleState:"Active"}` (no `activate` action on these); check `DataType` rows |
| Stream field activation `Could not find object [<stream model id>]` | StreamModel still Draft | activate StreamModel first |
| Created business entity invisible | created Draft | `"LifeCycleState": "Active"` in create |
| Entity created with API token never shows; update/delete `Access denied`; still blocks `Uniqueness` | `OwnerTenantId` default `$(TenantId)` empty under token auth (same in scripts) | always send `OwnerTenantId` (tenant by `Code`; scripts `Validator.TenantId`); orphans unrepairable: recreate with another unique value |
| `create` with own `Id` fails `No such entity found. Ids='...'` | non-empty `Id` = clone source | omit `Id`, read it from the response |
| `update` Succeeded, `Revision` bumps, nothing changes | values in `argumentValues.fields`; or field `Readonly`; or `DataType` change | values in `entities[].fields`; `Changeable`; new field name for new type |
| ModelAction `create` Succeeded, nothing exists | no-op over `executeaction` | add to model `Actions[]`, `directimport` whole model |
| Custom action Succeeded, does nothing; `Unexpected character encountered while parsing value` | selected ids sent in `entities` | ids in `entityIds`, args in `argumentValues.fields` |
| `Field 'Name' with value 'X' is not unique` after delete | soft-deleted record still counts | `delete` with `Force: true` (SystemAdmin) or `clean` (collection-wide; ask first) |
| `delete` of a ModelField changed nothing | silent no-op | re-query; set `Inactive` |
| Script deploys, first run `Scripter: Error compiling` | lazy compilation | run each new script once; extract nested-quote interpolations to locals |
| Action response shows old values | response is pre-script state | re-query |
| `directimport` `Failed` but model exists; re-import `Succeeded` yet `Table ... doesn't exist` | model persisted, DB init failed (e.g. view column missing); unchanged entity skips init | import master model first, change something trivial, import again (third run creates the view); verify by query |
| `Child` reference field "missing" | not a column; absent with `WithChildren:false` | query `WithChildren:true` |
| Changed generated default view, nothing changes | generated once | import same Name with existing Id, or delete generated rows |
| Optional single-`Guid` reference `No such entity found. Ids='0000...'` | empty ref validated against target (per field) | supply a real value |
| Enum write `"Person"` -> `input string ... not in a correct format` | custom enums are ints on write | integers, also in `DefaultValue` |
| `EnumType[]` field breaks create | not a valid type | reference table or separate fields |
| `FieldDefinition` accepted, view load fails | plain strings | `{Name, Value}` objects |
| `Error reading JArray from JsonReader` | value sent as string, or PS unwrapped one-element array | real JSON array, `@(...)` |

## Streams and data
| Symptom | Cause | Fix |
|---|---|---|
| Stream 404 via `/api/v2/<tenant>/streams/...` | stream routes have no tenant | `/api/v2/Streams/...` |
| `CreateStreamData` Succeeded, row missing | async ingest | poll; no whole-stream counts in parallel tests |
| Live widget grey `<Unknown>` though rows exist | `DataState != 1` (sent `State: 0`, or computed row not Good) | write `State: 1`; compare `== Good`, never `Max`/`>` (not severity ordered) |
| Agent running, no data | agent Draft | approve (SystemAdmin) |
| Report/widget empty | stream/field name differs from agent, wrong `StreamEntityField`, wrong `SourceType`, undeclared field | compare exactly; `ReadHead` |
| Renamed stream field lost history | re-added with new Id | rename in place keeping Id |
| Derived row has wrong quantities | row inheritance clones head row | `Message.Fields.Remove(name)`; guard same context |
| Counters double after restart | non-idempotent preprocessing | derive from message and previous row only |
| Stream report shows other tenants' data | stream rows have no tenant | `SourceType: Model` or module source |
| Bad imports land silently | pipeline entity output = upsert on Id, no validation | deterministic Ids; row errors in agent |
| Old stream data vanishes | `RetentionDays > 0` enforced since 2.2 | `0` for data you keep |
| Page list stuck on spinner after refresh (sporadic) | grid `Data` null during refresh | `UsePagination: true` |

## Views, menu, UI
| Symptom | Cause | Fix |
|---|---|---|
| Menu item icon/precondition ignored; item disappears | overwritten from ModelAction; unknown `ActionName` removed | only the six surviving keys; change the action |
| Button should hide per state | precondition cannot differ per view; typo fails open | item `VisibilityExpression` (ActionBar/TextView) |
| Tab empty without licensed module | tabs have no `ModuleId` | gate widgets or menu item |
| Menu item clickable, view errors | menu `Policy` != view `Policy` | align |
| Menu `???` | `LanguageText.Name` lacks `$`; root-level `Separator` in old clients | add `$`; separators inside groups |
| 2.3: menu change no effect | wrote old `NavigationMenu` setting | update `NavMenu` record |
| Raw `$Key` shown | no `LanguageText` row on instance | create EN/DE rows, include in files |
| List shows 5 rows | `UsePaginationSize` fallback | `UsePagination: true`, `UsePaginationSize: 50` |
| Detail tab fails on child reference | `Child` fields not in view schemas | `IncludeChildEntitiesAsTabs` or filtered `EntityListWidget` |
| Edit dialog throws parser text (`Unexpected token(hasField)`, `Unexpected end.`) | malformed field expression, or stale cached client | `readField<bool>`, typed enums/ints, Ctrl+F5 |
| `n.hasField("Type","Person")` never true / enum filter wrong rows | enum arrives as int; string compare coerces to 0 | `readField<int>` / `getValue<int>` |
| Required + `VisibilityExpression` fails | hidden required field | `EnabledExpression` |
| `SingleValueDonut` shows 2100 % | meant for 0..1 | `LifeGaugeWidget`/`LifeValueWidget` |
| Report `sum`/`average` error on Model source | stream-only methods | `count` or plain value |
| Scheduler tab empty | error swallowed | call `getscheduleddata` by REST |
| Dashboard layout broken | column widths sum > 12 | <= 12 |
| `queryentitylist` misses rows | Deleted/Archived hidden; templates hidden without `MainFilter`; tenant route; empty `OwnerTenantId` | `LifeCycleFilter`; `global` for cross-tenant reads |
| Script `queryAllAsync` returns other tenants -> `Access denied` | not tenant scoped | filter by `Validator.TenantId` |

## Encoding, tooling, secrets
| Symptom | Cause | Fix |
|---|---|---|
| Umlauts garbled after round-trip, untouched lines change | PS 5.1 `Get-Content` default ANSI | `-Encoding UTF8`; write UTF-8 no BOM; body as UTF-8 bytes |
| `.ps1` literals with umlauts corrupt | file without BOM | UTF-8 BOM or ASCII literals |
| `ConvertTo-Json` cuts nested objects | depth 2 | `-Depth 30` |
| `Invoke-RestMethod` hides server message | PS 5.1 | read response stream (`rest-api.md`) |
| `-SkipCertificateCheck` unknown | PS 5.1 | `ServerCertificateValidationCallback` (test hosts only) |
| Git Bash mangles `/api/...` | MSYS path conversion | `MSYS_NO_PATHCONV=1` |
| REST `export` action NullReferenceException | server-side, tested builds | build exports from queries |
| Token exposed in transcript/script | persisted | env var `HOS_API_TOKEN` per session, role-capped/read-only token, revoke after use |
| Rollout/package archives hold secrets and licenses | by design | never commit; ignore licenses, `Secrets/`, `*.pfx`, build output |
| Restore with "Delete Missing Objects" removed config | deletes non-built-in entities missing from snapshot | DB dump first; leave off unless intended |

## Rules of thumb
- Query to prove; diff two exports after deploy.
- Fixed GUIDs, files in git, small import steps, test tenant first.
- Update in place keeping Ids; never rename or delete+recreate.
- Generated views and translations are instance data: deploy deliberately.
