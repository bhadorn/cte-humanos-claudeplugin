# Model actions, scripts, expressions: caveats

Docs: `Manuals/BusinessModels/ModelActions|Expressions`, `Manuals/Objects/ClassObjects`, `DynamicModelDeploymentPitfalls` (script method signatures). JSON: `import-shapes.md`. Symptoms (lazy compile, no `activate`, create no-op, tenant scoping, enum compare, dialogs): `gotchas.md`.

## Actions
- `VisibleRankOnListView`/`VisibleRankOnDetailsView` = 0 hides the action.
- `clean` purges the **whole collection** (default older than 30 days, `RetentionDays: 0` = now): ask first.
- `AgentAccessible` **replaces** `ExecutionPolicy`; `query` exposes every field of the model: never on `User`/`Secret`/personal data.
- `Task` entity runs an action on a schedule: `ModelId`, `ActionName`, `Arguments`, `TriggerConfig`; `clean` needs a SystemAdmin `UserId`.

## Scripts
- `checkCode` validates without storing. No nested quotes inside interpolation holes (`$"{M(\"x\")}"`).
- `throw new InvalidOperationException("$Key|{0}|{1}")` shows a translated message (`$Key|v1|v2` fills `{0}`,`{1}`).
- `queryAllAsync(null, Token)` not tenant scoped. Creating tenant-based entities: `OwnerTenantId = Validator.TenantId`, `LifeCycleState = 1`. Method `createNewEntityAsync` (not `createEntityAsync`); `updateEntityAsync` takes the id on `Data.Id`.
- `Readonly` fields unwritable even from scripts; dates never null (`getValue<DateTime?>` throws); `JArray` field needs a JSON array.
- `EncodedString` argument arrives encrypted, logged as `<secret>`.

## Expressions
| Dialect | Parameter | Where |
|---|---|---|
| Database query | `entity` (`entity.getValue<T>("F")`) | REST `MainFilter`, list `EntityFilter`, widget `Filter`, `EntitySelectionExpression`, report `Query`, snapshot `FilterQuery`, task selection |
| Single dataset | `n` (`n.readField<T>("F", default)`, `n.hasField`) | `EnabledExpression`/`VisibilityExpression`, field `Notifications`, stream `TriggerCondition` |
| Dataset list | `list` | action `PreConditionExpression` |
- `getValue` absent in `n`; `hasField`/`readField` absent in `entity`.
- Enums: a non-numeric string becomes 0 and matches member 0. Use `getValue<int>`/`readField<int>` (custom enums are ints; `readField<int>` throws on a name) or a typed platform enum.
- `n.hasField("F", true)` (bool literal) does not parse, breaks the **whole** edit dialog: `n.readField<bool>("F", false)`. `hasField(name, "text")` only for real string fields.
- `.Contains(...)` on `Guid[]` columns unsupported in `entity`; works on string columns holding ids.
- `Unexpected end.` = unresolvable type or truncated string, not bracket balance. Parse failure in a precondition fails **open**.
- `$(...)` variables are substituted by the portal before the DB query.
