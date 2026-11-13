# Gateway and runtime: undocumented behaviour

Docs: `Installation/InstallationGateway.md`, `Manuals/HumanOS.Runtime/{BasicConfiguration,ProblemResolving,FleetManagement}.md`, `Manuals/HumanOS.IoT.Designer/{Build,Publish,LicenseManagement}.md`, `Manuals/HumanOS.SAL.ServiceController/`.

## Start and observe

```powershell
$rt = 'C:\Program Files (x86)\CyberTech\HumanOS.IoT.Runtimes\HumanOS.IoT.Runtime-win.trial.x86-v2.12'   # runtime folders: win.trial.x86, win.x86, win.x64, linux.x64, linux.arm
& "$rt\HumanOS.IoT.Gateway.exe" "-AppSettingsDir=$PWD\Build\default"      # no trailing backslash, quote paths with spaces
```

* `-AppSettingsDir` is the only required argument (Designer's Run button starts exactly this). `-VaultKey=<key>` only for `VaultKeyPerArgument` targets.
* Startup 10-30 s (plugin scan); stop with Ctrl+C. Run as console first (Windows asks firewall access, e.g. OPC-UA 4840), register service afterwards.
* Run in a background process and tail the log; never block the tool call; stop leftover gateways when done.
* Log: `Build\<target>\Data\Logs\<ServiceName>-<date>.log`. More detail: `AppConfig.LogMinimumLevel=Debug|Verbose` (`--settings.AppConfig.LogMinimumLevel=...` at build time).
* Payload dumps (`PayloadDumpEnabled`): `Build\<target>\Data\Dumps\NodeSpaceDataLogger\<Publisher>.log` = exactly what would be sent to the platform; works on trial.

Healthy start log: `Starting HumanOS ... (Version 2.12.x.y)`, `LicenseManager: ... License ok.`, `Kernel: Starting HumanOS Kernel...` (settings dump with `ServiceId`, `SharedSpaceType`, `LazyCompiling`), `Loaded N common assemblies for scripting`, `Plug-in [X_{guid}] found/enabled/instantiated`, `OpcUaServer: Server is running`. Licensed platform agent: `Successfully connected to the master controller`, `Actor '<name>' registered`.

## Script compilation happens here

`HumanOS.LazyCompiling=false` (default): scripts compile **at gateway start**; error:
`[ERR] <Name>: Preloading the script failed at load time. Scripter: Error compiling '<file>'. <file>: (27,17)-(27,29): Error CS0103: ...`
and the gateway **keeps running without that script**. Real check: build, start, grep log for `[ERR]`, `Error CS`. After a change: rebuild, restart (the copy under `Config\` runs).

## Trial runtime

Free, all features, any device address accepted; **expires 1 h after each start** (restart); shared node space ignored (`TRIAL VERSION: Shared nodespace is not supported.`) → **no platform connection**. Needs no License Portal credentials. Use for devices, scripts, OPC-UA server, MQTT, payload dumps.

## Licenses

* Device license per *protected* driver (`plugins.md`); device `Id` = license id. Missing program license: `LicenseManager: Unexpected error evaluating the program license ... Could not find ...\Licenses\HumanOS.lic`; license must match installed **edition**.
* Project license source: `Global` (BuildTool needs `--user/--password`) or `ProjectSpecific` (`<project>\.humanos\Licenses\`, no credentials). Cloned project with someone else's license: Designer, project root, "Project License", edit.
* Distribution `Rental`: `LicenseManager Source: Http`, `Path` **must end with `/api/v2`**, secret `AccessTokenSecret`.
* `Build/`, `Licenses/`, `Secrets/`, `*.lic` stay out of git.

## Windows service / update

* `RegisterService.ps1` / `UnRegisterService.ps1` generated into `Build\<target>\` (run as admin). Service name unique per host (use `$(Target.Name)`). Run-as user needs write access to `Data\` (else `UnauthorizedAccessException`, `attempt to write a readonly database`).
* Directory publish + `UpdateManager.SourcePath/BackupPath`: new package applied on **service restart**; replaces `Config`, `Licenses`, `Secrets`, `appsettings.json` (a secret missing from the new package is removed). Raise project `Version` per published change.
* Linux/Docker: see install doc; paths case sensitive, no `%ENV%` variables.

## Persistence recovery

Node space serialization errors at start (typically after model changes): back up `Data\`, then `-cleanRepository=nodes|all|history` as in `ProblemResolving.md`, or use an in-memory node space. SQLite `database is locked`: connection `BusyTimeout=5000;EnableWAL=true;`.
