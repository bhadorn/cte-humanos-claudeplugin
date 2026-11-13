# Views, dashboards, reports, menu: caveats

Docs: `Manuals/ViewDesign/*` (incl. `Widgets/<WidgetName>`), `Manuals/Reporting/Reports`, `Manuals/RestApi/Views`. JSON: `import-shapes.md`. Symptoms (5 rows, columns > 12, tabs/modules, donut, scheduler, menu policy/`???`): `gotchas.md`.

## Views
| Topic | Caveat |
|---|---|
| Inheritance | views **not inherited** from base models |
| Generated default views | `ListView`, `DetailsDashboard` created once; customise: same Name + existing Id; rebuild: delete generated rows |
| Entity-specific details view | view `<ViewName>_<entity-id-lowercase>` overrides the standard one for that entity |
| Dashboard columns | sum of `Columns[].Value` per row <= 12 |
| Widget `VisibilityExpression` | sees only the surrounding entity, evaluated once, no live values: `HideWhenEmpty` on live widgets |
| Widget `Buttons` | drawn only by `ActionBarWidget`, `TextViewWidget` |
| Widget `ModuleId` | only on widgets and report tables |
| Widget names | exactly the doc page file name; old-note `ActionCommandWidget` = `ActionWidget` |
| Scheduler | generic path needs `StartFieldName`, durations `EndFieldName` or a module |
| Edit dialog | field order = `ModelField.DisplayRank`, not `VisibleFields` |

## Menu items (view menus, widget `Buttons`)
Only `ActionName`, `Danger`, `DialogTitle`, `NavigateOnSuccess`, `SubItems`, `VisibilityExpression` (ActionBar/TextView only) survive. `Icon`, `DisplayName`, `SelectionMode`, `ExecutionPolicy`, `PreCondition*` overwritten from the ModelAction; unknown `ActionName` removed. `PreConditionExpression` cannot differ per view, fails **open** on parse error. `ToolMenu` items used as configured.

## Reports
- `ReportSchema.Name` unique across all reports, case-insensitive, deleted included; report in use cannot be renamed/deleted. `EntityChartWidget.ReportSchemaName` resolves against it. Only `Active` served.
- `Stream`/`StreamingHead` sources return all tenants' rows.
- `sum`, `dailySum`, `dailyCount`, `average`, `averagePerLabel` stream-only; `Model` source: `count` or plain value. `average` = one mean over all samples in range (not per day). **2.3:** `averagePerLabel` (one point per value label; line charts over raw samples).
- Stacked counts: same `ValueLabelTemplate` on all series.

## Navigation menu
- **2.3:** record `NavigationMenu` of model `NavMenu` (field `Schema`); old setting no longer read (kept as `NavigationMenu.Backup22`). **2.2:** `Setting` `NavigationMenu`. Check which exists. JSON: `import-shapes.md`.
- Server prunes items by policy, module, target existing and `Active`; paths with query string (`/System?tab=Agents`) or unknown routes dropped.
- What users see: `GET <tenant>/authview/getnavigationmenu`.
