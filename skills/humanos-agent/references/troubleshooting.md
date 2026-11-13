# Troubleshooting: message → cause → fix

Literal 2.12 messages. More: `Manuals/HumanOS.Runtime/ProblemResolving.md`, per-driver `TroubleShooting`/`GoodToKnow` (`docs-map.md`).

## Build

| Message | Cause | Fix |
|---|---|---|
| `[ERR] : Line: N; Position: M; ...` + `CheckProject: Parsing ScriptFile ... failed`, final `Ok`, exit 0 | script syntax / unresolved name | fix; always scan for `[ERR]` |
| `Parsing JsonFile <path> failed` | invalid JSON (comments, trailing commas) | fix file |
| `Validation of plugin <X> failed` | plugin config rejected | read following text; compare with plugin `_blank` settings |
| `Could not find template for device` / `does not match its template schema` | device points to missing/changed template | restore `DeviceTemplateId`; re-apply template or align by hand |
| `Unknown prefix 'slow'` (or `fast`/`medium`), OPC UA node | OpcUaControl doc shows `opc:Fast;`/`opc:Medium;`/`opc:Slow;ns=...`; 2.12 accepts only `opc:Available` and `opc:SignOfLife` | plain node id `ns=2;s=...` |
| `Device license '<id>' not found.` / `does not match the plugin id` / `the address` | licensed target: license missing or device changed | Designer License Manager; address change needs license update |
| build fails on licensed `Global` project | missing/invalid `--user/--password` | env-var credentials, `ProjectSpecific`, or trial target |
| project does not load, mentions MachineLearning/PeMiL | processor plugin in `Plugins` | remove (`plugins.md`) |

## Gateway start

| Message / symptom | Cause | Fix |
|---|---|---|
| `Preloading the script failed at load time ... Error CSxxxx` | C# compile error (BuildTool missed it) | fix, rebuild, restart; gateway runs without that script |
| `No valid configuration for device [<id>] found.` | driver `settings.json` lacks `Devices[].MainTaskProcessor` | add (`h2proj-format.md`) |
| no devices / OPC-UA server empty | `DeviceDetectors` missing/disabled; device not in `Config\Devices`; empty `ServiceRule` ids | add plugin + settings; check build output and rules |
| configuration not found | `-AppSettingsDir` with trailing `\`, wrong folder, unquoted spaces | fix argument |
| `Could not find shared nodespace type '<type>'` | typo in `HumanOS.SharedNodeSpace.Type` | exact `...TSharedNodeSpaceHub, HumanOS.PeMiL.MQTTMessageBus`; bus not in `Plugins[]` |
| `TRIAL VERSION: Shared nodespace is not supported.` | trial runtime | licensed runtime |
| `No shared nodespace configured.` | `SharedNodeSpace.Type` empty | fine standalone; platform needs Type + `MasterSystemControllerId` |
| `LicenseManager: Unexpected error evaluating the program license ... HumanOS.lic` | no/invalid license for edition | `gateway-runtime.md` |
| `Could not find node '<guid>'` (NodeSpaceDataLogger) | `Connection` node unreachable: trial, not connected, wrong id | licensed runtime; check master connection and id |
| `UnauthorizedAccessException`, `attempt to write a readonly database` | `Data\` not writable for run-as user | grant access |
| node space serialization errors | old persisted data after model change | back up, `-cleanRepository` (`gateway-runtime.md`) |

## Devices and data

| Symptom | Cause | Fix |
|---|---|---|
| device never connects | address, firewall, wrong driver architecture | FANUC/Heidenhain/Andron/Balluff/Makino/NUM need x86 runtime; read driver manual |
| FANUC `FOCAS3 does not support HSSB` / `BadNotSupported` | `focas3://hssb@...`; tool life/servo trace over FOCAS3 | `focas3://<ip>:8193` (500i) or FOCAS2 |
| MQTT node never updates | wildcard topic; `ContentType`/`Mapping:Value`; `Access.Receive` false | one exact topic per node; processing script for filters |
| nodes `BadNotActive` (0) after restart/disconnect | drivers set it on disconnect | wait for reconnect |
| logger sends nothing for a node | node lacks `NodeFilter` property; `ProcessOnlyValidData` and state not Good; missing `ServiceRule` | add property, check state/rule, inspect payload dump |
| stream rejects data / fields missing | `DataSets[].Name`/field names differ from StreamModel; `State` 0 | align names; `State` 1 |
| values stale though device fine | task processor slices too large; cycle-time violation restarts connection | tune `Max/MinSchedulingTimeSlice`, check `EnableCycleTimeSurveillance` log |
| MySQL `ERROR 1101 BLOB, TEXT ... default value` | MySQL 8 with entity defaults | MariaDB or avoid TEXT defaults |
| high CPU | script `while` without delay; antivirus | `await Task.Delay`; exclude folders |

## Platform connection

| Symptom | Cause | Fix |
|---|---|---|
| connected, **no data on platform** | agent pending (Draft) or blocked (Inactive); or trial | approve (humanos-platform) |
| agent not listed | trial; no `SharedNodeSpace`/`MasterSystemControllerId`; broker unreachable; > 100 pending agents | log for `master controller`; broker host/port/TLS |
| host conflict | same `ServiceId` on two hosts | new target/`ServiceId`, rebuild |
| platform log sink silent | *Report to Platform* off; `HumanOS.Kernel.Base` missing in `Serilog:Using`; `No access token in secret` | fix target Platform tab |
| rental license `Unauthorized`/`Unavailable` | token secret wrong, or `LicenseManager:Path` lacks `/api/v2` | fix |
| publish fails: secret missing/locked | `PlatformPublishTokenSecret` not set | store platform API token as project secret |
| files not updated after directory publish | gateway cannot read update folder; wrong `UpdateManager.SourcePath` | fix path/account, restart service |
