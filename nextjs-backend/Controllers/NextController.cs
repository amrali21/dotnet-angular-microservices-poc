using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using nextjs_backend.Models;
using nextjs_backend.Models.FrontEnd;

namespace nextjs_backend.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class NextController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        public NextController(nextjstestContext nextjstestContext)
        {
            _nextjstestContext = nextjstestContext;
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> fetchFilteredInvoices(string? query, int itemsPerPage, int offset) // linq error
        {
            var output = await (from i in _nextjstestContext.Invoices
             join c in _nextjstestContext.Customers
             on i.CustomerId equals c.Id
             where query == null || (c.Name.Contains(query) || c.Email.Contains(query) /*|| i.Amount.ToString().Contains(query) ||
             i.Date.ToString().Contains(query) || i.Status.ToString().Contains(query)*/)
             select new
             {
                 id = i.Id,
                 amount = i.Amount,
                 date = i.Date,
                 status = i.Status,
                 name = c.Name,
                 email = c.Email,
                 image_url = c.ImageUrl
             }).Skip(offset).Take(itemsPerPage).ToListAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> fetchInvoicesPages(string? query) // linq error
        {
            var output = await (from i in _nextjstestContext.Invoices
                                join c in _nextjstestContext.Customers
                                on i.CustomerId equals c.Id             
                                where query == null || (c.Name.Contains(query) || c.Email.Contains(query) /*|| (string)(object)i.Amount.Contains(query) ||
                                i.Date.ToString().Contains(query) || i.Status.ToString().Contains(query)*/)
                                select i.Id).CountAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> fetchInvoiceById(string id)
        {
            var output = await (from i in _nextjstestContext.Invoices
                                where i.Id == id
                                select new
                                {
                                    id = i.Id,
                                    customer_id = i.CustomerId,
                                    amount = i.Amount / 100,
                                    status = i.Status
                                }).FirstOrDefaultAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> fetchCustomers()
        {
            var output = await (from c in _nextjstestContext.Customers
                                orderby c.Name
                                select new
                                {
                                    id = c.Id,
                                    name = c.Name,
                                    email = c.Email,
                                    image_url = c.ImageUrl
                                }).ToListAsync();               

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> fetchFilteredCustomers()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> getUser(string email)
        {
            var output = (from u in _nextjstestContext.Users
             where u.Email == email
             select new
             {
                 id = u.Id,
                 name = u.Name,
                 email = u.Email,
                 password = u.Password
             }).ToListAsync();

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> insertInvoice([FromBody] InsertedInvoice invoice)
        {

            try
            {
                await _nextjstestContext.Invoices.AddAsync(new Invoice
                {
                    Id = _nextjstestContext.Invoices.Max(i => i.Id) + 1,
                    CustomerId = invoice.customerId,
                    Amount = invoice.amount,
                    Status = invoice.status
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
        public async Task<IActionResult> updateInvoice(/*[FromBody] UpdatedInvoice invoice*/)
        {
            UpdatedInvoice invoice;
            try
            {
                using StreamReader streamReader = new(Request.Body);
                invoice = JsonConvert.DeserializeObject<UpdatedInvoice>(await streamReader.ReadToEndAsync());

                if (invoice == null)
                    return BadRequest("Can't deserialize");
            }
            catch
            {
                return BadRequest("Couldn't parse formula");
            }
            Invoice? oldInvoice = await _nextjstestContext.Invoices.FirstOrDefaultAsync(i => i.Id == invoice.id);
            try
            {
                oldInvoice.CustomerId = invoice.customerId;
                oldInvoice.Amount = invoice.amount;
                oldInvoice.Status = invoice.status;

                await _nextjstestContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteInvoice(string id)
        {
            if (!await _nextjstestContext.Invoices.AnyAsync(i => i.Id == id))
                return BadRequest("invoice not found");

            Invoice invoice = await _nextjstestContext.Invoices.FirstAsync(i => i.Id == id);

            try
            {
                _nextjstestContext.Invoices.Remove(invoice);
                await _nextjstestContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }

            return Ok();
        }

    }
}
