using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nextjs_backend.Models;

namespace nextjs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NextController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        public NextController(nextjstestContext nextjstestContext)
        {
            _nextjstestContext = nextjstestContext;
        }

        public async Task<IActionResult> getLatestInvoices()
        {
            var output = await (from invoice in _nextjstestContext.Invoices
                                join customer in _nextjstestContext.Customers on
                                invoice.CustomerId equals customer.Id
                                orderby invoice.Date
                                select new
                                {
                                    amount = invoice.Amount,
                                    name = customer.Name,
                                    image_url = customer.ImageUrl,
                                    email = customer.Email,
                                    id = invoice.Id
                                }).Take(5).ToListAsync();

            return Ok(output);
        }

        public async Task<IEnumerable<Revenue>> getRevenue()
        {
           return await _nextjstestContext.Revenues.ToListAsync();
        }
    }
}
