# Glossary (HumanOS-specific terms)

## Runtime

| Term | Meaning |
|---|---|
| Agent | Deployed Runtime instance connected to the Platform (Automation > Agents), identified by `ServiceId`. Not an AI agent |
| Gateway | `HumanOS.IoT.Gateway.exe`, runs the built configuration |
| Target | One deployable agent in a project; `default` target holds shared plugins/scripts, others inherit |
| ServiceId | Unique per agent; never copy a config with the same one to a second machine |
| Build / Publish | Designer: Build = gateway config; Publish = hand it to the Platform |
| Actor / Skill | Actor = information-model participant (device, service); Skill = capability the Platform can call (action/pipeline) |
| DataNode | Single value in the node space (address, type, unit, trigger/access settings) |
| NodeSpace | In-memory tree of actors, devices, nodes |
| Shared Node Space | NodeSpace shared agent <-> Platform over MQTT bus; licensed runtime only |
| UHAL | Ubiquitous (glossary: "Universal") Hardware Abstraction Layer: machine drivers/connectors |
| PeMiL | Pervasive Middleware Layer (manual) / "Persistence and Middleware Layer" (glossary): shared node space, databases, buses, processors (ML) |
| PeSeL | Pervasive Service Layer (manual) / "Persistence and Sending Layer" (glossary): OPC-UA server, MQTT client, REST, MCP, data loggers |
| Connector / Plugin | UHAL/PeMiL/PeSeL component with own manual and `settings.json` |
| NodeSpaceDataLogger | PeSeL publisher sending DataNodes to Platform streams |
| OEE, MDE, DNC | Standard information models: machine state, production data, NC program exchange |

## Platform

| Term | Meaning |
|---|---|
| HAP | HumanOS Automation Platform = Platform |
| ctProduction | CyberTech MES on the Platform; ships models/views/reports as a product package |
| Tenant | Department/customer; users see only own tenant's objects |
| Owner-Tenant / Additional Tenants | Tenant owning an object / tenants it is shared with |
| Business Owner | Tenant that sets up the Platform |
| Global-Tenant | Created at setup; global admins see all tenants' objects; visibility only, owns nothing |
| Policy | Permission name (`POLICY_SystemAdmin`) on model actions, views, nav entries; a view's policy must match its menu entry |
| Model | Business object schema, inherits an abstract model (min. `AbstractMainEntity`); fields/actions inherited, views not |
| Stream | Time series with named fields, `RefId`, retention |
| Pipeline / Task | Pipeline automates a process/external call (Jinja); Task starts pipelines manually/scheduled |
| Snapshot | Saved configuration (Snapshot Configuration), not a full backup. Also: what an agent actually runs |
| Product package | Update package of a product for one Platform version |
| Fleet Management | Agent approval, config rollout, license distribution, remote diagnostics |
| Approval (Zulassung) | New agent `Draft` (inert, data discarded) until SystemAdmin approves (`Active`); `Inactive` = blocked |
| Rollout states | Not Defined, Pending, Delivered, Applied, Modified (drift), Failed |
| Master controller | Platform side an agent connects to (`MasterSystemControllerId`) |
