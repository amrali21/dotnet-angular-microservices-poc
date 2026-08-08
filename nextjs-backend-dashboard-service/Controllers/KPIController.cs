using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nextjs_backend_dashboard_service.Models;
using nextjs_backend_dashboard_service.Services;

namespace nextjs_backend_dashboard_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class KPIController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        KpiService _kpiService;
        public KPIController(nextjstestContext nextjstestContext, KpiService kpiService)
        {
            _nextjstestContext = nextjstestContext;
            _kpiService = kpiService;
        }

        [HttpGet]
        public async Task<IActionResult> getRevenue(int? year)
        {
            int targetYear = year ?? DateTime.UtcNow.Year;

            var output = await _nextjstestContext.Revenues
                .Where(r => r.Year == targetYear)
                .Select(r => new
                {
                    month = r.Month,
                    revenue = r.Revenue1 / 100
                }).ToListAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> getCardData()
        {
            return Ok(await _kpiService.GetCardDataAsync());
        }

        [HttpPut]
        public async Task<IActionResult> updateKpis([FromBody] UpdatedKpis kpis)
        {
            try
            {
                await _kpiService.SetMetricAsync(KpiService.TotalBilledMetric, kpis.totalBilled);
                await _kpiService.SetMetricAsync(KpiService.CollectedMetric, kpis.collected);
                await _kpiService.SetMetricAsync(KpiService.OutstandingMetric, kpis.outstanding);
                await _kpiService.SetMetricAsync(KpiService.CustomersMetric, kpis.customers);
            }
            catch
            {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> updateRevenue([FromBody] UpdatedRevenue revenue)
        {
            try
            {
                await _kpiService.UpsertRevenueAsync(revenue.month, revenue.year, revenue.revenue);
            }
            catch
            {
                return StatusCode(500);
            }

            return Ok();
        }
    }

    public class UpdatedKpis
    {
        public int totalBilled { get; set; }
        public int collected { get; set; }
        public int outstanding { get; set; }
        public int customers { get; set; }
    }

    public class UpdatedRevenue
    {
        public string month { get; set; } = null!;
        public int year { get; set; }
        public int revenue { get; set; }
    }
}
