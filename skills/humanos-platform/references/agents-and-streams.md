# Agent to platform contract

Docs: `Manuals/Streams/*`, `Manuals/RestApi/Streams`, `Manuals/FleetManagement/*`. Agent side (logger, node space, payload script, rollout): `../../humanos-agent/references/platform-connection.md`. Term "agent" = HumanOS Runtime gateway (not a Claude agent). Build the platform side first; the agent matches it exactly.

## Ways data reaches a stream
| Path | Note |
|---|---|
| MQTT (standard) | platform subscribes `datalake/stream` (QoS 2, ack `datalake/stream/ack`); payload below; sent by the agent's NodeSpaceDataLogger |
| REST | `POST /api/v2/Streams/CreateStreamData` (no tenant segment; `rest-api.md`) |
| Scripts, pipelines, tasks | write streams themselves |
| Manual | SystemAdmin edits rows (stored as entered) |

## Payload contract
```json
{ "Messages": [ { "Stream": "HallTemperatureStream",   // exact StreamModel.Name
                  "RefId": "<guid>",                   // device/entity id; model-linked streams: Id of an entity of RefModelId
                  "State": 1,                           // EDataState: 1 Good (normal), 0 BadNotActive, 2 BadSourceError, 3 BadNoData
                  "TimeStamp": "2026-02-17T07:50:05Z",  // message time (UTC)
                  "Fields": { "Hall": "H1", "Temperature": 21.4, "TimeStamp": "2026-02-17T07:50:04Z" } } ] }   // names = StreamModelField.Name; Fields.TimeStamp = time of the value
```
| Rule | Consequence |
|---|---|
| `State` 1 for valid data | `0` hides value in live widgets |
| Unknown stream name | dropped (REST: `Failed` without reason; MQTT: platform log only) |
| Undeclared field | ignored or rejected: declare first |
| Same data `TimeStamp` + `RefId` | replaces earlier row |
| Identical consecutive values | dropped at head unless `StoreDuplicatedEntries: true` |
| `RefId` not an entity of `RefModelId` | reports/live widgets find no entity |
| Entity writes via pipeline/agent | upsert by `Id`, no required checks/auto-generation: deterministic Ids, validate in agent |
| Delivery | async, at-least-once; `Succeeded` = queued (rows after seconds) |

## Approval (#1 cause of "agent runs, no data")
- New agent appears under `Automation > Agents` as `Draft` (pending): **inert** (data discarded, no commands, licenses or rollout). `Active` = approved, `Inactive` = blocked.
- Approve: action `agentApprove` (block `agentBlock`), SystemAdmin only. REST: `executeaction` with `collectionId` of the Agent model (resolve by name), `entityIds:[agentId]`. Never set `LifeCycleState` by hand.
- Max 100 pending agents. Same service id on two machines = conflict. **2.2:** agents active before the update were auto-approved; new ones need approval.
- Built-in restricted streams: `AgentStateStream`, `AgentLogStream`, `AgentSensorStream`.
- Rollout package holds the agent's secrets and licenses: never commit.
- Registered objects of products that auto-register devices arrive `Draft`: `update` them `Active`.

## New data source, in order
1. Stream model + fields (+ preprocessing, retention, watchdog), import Active.
2. Agent logger with exact stream/field names; build, deploy.
3. Approve the agent.
4. Check `Automation > Data Streams` (row count, queue) or `ReadHead`.
5. Then reports, live widgets, notification events.

## Missing values: check order
source/simulator -> broker topic -> agent log (connected to master, broker) -> payload dump -> agent approved -> platform stream list -> report/widget (stream name, field name, `StreamEntityField`, tenant).
