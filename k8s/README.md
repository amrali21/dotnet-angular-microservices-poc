# Kubernetes deployment (K8s-native discovery)

This folder runs the whole stack on Kubernetes using the **K8s-native discovery**
design — the same services as `docker-compose.yml`, but:

- **Eureka is gone.** Discovery is done by Kubernetes DNS + ClusterIP Services.
  The gateway routes straight to `invoice-service:5052`, `cust-service:5246`,
  `dashboard-service:5208`, `auth-service:5109`, and ClusterIP still load-balances
  across replicas.
- **One public entry point** (an Ingress) instead of publishing host ports.
  Only the Ingress is reachable from outside; every Service is internal.
- **The database is externalized** into a Secret (no more `host.docker.internal`).
- **JWT auth**: `auth-service` owns ASP.NET Core Identity and issues tokens;
  the gateway and the 3 downstream services validate them independently using a
  shared signing key (Secret: `jwt-secret`).
- **Autoscaling** via HPAs on CPU.
- **RabbitMQ** (`05-rabbitmq.yaml`) is the async link between invoice/cust-service
  (publishers) and dashboard-service (consumer), reachable as `rabbitmq:5672`.
  Credentials live in `rabbitmq-secret`; the broker address is in
  `02-common-config.yaml`.

```
                    Ingress (myapp.local)  ── only public door
                      /            → angular-frontend (SPA, ACTION_URL='')
                      /InvoiceGW   → api-gateway → invoice-service
                      /CustomerGW  → api-gateway → cust-service
                      /Dashboard   → api-gateway → dashboard-service
                      /AuthGW      → api-gateway → auth-service
   external SQL Server  ←─ (Secret: db-secret)
   JWT signing key      ←─ (Secret: jwt-secret)
   [eureka-server: deleted]
```

## Files

| File | What it is |
|------|------------|
| `00-namespace.yaml`            | `microservices` namespace |
| `01-db-secret.yaml`            | DB connection string — **edit before deploying** |
| `02-common-config.yaml`       | Shared env: `ASPNETCORE_ENVIRONMENT=Production` + Eureka-off flags |
| `03-gateway-ocelot-config.yaml`| Eureka-free `ocelot.json`, mounted over the gateway image |
| `04-jwt-secret.yaml`           | JWT signing key/issuer/audience — **edit before deploying** |
| `04-rabbitmq-secret.yaml`      | RabbitMQ broker credentials — **edit before deploying** |
| `05-rabbitmq.yaml`             | RabbitMQ broker: Deployment + ClusterIP Service |
| `10/11/12-*-service.yaml`      | The 3 .NET services: Deployment + ClusterIP Service |
| `13-api-gateway.yaml`          | Ocelot gateway: Deployment + ClusterIP Service |
| `14-angular-frontend.yaml`     | Angular SPA (nginx): Deployment + ClusterIP Service |
| `15-auth-service.yaml`         | Identity/JWT auth service: Deployment + ClusterIP Service |
| `20-ingress.yaml`              | Single public entry point, path-based routing |
| `30-hpa.yaml`                  | HorizontalPodAutoscalers (CPU 70%) |
| `kustomization.yaml`           | Applies everything together |

---

## Prerequisites (local, with minikube)

```bash
minikube start
minikube addons enable ingress           # ingress-nginx controller
minikube addons enable metrics-server    # required by the HPAs
```

A SQL Server reachable from the cluster (managed instance, VM, or host reachable
by IP:port). A named instance like `HOST\SQLEXPRESS` generally is **not**
reachable from a pod — use `Data Source=<host>,1433;...`.

**SQL Server TCP/IP must be enabled** (it is off by default on SQL Server Express):
1. Open **SQL Server Configuration Manager**
2. SQL Server Network Configuration → Protocols for SQLEXPRESS → enable **TCP/IP**
3. TCP/IP Properties → IP Addresses tab → IPAll → set **TCP Port = 1433**, clear **TCP Dynamic Ports**
4. Restart the **SQL Server (SQLEXPRESS)** service
5. Allow port 1433 through Windows Firewall:
   ```powershell
   New-NetFirewallRule -DisplayName "SQL Server 1433" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
   ```

**CoreDNS must be patched** so pods can resolve `host.minikube.internal` (Minikube adds
this hostname to the node's `/etc/hosts`, but pods use CoreDNS which doesn't see that file):
```bash
kubectl apply -f k8s/coredns-patch.yaml
kubectl rollout restart deployment coredns -n kube-system
```
`coredns-patch.yaml` hardcodes `192.168.65.254` — verify this matches your setup with:
```bash
minikube ssh "grep host.minikube.internal /etc/hosts"
```

---

## 1. Build the images

Images are named for **Docker Hub** (`xamrxxx/<service>`) in `docker-compose.yml`.
Compose builds and tags all of them in one shot — the push overlay flips the
frontend to the kubernetes Angular config (`ACTION_URL=''` -> relative, same-origin
API), so you don't build it separately:

