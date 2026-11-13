# Worked example: hall temperature monitoring

One OPC UA node per hall delivers the temperature. The agent samples every 10 s into platform stream `HallTemperature`.
A `Hall` dashboard shows the live value and a 24 h line chart; e-mail notification above 30 degrees Celsius.

| Part | State |
|------|-------|
| `Agents/HallTemperature.Agents/` | Built with BuildTool (no `[ERR]`), started on trial gateway: payload script compiled, OPC UA device detected. Platform link not testable on a trial runtime. |
| `Platform/*.json` | **Not import-tested.** Derived from real exports (envelope, keys, collection ids) and public docs. Import into a **test tenant** first. Items marked (verify) are unconfirmed. |

```
OPC UA server (node per hall)
 -> HumanOS.UHAL.OpcUaControl (data node per hall, property HallId)
 -> HumanOS.PeSeL.NodeSpaceDataLogger (publisher HallTemperatureLogger, SamplingRate 10, payload script)
 -> shared node space (MQTT) -> stream HallTemperature (RefId = Hall.Id)
 -> dashboard HallDashboard (LifeValueWidget + EntityChartWidget), notification event "Hall Over Temperature"
```

## Name contract (exact, case sensitive)

| Name / id | Platform | Agent |
|-----------|----------|-------|
| Stream name | `StreamModel.Name` = `HallTemperature` (`20-...`) | `DataSets[].Name` (`NodeSpaceDataLogger/settings.json`); payload script writes `"Stream"` |
| Field | stream field `Temperature` (System.Double) | payload `Fields.Temperature` |
| Reporting object | `Hall.Id` (`15-...`: `3f6c1b52-8d0a-4e7b-9a41-5c2e7d8b1a01` Hall 1, `...1a02` Hall 2) | data node property `HallId` (same GUIDs), sent as `RefId` |
| Stream model of RefId | `StreamModel.RefModelId` = Hall model id `a13c4a80-d850-4122-a69d-0282b3cd6071` | - |
| Data state | `State` 1 = Good (enum: `humanos` skill) | `item.DataState` -> `State`; `ProcessOnlyValidData: true` |
| Widgets | `LifeValueWidget.Properties.StreamName` = `HallTemperature`, `FieldName` = `Temperature` | - |
| Report | `ReportSchema.Name` = `Hall.TemperatureGraph` = `EntityChartWidget.ReportSchemaName`; `Config.SourceName` = `HallTemperature`, `SourceFieldName` = `Temperature` | - |
| Notification | event `TriggerCondition` reads `Temperature`; `SourceStreamId` = stream id, `RefModelId` = Hall model id | - |
| Logger node filter | - | node property `EnableHallTemperature` = true (`NodeFilter`) |
| Menu | path `/Hall/ListView` (opens `HallDashboard` via `DetailsViewName`); view `Policy` `POLICY_Operator` = menu `Policy` | - |
| Language keys | `LanguageText.Name` `$Hall`, `$Halls`, ... (`60-...`) | - |
| Agent identity | listed and approved in Automation > Agents | `HumanOS.ServiceId` (unique), shared node space address and master id |

## Platform files, import order

Order: 10, 15, 20, 30, 40, 50 (one variant), 60. Model first (stream references it by id); views refer to stream/report
by name; menu late (server prunes items whose target is missing); until `60-` the UI shows raw `$Keys`. Import as
SystemAdmin into a test tenant (list menu "Import" or `POST /api/v2/<tenant>/entities/directimport`, `humanos-platform`
`references/import-export.md`). Fixed ids: re-import updates in place. In a customer repo the files go to
`src/Platform/` with the same numbering (`15-` and menu variants are example-specific).

| File | Content |
|------|---------|
| `10-model-Hall.json` | Model `Hall` (tenant based, Active, field `OpcNodeName`), views `DetailsDashboard`, `ListView`, entity dashboard `HallDashboard` (nested as exported) |
| `15-entities-Hall-samples.json` | 2 Hall entities, fixed ids (sent as `RefId`). Replace `<TENANT-ID>` first (import fails on the placeholder). Entity import shape (verify); alternative: create halls in UI, copy each `Id` into node property `HallId` |
| `20-streammodel-HallTemperature.json` | Stream `HallTemperature`, field `Temperature`, `StoreDuplicatedEntries: true` (10 s samples repeat values), watchdog 60 s, retention 365 days |
| `30-reportschema-HallTemperatureGraph.json` | Report `Hall.TemperatureGraph`, line chart |
| `40-notificationsource-HallTooHot.json` | Event `Hall Over Temperature`: `n.readField<double>("Temperature", 0) > 30`, delay 30 s, cool down 1 h, built-in `Notification` model (empty `NotificationModel`) |
| `50-navmenu-2.2-setting-NavigationMenu.json` | 2.2: Setting `NavigationMenu` (`Value` = menu JSON string) |
| `50-navmenu-2.3-NavMenu-record.json` | 2.3: record of model `NavMenu` |
| `50-navmenu-halls-group.json` | Only the group "Hall monitoring", to merge into an existing menu |
| `60-languagetext-Hall.json` | EN/DE texts for every `$Key` |

### Menu 2.2 vs 2.3

