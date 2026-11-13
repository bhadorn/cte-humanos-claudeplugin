# Concepts: tenants, roles, ids

Docs: `Manuals/Principles`, `Manuals/BusinessModels/Tenant|User`, `Manuals/RestApi/Authentication|Licenses`, `Manuals/LicenseManagement`. Only caveats and ids the docs lack.

## Tenants
| Fact | Note |
|---|---|
| `global` tenant | sees all tenants' objects, **owns nothing**; route `.../api/v2/global/...` |
| Visibility | tenant route returns only entities owned by or shared with (`AdditionalTenantIds`) that tenant; empty `OwnerTenantId` = invisible everywhere |
| After creating a tenant | user must log in again to get its policies |

## Roles and policies
- Role list: `SKILL.md`. Stored in `User.TenantRoles` as `{ "<tenant-guid>": "<RoleName>" }`, per tenant, assigned by SystemAdmin.
- **`TenantAdmin_BO` is derived**: claim added when a user holds `TenantAdmin` at the business-owner tenant (the tenant that set up the platform). Write `TenantAdmin`; it cannot be an API token's role either.
- Hierarchy low to high: Customer < Operator < Supervisor < TenantAdmin < TenantAdmin_BO < SystemAdmin. `POLICY_Operator` and `POLICY_Customer` are **separate branches**: both admit Supervisor and above; neither admits the other.
- `POLICY_Guest` has an empty role list: not for access control; use `POLICY_Customer` or above.
- Policy names go into `ModelAction.ExecutionPolicy`, `ModelView.Policy`, `ModelField.Policy` (restricts writes), menu item `Policy`.
- Built-in actions (`create/update/delete/clone/clean/import/export`) default to `POLICY_SystemAdmin`; open per model via `ActionConfiguration.<action>.ExecutionPolicy`.
- API token with `role` is capped to it; `readOnly` = reads only. **2.3:** tokens issued before 2.3 are inactive: reissue.

## Modules
`ModuleId` gates widgets, report tables, report schemas (`SourceType: Module`), menu items (`Modules: [guid]`). **Detail-view tabs cannot be gated.** Module ids: `GET /api/v2/licenses/getstate` on the instance.

## Collection ids not in the public docs (same on every instance)
| Model | Id |
|---|---|
| StreamModel | `09A313D2-B9E4-4D54-99C2-B95B0C21A707` |
| StreamModelField | `CE0737E6-D4E0-41B6-B817-9F6175670AC9` |
| NotificationSource | `87451EAC-4FBB-4C07-8B40-DC212F1A351E` |
| Setting | `949BE6A9-9428-46BE-82BB-B19910B1DADA` |
| Task | `7BF2542F-9E10-4076-A52E-14D59A424ED5` |
| SnapshotConfig | `0121B1CC-57E8-4B1D-AAAF-69DAEDBDEDCA` |
| PipelineTemplate | `D1CAFF1F-95F2-4C13-91B5-92EA5575AC8D` |
| AuthorizationPolicy | `762891E8-839B-4124-807B-D3F70A0926CA` |
| Role | `35BF427F-5C7B-4993-A10D-4268E67615B3` |
| Application | `39A960CA-ACC8-49CE-9C4E-AB3E104A1EB3` |
| `AbstractMainEntity` (abstract) | `B0AE50BE-597B-4A68-AB25-CA5FDBD76B00` |
| `AbstractTenantBasedEntity` (abstract) | `D8149CEE-BC85-47B9-A14E-60A3C79A354D` |
Public in docs: Model, ModelField, ModelAction, ModelView, DataType, Tenant, User, Language, LanguageText, NavMenu, Page, Document, Image. Custom models get their own generated id. Other ids: `queryentitylist` on Model collection, `MainFilter: entity.getValue<string>("Name") == "<ModelName>"`.
Model `BaseModelId` must be an **abstract** model. Notification models inherit `AbstractNotificationEntity` (look up by name).

## Settings
`Setting` (`Name`, `DataType`, `Value` string, `DisplayRank`) is read in field defaults as `$(Setting.<Name>)`. Never export/restore `PlatformVersion`/`ProductVersion`. `appsettings.json` is not an entity: `Setup/PlatformConfiguration`.
