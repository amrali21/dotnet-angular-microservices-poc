# .NET + Angular Microservices POC

A proof-of-concept microservices stack: three .NET backend services behind an
Ocelot API gateway, an Angular SPA frontend, and (for local/Docker runs) a
Eureka service-discovery server. The same stack can also run on Kubernetes,
where Eureka is dropped in favor of native Kubernetes service discovery.

## Services

| Service | Folder | Tech | Local URL | Description |
|---------|--------|------|-----------|-------------|
| **Invoice service** | `nextjs-backend-invoice-service/` | .NET (ASP.NET Core) | `https://localhost:7052` (`:5052` http) | Main backend API — manages invoices. |
| **Customer service** | `nextjs-backend-cust-service/` | .NET (ASP.NET Core) | `https://localhost:7099` (`:5246` http) | Manages customers. |
| **Dashboard service** | `nextjs-backend-dashboard-service/` | .NET (ASP.NET Core) | `https://localhost:7063` (`:5208` http) | Serves dashboard KPIs / aggregated metrics. |
| **API gateway** | `next-api-gateway/` | .NET + Ocelot | `https://localhost:7019` (`:8080` http) | Single entry point. Routes `/InvoiceGW`, `/CustomerGW`, `/Dashboard` to the matching service. |
| **Eureka server** | `eureka-server/` | Java / Spring Boot | `http://localhost:8761` | Service registry/discovery. Used in local and Docker modes only — **not** used on Kubernetes. |
| **Angular frontend** | `angular-frontend/` | Angular SPA | `http://localhost:4200` | Web UI; calls the backend through the gateway. |

All three .NET services share a SQL Server database (`nextjs-test`).

### How discovery works
- **Local / Docker:** services register themselves with **Eureka**, and the
  Ocelot gateway resolves downstream services by name through Eureka.
- **Kubernetes:** Eureka is removed. Each service is exposed by a Kubernetes
  **Service** (ClusterIP), and the gateway routes directly to those service
  names. ClusterIP load-balances across pods.

---

## Running the project

You can run the stack in three modes.

### 1. Local (dotnet + Eureka)

Start the Eureka discovery server first, then launch all the .NET services and
the frontend with the batch script.

```powershell
# 1. Start Eureka (in its own terminal; leave it running)
cd eureka-server
.\gradlew bootRun

# 2. From the repo root, build + start all services and the Angular frontend
run-all.bat
#   run-all.bat nobuild   # skip the build, just start everything
```

`run-all.bat` opens a separate window per service:

- invoice  → `https://localhost:7052`
- customer → `https://localhost:7099`
- dashboard→ `https://localhost:7063`
- gateway  → `https://localhost:7019`
- frontend → `http://localhost:4200`

> Requires the .NET SDK, the Angular CLI (`ng`), a JDK (for Eureka), and a local
> SQL Server with the `nextjs-test` database.

### 2. Docker Compose

Builds and runs every service — **including Eureka** — as containers on a shared
network. The database stays on the host machine (`host.docker.internal\SQLEXPRESS`).

```bash
# DB credentials come from a .env file (SQL_USER / SQL_PASSWORD)
docker-compose up --build
```

Exposed ports: frontend `4200`, gateway `8080`, invoice `5052`, customer `5246`,
dashboard `5208`, Eureka `8761`. Services wait for Eureka to be healthy before
starting.

### 3. Kubernetes

On Kubernetes **Eureka is not used**. Instead, each deployment is exposed by a
Kubernetes Service, and the API gateway routes traffic directly to those
services (Kubernetes DNS + ClusterIP handle discovery and load-balancing).
Every service has its **own Deployment**, so each can be scaled independently
for maximum scalability (with HPAs autoscaling on CPU).

A single **Ingress** is what allows connections into the Kubernetes cluster from
outside — it is the only public entry point. It routes by path, and the most
important entry point is **`/`**, which points to the Angular frontend (the
`/InvoiceGW`, `/CustomerGW`, `/Dashboard` paths go to the gateway).

```bash
kubectl apply -k k8s/

# route local traffic to the ingress controller (instead of using `minikube tunnel`)
kubectl port-forward -n ingress-nginx service/ingress-nginx-controller 8090:80
```

Then open the app at **<http://myapp.local:8090>** (after mapping `myapp.local`
to the minikube IP in your hosts file — see the k8s README).

See **[k8s/README.md](k8s/README.md)** for full details — image builds, the DB
secret, ingress setup, autoscaling, and moving to a cloud provider.
