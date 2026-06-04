# Project Context

## Multi-Project Convention

When the user says **"make changes across 3 projects"** (or similar phrasing referring to multiple projects), it means apply the changes to all three of these .NET service projects:

1. `nextjs-backend-invoice-service/` — the main backend API
2. `nextjs-backend-cust-service/` — the customer service
3. `nextjs-backend-dashboard-service/` — the dashboard service

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
