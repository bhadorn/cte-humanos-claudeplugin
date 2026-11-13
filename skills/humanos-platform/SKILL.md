---
name: humanos-platform
description: Use when configuring the HumanOS Platform: tenants, roles, models, fields, streams, views, dashboards, reports, menus, actions, scripts, expressions, notifications, translations, import/export, snapshots, REST API, agent data in streams.
---

# HumanOS Platform configuration

Holds only what the public docs lack: verified shapes, ordering rules, silent failures, the agent-to-platform contract. Docs: `references/docs-map.md`. Customers run **Platform 2.2**; 2.3 differences carry a version note. Instance wins: check version (`/platform/ChangeLog`) and its Swagger (`https://<host>/swagger/v2/swagger.json`, unless disabled).
No platform source: everything is configuration (JSON entities over REST or web UI). **Never guess names, enums, field types or JSON keys**: use exact names here, the doc page, or the instance (`getentityschema`, Swagger, an existing entity as template).

## Core model
- Everything is an **entity** of a *model*, config objects too (`Model`, `ModelField`, `ModelAction`, `ModelView`, `DataType`, `StreamModel`, `ReportSchema`, `NotificationSource`, `Setting`, `Task`, `LanguageText`, `NavMenu`, `Page`). Tenant-based models carry `OwnerTenantId`. REST routes carry the tenant *code*; stream routes do not.
- Roles: `Guest`, `Customer`, `Operator`, `Supervisor`, `TenantAdmin`, `TenantAdmin_BO` (derived, never assigned), `SystemAdmin` -> `concepts.md`.
- `LifeCycleState` `Draft 0, Active 1, Inactive 2, Archived 3, Deleted 4`: only Active is live. Number in files, name on REST reads.
- `DataState` (`EDataState`): `BadNotActive 0, Good 1, BadSourceError 2, BadNoData 3, BadStorageFull 10, BadStorageEmpty 11`. Normal data = **1**.

## Workflow
1. Requirements -> spec, acceptance criteria, test data (who sees/edits what).
2. Model design (entities, fields, `DataType` enums, references, uniqueness, tenant base) -> `models-and-streams.md`
3. Streams for agent data; names match the agent exactly -> `agents-and-streams.md`
4. Views, dashboards, reports, menu, translations -> `ui-views-dashboards-reports.md`, `notifications-translations.md`
5. Actions, expressions, notifications -> `actions-expressions-scripts.md`
6. Import files `src/Platform/NN-<topic>.json` (once-generated GUIDs, NN = dependency order, layout in `import-export.md`) -> `import-shapes.md`, `import-export.md`
7. Import into a TEST tenant/instance first; **verify by querying**, not by `Succeeded`; test one user per role.
8. Back up (DB dump before risky steps; snapshot; optional full export), then production: same files, same order, verify again.

## Quick reference
| Need | Exact thing |
|---|---|
| Base URL | `https://<host>/api/v2/<tenantcode>/...`; streams `https://<host>/api/v2/Streams/...` |
| Auth | `Authorization: Bearer <JWT or hos_... token>`; token in env var `HOS_API_TOKEN` (never in files/chat) |
| Success check | `state` = `Succeeded`/`PartiallySucceeded`; no `success` field; HTTP 200 can still be a failure |
| Write | `entities/executeaction` `{actionName, collectionId, entities:[{fields}], entityIds:[], argumentValues:{fields}}` |
| Bulk config | `entities/directimport` with the export envelope |
| Collection ids | stable on every instance; non-public in `concepts.md`, public on the model doc pages |
| Enum writes | custom enum = **integer** on write, name on read |
| Text keys | `$Key` everywhere; `LanguageText.Name` **includes the `$`** |
| Menu path | `/Model/ListView`, `/Model/Dashboard/<ViewName>`, `/Pages/<Name>`, `/`; menu `Policy` = view `Policy` |

## Top gotchas (all, with symptoms: `gotchas.md`)
1. New `Model`/`ModelField`/`ModelAction`/`DataType`/`StreamModel` are **Draft** and inert: `LifeCycleState: 1`.
2. Tenant-based entities via API token: always send `OwnerTenantId`, else invisible ghosts.
3. `directimport` `Succeeded` is not proof: query.
4. Values in `entities[].fields`, selected ids in `entityIds`, action arguments in `argumentValues.fields`; wrong place = silent no-op.
5. Never compare enums as strings in expressions (`== "New"` matches 0).
6. Menu items keep only `ActionName, Danger, DialogTitle, NavigateOnSuccess, SubItems, VisibilityExpression`.
7. Stream reports return **all tenants'** rows.
8. Script compile errors appear at first execution: run each new script once.
9. Agent runs, no data: agent not approved (Draft).
10. Stream `State: 0` hides live values: send 1.
11. PowerShell 5.1: UTF-8 in and out; no `??`/`?:`/`-SkipCertificateCheck`.
12. Soft delete keeps names/unique values taken.

## References
| File | Task |
|---|---|
| `references/docs-map.md` | doc page paths |
| `references/concepts.md` | tenants, roles/policies, non-public collection ids, settings |
| `references/models-and-streams.md` | model/field/enum/stream caveats, preprocessing |
| `references/import-shapes.md` | annotated JSON shapes (all entity types) |
| `references/import-export.md` | envelope rules, order, verify, full export, snapshots |
| `references/ui-views-dashboards-reports.md` | view/widget/report/menu caveats |
| `references/actions-expressions-scripts.md` | script scope, expression pitfalls |
| `references/notifications-translations.md` | notifications, translations, tasks |
| `references/rest-api.md` | call shapes, PowerShell 5.1 helper |
| `references/agents-and-streams.md` | agent-to-platform contract, approval |
| `references/gotchas.md` | symptom -> cause -> fix |
| `../humanos-project/examples/hall-temperature/Platform/` | complete numbered import set (model, stream, report, notification, menu, texts) |
