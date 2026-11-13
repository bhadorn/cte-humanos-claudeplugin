# Plan: <project>

Approved by <name> on <date>. Order fixed. One commit per step; done = check ran, output read.

## 1. Platform model (`src/Platform/`)
- [ ] 1.1 Model `<Name>` in `10-model-<Name>.json`. Check: test-tenant import succeeds, twice; fields present.
- [ ] 1.2 Stream model `<Stream>` in `20-streammodel-<Stream>.json`. Check: import ok; stream Active; names = name contract.

## 2. Views and dashboards
- [ ] 2.1 Report schema. Check: chart widget shows test values.
- [ ] 2.2 Dashboard/list views, policy per role. Check: test user per role; unauthorized role does not see it.
- [ ] 2.3 Navigation menu (2.2 setting `NavigationMenu`, 2.3 `NavMenu`; merge, do not replace). Check: entry opens view.
- [ ] 2.4 Language texts. Check: no raw `$Key` in UI.
- [ ] 2.5 Notification event. Check: `sendTestRule` arrives; live: channel + rule created on instance.

## 3. Agent (`src/Agents/<Name>.Agents/`)
- [ ] 3.1 Project from closest example, new GUIDs, unique `HumanOS.ServiceId`. Check: BuildTool `Ok`, no `[ERR]`.
- [ ] 3.2 Device template, device, plugin settings, scripts. Check: gateway log: C# compiled, device detected, no `[ERR]`; stop gateway after.
- [ ] 3.3 Stream publisher. Check: dump shows `Stream`, `RefId`, `State` 1, `Fields`.
- [ ] 3.4 Approve agent (SystemAdmin), shared node space. Check: Automation > Agents Active, health ok.

## 4. Tests (`doc/Acceptance.md`)
- [ ] 4.1 Data chain source, dump, stream row, widget, notification. Check: value at each hop.
- [ ] 4.2 Failure cases (source down, broker unreachable). Check: as specified.

## 5. Release
- [ ] 5.1 CHANGELOG, review, backup, apply from committed files. Check: Rollout State Applied, dashboard shows data.

## Open points
| # | Point | Owner |
|---|-------|-------|
