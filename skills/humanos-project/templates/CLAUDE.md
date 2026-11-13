# <Project name>

HumanOS Platform <2.2>, Runtime <2.12>. doc/ (Requirements, Specification, Plan) is approved before implementation. Language of doc/: <English>.

## Sources (no HumanOS source code here; cite one or mark unverified)

- Runtime docs https://doc.cybertech.swiss/runtime/intro ; local: C:\Program Files (x86)\CyberTech\HumanOS.BuildTool\Doc\
- Platform docs https://doc.cybertech.swiss/platform/intro
- Examples (file llms.txt): https://github.com/bhadorn/cte-humanos-runtime-usecases
- Agent-to-Platform: https://github.com/bhadorn/cte-ctproduction-agents

## Rules

- "Agent" = HumanOS agent (deployed Runtime instance), never an AI agent; say "Claude subagent" for that.
- Layout: `doc/`, `src/Platform/` (numbered import JSON, source of truth, keep fixed ids), `src/Agents/<Name>.Agents/` (.h2proj), `CHANGELOG.md`.
- Order: requirements, specification, plan (platform model, views/dashboards, agent, tests). Implement after approval; one commit per step.
- Names (streams, fields, models, actions) identical in Platform and agent.
- Verify with shown tool output, never "done" without: test-tenant import, BuildTool, gateway log, dashboard values. Test tenant first; production only from committed files after backup.
- Build/check: `HumanOS.BuildTool.exe <proj>.h2proj --build --target=<Target>`. Read all output: `[ERR]` can appear despite `Ok`/exit 0. It checks syntax only; start the gateway to compile C#.
- New nodes get new GUIDs; one ServiceId per machine. New agent stays inactive until SystemAdmin approves it (Automation > Agents).
- Never commit licenses, certificates, keys, passwords/tokens, Designer vault key, customer data, Build/, downloaded configuration packages.
- Unknown = open point, do not guess.

## Corrections log

One line per Claude mistake about HumanOS.

- (none yet)
