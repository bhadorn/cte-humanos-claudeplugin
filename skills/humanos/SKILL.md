---
name: humanos
description: Use when a task mentions HumanOS, HumanOS Runtime/Platform, IoT Designer, BuildTool, IoT gateway, ctProduction or CyberTech, HumanOS terms (tenant, role, stream, agent, UHAL, PeMiL), or before answering any HumanOS question from memory.
---

# HumanOS (CyberTech Engineering)

- **Runtime** (edge): gateways/agents connecting machines, PLCs, sensors. Designed in **IoT Designer** (`.h2proj`), built with **HumanOS.BuildTool**, run by `HumanOS.IoT.Gateway.exe`.
- **Platform** (web): tenants, users, roles, models, streams, views, dashboards, reports, actions, notifications; fleet management of agents (approval, config rollout, diagnostics).

Agents send time series to Platform **streams** and run Platform **actions**/pipelines; names must be identical on both sides. Written for **Runtime 2.12, Platform 2.2**.

## Rules

1. **Not public knowledge: never answer from memory.** Take role names, field types, plugin names, JSON keys, CLI flags, script APIs from these skills, public docs or a working example. Else say "unverified" and ask.
2. **"Agent" = HumanOS agent** (deployed Runtime instance), never an AI agent; Claude's helpers are "subagents".
3. **Name the source** (doc URL, example repo path, or the instance: Swagger, exported entity).
4. **Prove before "done"**: import into test tenant, BuildTool output, gateway log lines, or dashboard values.
5. **Test tenant first**; production only gets committed, tested files, after a backup.
6. **No secrets** (licenses, tokens, passwords, certificates, vault keys, `Build/`) in git or chat; read tokens from env vars.

## Which skill

| Task | Skill |
|---|---|
| New app/repo, requirements → spec/plan, acceptance, release to a running installation | `humanos-project` |
| Tenants, roles, models, fields, streams, views, dashboards, widgets, reports, menus, actions, expressions, notifications, translations, import/export, REST | `humanos-platform` |
| Designer project, devices/connectors, C# scripts, BuildTool, gateway, trial runtime, licenses, connecting/approving an agent | `humanos-agent` |
| Term lookup | `references/glossary.md` |

Spanning both sides: design the platform first (models, streams, action names = the contract), then build the agent. Worked example (OPC UA → stream → dashboard, chart, e-mail alert): `../humanos-project/examples/hall-temperature/`.

## Facts owned here

* **Roles** (per tenant): Guest, Operator, Customer, Supervisor, TenantAdmin, SystemAdmin, plus derived TenantAdmin_BO (never assigned). Not "Admin/User/Viewer".
* **`EDataState`** (stream `State`): 0 BadNotActive, **1 Good (send this)**, 2 BadSourceError, 3 BadNoData, 10 BadStorageFull, 11 BadStorageEmpty. `State: 0` makes live widgets show no value.

## Main links

Pattern: `https://doc.cybertech.swiss/<runtime|platform>/<path>`, no `.md`; folder indexes end with `/`; `#anchors` work. Unversioned = latest (old: `/runtime/2.11/...`, `/platform/2.1/...`). Full topic lists: `../humanos-agent/references/docs-map.md`, `../humanos-platform/references/docs-map.md`.

| What | URL / location |
|---|---|
| Intro | `runtime/intro`, `platform/intro` |
| Platform principles (tenants, roles) | `platform/Manuals/Principles` |
| Runtime tutorials / BuildTool | `runtime/Tutorials/`, `runtime/Manuals/HumanOS.BuildTool/` |
| Platform REST API | `platform/Manuals/RestApi/` |
| Release notes | `runtime/ReleaseNotes`, `platform/ChangeLog` |
| Glossaries | `runtime/Glossary`, `platform/Glossary` |
| EdgeDevice 1.0 (separate product) | `edgeDevice/specification` |
| AI index of docs | `https://doc.cybertech.swiss/llms.txt` (unverified) |
| Use cases (~20 Designer projects; read `llms.txt` first: <https://raw.githubusercontent.com/bhadorn/cte-humanos-runtime-usecases/main/llms.txt>) | <https://github.com/bhadorn/cte-humanos-runtime-usecases> |
| ctProduction agents (agent → platform reference; needs license; release mirror, never push) | <https://github.com/bhadorn/cte-ctproduction-agents> |
| Local Runtime docs (installed with BuildTool; offline/grep; matches installed version; same structure as web, e.g. `Doc\Manuals\HumanOS.UHAL.MqttControl\` = `/runtime/Manuals/HumanOS.UHAL.MqttControl/`) | `C:\Program Files (x86)\CyberTech\HumanOS.BuildTool\Doc\` |

## Common mistakes

| Wrong | Right |
|---|---|
| BuildTool printed `Ok`, scripts fine | Read `[ERR]` lines; C# compiles at gateway start: check gateway log |
| Agent runs, so data arrives | New agent stays Draft until SystemAdmin approves; trial runtime cannot connect |
| `directimport` `Succeeded` = done | Query the result; check entities created as Draft |
