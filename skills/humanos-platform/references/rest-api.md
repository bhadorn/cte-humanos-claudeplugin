# REST API: call shapes and helper

Docs: `Manuals/RestApi/*`. Instance OpenAPI `https://<host>/swagger` (unless disabled) is the authority for your version/product endpoints. Base URL, auth, `state` check, `executeaction` envelope: `SKILL.md`.

## Conventions
- Stream, accounts, licenses, product routes have **no** tenant segment (stream route with tenant = 404).
- Token `hos_...` or JWT in env var `HOS_API_TOKEN`, per session; never in file/repo/log/chat; revoke after use.
- `state`: `Succeeded` | `PartiallySucceeded` | `Failed` | `Abort`; HTTP 200 + `Failed` is normal for business errors.
- `queryentitylist`: 20 rows default, page with `Count`/`Offset` to `totalCount`. Hidden by default: Deleted/Archived (without `LifeCycleFilter`), templates (without `MainFilter`), other tenants' rows (tenant route).
- Exact field names: `GET <t>/entities/getentityschema?CollectionId=`.

## executeaction variants
| Intent | Body essentials |
|---|---|
| create | `entities[0].fields` incl. `LifeCycleState: "Active"` and `OwnerTenantId` (tenant-based). **No `Id`** (non-empty = clone source) |
| update | `entities: [{Id, fields:{...}}]`, one entity; values in `entities[].fields`, never `argumentValues`; optional `ExpectedRevision` |
| activate config entity | `update` `{LifeCycleState:"Active"}` (StreamModel before its fields) |
| delete | `entityIds:[id]`; permanent `argumentValues:{fields:{Force:true}}` (SystemAdmin) |
| custom action | `entityIds:[...]` (not `entities`), args in `argumentValues.fields` |
| clean | `argumentValues:{fields:{RetentionDays:0}}` (whole collection: ask first) |
Config create may return Draft: follow with `update`. Verify every state change by query.

## Stream calls (no tenant segment)
`POST /api/v2/Streams/CreateStreamData`: body = payload of `agents-and-streams.md` plus top-level `"DirectImport": false`. `true` writes past the queue (history).
`ReadStream {StreamName, From, To, QueryFilter, FieldNameFilter, MaxCount}`, `ReadHead {StreamName, QueryFilter}`, `QueryStreams {}` (all streams, queue depth, state).
Unknown stream: `Failed` without reason; other messages of the batch are stored. Whole batch refused for `RestrictedWrite` (SystemAdmin), `DisableRestReports`, full queue.

## PowerShell 5.1 helper
```powershell
# self-signed test hosts only; drop for trusted certificates
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
$Base = 'https://localhost'; $Tenant = 'abc'; $Token = $env:HOS_API_TOKEN

function Invoke-Hos([string]$Path, $Body, [string]$Method = 'Post') {
  $uri = "$Base/api/v2/$Path"
  $h = @{ Authorization = "Bearer $Token"; Accept = 'application/json' }
  try {
    if ($Method -eq 'Get') { $r = Invoke-RestMethod -Uri $uri -Method Get -Headers $h }
    else {
      $bytes = [System.Text.Encoding]::UTF8.GetBytes(($Body | ConvertTo-Json -Depth 30))
      $r = Invoke-RestMethod -Uri $uri -Method Post -Headers $h -ContentType 'application/json; charset=utf-8' -Body $bytes
    }
  } catch {
    $s = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    throw "HTTP error: $($s.ReadToEnd())"      # PS 5.1 hides the body otherwise
  }
  if ($r.state -and $r.state -notin 'Succeeded','PartiallySucceeded') { throw "state=$($r.state) reason=$($r.reason)" }
  return $r
}
$q = Invoke-Hos "$Tenant/entities/queryentitylist" @{ CollectionId = '505eda80-a750-4cbd-bed8-d2fcf771f4e4'; Count = 100; WithChildren = $false }
$q.entities | ForEach-Object { $_.fields.Name }

# push an import file (read as UTF-8, send raw bytes: no re-serialising)
$json = Get-Content -Raw -Encoding UTF8 .\src\Platform\10-models.json
Invoke-RestMethod -Uri "$Base/api/v2/$Tenant/entities/directimport" -Method Post -Headers @{ Authorization = "Bearer $Token" } `
  -ContentType 'application/json; charset=utf-8' -Body ([System.Text.Encoding]::UTF8.GetBytes($json))
```
- `ConvertTo-Json` needs `-Depth 20+`; `ConvertFrom-Json` unwraps single-element arrays (`@(...)`) and turns ISO dates into local `DateTime` (`.ToString('o')`).
- Shell state does not persist between tool calls: re-set `$Token` and functions each script. `.ps1` with umlauts needs UTF-8 BOM, or ASCII literals.

## curl (Git Bash)
```bash
export MSYS_NO_PATHCONV=1      # Git Bash rewrites /api/... otherwise
curl -sk -X POST "https://localhost/api/v2/abc/entities/directimport" -H "Authorization: Bearer $HOS_API_TOKEN" \
  -H "Content-Type: application/json" --data-binary @10-models.json    # -k only for self-signed test hosts
```
