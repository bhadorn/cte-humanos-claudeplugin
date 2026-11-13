# Public examples to copy

Start from the closest working project, rename, new GUIDs (keep plugin/driver GUIDs).

* Use cases (22 + tutorial projects): <https://github.com/bhadorn/cte-humanos-runtime-usecases>; read `llms.txt` first. Project = `src/<Name>/` with `<Name>.h2proj`, `Readme.md`, `default/`, `DeviceTemplates/`. Most run on trial runtime. Tutorial projects: `src/Tutorial/` (docs: `Tutorials/`).
* ctProduction agents: <https://github.com/bhadorn/cte-ctproduction-agents> (`src/Workplace.Agents/`): licensed runtime + platform; agent-to-platform reference.

| I need ... | Copy (under `src/`) |
|---|---|
| minimal agent, no hardware | `TemperatureReporter` |
| timers | `TimingExample` |
| file / Excel | `FileReader` |
| MQTT in/out, SparkPlug | `Tutorial/Tutorial.Scripting`, `SparkPlugAlarming`, `SQLReader` (mutual TLS) |
| OPC-UA machine → OPC-UA server | `FanucRobotOpcUa`, `Traub` |
| FANUC / Heidenhain / Sinumerik | `FANUC.DataAggregation`, `FanucControl500i`, `Heidenhain/*`, `SinumerikPowerLine` |
| Siemens S7 structured data | `SiemensS7.ComplexData` |
| TCP/serial raw protocol | `TCPIPDemo`, `MoriSeiki` |
| REST client/server | `AnsibleAgent`, `Tutorial/Tutorial.WebService` |
| SQL | `SQLReader` |
| state machines | `FLAMESDemo` |
| ML / MCP | `ML.ImageClassification`, `FanucMcpServer` |
| CSV / Influx logging | `Tutorial/Tutorial.CSVDataLogger` |
| **agent → platform** (shared node space, `NodeSpaceDataLogger`, `TStreamingPayload_v2.cs`, actor/skills, pipeline scripts `default/Scripts/Workflows/Platform.*.cs`) | `cte-ctproduction-agents/src/Workplace.Agents` |

## Pitfalls when copying

* New `Targets[].Id`, `HumanOS.ServiceId`, device `Id`s (unique per platform; = device license ids on licensed targets).
* h2proj holds the author's encrypted vault key: use your own vault; no real secrets in published projects; don't copy `Build/`.
* Examples target trial `win-x86`; for platform agents switch to licensed runtime, set project license, add shared node space settings (`platform-connection.md`).
* Examples may need hardware/simulators from their README (FANUC NCGuide, Heidenhain simulator, MySQL, broker, webcam).
* Copy `DeviceTemplates/*.json` property flags (`Enable...`) together with the logger `NodeFilter`.
