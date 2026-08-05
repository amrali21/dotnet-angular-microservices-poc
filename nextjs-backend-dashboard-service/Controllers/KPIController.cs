using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nextjs_backend_dashboard_service.Models;

namespace nextjs_backend_dashboard_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class KPIController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        public KPIController(nextjstestContext nextjstestContext)
        {
            _nextjstestContext = nextjstestContext;
        }

        [HttpGet]
        public async Task<IActionResult> getRevenue()
        {
            var output = await _nextjstestContext.Revenues.Select(r => new
            {
                month = r.Month,
                revenue = r.Revenue1
            }).ToListAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> getCardData()
        {
            var kpi = await _nextjstestContext.Kpis.FirstOrDefaultAsync();

            return Ok(new
            {
                totalBilled = kpi?.TotalBilled ?? 0,
                collected = kpi?.Collected ?? 0,
                outstanding = kpi?.Outstanding ?? 0,
                customers = kpi?.Customers ?? 0
            });
        }

        [HttpPut]
        public async Task<IActionResult> updateKpis([FromBody] UpdatedKpis kpis)
        {
            try
            {
                await _nextjstestContext.Kpis.ExecuteDeleteAsync();
                await _nextjstestContext.Kpis.AddAsync(new Kpi
                {
                    TotalBilled = kpis.totalBilled,
                    Collected = kpis.collected,
                    Outstanding = kpis.outstanding,
                    Customers = kpis.customers
                });
                await _nextjstestContext.SaveChangesAsync();
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
                Revenue? existing = await _nextjstestContext.Revenues.FirstOrDefaultAsync(r => r.Month == revenue.month);
                if (existing == null)
                {
                    await _nextjstestContext.Revenues.AddAsync(new Revenue
                    {
                        Month = revenue.month,
                        Revenue1 = revenue.revenue
                    });
                }
                else
                {
                    existing.Revenue1 = revenue.revenue;
                }

                await _nextjstestContext.SaveChangesAsync();
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
        public int revenue { get; set; }
    }
}
