# C# scripts in agents (2.12): undocumented facts

Docs: `Manuals/HumanOS.Runtime.SDK/` (classes, `PeSelScripts`, `UHALStreamScripts`, `PlatformIntegration`), `Tutorials/` (4 scripts, 5 workflows, 6 aggregation), `Development/` (IDE; debugging only with x64 runtime), `Manuals/HumanOS.Runtime/GenericInformationModelNodes/{Rules,Timers,Workflows}.md`.

## Rules

1. C# (Roslyn), one public top-level class per file, compiled by the gateway **at start** (`gateway-runtime.md`). Namespace free.
2. **Start from a template or working example** (`examples.md`; Designer templates `...\HumanOS.IoT.Designer\Library\Scripts\Blank_*.cs`). Use no API absent from SDK docs or an example.
3. Script kind = where it is registered (plugin `ScriptFiles`, device `TaskProcessors[].ScriptFile`, processing network `InitialData`, `Scripts.Rules|Commands|Workflows|Processing`); must derive from the matching base class below. A plugin accepts only its own types.
4. Never block: no `while` without `await Task.Delay` (CPU). Prefer `...Async`. Queries (`getNode`, JSON) return null for missing items.
5. Log with `Logger.writeInfo/writeDebug/writeWarning/writeError`; in pipeline operation scripts `writeInfo/writeError` also report to the platform job.

## Base classes by extension point

| Registered as | Base class (namespace) | Entry point |
|---|---|---|
| WebControl task processor (polling) | `TAbstractHttpStreamScriptObject` (`HumanOS.Kernel.UHAL.Script`) | `void handleStream(IKernelAccess, TDeviceSchemaInfo, IHttpStream)` |
| TcpClient / Serial / Bluetooth / ProGlove task processor | `TAbstractStreamScriptObject2` | `connectToDeviceAsync`, `disconnectFromDeviceAsync`, `handleStreamAsync(Kernel, DeviceInfo, IDataStream, Token)` |
| MqttControl processing | `TAbstractMqttProcessingScriptObject` (`HumanOS.UHAL.MqttControl.Script`) | `doProcess(Kernel, DeviceInfo, Logger, IMqttClient)` |
| Device command logic | `TAbstractLogicScriptObject2` (>= 2.11; older `TAbstractLogicScriptObject` deprecated) | `Task executeCommandAsync(IKernelAccess, IGroupRelation DeviceNode, ICommandCallContext, CancellationToken)` |
| State machine in a device | `TAbstractStateMachineScript` (`HumanOS.Kernel.Processing.StateMachine`) | `onEnterStateAsync`, `onExitStateAsync`, `onInternalTransitionAsync`, `processAsync` |
| Processing network node (`CSharpScriptProcessingNode`, `InitialData: ["X.cs"]`) | `TAbstractProcessingScriptObject` (`HumanOS.Kernel.Processing`) | `void process(IProcessingNode)`; ports via `getProperty<T>/setProperty<T>`, ports carry `PortMatchId` |
| Rule (`GenericRule`) | `TAbstractRuleScriptObject` (`HumanOS.Kernel.DataModel.Rules`) | `void execute(IKernelAccess, IVariablePool)`; `OnTimer` fires only with an `ITimingNode` timer in the same group |
| Global command | `TAbstractCommandScriptObject2` (`HumanOS.Kernel.Script`) | `Task<TCommandResult> executeCommandAsync(ICommandCallContext, CancellationToken)` |
| Workflow activity | `TAbstractOperationScriptObject` (`HumanOS.Kernel.Workflow.Activity`) | `Task runAsync(IKernelAccess, IActivity, CancellationToken)` |
| **Platform pipeline job** | `TAbstractPipelineOperationScriptObject` (`HumanOS.PeMiL.PlatformBinding.Pipelines`) | below |
| NodeSpaceDataLogger / RestDataLogger payload | `TAbstractDataLoggerScriptObject<string>` (`HumanOS.Kernel.PeSeL.DataLogger`) | `initialize`, `string[] processPayload(Kernel, Context, List<TDataSet>)`, `postProcess` |
| MQTTClient / RabbitMq / AzureIoT payload | `TAbstractDataLoggerScriptObject<byte[]>` | same, returns `byte[][]`; `Context.setValue("Topic", ...)` sets topic |
| MQTTClient / RabbitMq consumer | `TAbstractDataConsumerScriptObject<string>` | parse incoming payloads |
| WebService | `TAbstractWebScriptObject` (`HumanOS.PeSeL.WebService.Script`) | `handleGet/Post/Patch/Delete(...)` |
| McpServer tool | `TAbstractMcpScriptObject` (`HumanOS.PeSeL.McpServer.Script`) | `Task<string> executeAsync(IKernelAccess, TMcpToolCallContext, CancellationToken)` |

