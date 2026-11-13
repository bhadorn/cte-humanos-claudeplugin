# HumanOS plugin for Claude Code

Gives Claude Code the exact names, formats and pitfalls of HumanOS (CyberTech Engineering), which is not public
knowledge, plus links to the public docs. Covers HumanOS Runtime agents (IoT Designer, devices, C# scripts,
BuildTool, gateway), the Platform (tenants, roles, models, streams, dashboards, reports, import, REST API) and the
project workflow from requirements to release. Written for **Runtime 2.12**, **Platform 2.2**.

## Install

```text
/plugin marketplace add <path or git URL of this repository>
/plugin install humanos@humanos
```

## Skills

| Skill | Use for |
|---|---|
| `humanos` | Entry point: rules, main links, glossary |
| `humanos-project` | Repo setup, CLAUDE.md/spec/plan templates, verification, acceptance, changes to running installations, worked example |
| `humanos-platform` | Models, streams, views, dashboards, reports, menus, actions, notifications, import/export, REST API |
| `humanos-agent` | `.h2proj`, plugins, scripts, BuildTool, gateway, licenses, platform connection, agent approval |

Skills load when the task matches; or call one, e.g. `/humanos:humanos-agent`. New project: ask Claude to set one up and
keep the knowledge-sources section of the `CLAUDE.md` template.

## Sources

- Runtime docs <https://doc.cybertech.swiss/runtime/intro>, Platform docs <https://doc.cybertech.swiss/platform/intro>
- Use cases <https://github.com/bhadorn/cte-humanos-runtime-usecases> (`llms.txt`), reference agents <https://github.com/bhadorn/cte-ctproduction-agents>
- Local Runtime docs: `C:\Program Files (x86)\CyberTech\HumanOS.BuildTool\Doc\`

Unconfirmed facts are marked "(verify)"/"(unverified)": check them. Never put licenses, tokens, passwords,
certificates or the Designer vault key into chat or git.
