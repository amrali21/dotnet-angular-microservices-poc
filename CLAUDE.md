# Project Context

## Multi-Project Convention

When the user says **"make changes across 4 projects"** (or similar phrasing referring to multiple projects), it means apply the changes to all four of these .NET service projects:

1. `ledgerly-backend-invoice-service/` — the main backend API
2. `ledgerly-backend-cust-service/` — the customer service
3. `ledgerly-backend-dashboard-service/` — the dashboard service
4. `ledgerly-backend-auth-service/` — issues/validates JWTs used by the other three

## RabbitMQ Event Bus

Services share a single topic exchange, `ledgerly.events`. `invoice-service` and
`cust-service` only publish (`RabbitMqPublisher`, fire-and-forget — a broker
hiccup is logged, never fails the request). `dashboard-service` is the only
consumer (`RabbitMqConsumerService`), binding `invoice.*`/`customer.*` to
`dashboard.kpi.queue` to keep its KPI table in sync. The consumer retries its
initial broker connection with a 5s backoff instead of crashing the host —
`depends_on: service_healthy` on `rabbitmq` reduces but doesn't eliminate the
startup race, since RabbitMQ's ping-based healthcheck can pass slightly before
the AMQP listener is ready.

## Kubernetes Deployment (`k8s/`)

The stack also runs on Kubernetes using a **K8s-native discovery** design (an
alternative to `docker-compose.yml`, not a replacement of it). Apply with
`kubectl apply -k k8s/`; full instructions live in [k8s/README.md](k8s/README.md).

Key design decisions (and why they differ from compose):

- **Eureka is dropped.** Discovery uses Kubernetes DNS + ClusterIP Services. The
  gateway routes directly to `invoice-service:5052`, `cust-service:5246`,
  `dashboard-service:5208`; ClusterIP load-balances across replicas.
- **Eureka client disabled by config, not code.** `02-common-config.yaml` sets
  `eureka__client__enabled/shouldRegisterWithEureka/shouldFetchRegistry=false`.
  The `eureka:` section stays in `appsettings.json` so Steeltoe still resolves a
  client type (avoids a "no discovery client" startup error) but does nothing.
  The Steeltoe/Ocelot Eureka packages are still compiled in — disabled, not
  removed. Removing them is optional cleanup across the gateway + 3 services.
- **Gateway routing is swapped at deploy time.** `03-gateway-ocelot-config.yaml`
  is a Eureka-free `ocelot.json` (uses `DownstreamHostAndPorts`) mounted over
  `/app/ocelot.json` in the api-gateway pod — no gateway image rebuild needed.
- **One public entry point.** `20-ingress.yaml` routes by path on a single host:
  `/InvoiceGW|/CustomerGW|/Dashboard` → api-gateway, `/` → angular-frontend.
  All Services are ClusterIP (internal); only the Ingress is public.
- **Database is externalized into a Secret** (`01-db-secret.yaml`), replacing the
  compose `host.docker.internal\SQLEXPRESS`. Local testing uses
  `host.minikube.internal,1433` (static TCP port, no instance name); cloud uses a
  managed instance. The connection string overrides `appsettings.json` via the
  `ConnectionStrings__DefaultConnection` env var.
- **Frontend API URL is build-time.** A new Angular `kubernetes` build config
  (`app.settings.kubernetes.ts`, `ACTION_URL=''`) makes the client-side SPA call
  the API relative/same-origin through the Ingress (no CORS). Selected via the
  frontend Dockerfile `--build-arg NG_CONFIG=kubernetes` (defaults to `docker`,
  so the compose build is unchanged). Changing `ACTION_URL` requires an image
  rebuild + rollout; DB/Eureka config changes only need a ConfigMap/Secret edit
  and `kubectl rollout restart`.
- **Autoscaling** via `30-hpa.yaml` (CPU 70%); needs metrics-server. Pods set
  resource requests (required for HPA). Probes are TCP-socket (no assumption that
  a `/health` endpoint exists).
