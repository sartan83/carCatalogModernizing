# carCatalogModernizing — phased modernization plan

Maps 1:1 onto the ten target specifications. Baseline is `main` after the `CarCatalog*` rename: three .NET Framework 4.7.2 apps (MVC 5, WebForms, WCF+WinForms), ~5.8k C# LOC, no tests, no CI, no containers.

Effort is expressed in **Devin sessions** (one session ≈ a focused end-to-end work block, ~1-2 human-weeks of equivalent scope). Phases are ordered by dependency, and every phase ends in a mergeable, buildable state.

## Locked decisions

| Decision | Choice | Consequence |
| --- | --- | --- |
| WebForms | **Blazor Server** rewrite (not dropped) | closest event/postback model to WebForms; Phase 5 stays at 1.5-2 sessions |
| Runtime | **.NET 8 (LTS)** | support to Nov 2026; a .NET 10 bump later is a TFM change, not a port |
| Database | **PostgreSQL** | EF Core provider `Npgsql`; adds a schema/data migration pass in Phase 4 (see below) |
| Observability | **OpenTelemetry** (traces + metrics + logs, OTLP) | backend-agnostic; collector target still to pick before Phase 6 |
| Broker | **Kafka** | durable, replayable log + Schema Registry — required by the outbox/projection pattern in Phase 8 |
| WinForms | **kept** on .NET 8 WinForms (Windows-only) — assumed, say if it should be retired | the SOAP contract must survive, so Phase 4 keeps CoreWCF alongside the REST API |

### What PostgreSQL changes

Postgres is the better Linux/Kubernetes story, but it is not a free swap — it lands inside Phase 4:

- `Npgsql.EntityFrameworkCore.PostgreSQL` instead of the SQL Server provider; migrations regenerated from scratch.
- The hi-lo sequence scripts (`dbo.catalog_*`, raw T-SQL today) are replaced by EF Core `UseHiLo` on native Postgres sequences.
- Identifier casing: SQL Server is case-insensitive, Postgres folds unquoted identifiers to lowercase. Table/column names stay logically unchanged (`CatalogItem`, `CatalogBrand`, `CatalogType`), but a naming convention must be chosen once (quoted PascalCase or snake_case mapping) and applied consistently.
- `datetime2` → `timestamptz`, `money`/`decimal` precision, and `bit` → `boolean` type mapping reviewed against the golden-data test from Phase 0.
- Local dev and CI run Postgres in Docker; LocalDB disappears entirely.

### Why Kafka over NATS

```
                 NATS (JetStream)              Kafka
  replay         limited retention/window      full log replay, per-consumer offsets
  projections    rebuild is awkward            rebuild = reset offset to 0
  schemas        bring your own                Schema Registry + compatibility rules
  ops cost       very low                      higher (ZK-less KRaft, still heavier)
```

