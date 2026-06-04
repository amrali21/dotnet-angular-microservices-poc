export let ACTION_URL: string = ''
// EMPTY ON PURPOSE for Kubernetes.
// The Angular app and the API gateway are served behind the SAME Ingress host,
// so client-side API calls become relative (same-origin), e.g. the browser
// requests  /InvoiceGW/Invoice/fetchFilteredInvoices  which the Ingress routes
// to the api-gateway. This avoids a hard-coded host and avoids CORS.
// See k8s/20-ingress.yaml and k8s/README.md.
