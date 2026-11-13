# Import shapes (minimal, annotated)

Real exports, shortened. `//` comments are annotations: **remove them**. Rules (ids, order, encoding, verify): `import-export.md`. Property meaning: doc pages (`docs-map.md`).

**Common to every entity (add to each `Fields`, omitted below):** `"BuiltIn": false, "IsTemplate": false, "LifeCycleState": 1, "TemplateId": "00000000-0000-0000-0000-000000000000"`.
**Wrapper** of every entity, top-level and nested child: `{ "CollectionId": "<its collection>", "Id": "<guid>", "TimeStamp": null, "Fields": { "Id": "<same guid>", ... } }`.
**File** = one collection: `{ "CollectionId": "<c>", "EntityName": "<model name>", "ExportedBy": null, "ExportedAt": null, "Entities": [ <wrapper>, ... ] }`.

| EntityName | CollectionId |
|---|---|
| DataType | `86f3f5f1-6ee7-4af8-a12d-05913e69cc58` |
| Model | `505eda80-a750-4cbd-bed8-d2fcf771f4e4` |
| ModelField | `18797c62-1f80-421e-9e1c-8a0faa98cfa7` |
| ModelAction | `c9ff4748-01e4-4fe0-b297-e99df3d4066b` |
| ModelView | `de1c0351-9ed5-4ae9-8a29-160b839dd45e` |
| StreamModel | `09a313d2-b9e4-4d54-99c2-b95b0c21a707` |
| StreamModelField | `ce0737e6-d4e0-41b6-b817-9f6175670ac9` |
| ReportSchema | `c7fb2c24-9721-45af-b57d-31a225631cb8` |
| LanguageText | `8ea18705-73b9-46e7-87ce-f9fe1098e49b` |
| NavMenu | `154C705E-DB86-41A9-AD20-018CFE41D652` |
Others: `concepts.md`.

## DataType (custom enum), `00-datatypes`
```json
{ "Id": "aea824ee-7fc4-4e42-9542-80f589942afa", "Name": "EPriority", "DisplayName": "$PriorityType",
  "BasedOn": "System.Int32",
  "FieldDefinition": [ { "Name": "Medium", "Value": 0, "DisplayName": "$Medium", "Icon": "equal", "IconColor": "#FF9933" },
                       { "Name": "High",   "Value": 1, "DisplayName": "$High" } ],   // {Name, Value} objects, never plain strings
  "StringLength": 0, "ValidationPattern": "", "ValidationHint": "" }
```

## Model with one field and one action, `10-models`
```json
{ "Id": "f935d924-ff43-4655-b511-290968a51f4c", "Name": "Hall", "DisplayName": "$Hall", "Description": "",
  "TableName": "plant.halls",                                    // unique, immutable
  "Type": "SimpleCollection", "Abstract": false,                  // ReadOnlyView needs MasterCollectionId
  "BaseModelId": "d8149cee-bc85-47b9-a14e-60a3c79a354d",          // AbstractTenantBasedEntity
  "Inheritance": ["b0ae50be-597b-4a68-ab25-ca5fdbd76b00", "d8149cee-bc85-47b9-a14e-60a3c79a354d"],  // whole chain; in every export, include it
  "MasterCollectionId": "00000000-0000-0000-0000-000000000000",
  "ActionConfiguration": { "create": { "ExecutionPolicy": "POLICY_TenantAdmin", "PipelineTemplateId": "00000000-0000-0000-0000-000000000000" },
                           "update": { "ExecutionPolicy": "POLICY_TenantAdmin", "PipelineTemplateId": "00000000-0000-0000-0000-000000000000" } },
  "Fields": [ { "CollectionId": "18797c62-1f80-421e-9e1c-8a0faa98cfa7", "Id": "d7c6ab4d-f01e-434c-aba6-51ae5b05c6e8", "TimeStamp": null,
    "Fields": { "Id": "d7c6ab4d-f01e-434c-aba6-51ae5b05c6e8", "ModelId": "f935d924-ff43-4655-b511-290968a51f4c",
      "Name": "Limit", "DisplayName": "$Limit", "DataType": "System.Double",
      "EditMode": "Changeable", "InputMethod": "Manual", "Required": false, "Uniqueness": "None", "DisplayRank": 1,
      "ReferencedCollectionId": "00000000-0000-0000-0000-000000000000", "ReferenceType": "Unspecified" } } ],
  "Actions": [ { "CollectionId": "c9ff4748-01e4-4fe0-b297-e99df3d4066b", "Id": "<guid>", "TimeStamp": null,
    "Fields": { "Id": "<guid>", "ModelId": "f935d924-ff43-4655-b511-290968a51f4c", "Name": "resetLimit", "DisplayName": "$ResetLimit",
      "SelectionMode": "SingleSelection", "ExecutionPolicy": "POLICY_Supervisor",
      "VisibleRankOnListView": 1, "VisibleRankOnDetailsView": 1,     // rank 0 hides the action
      "Arguments": [],                                                // real JSON array
      "Script": "...C# body..." } } ],                                // compiled at first execution only
  "Views": [ ] }                                                      // ModelView wrappers, below
```
Enum fields by **name** (`"EditMode": "Changeable"`); `DisplayRank` orders the editor dialog. Reference field (child side) adds `"DataType": "System.Guid", "ReferencedCollectionId": "<target model id>", "ReferencedFieldName": "Id", "ReferenceType": "Parent"`. Re-import the **whole model file** after any change.

