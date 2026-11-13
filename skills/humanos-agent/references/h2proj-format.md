# .h2proj: what to know before editing

Docs: `Manuals/HumanOS.IoT.Designer/` (Introduction, CreateProject, Target, Plugins, DeviceTemplates, SecretManagement), `Manuals/HumanOS.Runtime/BasicConfiguration.md` (appsettings keys), `DeviceInformationModel.md` (device JSON).

**Never write an `.h2proj` or `settings.json` from memory.** Copy a working project (`examples.md`) or a Designer template (`C:\Program Files (x86)\CyberTech\HumanOS.IoT.Designer\Library\ProjectTemplates\`, Designer installed), change names, **generate new GUIDs** for target `Id`, `HumanOS.ServiceId`, device `Id`, template `Id`, script `Id`s, node ids. Keep fixed plugin/driver GUIDs (`plugins.md`).

## Layout facts

* `default/` is inherited by every other target (a plugin config in another target overwrites it). Pattern: `default` = shared plugins/scripts; a second target (e.g. `HOS.MyAgent`) adds the device and is the running agent.
* One target = one agent (own `HumanOS.ServiceId`). Never copy a built `Build\<target>\` to a second machine (host conflict).
* `Build\<target>\` is generated: never edit/commit. Rebuild replaces `Config\`, `Secrets\`, `Licenses\`, `appsettings.json` (experience: also wipes persisted node space in `Data\`; back up first).
* BuildTool does not re-derive devices from templates: editing by hand = change template **and** every device file built from it. Designer "Generate New Ids" re-apply changes all node ids and breaks OPC-UA clients.
* `globalids.json` maps node paths to stable GUIDs (loggers use `item.GlobalId`, platform entities refer to them). Adding nodes by hand: let Designer regenerate it or add matching entries.
* Project `Version` (2-4 dot-separated digit groups) → `appsettings.json` `Version` = configuration identity reported to the platform: raise on every change.

## Annotated excerpt (platform-connected agent target)

Verified shape (trimmed from a public agent project; 2.12 uses `"SyntaxVersion": "2.0"`, `"RuntimeVersion": "2.12"`).

```jsonc
{
  "Name": "HOS.MyAgent", "Id": "<new guid>",
  "Runtime": "HumanOS.IoT.Runtime win-x64",       // exact name from BuildTool --list-runtimes; "... win-x86 TRIAL" for local tests
  "RuntimeVersion": "2.12",
  "PublishDestination": "Directory",              // None | Directory | Platform
  "DataBasePluginId": "a3b98ddf-93fb-42a4-9de3-f711cce42011",       // SQLite persistence (selected here, NOT in Plugins[])
  "SharedNodeSpacePluginId": "0dae9067-44eb-40b9-9385-dd6efb353e11", // MQTT shared node space (selected here, NOT in Plugins[])
  "AppSettings": {
    "HumanOS.ServiceId": "<new guid per agent>",
    "HumanOS.ServiceName": "HumanOS.IoT.Gateway.$(Target.Name)",
    "HumanOS.LocalNodeSpace.Type": "HumanOS.Kernel.DataModel.Space.TPersistantMemoryNodeSpace, HumanOS.Kernel",
    "HumanOS.Repository.Type": "HumanOS.PeMiL.SQLiteDataBase.TNodeRepository, HumanOS.PeMiL.SQLiteDataBase",
    "HumanOS.SharedNodeSpace.Type": "HumanOS.PeMiL.MQTTMessageBus.TSharedNodeSpaceHub, HumanOS.PeMiL.MQTTMessageBus",
    "HumanOS.SharedNodeSpace.Address": "<platform MQTT broker host>",
    "HumanOS.SharedNodeSpace.MasterSystemControllerId": "<ServiceId of the platform server>",
    "HumanOS.SharedNodeSpace.MasterAgentOrchestratorId": "<same value>"   // set by the example agents; not in the public docs
  },
  "Plugins": [
    { "Name": "HumanOS.UHAL.DeviceDetectors", "Id": "9D078B95-6322-44D2-989D-87A1FCA283B9", "Path": "HumanOS.UHAL.DeviceDetectors" },
    { "Name": "HumanOS.UHAL.OpcUaControl",     "Id": "<driver guid from the catalog>",     "Path": "HumanOS.UHAL.OpcUaControl" },
    { "Name": "HumanOS.PeSeL.NodeSpaceDataLogger", "Id": "F433A67F-4216-4BAA-9CBE-555FD4905682", "Path": "HumanOS.PeSeL.NodeSpaceDataLogger",
      "ScriptFiles": [ { "Path": "TStreamingPayload_v2.cs", "Id": "<new guid>", "Name": "TStreamingPayload_v2.cs" } ] }
    // no MQTTMessageBus, no PlatformBinding, no MachineLearning here
  ]
}
```

* Trial runtime: leave `SharedNodeSpace.Type` empty (a value is ignored with a warning).
* Designer plugin selection sets `SharedNodeSpacePluginId` **and** `SharedNodeSpace.Type`; clearing empties the type and resets address to `localhost`. By hand set both.
* Shared node space resolves no secrets: broker `Username`/`Password` as `$(ENV_VAR)` (MQTT bus `Configuration.md`).
* Every script file needs its own GUID in `ScriptFiles[]` / `Scripts.*[]`.

## Plugin `settings.json` gotchas

* Driver without `Devices[].MainTaskProcessor`: `No valid configuration for device [<id>] found.` (FileReader creates a default, only warns). Defaults: `...\HumanOS.IoT.Designer\Library\Plugins\<PluginName>\_blank\settings.json` (Designer installed) or a working example.
* Empty `ServiceRule` ids = no rule created: copy from `_blank` or example.
* Strict JSON (no comments/trailing commas): `Parsing JsonFile ... failed`.

## Device JSON facts

* **Properties are the tag mechanism**: loggers select nodes by `NodeFilter` (e.g. `node.hasProperty<bool>("EnableMyStream", true)`); a node without the property is silently not logged; property type in the filter must match.
* Device `Id` in device JSON = `Id` in `Target.Devices[]` = device license id (protected drivers); unique across all agents of one platform.
* `$(PropertyName)` in an address resolves a device property; template `Variables` resolve at build.
* Repeated `PortMatchId` across (sub)devices wires processing-network data nodes wrongly: keep unique per device.

## Secrets

Credentials via Designer secret + reference by name (`SecretName`, `ConnectionSecret`, `PlatformPublishTokenSecret`), never in addresses, `settings.json`, h2proj, chat. `ProjectVaultKey` (h2proj) and `VaultKey` (`appsettings.json`) are key material: never print/commit.
