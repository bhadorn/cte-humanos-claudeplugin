# HumanOS.BuildTool: undocumented behaviour

Docs: `Manuals/HumanOS.BuildTool/HumanOS.BuildTool.md`, `Installation/InstallationBuildTools.md`. Call: `HumanOS.BuildTool.exe "<proj>.h2proj" --build --target=<name>` (no `--target` = all). `--extract-runtimes` lists targets with runtime; `--list-runtimes` installed runtimes.

## Output and exit codes (2.12)

| Situation | Output | Exit |
|---|---|---|
| success | `Building target(s)...`, `<target>:  HumanOS.IoT.Runtime win-x86 TRIAL 2.12`, `Ok` | `0` |
| project not found | `Could not find project file '...'.` then `Failed` | `-1` |
| unknown `--target` | `No target 'xx' found.` then `Failed` | `-1` |
| licensed `Global` project, missing/wrong `--user`/`--password` | build fails | `-1` |
| **script does not compile** | `[ERR] : Line: 28; Position: 18; ...` and `[ERR] : CheckProject: Parsing ScriptFile <path> failed. CSharp parsing failed.` | **`0`, last line still `Ok`** |

* Exit 0/`Ok` ≠ clean: scan whole output for `[ERR]`, `[WRN]`, `Failed`.
* Build checks only syntax and unresolved names; wrong usings, base classes, overloads, API mismatches show **only at gateway start** (`gateway-runtime.md`).
* Ignore `[WRN] : Optional scripting assembly 'SkiaSharp' not available for scripting`.
* Other `CheckProject` messages: `Device 'X' in target 'Y' does not match its template schema 'Z'`, `Could not find template for device ...`, `Parsing JsonFile ... failed`, `Validation of plugin X failed` (fixes: `troubleshooting.md`).
* Trial targets: `Downloading device licenses ignored for trial version.`, no credentials needed. Otherwise pass `--user`/`--password` from env vars.
* PowerShell: quote designer variables: `--publish-path=C:\Temp\'$(Target.Name)'`.
* No running Designer needed.