```bash
# point your shell at minikube's docker daemon (so images land where the cluster sees them)
eval $(minikube docker-env)          # PowerShell: & minikube -p minikube docker-env | Invoke-Expression

# from the repo root — builds & tags every service at once:
docker compose -f docker-compose.yml -f docker-compose.push.yml build
```

> If you don't use `minikube docker-env`, build normally and load each image:
> ```bash
> minikube image load xamrxxx/invoice-service:latest
> minikube image load xamrxxx/cust-service:latest
> minikube image load xamrxxx/dashboard-service:latest
> minikube image load xamrxxx/auth-service:latest
> minikube image load xamrxxx/api-gateway:latest
> minikube image load xamrxxx/angular-frontend:latest
> ```

## 2. Set the DB, JWT, and RabbitMQ secrets

`01-db-secret.yaml`, `04-jwt-secret.yaml`, and `04-rabbitmq-secret.yaml` are
**gitignored** (they hold real credentials). Create them from the committed
templates:

```bash
cp k8s/01-db-secret.example.yaml k8s/01-db-secret.yaml
# edit k8s/01-db-secret.yaml -> set the real Password (and host for cloud)

cp k8s/04-jwt-secret.example.yaml k8s/04-jwt-secret.yaml
# edit k8s/04-jwt-secret.yaml -> set a real random Jwt__Key, e.g.: openssl rand -base64 64

cp k8s/04-rabbitmq-secret.example.yaml k8s/04-rabbitmq-secret.yaml
# edit k8s/04-rabbitmq-secret.yaml -> set real RABBITMQ_DEFAULT_PASS / RabbitMQ__Password
```

`kustomization.yaml` references all three files, so they must exist locally
before `kubectl apply -k`. For real environments prefer `kubectl create secret`
or an external-secrets operator over a file on disk.

> **Troubleshooting**: `auth-service`, `api-gateway`, `invoice-service`,
> `cust-service`, and `dashboard-service` all read `Jwt__Key`/`Jwt__Issuer`/
> `Jwt__Audience` from the same `jwt-secret`. If any protected call 401s
> unexpectedly, check that all 5 Deployments picked up the same Secret values
> (`kubectl rollout restart` after editing the Secret — env vars don't
> hot-reload into running pods).

## 3. Deploy

```bash
kubectl apply -k k8s/
kubectl get pods -n microservices -w      # wait until all are Running/Ready
```

## 4. Reach it

```bash
# map the Ingress host to the cluster
echo "$(minikube ip)  myapp.local" | sudo tee -a /etc/hosts
#   Windows: add "<minikube ip>  myapp.local" to C:\Windows\System32\drivers\etc\hosts
#   (open Notepad as Administrator to edit the hosts file)
```

Open <http://myapp.local/> — the SPA loads, and its API calls go to
`http://myapp.local/InvoiceGW/...` etc., routed through the gateway.

---

## Operating notes

```bash
# scale by hand (or let the HPA do it)
kubectl scale deployment/invoice-service --replicas=4 -n microservices
kubectl get hpa -n microservices

# logs / status
kubectl logs -l app=api-gateway -n microservices
kubectl describe ingress microservices-ingress -n microservices

# tear down
kubectl delete -k k8s/
```

### Changing config
- **DB string / Eureka flags** → edit the Secret/ConfigMap, then
  `kubectl rollout restart deployment/<name> -n microservices`. **No image rebuild.**
- **`ACTION_URL` (frontend)** → it is baked into the JS at build time, so it
  requires **rebuilding the angular-frontend image** and rolling it out. Use a
  fresh tag per build (`angular-frontend:k8s-2`) so the rollout actually pulls it.

## Going to the cloud (EKS/AKS/GKE)
1. Push the images to Docker Hub so the cluster can pull them (`docker login` once).
   **Use a new immutable tag per release** (`v1`, `v2`, a build number, or the git
   short SHA) — never reuse a tag, because `imagePullPolicy: IfNotPresent` means a
   node that cached an old `:v1` won't re-pull a changed `:v1`:
   ```bash
   $env:TAG = "v1"
   docker compose -f docker-compose.yml -f docker-compose.push.yml build
   docker compose -f docker-compose.yml -f docker-compose.push.yml push
   ```
   Then bump `newTag: "v1"` in the `images:` block of `kustomization.yaml` (it
   already points at `xamrxxx/<service>`) to the same tag and `kubectl apply -k k8s/`.
   The `TAG` you push with and the `newTag` you apply with must match — that one
   value is your release version.

   > The local image and the pushed image are the **same content** — a tag is just
   > a pointer, so you build once, test locally, and push the identical bits.
2. The ingress controller gets a real external load-balancer IP/hostname; create
   a DNS record (`A`/`CNAME`) for your domain pointing at it, and set that domain
   as the Ingress `host:`.
3. Add TLS via cert-manager + a `tls:` block on the Ingress for HTTPS.
4. Use a managed SQL instance for `db-secret`.
