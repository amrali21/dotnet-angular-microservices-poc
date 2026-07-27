# Ledgerly — billing console

The Angular SPA for this POC. It talks to the three .NET microservices through
the Ocelot API gateway and never calls a service directly.

| Screen | Route | Backing service |
| --- | --- | --- |
| Overview (KPIs + monthly revenue) | `/` | `dashboard-service` |
| Invoices (list, edit, delete) | `/invoices`, `/invoices/:id` | `invoice-service` |
| Customers (list, profile) | `/customers`, `/customers/:id` | `cust-service` |

## Running it

```bash
npm install
npm start          # http://localhost:4200
npm test           # Karma + Jasmine
npm run build      # production bundle -> dist/ledgerly/browser
```

The gateway URL is a build-time constant, picked per configuration:

| Configuration | File | `ACTION_URL` |
| --- | --- | --- |
| default / `production` | `src/app/app.settings.ts` | `https://localhost:7019` |
| `docker` | `src/app/app.settings.docker.ts` | the gateway's compose address |
| `kubernetes` | `src/app/app.settings.kubernetes.ts` | `''` (same-origin, via Ingress) |

Changing it means a rebuild — see the notes in the repo-root `README.md`.

## UI conventions

- **Design tokens live in `src/styles.css`**, declared once per theme on
  `<html data-theme="light|dark">`. Components are written against the tokens
  (`--surface`, `--text-2`, `--status-good`, …), never against raw colours, so
  the two themes stay in sync.
- **Theme switching** is handled by `Services/Theme/theme.service.ts`. It flips
  `data-theme` and swaps the PrimeNG Aura stylesheet, which is copied to
  `/themes/aura-{light,dark}-indigo/` by the asset pipeline in `angular.json`.
  `index.html` applies the stored choice before first paint to avoid a flash.
- **Components come from PrimeNG 17** (Aura indigo) with PrimeFlex for layout.
  Plain `.btn` / `.surface-card` / `.status-badge` primitives in `styles.css`
  cover the cases where a full PrimeNG component would be overkill.
- **The revenue chart is hand-rolled SVG** in `Components/Home`, so there is no
  charting dependency. Geometry is computed in the component; the template only
  renders it. It ships a table view, keyboard-focusable bars, and a single-hue
  series validated for contrast in both themes.
- **Money formatting is centralised** in `Shared/display.ts`. Invoice amounts
  are stored in cents by the API and shown in dollars; the `revenue` table is
  already in dollars. `formatCents` / `formatDollars` keep that explicit.
- **Avatars are generated from initials.** The seed data's `image_url` values
  point at assets that only exist in the original Next.js sample, so they are
  never requested.

## Known gaps

- No auth service, so the user chip in the top bar is static.
- `cust-service` has no fetch-by-id endpoint, so `/customers/:id` reads the
  record from the loaded list and shows an explanatory state if opened directly.
