# Plugins: undocumented rules

Names, **GUIDs**, runtimes: public catalog `Manuals/HumanOS.Runtime/Plugins.md`. Address formats, properties, commands: `Manuals/<PluginName>/` (`docs-map.md`). Not in the catalog = does not exist; ask CyberTech. Plugin/driver GUIDs are fixed product ids: keep when copying.

## GUIDs missing from the catalog

| Plugin | Id |
|---|---|
| `HumanOS.UHAL.NoDevice` | `9CCCEFEC-180B-4D47-AC70-99360649856A` |
| `HumanOS.UHAL.AllenBradleyControl` (experimental) | `E6A0F206-EC58-4581-933A-6072DE31B5E3` |

## Rules for `Targets[].Plugins[]`

| Rule | Detail |
|---|---|
| Always list `HumanOS.UHAL.DeviceDetectors` (`9D078B95-6322-44D2-989D-87A1FCA283B9`) | plus `<target>\HumanOS.UHAL.DeviceDetectors\settings.json` = `{ "Disabled": false }`; else no device instantiated. The detector page does not say it is mandatory. |
| Never list `HumanOS.PeMiL.MachineLearning`, `PlatformBinding`, `LanguageModels` | auto-loaded. PlatformBinding page says "required for platform projects" = loaded by runtime, not listed. Listing breaks project loading. |
| Never list the shared node space bus (`HumanOS.PeMiL.MQTTMessageBus`, `RmQMessageBus`) | selected via target `SharedNodeSpacePluginId` + `HumanOS.SharedNodeSpace.Type` (`platform-connection.md`) |
| Persistence (SQLite/MySQL/PostgreSQL) | selected by target database plugin + `HumanOS.Repository.*`, not hand-listed |
| Every driver needs `settings.json` with `Devices[].MainTaskProcessor` | else `No valid configuration for device [<id>] found.` |
| Service plugins bind via `ServiceRule` | empty `BindingRuleId`/`UnbindingRuleId` = no rule = no data; copy ids from an example |
| Script kinds | a plugin accepts only its own script types (`scripting.md`) |

## Driver caveats

| Topic | Caveat |
|---|---|
| `win-x86` only | FANUC, Heidenhain, Andron, Balluff, Makino, NUM (Axium/Flexium/FlexiumPro): 32-bit vendor libs; check manual "Supported Platforms" |
| Device license | "protected" drivers need one on non-trial runtimes; device `Id` = license id; changing the address invalidates it. Not needed: FileReader, NoDevice, Makino, services |
| FANUC | `focas3://hssb@...` unsupported (HSSB needs FOCAS2); FOCAS3 (500i) answers `BadNotSupported` for tool life management and servo trace (use FOCAS2 or drop the skill) |
| MQTT driver | one node = one exact topic, no wildcards; filters via MQTT processing script; node needs `Access.Receive: true` |
| FileReader | relative address resolves against gateway `Data` folder (`${AppConfig:CommonAppPath}`); prefer absolute paths |
| Disconnect | drivers set nodes `BadNotActive` (expected after restart/reconnect) |

## Service caveats

| Plugin | Caveat |
|---|---|
| `HumanOS.PeSeL.OPCUAServer` | one server per gateway; exposes all bound devices; default port 4840; certificate created on first start (`Missing certificate ... Try to create certificate` is normal) |
| `HumanOS.PeSeL.MqttServer` | built-in broker, not the platform broker |
| `HumanOS.PeSeL.NodeSpaceDataLogger` | agent → platform stream data (`platform-connection.md`) |
| Loggers (any) | node logged only if it carries the `NodeFilter` property; each publisher needs its own `ServiceRule` |
