using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nextjs_backend_dashboard_service.Models;

namespace nextjs_backend_dashboard_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        public DashboardController(nextjstestContext nextjstestContext)
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
            return Ok(new
            {
                invoiceCount = _nextjstestContext.Invoices.Count(),
                customerCount = _nextjstestContext.Customers.Count(),
                invoiceStatus = new
                {
                    paid = _nextjstestContext.Invoices.Where(i => i.Status == "paid").Sum(i => i.Amount),
                    pending = _nextjstestContext.Invoices.Where(i => i.Status == "pending").Sum(i => i.Amount),
                }
            });
        }
    }
}
