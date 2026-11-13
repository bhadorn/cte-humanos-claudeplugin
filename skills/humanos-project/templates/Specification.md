# Specification: <project>

Status: draft | approved by <name> on <date>. From `doc/Requirements.md`. Platform <2.2>, Runtime <2.12>. Unknowns = open points, never guessed.

## 1. Goal and users
Goal; tenant(s); roles (see `humanos` glossary):
| Role | Sees | May change |
|------|------|-----------|

## 2. Data model
| Model/stream | Field | Type | Required | Remark (source, unit) |
|--------------|-------|------|----------|-----------------------|
Streams: name, `RefModelId`, retention, duplicates stored (yes/no). `State` 1 = Good (DataState: `humanos` glossary).

## 3. Actions and flows
Model actions, pipelines, tasks, notifications (event, condition, recipients, channel).

## 4. UI
Menu entries (path, policy), list views, details/dashboards (widgets, stream and field names), reports (name, type, source).

## 5. Interfaces (each becomes a HumanOS agent)
| System | Protocol | Direction | Addresses | Frequency | Error cases | Credentials (where kept, never git) |
|--------|----------|-----------|-----------|-----------|-------------|--------------------------------------|

## 6. Name contract
Names identical in Platform and agent (stream, field, model, action, report, `RefId`). Example: `examples/hall-temperature/README.md`.
| Name | Platform | Agent |
|------|----------|-------|

## 7. Acceptance criteria
Input, expected result, where observed; copied to `doc/Acceptance.md`.

## 8. Open points
| # | Question | Owner | Due |
|---|----------|-------|-----|
