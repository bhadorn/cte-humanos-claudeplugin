# Notifications, translations, tasks: caveats

Docs: `Manuals/Notifications/*`, `Manuals/Pipelines/Templating` (Liquid), `Manuals/Languages/`.

## Notifications
- Chain: event (`NotificationSource`) -> notification entity -> rules -> channel -> delivery. Ship **only events**; rules and channels hold recipients/credentials, created live per instance.
- Stream-bound events nest in the stream model's `NotificationSources[]` (collection id: `concepts.md`). Event keys: `Name, Kind (Stream|Entity|Pipeline), Type (Information 0|Warning 1|Error 2), SourceStreamId, RefModelId, NotificationModel, TriggerCondition, DelayTime, CoolDownTime, NotificationTitle, MessageTemplate, ShortMessageTemplate, PipelineTemplateId, PipelineStatuses`.
- `TriggerCondition` dialect: `n` for `Stream` (the message), `entity` for `Entity`. `Entity` events fire on false-to-true transitions, not for entities already true at platform start.
- `DelayTime`/`CoolDownTime` count from the first message that made the condition true. An unacknowledged notification blocks further ones of same source and object.
- Failed deliveries not retried (`resendDelivery`). One channel per rule: clone for SMS plus e-mail. Users need e-mail (SMS: `MobilePhone` E.164).
- **2.3:** update resets built-in notification views; edits lost.

## Translations
- Keys are **instance data**: `$Key` without `LanguageText` row shows raw (`???` in menus). Create rows (EN required, DE) and include in your files.
- `LanguageText.Name` includes the leading `$`; new keys `$` + letter + up to 63 letters/digits/`_`/`.`; use a unique prefix.
- `BuiltIn` texts: `Name/EN/DE` not editable. Updating needs the existing `Id`.
- Usage: `GET <tenant>/authview/gettextusage?strKey=$Key`.

## Tasks
- `TriggerConfig.Mode` enum stored **by name** (numbers show empty selection). Built-in task `Synchronize Licenses` runs every 6 h.
