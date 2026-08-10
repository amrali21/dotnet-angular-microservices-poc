using Microsoft.EntityFrameworkCore;
using ledgerly_backend_dashboard_service.Events;
using ledgerly_backend_dashboard_service.Models;
using ledgerly_backend_dashboard_service.Models.FrontEnd;

namespace ledgerly_backend_dashboard_service.Services
{
    public class KpiService
    {
        public const string TotalBilledMetric = "Total Billed";
        public const string CollectedMetric = "Collected";
        public const string OutstandingMetric = "Outstanding";
        public const string CustomersMetric = "Customers";

        private const string PaidStatus = "paid";

        private readonly ledgerlytestContext _context;

        public KpiService(ledgerlytestContext context)
        {
            _context = context;
        }

        public async Task<int> GetMetricAsync(string kpiName)
        {
            Kpi? kpi = await _context.Kpis.FirstOrDefaultAsync(k => k.KpiName == kpiName);
            return kpi?.KpiValue ?? 0;
        }

        /** Absolute set — used by the manual KPI/updateKpis override endpoint. */
        public Task SetMetricAsync(string kpiName, int value)
        {
            return _context.Kpis
                .Where(k => k.KpiName == kpiName)
                .ExecuteUpdateAsync(s => s.SetProperty(k => k.KpiValue, value));
        }

        /**
         * Atomic increment/decrement — one UPDATE kpivalue = kpivalue + @delta
         * statement, no read-then-write round trip, so concurrent consumers can't
         * race and lose an update. Assumes the row already exists (seeded).
         */
        public Task AdjustMetricAsync(string kpiName, int delta)
        {
            return _context.Kpis
                .Where(k => k.KpiName == kpiName)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    k => k.KpiValue,
                    k => k.KpiValue + delta < 0 ? 0 : k.KpiValue + delta));
        }

        public async Task<KpiCardData> GetCardDataAsync()
        {
            return new KpiCardData
            {
                totalBilled = await GetMetricAsync(TotalBilledMetric),
                collected = await GetMetricAsync(CollectedMetric),
                outstanding = await GetMetricAsync(OutstandingMetric),
                customers = await GetMetricAsync(CustomersMetric),
            };
        }

        public async Task UpsertRevenueAsync(string month, int year, int revenue)
        {
            Revenue? existing = await _context.Revenues.FirstOrDefaultAsync(r => r.Month == month && r.Year == year);
            if (existing == null)
            {
                await _context.Revenues.AddAsync(new Revenue { Month = month, Year = year, Revenue1 = revenue });
            }
            else
            {
                existing.Revenue1 = revenue;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ApplyInvoiceCreatedAsync(InvoiceCreatedEvent e)
        {
            await AdjustMetricAsync(TotalBilledMetric, e.Amount);

            if (e.Status == PaidStatus)
            {
                await AdjustMetricAsync(CollectedMetric, e.Amount);
                await ApplyRevenueDeltaAsync(e.Date, e.Amount);
            }
            else
            {
                await AdjustMetricAsync(OutstandingMetric, e.Amount);
            }
        }

        public async Task ApplyInvoiceUpdatedAsync(InvoiceUpdatedEvent e)
        {
            // Amount is immutable after creation — a status change just moves the
            // same amount between the outstanding/collected buckets (and revenue).
            if (e.OldStatus == e.NewStatus)
                return;

            if (e.OldStatus == PaidStatus)
                await AdjustMetricAsync(CollectedMetric, -e.Amount);
            else
                await AdjustMetricAsync(OutstandingMetric, -e.Amount);

            if (e.NewStatus == PaidStatus)
                await AdjustMetricAsync(CollectedMetric, e.Amount);
            else
                await AdjustMetricAsync(OutstandingMetric, e.Amount);

            if (e.OldStatus == PaidStatus)
                await ApplyRevenueDeltaAsync(e.Date, -e.Amount);
            if (e.NewStatus == PaidStatus)
                await ApplyRevenueDeltaAsync(e.Date, e.Amount);
        }

        public async Task ApplyInvoiceDeletedAsync(InvoiceDeletedEvent e)
        {
            await AdjustMetricAsync(TotalBilledMetric, -e.Amount);

            if (e.Status == PaidStatus)
            {
                await AdjustMetricAsync(CollectedMetric, -e.Amount);
                await ApplyRevenueDeltaAsync(e.Date, -e.Amount);
            }
            else
            {
                await AdjustMetricAsync(OutstandingMetric, -e.Amount);
            }
        }

        public Task ApplyCustomerCreatedAsync(CustomerCreatedEvent e)
        {
            return AdjustMetricAsync(CustomersMetric, 1);
        }

        public Task ApplyCustomerDeletedAsync(CustomerDeletedEvent e)
        {
            return AdjustMetricAsync(CustomersMetric, -1);
        }

        private async Task ApplyRevenueDeltaAsync(DateTime date, int delta)
        {
            string month = date.ToString("MMM");
            int year = date.Year;
            Revenue? existing = await _context.Revenues.FirstOrDefaultAsync(r => r.Month == month && r.Year == year);
            int current = existing?.Revenue1 ?? 0;
            await UpsertRevenueAsync(month, year, Math.Max(0, current + delta));
        }
    }
}
