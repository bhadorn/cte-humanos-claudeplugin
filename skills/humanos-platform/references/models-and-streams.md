# Models, fields, data types, streams: caveats

Docs: `Manuals/BusinessModels/*`, `Manuals/Streams/*`, `DynamicModelDeploymentPitfalls`. JSON: `import-shapes.md`. Symptoms (enum ints, `FieldDefinition`, optional Guid refs, `JArray`, required+hidden): `gotchas.md`.

## Models and fields
| Topic | Caveat |
|---|---|
| Built-in enum as field type | may lack a `DataType` row on the instance (create then fails): query `DataType`, else define own enum |
| `DataType` of existing field | cannot change (silently ignored): new field name, old one `Inactive` |
| Dates | `Date`/`DateTime` never null; unset = `0001-01-01`. **2.3:** dynamic columns NOT NULL |
| `EditMode: Readonly` | ignores every write, also scripts and create; use `ChangeableOnlyOnCreate` for generated names |
| `ProvidedForLinks` | must be true on fields shown from a referenced entity |
| References | `ReferenceType: Parent` on the child's field persists the relation. `Child` on the parent side is no column: never a list column or `VisibleFields` entry (use `IncludeChildEntitiesAsTabs` or `EntityListWidget` filtered on the child's parent field). Concrete model may reference an abstract one only as `Parent` |
| `InputMethod: AutoGenerateManualOverride` | unsupported for cross-field patterns: use `AutoGenerate`; no auto-generation on pipeline entity-output path |
| `TableName` | unique, immutable. `ReadOnlyView`: `MasterCollectionId` only at creation; query once after import |

## Stream models
- Field `DataType` and reference settings settable **only at creation**. One column per field named after `Name`. Rename keeping the `Id` (field Active) keeps history; new Id = new empty column. Never delete+recreate.
- Activate the `StreamModel` **before** its fields.
- `DataState` values: `SKILL.md`. `BadNotActive 0` is also written by the watchdog. Not severity-ordered: compare `== 1`, never `Max`/`>`. Live widgets treat anything but `Good` as no value (grey `<Unknown>`): write computed rows with `1`.
- Stream rows are **not tenant-aware**: reports return all tenants; for tenant-aware use a `Model` or module source.
- Ingest, duplicate and replace rules: `agents-and-streams.md`.
- `RestrictedWrite`: only SystemAdmin reports/edits. `DisableRestReports`: nobody over REST. `DisableManualChanges`: no manual edits.
- Child streams: `ParentStreamId` + optional `ParentFieldMapping` (`{"SrcField":{"TargetFieldName":"DstField"}}`); unmapped parent fields pass through; child script decides via `bSuccess`.
- Manual row edits (SystemAdmin) are stored as entered: no inheritance, no preprocessing.
- `RetentionDays > 0` enforced since 2.2 (hourly sweep); `0` keeps forever.

## Preprocessing (`PreprocessingScript`, C#)
Scope: `Message` (`Message.Fields` dictionary, `Message.readField<T>("Name", default)`), `nPreviousEntity` (previous row of same `RefId`, may be null), `bSuccess`, `DeclineReason`.
```csharp
if (Message.readField<Guid>("ProductionOrderId", Guid.Empty) == Guid.Empty) { DeclineReason = "no order"; bSuccess = false; }
```
- **Row inheritance:** new row clones the head row of same `RefId`, overwritten only by fields in `Message.Fields`. To inherit, `Message.Fields.Remove("Field")`; guard same context.
- **Idempotent:** delivery repeats; derive from message and previous row, never accumulate counters.
- Validate with action `checkCode` or the designer's stream test bench.
