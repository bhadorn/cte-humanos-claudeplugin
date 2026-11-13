# Verification

Verified = a tool confirmed it. Test tenant first.

| Artifact | Check | Tool | Evidence |
|----------|-------|------|----------|
| Platform import | succeeds; 2nd import changes nothing | list menu "Import" (SystemAdmin) or `POST /api/v2/<tenant>/entities/directimport` (state `Succeeded`) | import message |
| Views | each role sees/opens its entries | one test user per role | screenshot/description per role |
| Reports/dashboards | known test values appear unchanged | 2 values per object by hand in stream list (SystemAdmin), or real data | screenshot |
| Pipelines | run completes, jobs as expected | Automation > Job Pipelines, job details (input, output, agent logs) | status, logs |
| Agent project | build ok, scripts parse | `HumanOS.BuildTool.exe <proj>.h2proj --build --target=<Target>` | build output |
| Agent runtime | C# compiles, connection up, values arrive | `HumanOS.IoT.Gateway.exe -AppSettingsDir=<path to Build\<Target>>` (no trailing backslash), read log | log lines |
| Agent in Platform | approved, healthy, right config | Automation > Agents: Status, Health, Rollout State | list row |
| Code/config | matches plan, no secrets | review, then a human | findings by severity |

BuildTool checks script syntax only; C# errors appear at gateway start. After a script edit: rebuild and restart the gateway, else the old script runs.

## Typical gateway log lines

- Successfully connected to the master controller (needs license and shared node space)
- Actor 'Actor' registered (only if the project defines an actor with skills)
- connection to the source up (connector specific, e.g. MQTT broker)

## Data chain

source/simulator -> connector log -> DataNode value -> publisher payload dump (`Data\Dumps` under the build) ->
Platform stream -> report/dashboard. Fault lies between last good and first bad hop. Without Platform connection
(trial runtime, no license) the chain ends at the payload dump = exactly the message the stream would get.

## Rollout State (Automation > Agents)

| State | Meaning | Action |
|-------|---------|--------|
| Not Defined | nothing rolled out | normal for locally started agents |
| Pending | ordered, not delivered | wait; agent reachable? |
| Delivered | arrived, not active | restart agent or wait for maintenance window |
| Applied | runs expected version and content | none |
| Modified | version matches, content changed on machine | save snapshot, roll out same configuration again |
| Failed | last delivery failed, will retry | read reason at the agent |

Without machine access: Report to Platform on in the Designer; warnings/errors reach the agent log stream, "Collect logs" fetches the full log, an agent state stream shows silence > 30 s.

## Troubleshooting

| Symptom | Likely cause |
|---------|--------------|
| Agent runs, no data in Platform | not approved (Draft); shared node space/ServiceId missing; trial runtime |
| "Shared nodespace is not supported" | trial runtime; use a licensed runtime |
| "Could not find node '<id>'" | Platform stream node unreachable: no connection or wrong id |
| Gateway does not find its configuration | `-AppSettingsDir` trailing backslash or wrong path |
| Old script behaviour | not rebuilt / gateway not restarted |
| Menu entry clickable, view errors | view authorization differs from menu entry policy |
| Operator does not see a view | policy/view authorization; built-in actions need `POLICY_SystemAdmin` unless changed |
| Report empty | stream/field name differs from agent's; wrong grouping field |
| Dashboard row broken | column widths in a row sum > 12 |
| Import fails on reference | wrong order; import dependencies first |
| Host conflict reported | same ServiceId on two machines (copied configuration) |

Stream `State`: DataState enum in `humanos` skill; send 1 for normal values.
