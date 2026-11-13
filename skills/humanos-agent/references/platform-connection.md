# Agent side of the Platform connection

Platform side (streams, models, approval, payload contract, pipelines): `../../humanos-platform/references/agents-and-streams.md`. Here only agent configuration.
Docs: `Manuals/HumanOS.Kernel.PeMiL/HumanOS.Kernel.PeMiL.SharedNodeSpace.md`, `Manuals/HumanOS.PeMiL.MQTTMessageBus/Configuration.md`, `Manuals/HumanOS.Runtime/FleetManagement.md`, `Manuals/HumanOS.PeSeL.NodeSpaceDataLogger/`, `Manuals/HumanOS.Kernel.PeSeL/HumanOS.PeSeL.DataLoggerConfig.md`; platform `Manuals/FleetManagement/`, `Streams/Payload`.

## Checklist (all must hold)

| # | Requirement | Note |
|---|---|---|
| 1 | Licensed runtime, not trial | trial: `TRIAL VERSION: Shared nodespace is not supported.` |
| 2 | Target shared node space settings | `HumanOS.SharedNodeSpace.Type` = `HumanOS.PeMiL.MQTTMessageBus.TSharedNodeSpaceHub, HumanOS.PeMiL.MQTTMessageBus`, `.Address` = platform MQTT broker, `.MasterSystemControllerId` = ServiceId of the platform server (ask platform admin). Example agents also set `.MasterAgentOrchestratorId` to the same value (not in public docs). Excerpt: `h2proj-format.md` |
| 3 | Unique `HumanOS.ServiceId` per agent, meaningful `HumanOS.ServiceName` | same id on two hosts = host conflict |
| 4 | Network agent → broker | 1883, TLS 8883; broker down: agent keeps running locally |
| 5 | Agent approved on platform | pending = inert: data discarded, no commands, no rollout |
| 6 | Names match platform | stream model, field, action, skill names exactly; platform side first |

### Message bus is not in `Plugins[]`

Neither is PlatformBinding (runtime loads it). The bus is selected on the target by `SharedNodeSpacePluginId` (`0DAE9067-44EB-40B9-9385-DD6EFB353E11`; Designer's shared node space dropdown also writes `HumanOS.SharedNodeSpace.Type`). By hand set both. The runtime resolves the class from the assembly-qualified `Type` string; wrong name logs `Could not find shared nodespace type '<type>'`. The working public agent lists no bus in `Plugins[]`.

Broker credentials: `Username`/`Password` as `$(ENV_VAR)`; shared node space resolves no secrets (`ConnectionSecret` not evaluated). TLS: bus `Configuration.md`.

## NodeSpaceDataLogger publisher

Excerpt (`HumanOS.PeSeL.NodeSpaceDataLogger\settings.json`; copy the full file and `TStreamingPayload_v2.cs` from the public ctProduction agents):

```jsonc
{ "Publishers": [ {
    "Id": "<new guid>", "Name": "MyStreamLogger",
    "Connection": "<GUID of the platform node that accepts stream data; value from the example agents>",
    "DataSets": [ { "Name": "<exact platform StreamModel name>", "Type": "DataNode",
                    "NodeFilter": "node.hasProperty<bool>(\"EnableMyStream\", true)",
                    "Fields": [ { "Name": "Id",        "DataType": "System.Guid",     "Query": "item.GlobalId" },
                                { "Name": "DeviceId",  "DataType": "System.Guid",     "Query": "item.Node.getProperty<Guid>(\"DeviceId\")" },
                                { "Name": "TimeStamp", "DataType": "System.DateTime", "Query": "item.TimeStamp" },
                                { "Name": "Value",     "DataType": "System.Object",   "Query": "item.Value" },
                                { "Name": "State",     "DataType": "HumanOS.Kernel.DataModel.EDataState, HumanOS.Kernel.Base", "Query": "item.DataState" } ] } ],
    "PayloadScriptFile": "TStreamingPayload_v2.cs",
    "SamplerType": "OnlyLastValue", "SamplingRate": 60,
    "ProcessOnlyValidData": true,
    "NetworkTimeout": 5000, "PayloadDumpEnabled": true,
    "PayloadDumpFileName": "$(AppConfig:CommonAppPath)\\Dumps\\NodeSpaceDataLogger\\MyStreamLogger.log", "PayloadDumpRetentionTime": 2,
    "ServiceRule": { "BindingRuleId": "<guid>", "UnbindingRuleId": "<guid>", "Devices": [], "Drivers": [], "Properties": {} } } ] }
```

| Fact | Detail |
|---|---|
| `Connection` | GUID of platform node, from the working example; not in public docs (confirm with platform admin if it fails). Unreachable platform/wrong id: `Could not find node '<guid>'` (also on trial / not connected) |
| `DataSets[].Name` | = platform StreamModel name, unique; produced field names = stream field names |
| `NodeFilter` | selects nodes by device property flag (`h2proj-format.md`) |
| `SamplingRate` | seconds between sends; **`0` = only on change**; positive = periodic (doc: `StartingSecond`, `MinimalInterruptionTime`). Negative undocumented; example agents use e.g. `-10`, do not rely on it |
| `SamplerType` | `OnlyLastValue` = last value at interval end; `AllData` = all values of the interval as time series. Buffering after outages needs history on the nodes (doc: Buffering) |
| `ProcessOnlyValidData: true` | only `EDataState.Good` sent (state enum: `../../humanos/SKILL.md`); idle/bad nodes skipped |
| `DiscardPersistedData` | doc spelling `DiscardPersistedData`; working example and runtime use `DisgardPersistedData`: copy the example's |
| Default error policy | `ShutdownApplication`: unexpected logger error stops the whole gateway; consider `ReconnectLogger` |
| Test without platform | `PayloadDumpEnabled`: dump = exactly the stream message (works on trial) |

Message shape: contract in the platform file. `TStreamingPayload_v2.cs` builds one message per device; `RefId` = device property `WorkplaceId` if set, else device id; payload skeleton in `scripting.md`. `State` must be `1` for normal data (a workshop slide printed `0`: wrong).

## Platform → agent (actions, pipelines, skills)

* Device carries an actor (`Agents[]` in template, type `GenericActor`, property `Skills` = platform skill ids); log `Actor '<name>' registered`.
* Platform pipeline job = C# `TAbstractPipelineOperationScriptObject` in `Scripts.Workflows[]` (`scripting.md`).
* Publishing: project `PlatformURL` + `PlatformPublishTokenSecret` (secret holding a platform API token) + target `PublishDestination: Platform`; locally started agents show rollout state *Not Defined*. Rollout/approval: platform skill.
* Target Platform tab: *Report to Platform* needs `HumanOS.Kernel.Base` in `Serilog:Using` (else silent; `No access token in secret '<name>'` = secret missing); self-signed platform certificates need the separate *Ignore Certificate Errors* switches for log sink and license.

## Verify with evidence

1. Build: no `[ERR]`. Gateway log: no `[ERR]`, `Successfully connected to the master controller`, `Actor '...' registered`.
2. Platform: agent listed and Active.
3. Payload dump = expected message; stream shows same values after one sampling interval.
