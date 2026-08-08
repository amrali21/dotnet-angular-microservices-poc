using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using nextjs_backend.Events;
using nextjs_backend.Grpc;
using nextjs_backend.Models;
using nextjs_backend.Models.FrontEnd;
using nextjs_backend.Services;

namespace nextjs_backend.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        nextjstestContext _nextjstestContext;
        CustomerLookup.CustomerLookupClient _customerLookup;
        RabbitMqPublisher _publisher;
        public InvoiceController(nextjstestContext nextjstestContext, CustomerLookup.CustomerLookupClient customerLookup, RabbitMqPublisher publisher)
        {
            _nextjstestContext = nextjstestContext;
            _customerLookup = customerLookup;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> fetchFilteredInvoices(string? query, int itemsPerPage, int offset)
        {
            var invoices = await _nextjstestContext.Invoices.ToListAsync();

            CustomerIdListRequest idRequest = new();
            idRequest.Ids.AddRange(invoices.Select(i => i.CustomerId).Distinct());
            CustomerListReply customerList = await _customerLookup.GetCustomersByIdsAsync(idRequest);
            Dictionary<string, CustomerReply> customersById = customerList.Customers.ToDictionary(c => c.Id);

            var output = (from i in invoices
                          where customersById.ContainsKey(i.CustomerId)
                          let c = customersById[i.CustomerId]
                          where query == null || (c.Name.Contains(query) || c.Email.Contains(query))
                          select new
                          {
                              id = i.Id,
                              amount = i.Amount,
                              date = i.Date,
                              status = i.Status,
                              name = c.Name,
                              email = c.Email,
                              image_url = c.ImageUrl
                          }).ToList();

            return Ok(new
            {
                data = output.Skip(offset).Take(itemsPerPage).ToList(),
                count = output.Count
            });
        }

        [HttpGet]
        public async Task<IActionResult> fetchInvoicesPages(string? query)
        {
            var invoices = await _nextjstestContext.Invoices.ToListAsync();

            CustomerIdListRequest idRequest = new();
            idRequest.Ids.AddRange(invoices.Select(i => i.CustomerId).Distinct());
            CustomerListReply customerList = await _customerLookup.GetCustomersByIdsAsync(idRequest);
            Dictionary<string, CustomerReply> customersById = customerList.Customers.ToDictionary(c => c.Id);

            int count = (from i in invoices
                         where customersById.ContainsKey(i.CustomerId)
                         let c = customersById[i.CustomerId]
                         where query == null || (c.Name.Contains(query) || c.Email.Contains(query))
                         select i.Id).Count();

            return Ok(count);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> fetchInvoiceById(string id)
        {
            Invoice? invoice = await _nextjstestContext.Invoices.FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null)
                return NotFound();

            CustomerReply customer = await _customerLookup.GetCustomerByIdAsync(new CustomerIdRequest { Id = invoice.CustomerId });

            return Ok(new
            {
                id = invoice.Id,
                name = customer.Found ? customer.Name : null,
                customer_id = invoice.CustomerId,
                amount = invoice.Amount,
                status = invoice.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> insertInvoice([FromBody] InsertedInvoice invoice)
        {
            CustomerReply customer = await _customerLookup.GetCustomerByIdAsync(new CustomerIdRequest { Id = invoice.customerId });
            if (!customer.Found)
                return BadRequest("customer not found");

            Invoice newInvoice = new()
            {
                Id = _nextjstestContext.Invoices.Max(i => i.Id) + 1,
                CustomerId = invoice.customerId,
                Amount = invoice.amount,
                Status = invoice.status,
                Date = DateTime.UtcNow
            };

            try
            {
                await _nextjstestContext.Invoices.AddAsync(newInvoice);
                await _nextjstestContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }

            await _publisher.PublishAsync("invoice.created", new InvoiceCreatedEvent
            {
                InvoiceId = newInvoice.Id,
                CustomerId = newInvoice.CustomerId,
                Amount = newInvoice.Amount,
                Status = newInvoice.Status,
                Date = newInvoice.Date
            });

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
            string previousStatus = oldInvoice.Status;
            try
            {
                // Only the status is mutable after creation — amount/customer are fixed at insert time.
                oldInvoice.Status = invoice.status;

                await _nextjstestContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500);
            }

            await _publisher.PublishAsync("invoice.updated", new InvoiceUpdatedEvent
            {
                InvoiceId = oldInvoice.Id,
                CustomerId = oldInvoice.CustomerId,
                Amount = oldInvoice.Amount,
                OldStatus = previousStatus,
                NewStatus = oldInvoice.Status,
                Date = oldInvoice.Date
            });

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

            await _publisher.PublishAsync("invoice.deleted", new InvoiceDeletedEvent
            {
                InvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                Amount = invoice.Amount,
                Status = invoice.Status,
                Date = invoice.Date
            });

            return Ok();
        }

    }
}
