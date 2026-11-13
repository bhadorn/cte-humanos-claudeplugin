---
name: humanos-agent
description: Use when creating, building, running or debugging a HumanOS Runtime agent or IoT gateway - .h2proj, device templates, plugin settings.json, C# agent scripts, BuildTool, gateway log, machine connectors (OPC-UA, MQTT, Modbus, S7, FANUC), trial/license problems, connecting an agent to the Platform.
---

# HumanOS Runtime agents (customer edition, Runtime 2.12 / Platform 2.2)

No source code available. This skill holds only what the public docs lack. Everything else: `references/docs-map.md` (topic → local page + URL). Hub rules (never invent, sources, secrets, "agent"): `../humanos/SKILL.md`.

**Never invent** plugin names, GUIDs, config keys, address syntax, script APIs. Order: docs → working example (`references/examples.md`) → this skill. Not found: say so and ask. **Copy the nearest example project, then adapt.**

## Workflow

1. **Clarify** interface (protocol, address, data points/types, direction, frequency, auth) and target (standalone gateway or platform agent with exact stream/field/action names; design platform side first: `humanos-platform`).
2. **Project** from the closest example (`examples.md`, `h2proj-format.md`): new GUIDs except plugin/driver ids; one target per agent, unique `HumanOS.ServiceId`; runtime `win-x86 TRIAL` for tests, licensed for platform agents.
3. **Devices/plugins**: read driver manual, write template + device JSON, driver plugin + `settings.json` (`MainTaskProcessor`), always `HumanOS.UHAL.DeviceDetectors`, then service plugins (`plugins.md`). Credentials only as Designer secrets.
4. **Scripts** only where config cannot (`scripting.md`).
5. **Build**: read the whole output (`buildtool-cli.md`).
6. **Run + read log** (real compile check; `gateway-runtime.md`): background process, tail `Build\<target>\Data\Logs\*.log`, grep `[ERR]`/`[WRN]`; check data via OPC-UA client or payload dump.
7. **Platform connection** (licensed only; `platform-connection.md`): shared node space settings, unique ServiceId, run, admin approves, names match.
8. **Verify with evidence** before "done"; failures: `troubleshooting.md`.

## Facts not in the docs

| Thing | Fact |
|---|---|
| BuildTool | prints `Ok`, exit 0 even with `[ERR]` script errors; compile errors appear only at gateway start |
| Trial runtime | free, all features, **1 h per start**, no shared node space: cannot connect to platform; test via payload dump |
| `HumanOS.UHAL.DeviceDetectors` | mandatory (`9D078B95-6322-44D2-989D-87A1FCA283B9` + `settings.json` `{"Disabled": false}`) |
| Never in `Plugins[]` | `HumanOS.PeMiL.MachineLearning`, `PlatformBinding`, `LanguageModels` (auto-loaded); message bus `HumanOS.PeMiL.MQTTMessageBus` (selected by `SharedNodeSpacePluginId` + `HumanOS.SharedNodeSpace.Type`) |
| Driver `settings.json` | needs `Devices[].MainTaskProcessor`, else `No valid configuration for device` |
| x86-only drivers | FANUC, Heidenhain, Andron, Balluff, Makino, NUM: `win-x86` runtime |
| Platform link | `SharedNodeSpace.Type` + `.Address` + `.MasterSystemControllerId`, unique `ServiceId`, agent approved |
| Stream message | `Stream` = platform model name; `State` = `EDataState`, normal data **1** (enum: `../humanos/SKILL.md`) |
| Logger `SamplingRate` | seconds; `0` = only on change; negative undocumented |
| OPC UA node address | doc's `opc:Fast;`/`opc:Medium;`/`opc:Slow;` prefixes rejected by 2.12 (`Unknown prefix`); use plain `ns=2;s=...` |
| Pipeline job scripts | SDK page shows old `runJobAsync`; 2.12 uses `TPipelineExecutionContext` |
| Config version | raise project `Version` on every change |

## Common mistakes

* `.h2proj`, plugin ids or `settings.json` written from memory; GUIDs (ServiceId, device ids) reused from the example (host conflict on platform).
* Forgetting `DeviceDetectors`, `MainTaskProcessor` or `ServiceRule` ids (no devices/data).
* Data node not logged: lacks the property used in the logger `NodeFilter`.
* Stream/field names or `State` wrong: platform discards/mis-stores data.
* "Agent runs, no data": not approved, or trial runtime.
* Credentials in addresses, `settings.json`, h2proj, logs, chat; vault keys/licenses/`Build/`/`Secrets/`/`*.lic` committed.
* Hand-editing `Build\<target>\` (overwritten by next build).
* Foreground long-lived gateway in a tool call, or leftover gateway processes.
* Script `while` without `await Task.Delay`.

## Where to look

| Topic | File |
|---|---|
| Doc page per topic | `references/docs-map.md` |
| h2proj gotchas, annotated platform-agent excerpt, device JSON | `references/h2proj-format.md` |
| BuildTool output/exit codes | `references/buildtool-cli.md` |
| Plugin rules, GUIDs not in docs, driver/service caveats | `references/plugins.md` |
| Base classes per extension point, pipeline/payload skeletons | `references/scripting.md` |
| Gateway start, log, trial, licenses, service, recovery | `references/gateway-runtime.md` |
| Shared node space, approval, NodeSpaceDataLogger | `references/platform-connection.md` |
| Message → cause → fix | `references/troubleshooting.md` |
| Which example to copy | `references/examples.md` |
| Complete platform-connected agent (OPC UA → stream), builds clean on 2.12 | `../humanos-project/examples/hall-temperature/Agents/` |