Phase 8 rebuilds read-model projections from the event log, so replay is the deciding property. Managed Kafka (Confluent Cloud, MSK, Azure Event Hubs' Kafka endpoint) keeps the operational cost close to NATS.

---

## Spec → phase coverage

| Target specification | Delivered in |
| --- | --- |
| Dependencies management centralisation | Phase 1 |
| Content and version control of simulation and data | Phase 2 |
| Segregation of functionalities | Phase 3, 4 |
| .NET first | Phase 4, 5, 6 |
| Frameworks migration | Phase 4, 5, 6 |
| Linux transition | Phase 4, 7 |
| Transversal observability | Phase 6 |
| Elastic orchestration with Kubernetes | Phase 7 |
| State of the art messaging and streaming | Phase 8 |
| Python over proprietary DSL | Phase 2, 9 |

---

## Phase 0 — Safety net (prerequisite, ~1 session)

Nothing below is verifiable today beyond "it compiles". Before touching behaviour:

- Characterization tests around `CatalogService` / `CatalogDBInitializer`: paging, brand/type filters, CRUD, hi-lo ID allocation, both seed modes (`UseMockData`, `UseCustomizationData`).
- Golden-output test that snapshots the seeded catalog (brands, types, 12 items, prices, picture mapping) so every later phase can assert the data is unchanged.
- GitHub Actions CI: restore + build all three solutions + run tests. Windows runner initially (net472), switched to Linux as projects port.

**Exit:** red/green signal exists on every PR.
**Risk if skipped:** every later phase is a blind refactor.

## Phase 1 — Dependency centralisation (~0.5 session) — *spec: dependencies*

- Convert 5 projects from `packages.config` to SDK-style `PackageReference` (still net472 — no behaviour change).
- `Directory.Packages.props` with `ManagePackageVersionsCentrally=true`; `Directory.Build.props` for shared TFM, analyzers, nullable, deterministic builds.
- Resolve the existing version drift: `Newtonsoft.Json` 12.0.1 vs 6.0.4, `EntityFramework` 6.2.0 vs 6.1.3 → one pinned version each.
- Commit `packages.lock.json`; add `dotnet list package --vulnerable` + SBOM generation to CI; enable Renovate/Dependabot.
- Delete the half-finished `Convert-ToPackageReference.ps1` + XSLT once done.

**Exit:** one place defines every dependency version; CI fails on known-vulnerable packages.
**Why first:** it is cheap, independent, and makes every subsequent port a smaller diff (no `HintPath` churn, no `..\packages` symlink workaround).

## Phase 2 — Data & simulation content (~1 session) — *specs: data/simulation, Python*

- Collapse the three duplicated `Setup/` folders (CSV × 3 + `CatalogItems.zip`) into **one versioned dataset** at the repo root, with an explicit schema (JSON Schema or CSV + header contract) and a semantic version.
- Kill the second copy of the data: generate `PreconfiguredData.cs` (or replace it with a data file read at runtime) from the same dataset, so mock mode and customization mode cannot diverge.
- Content-address the picture assets (hash-named, manifest-mapped) instead of ordinal `1.png`…`12.png`; this also removes the N-Tier app's deliberately different item→file mapping.
- **Python tooling** (`tools/` — first Python in the repo): dataset validation, schema check, fixture generation, drift detection between dataset and DB. Wired into CI.

**Exit:** one dataset, one schema, validated in CI, consumed identically by all apps.
**Independent** of the .NET port — can run in parallel with Phase 1.

## Phase 3 — Segregation of functionalities (~1 session, still net472) — *spec: segregation*

Refactor **before** porting, so the port moves one codebase instead of three:

- Extract `CarCatalog.Domain` (entities, `CatalogBrand`/`CatalogType`/`CatalogItem`, invariants) and `CarCatalog.Infrastructure` (EF context, hi-lo, seeding) into shared projects referenced by all three apps. Entity and table names stay untouched.
- Extract an application layer (`CarCatalog.Application`) with the catalog use cases; presentation layers call it, never EF directly.
- Split the seeding/initialization concern out of the request path into a standalone `CarCatalog.DbMigrator` console project (this is what Phase 7 needs as a Kubernetes Job).

**Exit:** the domain exists once; a change to `CatalogItem` is a one-place edit; the three apps are thin presentation shells.

## Phase 4 — .NET 8 core: service + MVC (~1.5 sessions) — *specs: .NET first, frameworks, Linux*

- Retarget `Domain`/`Application`/`Infrastructure` to `net8.0`; **EF6 → EF Core 8** (`UseHiLo` replaces the raw sequence scripts, `DbInitializer` → migrations). This is the highest-risk technical step — Phase 0's golden-data test is the gate.
- Port `CarCatalogLegacyMVC` → **ASP.NET Core 8 MVC**: `Global.asax`/`App_Start` → `Program.cs`, `System.Web` → `Microsoft.AspNetCore.*`, Web API 2 controllers → ASP.NET Core controllers, Autofac → `Autofac.Extensions.DependencyInjection` (or built-in DI), `Web.config` → `IConfiguration` + env vars, bundling → a real asset pipeline.
- Port the WCF service to **CoreWCF**, keeping `ICatalogService` and the SOAP contract intact so the WinForms client keeps working; expose the same operations additionally as a REST API for new consumers.
- Drop `sessionState mode="InProc"`; the apps become stateless.
- Migrate persistence to **PostgreSQL** (see "What PostgreSQL changes" above): Npgsql provider, regenerated migrations, `UseHiLo` on native sequences, type-mapping review, Postgres in Docker for local dev and CI. LocalDB is removed.

**Exit:** MVC app + catalog service run on .NET 8 on Linux against a containerized database, with identical catalog behaviour.

## Phase 5 — WebForms rewrite (~1.5-2 sessions) — *specs: .NET first, frameworks*

No mechanical path exists — `System.Web.UI` was never ported. Rewrite the 10 `.aspx/.ascx/.Master` pages as **Blazor Server** components against the Phase 3 application layer:

- Catalog list with paging/filtering, item create/edit/delete, picture display.
- Server-side state replaces ViewState; the designer codegen (`.designer.cs`) disappears entirely.
- Decision point at the start of this phase: keep two web front-ends at all, or fold WebForms and MVC into one app. Folding saves ~1 session.

**Exit:** no `System.Web` anywhere; WebForms markup DSL is gone.

## Phase 6 — Transversal observability (~1 session) — *spec: observability*

- Replace log4net with `Microsoft.Extensions.Logging` + structured logging; no more per-pod rolling files.
- **OpenTelemetry** SDK: traces + metrics + logs, OTLP export to a collector; instrument ASP.NET Core, HttpClient, EF Core, and CoreWCF.
- Propagate W3C `traceparent` from the WinForms client through the service to the database so the full desktop → service → DB path is one trace.
- RED metrics per endpoint, `/healthz` + `/readyz` endpoints (also consumed by Phase 7 probes/HPA), and correlation IDs on every log line.
- Retire the Application Insights 2.9 Windows collectors (`PerfCounterCollector`, `WindowsServer`) — OTel exports to whatever backend you standardize on.

**Exit:** one trace covers a request end-to-end; dashboards and alerts are backend-agnostic.

## Phase 7 — Kubernetes & elasticity (~1 session) — *specs: Kubernetes, Linux*

- Multi-stage Dockerfiles (Linux, chiseled/distroless runtime) for the API, the MVC app, the Blazor app, and the DbMigrator.
- Helm chart (or Kustomize) per environment; config from ConfigMaps/Secrets, no `Web.config` transforms.
- `CarCatalog.DbMigrator` runs as an init container / pre-install Job — this is what removes the seed-on-first-request replica race.
- Liveness/readiness probes from Phase 6, resource requests/limits, HPA on RPS or CPU, PodDisruptionBudget, and rolling updates.
- CI/CD: build → test → scan → push image → deploy per environment.

**Exit:** the web apps and API scale horizontally under load; scaling is verified with a load test (Python, per spec 10).

## Phase 8 — Messaging & streaming (~1-1.5 sessions) — *spec: messaging*

- Define domain events on the state that other systems care about: `CatalogItemCreated`, `CatalogItemPriceChanged`, `StockLevelChanged`, `StockDepleted`, `CatalogItemRemoved`.
- **Transactional outbox** in the catalog service (a Postgres table written in the same transaction as the state change) so the database and the event log cannot diverge; a relay publishes to **Kafka**.
- **Confluent Schema Registry** with Avro or Protobuf contracts and enforced compatibility rules; consumer-driven contract tests in CI.
- First consumers: a read-model/projection for catalog browse, and a stock-alert consumer — enough to prove the pattern end-to-end.
- Idempotent consumers, dead-letter topic, replay procedure documented.

**Exit:** state changes are observable as a stream; new consumers are added without touching the catalog service.

## Phase 9 — Python over proprietary DSL (~0.5 session) — *spec: Python*

By this point the Microsoft-proprietary DSLs are gone as a by-product (WebForms markup and designer codegen in Phase 5, `Web.config`/transforms in Phase 4, `system.serviceModel` XML in Phase 4, PowerShell+XSLT in Phase 1). This phase consolidates the Python layer that replaces them **outside** the request path:

- Data/simulation pipeline and validators (from Phase 2) promoted to a maintained `tools/` package with its own tests and lockfile (uv/poetry).
- Migration and verification scripts (schema drift, data parity between legacy and modern, cutover checks).
- Load generation and scale validation for Phase 7.
- Stream analytics/consumer prototypes against Phase 8 topics.

**Exit:** every non-request-path automation is Python; no proprietary DSL remains except MSBuild project files.

---

## Sequencing

```
 Phase 0  safety net ──┐
 Phase 1  deps         ├──► Phase 3 segregation ──► Phase 4 .NET 8 (svc + MVC) ──┬──► Phase 6 observability ──┐
 Phase 2  data ────────┘                                    │                    ├──► Phase 7 Kubernetes ─────┼──► Phase 8 messaging ──► Phase 9 Python
                                                            └──► Phase 5 WebForms rewrite ───────────────────┘
      (0,1,2 can run in parallel)                                          (5 is parallel to 6/7)
```

| Phase | Sessions |
| --- | ---: |
| 0 safety net | 1 |
| 1 dependencies | 0.5 |
| 2 data & simulation | 1 |
| 3 segregation | 1 |
| 4 .NET 8 service + MVC | 1.5 |
| 5 WebForms → Blazor | 1.5-2 |
| 6 observability | 1 |
| 7 Kubernetes | 1 |
| 8 messaging | 1-1.5 |
| 9 Python tooling | 0.5 |
| **Total** | **10-11 sessions** |

Wall-clock is dominated by external waits, not by the work: cluster/registry provisioning, broker provisioning, and your review cycles.

## Still open

1. **WinForms** — keep (assumed) or retire? Retiring it frees Phase 4 from the SOAP contract and lets the service be REST/gRPC only.
2. **OTel collector backend** — Grafana stack (Tempo/Loki/Mimir), Datadog, Azure Monitor…? Needed before Phase 6; does not change the instrumentation code.
3. **Kafka hosting** — managed (Confluent Cloud / MSK / Event Hubs Kafka endpoint) or self-hosted on the cluster? Affects Phase 8 ops effort, not design.
4. **Two web front-ends or one** — decided at the start of Phase 5; folding MVC and Blazor into one app saves ~1 session.

---

## Execution model

One phase = one branch = one PR (occasionally two, noted below). Nothing is merged that does not build, and no phase depends on an unmerged predecessor except where the sequencing diagram says so.

| # | Branch | PR title | Merges when |
| --- | --- | --- | --- |
| 0 | `phase0/safety-net` | Add characterization tests and CI | CI green on all three solutions, golden-data test passing |
| 1 | `phase1/central-deps` | Centralize dependency management (PackageReference + Directory.Packages.props) | all solutions build with identical output, lock files committed, vulnerability gate active |
| 2a | `phase2/dataset` | Unify catalog dataset into a single versioned source | one dataset, all three apps seed identically, golden-data test unchanged |
| 2b | `phase2/data-tooling` | Add Python dataset validation and fixture generation | `tools/` tests green, validation runs in CI |
| 3 | `phase3/segregate-domain` | Extract Domain/Application/Infrastructure and DbMigrator | no EF access from presentation code, domain defined once |
| 4a | `phase4/net8-core` | Port Domain/Application/Infrastructure to .NET 8 + EF Core 8 + Npgsql | golden-data test passes against Postgres |
| 4b | `phase4/aspnetcore-mvc` | Port the MVC app to ASP.NET Core 8 | app runs on Linux, catalog behaviour identical |
| 4c | `phase4/corewcf` | Port the WCF service to CoreWCF and add the REST API | WinForms client works unchanged against CoreWCF |
| 5 | `phase5/blazor-catalog` | Replace the WebForms app with Blazor Server | feature parity with the WebForms pages, no `System.Web` left |
| 6 | `phase6/otel` | Replace log4net/AppInsights with OpenTelemetry | one end-to-end trace desktop → service → DB, health endpoints live |
| 7 | `phase7/k8s` | Containerize and deploy to Kubernetes with HPA | scale-out verified under load, migrator runs as a Job |
| 8a | `phase8/outbox` | Add domain events and the transactional outbox | events emitted transactionally, contract tests green |
| 8b | `phase8/kafka-consumers` | Publish to Kafka and add the first consumers | projection rebuildable from replay |
| 9 | `phase9/python-tooling` | Consolidate automation into the Python tooling package | migration/load/analytics scripts maintained and tested |

### Per-phase definition of done

Every PR in the table must satisfy all of:

1. Builds on CI (Windows runner up to Phase 3, Linux from Phase 4).
2. Golden-data test passes — the seeded catalog is byte-identical to the Phase 0 snapshot.
3. No new dependency version outside `Directory.Packages.props`.
4. `CatalogItem` / `CatalogBrand` / `CatalogType` and the logical table names unchanged.
5. The phase's stated **Exit** condition is demonstrated in the PR description (test output, screenshot, trace, or `kubectl` output as appropriate).

### Rollback posture

Phases 0-3 are behaviour-preserving refactors on .NET Framework: revert the PR and the previous state returns exactly. From Phase 4 the database engine changes, so the rollback unit is the environment, not the commit — keep the legacy stack deployable until Phase 5 completes, and run both against the same dataset to compare.

## Non-negotiables carried through every phase

- `CatalogItem`, `CatalogBrand`, `CatalogType` and the logical table/column names stay unchanged (subject to the Postgres identifier-casing convention chosen in Phase 4).
- Every phase ends buildable and mergeable — no long-lived integration branch.
- The Phase 0 golden-data test guards the catalog content through all ten phases.
