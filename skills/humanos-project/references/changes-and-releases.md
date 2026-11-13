# Changes and releases

Git: everything built as a file (requirements, spec, plan, `src/Platform` JSON, Designer projects, scripts, CHANGELOG).
Never-commit list: `SKILL.md`; patterns: `templates/gitignore` (mirror in CLAUDE.md). Keep the Designer project and
Platform files of one feature in one branch (names must match). `main` = what runs; one commit per plan step.

```
wish -> spec + plan -> branch -> implement -> build + test (test tenant/simulator)
     -> review + changelog -> backup -> production (import, publish, roll out) -> check
```

## Platform configuration

1. Test: import into a test tenant (ideally seeded with a production configuration snapshot), run acceptance criteria.
2. Backup production: database (full dump), document store, config folder (`appsettings.json`, secrets). A Platform snapshot saves only the models/data of its snapshot configuration: not a backup. Dump the DB before any restore.
3. Import the same files in numbered order. Fixed ids: repeat is safe, a failed import can be repeated.
4. Check: import result, views per role, dashboard.
5. Snapshot restore "Delete Missing Objects" deletes every non-built-in entity missing from the snapshot, irreversible.

## Platform version update

1 read release/update notes; 2 backup; 3 run installer (DB migrates in place at first start); 4 only then the product
package (System page, Product tab; built for one Platform version, not checked automatically). No automatic way back
(also product updates): restore backup. Try on a test platform with a production snapshot first.
2.2 (update notes): from 2.1.8 or later; agents need Runtime 2.10+; users sign in again, API tokens reissued; agents
active in 2.1 are approved by the update, newly registered agents wait for approval.

## Agents

Binaries = installed Runtime; configuration = Designer project in git. Only configuration is rolled out.

1. Designer: build + Publish (needs a SystemAdmin token). Every published version is kept.
2. Platform, agent's Configurations tab: choose version per agent, roll out. "Restart now" = immediate, else next restart (maintenance window). Offline agents fetch later.
3. Rollback = roll out an earlier version. Agent snapshot captures what actually runs incl. on-site changes (newest 5 kept per agent).
4. A rollout replaces the agent's secrets; set "Keep licenses" only when needed.
5. Check Rollout State = Applied, then log and data.
6. New object (second hall, machine): copy group in template and device, new GUIDs, adjust address/property, extend dashboard, test with simulator, roll out.

## Backup / rollback

| Layer | Backup | Roll back |
|-------|--------|-----------|
| Platform config | DB dump, document store, config folder | restore backup |
| Platform version | same; Platform before product package | restore backup |
| Agent config | snapshot of what runs | roll out earlier published version |

## First release

Test tenant accepted -> decide who may import to production -> backups -> import + rollout -> check -> tag deployed
commit (`release-1`). Licenses (Platform, agents) expire: track outside git.
