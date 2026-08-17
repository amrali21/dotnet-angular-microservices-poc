using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ledgerly_backend.Events;
using ledgerly_backend.Grpc;
using ledgerly_backend.Models;
using ledgerly_backend.Models.FrontEnd;
using ledgerly_backend.Services;

namespace ledgerly_backend.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        ledgerlytestContext _ledgerlytestContext;
        CustomerLookup.CustomerLookupClient _customerLookup;
        RabbitMqPublisher _publisher;
        ILogger<InvoiceController> _logger;
        public InvoiceController(ledgerlytestContext ledgerlytestContext, CustomerLookup.CustomerLookupClient customerLookup, RabbitMqPublisher publisher, ILogger<InvoiceController> logger)
        {
            _ledgerlytestContext = ledgerlytestContext;
            _customerLookup = customerLookup;
            _publisher = publisher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> fetchFilteredInvoices(string? query, int itemsPerPage, int offset)
        {
            try
            {
                IQueryable<Invoice> invoiceQuery = _ledgerlytestContext.Invoices;

                if (!string.IsNullOrEmpty(query))
                {
                    // Search is against customer name/email, which live in cust-service —
                    // resolve matching customer ids there first, so the invoices query
                    // below can filter and paginate in SQL without ever reading the whole table.
                    CustomerIdListReply searchReply = await _customerLookup.SearchCustomerIdsAsync(new CustomerSearchRequest { Query = query });
                    HashSet<string> matchingIds = searchReply.Ids.ToHashSet();
                    invoiceQuery = invoiceQuery.Where(i => matchingIds.Contains(i.CustomerId));
                }

                int count = await invoiceQuery.CountAsync();
                List<Invoice> page = await invoiceQuery
                    .OrderByDescending(i => i.Date)
                    .ThenBy(i => i.Id)
                    .Skip(offset)
                    .Take(itemsPerPage)
                    .ToListAsync();

                CustomerIdListRequest pageRequest = new();
                pageRequest.Ids.AddRange(page.Select(i => i.CustomerId).Distinct());
                CustomerListReply pageReply = await _customerLookup.GetCustomersByIdsAsync(pageRequest);
                Dictionary<string, CustomerReply> customersById = pageReply.Customers.ToDictionary(c => c.Id);

                List<InvoiceListItem> data = page
                    .Where(i => customersById.ContainsKey(i.CustomerId))
                    .Select(i => new InvoiceListItem
                    {
                        id = i.Id,
                        amount = i.Amount,
                        date = i.Date,
                        status = i.Status,
                        name = customersById[i.CustomerId].Name,
                        email = customersById[i.CustomerId].Email,
                        image_url = customersById[i.CustomerId].ImageUrl
                    }).ToList();

                return Ok(new PagedResult<InvoiceListItem>
                {
                    data = data,
                    count = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch filtered invoices for query {Query}", query);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> fetchInvoiceById(int id)
        {
            try
            {
                Invoice? invoice = await _ledgerlytestContext.Invoices.FirstOrDefaultAsync(i => i.Id == id);
                if (invoice == null)
                    return NotFound();

                CustomerReply customer = await _customerLookup.GetCustomerByIdAsync(new CustomerIdRequest { Id = invoice.CustomerId });

                return Ok(new InvoiceDetail
                {
                    id = invoice.Id,
                    name = customer.Found ? customer.Name : null,
                    customer_id = invoice.CustomerId,
                    amount = invoice.Amount,
                    status = invoice.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch invoice {InvoiceId}", id);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> insertInvoice([FromBody] InsertedInvoice invoice)
        {
            CustomerReply customer = await _customerLookup.GetCustomerByIdAsync(new CustomerIdRequest { Id = invoice.customerId });
            if (!customer.Found)
                return BadRequest("customer not found");

            Invoice newInvoice = new()
            {
                CustomerId = invoice.customerId,
                Amount = invoice.amount,
                Status = invoice.status,
                Date = DateTime.UtcNow
            };

            try
            {
                await _ledgerlytestContext.Invoices.AddAsync(newInvoice);
                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert invoice for customer {CustomerId}", newInvoice.CustomerId);
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse updateInvoice request body");
                return BadRequest("Couldn't parse formula");
            }
            Invoice? oldInvoice = await _ledgerlytestContext.Invoices.FirstOrDefaultAsync(i => i.Id == invoice.id);
            string previousStatus = oldInvoice.Status;
            try
            {
                // Only the status is mutable after creation — amount/customer are fixed at insert time.
                oldInvoice.Status = invoice.status;

                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update invoice {InvoiceId}", oldInvoice.Id);
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
        public async Task<IActionResult> deleteInvoice(int id)
        {
            if (!await _ledgerlytestContext.Invoices.AnyAsync(i => i.Id == id))
                return BadRequest("invoice not found");

            Invoice invoice = await _ledgerlytestContext.Invoices.FirstAsync(i => i.Id == id);

            try
            {
                _ledgerlytestContext.Invoices.Remove(invoice);
                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete invoice {InvoiceId}", invoice.Id);
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
