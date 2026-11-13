---
name: humanos-project
description: Use when starting a HumanOS project or repo, turning a process into requirements, specification or plan, planning build order, verifying, acceptance testing, or deploying changes to a running HumanOS installation (test to production, import, agent rollout).
---

# HumanOS project workflow

Build order and gates for a HumanOS application. How-to: `humanos-platform`, `humanos-agent`. Terms, roles, DataState,
links, roles, DataState: `../humanos/SKILL.md`; terms: `../humanos/references/glossary.md`. Written for Platform 2.2, Runtime 2.12; project CLAUDE.md states the versions in use.

| # | Phase | Result | Gate |
|---|-------|--------|------|
| 1 | Repo + CLAUDE.md | skeleton, committed | user confirms layout |
| 2 | Requirements | `doc/Requirements.md` | 5 questions answered, open points listed |
| 3 | Specification | `doc/Specification.md` | user approves |
| 4 | Plan | `doc/Plan.md`, checkbox + check per step | user approves ("go") |
| 5 | Implement in plan order | `src/Platform/`, `src/Agents/` | each step has its check |
| 6 | Verify | evidence per step | tool output, not assertion |
| 7 | Acceptance | `doc/Acceptance.md` | user signs off |
| 8 | Release/change | changelog, backup, import/rollout | test tenant first |

No implementation before plan approval. Unknowns are **open points**, never guessed.

## 1. Repository

```
<project>/
  CLAUDE.md  CHANGELOG.md  .gitignore
  doc/        Requirements, Specification, Plan, Acceptance (.md)
  src/Platform/             authored, numbered import JSON (10-model-..., 20-streammodel-...), fixed ids: SOURCE OF TRUTH
  src/Agents/<Name>.Agents/ one .h2proj per agent family
  Snapshot/                 optional full instance export, NOT in git if customer data
```

- Start: copy `templates/CLAUDE.md`, `templates/gitignore` (as `.gitignore`); `git status` shows no secret; first commit.
- Full export (Platform.Config layout, `../humanos-platform/references/import-export.md`) = snapshot for diff/backup only; do not work from it.
- Never commit: licenses, certificates, private keys, passwords/tokens, Designer vault key, customer data, `Build/`, downloaded config packages (contain agent secrets/licenses).
- Templates: `templates/` (CLAUDE.md, gitignore, Requirements, Specification, Plan, Acceptance, CHANGELOG). Worked example: `examples/hall-temperature/README.md`.

## 2-3. Requirements, specification

| Question | Maps to |
|----------|---------|
| Who works with it? | tenant, roles |
| What happens, in order; manual/automatic? | model actions, pipelines, tasks, notifications |
| Which objects/values? | models (master data) vs streams (time series) |
| Where does data come from/go? | external system = HumanOS agent (connector) |
| What triggers it? | schedule, new value, click, threshold |

Acceptance criteria name input, expected result, where observed ("Simulator sends 21.4 for hall 1; within 2 min stream
`HallTemperatureStream` has row Hall 1 / 21.4 and the dashboard shows it"). Interview guide, spec outline: `references/requirements-to-plan.md`.

## 4. Plan: fixed build order

1. Platform model: models, streams (names/fields exactly as the agent sends them)
2. Views, reports, dashboard, navigation menu with matching policies
3. Agent: devices, connectors, scripts, stream publisher
4. Tests against acceptance criteria

Why: stream/action must exist before the agent writes to it. Write a **name contract** table (stream, field, model,
action, report, RefId; see example): a name mismatch is the classic defect. Send `State` 1.
Plan review checklist: `references/requirements-to-plan.md`.

## 5-6. Implement, verify

| Artifact | Proof |
|----------|-------|
| Platform import | test tenant reports success, also on 2nd import |
| Views | log in as a test user per role; menu entry opens view |
| Reports/dashboard | known test values appear unchanged |
| Agent build | `HumanOS.BuildTool.exe <proj>.h2proj --build --target=<Target>` (syntax only; read `[ERR]`) |
| Agent start | gateway log: C# compiled, master controller connected, source up |
| Platform side | Automation > Agents: Active, Health ok, Rollout State as expected |
| Data | value at each hop; fault lies between last good and first bad hop |

Details, log lines, rollout states, symptoms: `references/verification.md`. "Done" = command ran, output read and shown.
A new agent is inert (Draft, data discarded) until a SystemAdmin approves. Trial runtime cannot reach the Platform.

## 7. Acceptance

Per criterion: OK / Warning (limitation) / Error, with evidence. A human with domain knowledge signs off; a Claude review checks what was written, not what was meant.

## 8. Changes to running installations

One change = one branch; update spec + plan first; test in test tenant/simulator; changelog; **backup**; apply from
committed files (import, publish, roll out); check Rollout State Applied, dashboard, log. Never hand-edit production.
Procedure, backup/rollback table: `references/changes-and-releases.md`.

## Working with Claude

- Say "HumanOS agent", never bare "agent" (AI agent = Claude subagent).
- Sources first: every HumanOS statement needs a doc link or file; no source = open point. Sources listed in CLAUDE.md.
- Record each HumanOS correction in the project CLAUDE.md corrections log.
- New agent request gives Claude: what/from where/how often/failure cases; Platform names already in place; closest example (use cases repo `llms.txt`); source interface (topic, payload, simulator). Claude uses new GUIDs, proves with BuildTool output + gateway log.
- Never paste secrets into chat or git. Never copy an agent configuration to a second machine without a new ServiceId.