Pipeline job scripts go in `Scripts.Workflows[]` (e.g. `Platform.<Name>.cs`); the platform pipeline names the skill/actor.

## Pipeline job: SDK page outdated

`Manuals/HumanOS.Runtime.SDK/PlatformIntegration.md` shows `runJobAsync(Kernel, Activity, string strTriggerAction, IEnumerable<Guid> EntityIds, ...)`. 2.12 uses `TPipelineExecutionContext` (`.TriggerAction`, `.TriggerActionArguments` = JSON string, `.DataInput` = JSON list of `TEntityDataJM`):

```csharp
using HumanOS.Kernel; using HumanOS.Kernel.DataModel; using HumanOS.Kernel.Utils; using HumanOS.Kernel.Workflow.Activity;
using HumanOS.PeMiL.PlatformBinding.JsonModels; using HumanOS.PeMiL.PlatformBinding.Pipelines;
using Newtonsoft.Json; using System; using System.Collections.Generic; using System.Linq; using System.Threading; using System.Threading.Tasks;

public class TMyJobOperation : TAbstractPipelineOperationScriptObject
{
  protected override async Task<TPipelineOutputJM> runJobAsync(IKernelAccess Kernel, IActivity Activity,
                                                               TPipelineExecutionContext PipelineContext, CancellationToken Token)
  {
    TPipelineOutputJM Retval = new TPipelineOutputJM();
    try
    {
      var dicArgs = JsonConvert.DeserializeObject<Dictionary<string, object>>(PipelineContext.TriggerActionArguments);
      var lstData = JsonConvert.DeserializeObject<List<TEntityDataJM>>(PipelineContext.DataInput) ?? new List<TEntityDataJM>();
      switch (PipelineContext.TriggerAction)          // action names as defined in the platform pipeline
      {
        case "start":
          var Job = new TEntityDataJM { Id = lstData.First().Id, CollectionId = lstData.First().CollectionId };
          Job.Fields["State"] = 400;                  // field names/values of the platform model
          Retval.Data.Add(Job);                       // entity modifications returned to the platform
          break;
        default: throw new ArgumentException($"Invalid trigger action '{PipelineContext.TriggerAction}'.");
      }
    }
    catch (Exception Exc) when (Exc.isNotCancelException()) { writeError(Exc.Message); throw; }
    return await Task.FromResult(Retval);
  }
}
```

## Logger payload skeleton (platform stream message)

Builds the message of the platform contract (`platform-connection.md`); full file: `TStreamingPayload_v2.cs` in the public ctProduction agents.

```csharp
public class TMyPayload : TAbstractDataLoggerScriptObject<string>      // usings: HumanOS.Kernel, ...PeSeL.DataLogger, Newtonsoft.Json.Linq
{
  public override void initialize(IKernelAccess Kernel, TPayloadProcessingContext Context) { }
  public override void postProcess(IKernelAccess Kernel, TPayloadProcessingContext Context) { }
  public override string[] processPayload(IKernelAccess Kernel, TPayloadProcessingContext Context, List<TDataSet> lstData)
  {
    JArray jMessages = new JArray();
    foreach (TDataSet DataSet in lstData)                              // DataSet.Name = StreamModel name
    {
      jMessages.Add(new JObject {
        ["Stream"] = DataSet.Name, ["RefId"] = DataSet.getFieldValue<Guid>("DeviceId"),
        ["TimeStamp"] = DataSet.getFieldValue<DateTime>("TimeStamp").ToString("o"),
        ["State"] = DataSet.getFieldValue<int>("State"),               // 1 = Good
        ["Fields"] = new JObject { ["Temperature"] = DataSet.getFieldValue<double>("Value") } });
    }
    return new[] { new JObject { ["Messages"] = jMessages }.ToString() };
  }
}
```

## Verify

Build, read `[ERR]` (exit stays 0); start gateway, grep `Preloading the script failed at load time` / `Error CS`; exercise path with simulator data, read log or payload dump.
