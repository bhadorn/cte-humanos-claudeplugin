# Platform docs map

URL = `https://doc.cybertech.swiss/platform/<path>`. Paths ending `/` are folder indexes. Older versions: `/platform/<version>/...`. Runtime docs: `/runtime/intro`. Release notes: `ChangeLog` (`ReleaseNotes` does NOT exist). Other public sources: `github.com/bhadorn/cte-humanos-runtime-usecases` (llms.txt), `github.com/bhadorn/cte-ctproduction-agents`. Not in the docs: read the page, or query the instance.

| Topic | Path | Caveat |
|---|---|---|
| Intro, sidebar entry | `intro` | |
| Requirements | `Requirements` | unverified |
| Glossary | `Glossary` | |
| Setup (install, Docker, data migration, config) | `Setup/`, `Setup/PlatformInstallation` (DB, MQTT broker, ports), `Setup/DataMigration`, `Setup/PlatformConfiguration` | |
| Tenants, users, AD | `Setup/SetupTenant`, `Setup/SetupUser`, `Setup/ActiveDirectory` | |
| Tenants, roles, policies, tokens | `Manuals/Principles`, `Manuals/BusinessModels/Tenant`, `.../User` | |
| Models, fields, data types (type list, enums, auto-generate, defaults) | `Manuals/BusinessModels/BusinessModel`, `.../Model`, `.../ModelFields`, `.../DataTypes` | |
| Model actions, built-in actions, agent access | `Manuals/BusinessModels/ModelActions` | |
| Expressions | `Manuals/BusinessModels/Expressions/` | |
| Script classes | `Manuals/Objects/ClassObjects` | |
| Views | `Manuals/ViewDesign/`, `.../ViewDesign`, `.../ListView`, `.../DetailView`, `.../DashboardView`, `.../KanbanView`, `.../SchedulerView` | |
| Widgets (property tables) | `Manuals/ViewDesign/Widgets/`, `.../Widgets/<WidgetName>` | |
| Navigation menu | `Manuals/ViewDesign/NavigationMenu/` | |
| Reports | `Manuals/Reporting/Reports` | |
| Streams, payload | `Manuals/Streams/`, `.../Payload`, `.../TroubleShooting` | |
| Notifications | `Manuals/Notifications/`, `.../Notifications`, `.../NotificationSources`, `.../NotificationRules`, `.../NotificationService`, `.../NotificationModel` | |
| Liquid, pipelines | `Manuals/Pipelines/`, `.../Pipelines`, `.../Templating/` | |
| Forms | `Manuals/Forms/`, `.../Forms` | |
| Translations | `Manuals/Languages/` | |
| REST API | `Manuals/RestApi/` (`Authentication`, `Entity`, `Streams`, `Views`, `Licenses`, `Product`, `Swagger`) | |
| REST model-import traps | `Manuals/RestApi/DynamicModelDeploymentPitfalls/` | |
| Snapshots | `Manuals/SnapshotManagement` | partial backup/restore, not a full backup |
| Product package | `Manuals/ProductPackage` | unverified |
| Licenses | `Manuals/LicenseManagement/` | |
| Fleet (agents) | `Manuals/FleetManagement/`, `.../FleetManagement`, `.../ConfigurationRollout` | |
| Agent approval | `Manuals/FleetManagement/AgentApproval` | unverified; path from sources |
| Update, migration | `Maintenance/`, `Maintenance/Update_2.1_to_2.2`, `Maintenance/VersionMigration/`, `Maintenance/TroubleShooting` | |
| Tutorials | `Tutorials/` | unverified |
| ctProduction (MES product on the Platform; real models/streams/reports/notifications in `ctProduction/Design/*`) | `ctProduction/` | unverified |
