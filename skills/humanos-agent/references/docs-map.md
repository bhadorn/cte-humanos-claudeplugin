# Where in the docs (Runtime)

Read the page; do not rely on memory or this skill. Local copy: `C:\Program Files (x86)\CyberTech\HumanOS.BuildTool\Doc\` (paths below relative to it; may be older than web). Public URL: `https://doc.cybertech.swiss/runtime/<path without .md>`; a page named like its folder collapses to the folder URL (`Manuals/HumanOS.UHAL.OpcUaControl/`). Folder indexes end with `/`.

| Topic | Page | Caveat |
|---|---|---|
| Install BuildTool / Designer / Gateway, silent install | `Installation/InstallationBuildTools.md`, `InstallationDev.md`, `InstallationGateway.md` (index `Installation/`) | - |
| Requirements | web `Requirements` | unverified |
| BuildTool options, license handling | `Manuals/HumanOS.BuildTool/HumanOS.BuildTool.md` | `buildtool-cli.md` |
| Designer: project, target, plugins, templates, build, publish, secrets, licenses, StateMachine | `Manuals/HumanOS.IoT.Designer/{Introduction,CreateProject,Target,Plugins,DeviceTemplates,Build,Publish,SecretManagement,LicenseManagement,StateMachine}.md` | `h2proj-format.md` |
| Appsettings keys (service id, node space, repository, logging) | `Manuals/HumanOS.Runtime/BasicConfiguration.md` | - |
| Plugin catalog **with GUIDs** | `Manuals/HumanOS.Runtime/Plugins.md` | `plugins.md` (what not to list, missing GUIDs) |
| Plugin loading / `settings.json` / `ServiceRule` | `Manuals/HumanOS.Runtime/Plugins.md`, `Manuals/HumanOS.Kernel.PeSeL/HumanOS.PeSeL.ServiceRules.md`, `Manuals/HumanOS.Kernel.UHAL/HumanOS.Kernel.UHAL.Configuration.md` (driver settings, MainTaskProcessor) | empty rule ids = no rule = no data |
| Device information model, node types, rules, timers, workflows, actors/skills | `Manuals/HumanOS.Runtime/DeviceInformationModel.md`, `GenericInformationModelNodes/*.md` | - |
| Driver: address format, properties, commands | `Manuals/HumanOS.UHAL.<Driver>/{DriverConfiguration,DataAccess,Commands}.md` (e.g. `WebControl`, `MqttControl`, `OpcUaControl`) | `plugins.md` "Driver caveats" |
| Device detector | `Manuals/HumanOS.UHAL.DeviceDetectors/HumanOS.UHAL.DeviceDetectors.md` | **mandatory in every project** |
| Services: OPC-UA server, MQTT client/server, SparkPlug, CSV, Influx, SQL, REST logger, WebService, MCP | `Manuals/HumanOS.PeSeL.<Service>/{ServiceConfiguration,Publisher,Consumer}.md` (e.g. `OPCUAServer`, `MQTTClient`, `SparkPlugClient`, `McpServer`) | - |
| Common logger settings (SamplingRate s/0 = on change, SamplerType, filters, buffering, error policy, queries) | `Manuals/HumanOS.Kernel.PeSeL/HumanOS.PeSeL.DataLoggerConfig.md` | `platform-connection.md` |
| NodeSpaceDataLogger (agent → platform) | `Manuals/HumanOS.PeSeL.NodeSpaceDataLogger/{ServiceConfiguration,Publisher}.md` | `platform-connection.md` |
| Shared node space, fleet, master/agent | `Manuals/HumanOS.Kernel.PeMiL/HumanOS.Kernel.PeMiL.SharedNodeSpace.md`, `Manuals/HumanOS.PeMiL.MQTTMessageBus/{HumanOS.PeMiL.MQTTMessageBus,Configuration}.md`, `Manuals/HumanOS.PeMiL.PlatformBinding/` | `platform-connection.md` |
| Fleet management (rollout, rental licenses) | `Manuals/HumanOS.Runtime/FleetManagement.md` | - |
| Persistence | `Manuals/HumanOS.PeMiL.{SQLiteDataBase,MySQLDataBase,PostgreSQLDataBase}/Configuration.md`, `Manuals/HumanOS.Runtime/Historization.md` | - |
| Scripting SDK, classes, stream/PeSeL scripts | `Manuals/HumanOS.Runtime.SDK/{HumanOS.Runtime.SDK,NamespacesAndClasses,PeSelScripts,UHALStreamScripts,PlatformIntegration}.md`, `Classes/*.md` | `scripting.md` (outdated `runJobAsync`; `PlatformIntegration/` shows it) |
| IDE setup for scripts | `Development/ScriptingWithVisualStudio.md`, `ScriptingWithVSCode.md` | - |
| Tutorials (OPC-UA bridge, devices, logger, scripts, workflows, aggregation, complex types, REST; 8) | `Tutorials/` | - |
| Standard models (OEE, MDE, DNC) | `Models/` | - |
| Gateway: start, log, problem resolving, `-cleanRepository`, firewall, antivirus | `Manuals/HumanOS.Runtime/ProblemResolving.md`, `BasicConfiguration.md` | `gateway-runtime.md`, `troubleshooting.md` |
| Per-driver troubleshooting | `Manuals/HumanOS.UHAL.<Driver>/{TroubleShooting,GoodToKnow,BestPractices}.md` | - |
| Service controller (Windows service tray) | `Manuals/HumanOS.SAL.ServiceController/` | - |
| Manuals index (Kernel, PeMiL, PeSeL, UHAL, applications) | `Manuals/` | - |
| Release notes (latest 2.12.0 first), glossary | web `ReleaseNotes`, `Glossary` | - |
| Platform side | skill `humanos-platform`; `https://doc.cybertech.swiss/platform/intro` | - |
| Example projects | `examples.md` | - |