- **2.2 (customers run this)**: menu = system setting `NavigationMenu`, looked up **by name**; its `Id` differs per installation. Do **not** import the file as is: it carries placeholder `<ID-OF-EXISTING-NavigationMenu-SETTING>` so it cannot overwrite the menu. Export/query the existing setting, insert the group of `50-navmenu-halls-group.json` into its `Value` (edit as text, keep one JSON string), import with its own `Id`.
- **2.3**: replaced by record `NavigationMenu` of model `NavMenu` (field `Schema` = menu JSON, parent application `WebPortal`); an update keeps the old one as `NavigationMenu.Backup22`, no longer read. Ids in the 2.3 file are those of the built-in portal menu (verify against your 2.3 instance). Edit in web application designer or `NavMenu` list; merge the group, do not replace the menu.

## Agent project

`Agents/HallTemperature.Agents/`: target `HallAgent`, device `HallTemperatureServer` (OPC UA, nodes `Hall1Temperature`, `Hall2Temperature`), plugins `HumanOS.UHAL.DeviceDetectors`, `HumanOS.UHAL.OpcUaControl`, `HumanOS.PeSeL.NodeSpaceDataLogger` (payload script `HallTemperaturePayload.cs`). Placeholders (none secret):

| Placeholder | Where | Meaning |
|-------------|-------|---------|
| `<OPC-UA-SERVER-HOST>` | `Devices[].Address` in h2proj and `HallAgent/Devices/HallTemperatureServer.json` | `opc.tcp://host:4840` |
| node ids `Hall1.Temperature`, `ns=2` | device template and device (`Address`, variable `OpcNameSpace`) | real node ids |
| `<PLATFORM-MQTT-BROKER-HOST>`, `<PLATFORM-SERVICE-ID>` | target `AppSettings` | from the platform administrator |
| `Runtime` | target | `HumanOS.IoT.Runtime win-x86 TRIAL` = local tests only; platform link needs licensed `win-x64` + project license in Designer |
| `HumanOS.ServiceId` | target `AppSettings` | unique per agent; a copy needs a new one |

- Security: `SecurityMode: NotSet`, no vault key, no secrets (OPC UA anonymous, security `None`, `AcceptAll` certificates): **test setup only**. For OPC UA login create a Designer secret, set device property `SecretName`; needs a vault key kept outside git.
- Logger `Connection` `DE891DEF-4478-4A38-BB89-533F669AF645` = platform node accepting stream data, from public ctProduction agents (verify for your platform version).
- `SamplingRate: 10` + `SamplerType: OnlyLastValue` = last value per node every 10 s; stream needs `StoreDuplicatedEntries: true`, else identical consecutive values are dropped.
- OPC UA address prefix `opc:Slow;` (documented) was refused by the 2.12 gateway (`Unknown prefix 'slow'`): addresses have no prefix.
- `Build/` is generated, never copied or committed.

## Steps with expected evidence

1. **Import** platform files into the test tenant (order above). Evidence: `Succeeded` per file, again on 2nd import; model query with children shows fields and 3 views.
2. **Users**: one test user per role to check (Operator, Supervisor, TenantAdmin), with e-mail. Evidence: menu "Hall monitoring" visible for Operator and up; Guest does not see it.
3. **Build**: copy the project to a work directory, `"C:\Program Files (x86)\CyberTech\HumanOS.BuildTool\HumanOS.BuildTool.exe" HallTemperature.Agents.h2proj --build --target=HallAgent`. Evidence: `Ok` and **no `[ERR]`** (prints `Ok` also on script errors).
4. **Run**: `HumanOS.IoT.Gateway.exe "-AppSettingsDir=<abs path>\Build\HallAgent"`, read `Build\HallAgent\Data\Logs\*.log`. Evidence: `Compile C# script 'HallTemperaturePayload.cs': Creating instance completed`, `Device [HallTemperatureServer@...] detected`, no `[ERR]` for data nodes; licensed runtime also `Successfully connected to the master controller`.
5. **Approve**: Automation > Agents, new agent is `Draft`; SystemAdmin approves. Evidence: Active, health ok. No data before.
6. **Payload dump**: `Build\HallAgent\Data\Dumps\NodeSpaceDataLogger\HallTemperatureLogger.log` shows `{"Messages":[{"Stream":"HallTemperature","RefId":"3f6c1b52-...","State":1,"Fields":{"Temperature":21.4,...}}]}` every 10 s.
7. **Stream**: Automation > Data Streams > `HallTemperature` has rows for both RefIds, state Good, count grows ~every 10 s.
8. **Dashboard**: Hall monitoring > Halls > Hall 1. Evidence: live value = OPC UA value; header time range "last 24 hours" shows the series (selector wording: verify).
9. **Notification** (live items below): set one hall's OPC UA node above 30 for 30 s or more. Evidence: notification entity, then e-mail to rule recipients; send a test first with action `sendTestRule`.

## Live-only (not shipped; installation specific)

- **Channel** (System Settings > Notifications > Channels): type `EMail`, sender, host, port, secret for login.
- **Rule**: event `Hall Over Temperature`, the channel, recipients (users and/or roles e.g. Supervisor, or external addresses), `Send after` `00:00:00`.
- **Users** with e-mail for every recipient.
- Agent approval (step 5); Hall entities if `15-...` not imported.

## Roles in this example (role list: `humanos` skill)

| Role | May |
|------|-----|
| Guest | nothing (menu hidden) |
| Operator, Supervisor, TenantAdmin, TenantAdmin_BO, SystemAdmin | open menu, list, dashboard (`POLICY_Operator` and higher) |
| Customer | not in `POLICY_Operator`: no dashboard access |
| TenantAdmin and higher | create/edit/delete halls (`POLICY_TenantAdmin` on model actions) |
| SystemAdmin | import, approve agents, create channels |
