# From process to plan

## Interview (one question at a time, small scope)

- Sites/halls/machines? Who changes limits? Stream retention?
- Source failure (sensor down, broker unreachable, file missing): what must the user see?
- Roles (`humanos` skill): who watches, who sets limits, who configures? One tenant or one per department?
- Interface: protocol, topic/address, payload, direction, frequency, authentication, test access (simulator/file/account), network/firewall?
- Real device for tests, or simulator with the same values?

Unanswered = **open point** (question, owner, due). A device interface description (topics, payload) is a proposal until checked at the device.

## Specification outline

1 goal, tenant, roles; 2 data model (models, fields, relations; streams: name, fields, retention); 3 actions/flows;
4 UI (menu + policy per entry, list/detail, dashboards, reports); 5 interfaces (system, protocol, direction,
frequency, errors; becomes the agent); 6 acceptance criteria; 7 open points. Template: `templates/Specification.md`.

## Neutral example: hall temperature

| Item | Content |
|------|---------|
| Data | model `Hall` (Name, Limit); stream `HallTemperatureStream` (Gateway, Hall, Temperature, timestamp) |
| Source | IoT gateway, MQTT topic `<prefix>/<GatewayId>/<Hall>/temperature`, JSON `value` (proposal, verify at device) |
| Trigger | each new value; agent forwards last value per hall every minute |
| Evaluation | current per hall, history, daily average |
| Order | model + stream -> views, reports, dashboard, menu -> agent (MQTT connector, data logger) -> acceptance with simulator |

Buildable OPC UA variant: `examples/hall-temperature/README.md`.

## Plan review (before "go")

- Covers every spec section; every step has a runnable check
- Names match everywhere (stream, fields, model, action); agent names = Platform names
- Nothing unordered; each HumanOS statement has a doc/example source
- Failure cases covered (source down, agent not approved, import order); open points have owner and date

Plan = checkboxes with file paths and check per task, ticked as work proceeds. Skeleton: `templates/Plan.md`.
