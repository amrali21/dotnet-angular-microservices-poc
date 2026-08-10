using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ledgerly_backend_dashboard_service.Models;
using ledgerly_backend_dashboard_service.Models.FrontEnd;
using ledgerly_backend_dashboard_service.Services;

namespace ledgerly_backend_dashboard_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class KPIController : ControllerBase
    {
        ledgerlytestContext _ledgerlytestContext;
        KpiService _kpiService;
        ILogger<KPIController> _logger;
        public KPIController(ledgerlytestContext ledgerlytestContext, KpiService kpiService, ILogger<KPIController> logger)
        {
            _ledgerlytestContext = ledgerlytestContext;
            _kpiService = kpiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> getRevenue(int? year)
        {
            int targetYear = year ?? DateTime.UtcNow.Year;

            try
            {
                return Ok(await _ledgerlytestContext.Revenues
                    .Where(r => r.Year == targetYear)
                    .Select(r => new RevenuePoint
                    {
                        month = r.Month,
                        revenue = r.Revenue1 / 100
                    }).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch revenue for {Year}", targetYear);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> getCardData()
        {
            try
            {
                return Ok(await _kpiService.GetCardDataAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch KPI card data");
                return StatusCode(500);
            }
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update KPIs");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update revenue for {Month}/{Year}", revenue.month, revenue.year);
                return StatusCode(500);
            }

            return Ok();
        }
    }
}