## ModelView (in the model's `Views`)
```json
{ "Id": "<guid>", "ModelId": "<model id>", "Name": "Overview", "Type": "Dashboard", "Policy": "POLICY_Operator",
  "Schema": { "Menu": null, "Rows": [ { "Columns": [
      { "Value": 6, "Widgets": [ { "Name": "EntityDetailsWidget", "VisibleFields": ["Id", "Name"] } ] },
      { "Value": 6, "Widgets": [ { "Name": "LifeValueWidget", "Properties": { "StreamName": "HallTemperatureStream", "FieldName": "Temperature" } } ] } ] } ] } }
```
`Type` `ListView` `Schema`: `{ "UsePagination": true, "UsePaginationSize": 50, "Columns": [ { "FieldName": "Name" } ] }`. Details view: `{ "IncludeChildEntitiesAsTabs": true, "Tabs": [ { "Title": "$Info", "Dashboard": { "Rows": [ ] } } ] }`. Per widget only `Name` + used keys. Generated defaults `ListView`, `DetailsDashboard` exist per Model: customise with the **same Name and existing Id**.

## StreamModel with one field, `20-streammodels`
```json
{ "Id": "54dbbdac-9aed-475e-9d6d-932192578893",
  "Name": "HallTemperatureStream",                 // = "Stream" in the agent message, exact
  "TableName": "streams.halltemperature",          // unique, immutable
  "RefModelId": "<model the RefId points to>",     // optional; RefId must then be an entity Id of that model
  "RetentionDays": 3650,                            // 0 = keep forever; >0 deletes hourly (since 2.2)
  "StoreDuplicatedEntries": false, "EnableWatchDog": true, "WatchDogTimeoutMs": 30000,
  "ParentStreamId": "00000000-0000-0000-0000-000000000000", "PreprocessingScript": "",
  "RestrictedWrite": false, "DisableRestReports": false, "DisableManualChanges": false,
  "Fields": [ { "CollectionId": "ce0737e6-d4e0-41b6-b817-9f6175670ac9", "Id": "ffa2ab1f-316c-4cd1-b509-55221ea8525e", "TimeStamp": null,
    "Fields": { "Id": "ffa2ab1f-316c-4cd1-b509-55221ea8525e", "StreamModelId": "54dbbdac-9aed-475e-9d6d-932192578893",
      "Name": "Temperature", "DataType": "System.Double", "DefaultValue": "", "NonPersistant": false,   // DataType fixed at creation
      "ReferencedCollectionId": "00000000-0000-0000-0000-000000000000", "ReferenceType": "Unspecified" } } ],
  "NotificationSources": [ ] }
```
Built-in stream fields, do not redeclare: `Id`, `DataState`, `RefId`, `TimeStamp`, `Comment`.

## ReportSchema, `30-reportschemas`
```json
{ "Id": "<guid>", "Name": "Hall.Temperature", "SchemaType": "Chart",
  "Config": { "ChartType": "Line", "SourceType": "Stream", "SourceName": "HallTemperatureStream", "GroupedByField": "Hall",
              "Fields": { "ValueFormat": "{0:F1}", "LabelFormat": "{0:HH:mm}" },
              "DataSet": [ { "Label": "$Temperature", "SourceFieldName": "Temperature", "Color": "#008039" } ] } }
```
`Name` = the `ReportSchemaName` widgets use; unique case-insensitively across all reports incl. deleted.

## LanguageText, `60-languagetext`
```json
{ "Id": "<guid>", "Name": "$Hall", "EN": "Hall", "DE": "Halle", "Description": "" }
```
`Name` includes the `$`. Reuse the existing Id when updating.

## Navigation menu, `50-navmenu`
**2.3:** record `NavigationMenu` of model `NavMenu`, field `Schema`; update it, never create a second. **2.2:** `Setting` named `NavigationMenu`, `Value` = same JSON as string.
```json
{ "SubMenu": [ { "DisplayName": "$Home", "Path": "/", "Icon": "space_dashboard" },
  { "DisplayName": "$Halls", "Icon": "thermostat", "Policy": "POLICY_Operator",
    "SubMenu": [ { "DisplayName": "$HallList", "Path": "/Hall/ListView" }, { "DisplayName": "$Overview", "Path": "/Hall/Dashboard/Overview" } ] } ] }
```
