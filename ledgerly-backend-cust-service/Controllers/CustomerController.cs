using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ledgerly_backend_cust_service.Events;
using ledgerly_backend_cust_service.Models;
using ledgerly_backend_cust_service.Models.FrontEnd;
using ledgerly_backend_cust_service.Services;

namespace ledgerly_backend_cust_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        ledgerlytestContext _ledgerlytestContext;
        RabbitMqPublisher _publisher;
        ILogger<CustomerController> _logger;
        public CustomerController(ledgerlytestContext ledgerlytestContext, RabbitMqPublisher publisher, ILogger<CustomerController> logger)
        {
            _ledgerlytestContext = ledgerlytestContext;
            _publisher = publisher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> insertCustomer([FromBody] InsertedCustomer customer)
        {
            Customer newCustomer = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = customer.name,
                Email = customer.email,
                ImageUrl = customer.image_url
            };

            try
            {
                await _ledgerlytestContext.Customers.AddAsync(newCustomer);
                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert customer {Email}", newCustomer.Email);
                return StatusCode(500);
            }

            await _publisher.PublishAsync("customer.created", new CustomerCreatedEvent
            {
                CustomerId = newCustomer.Id
            });

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> fetchCustomers()
        {
            try
            {
                return Ok(await (from c in _ledgerlytestContext.Customers
                                  orderby c.Name
                                  select new CustomerSummary
                                  {
                                      id = c.Id,
                                      name = c.Name,
                                      email = c.Email,
                                      image_url = c.ImageUrl
                                  }).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch customers");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> fetchCustomerByID(string id)
        {
            try
            {
                return Ok(await (from c in _ledgerlytestContext.Customers
                                  orderby c.Name
                                  where c.Id == id
                                  select new CustomerSummary
                                  {
                                      id = c.Id,
                                      name = c.Name,
                                      email = c.Email,
                                      image_url = c.ImageUrl
                                  }).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch customer {CustomerId}", id);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> fetchFilteredCustomers(string? query, int itemsPerPage, int offset)
        {
            try
            {
                var output = (from c in _ledgerlytestContext.Customers
                              orderby c.Name
                              where ((query == "" || query == null) || c.Name.StartsWith(query))
                              select new CustomerSummary
                              {
                                  id = c.Id,
                                  name = c.Name,
                                  email = c.Email,
                                  image_url = c.ImageUrl
                              }).AsQueryable();

                return Ok(new PagedResult<CustomerSummary>
                {
                    data = await output.Skip(offset).Take(itemsPerPage).ToListAsync(),
                    count = output.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch filtered customers for query {Query}", query);
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> updateCustomer()
        {
            UpdatedCustomer customer;
            try
            {
                using StreamReader streamReader = new(Request.Body);
                customer = JsonConvert.DeserializeObject<UpdatedCustomer>(await streamReader.ReadToEndAsync())!;

                if (customer == null)
                    return BadRequest("Can't deserialize");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse updateCustomer request body");
                return BadRequest("Couldn't parse body");
            }

            Customer? existing = await _ledgerlytestContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.id);
            if (existing == null)
                return NotFound("Customer not found");

            try
            {
                existing.Name = customer.name;
                existing.Email = customer.email;
                existing.ImageUrl = customer.image_url;

                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer {CustomerId}", existing.Id);
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCustomer(string id)
        {
            if (!await _ledgerlytestContext.Customers.AnyAsync(c => c.Id == id))
                return BadRequest("Customer not found");

            Customer customer = await _ledgerlytestContext.Customers.FirstAsync(c => c.Id == id);

            try
            {
                _ledgerlytestContext.Customers.Remove(customer);
                await _ledgerlytestContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customer {CustomerId}", customer.Id);
                return StatusCode(500);
            }

            await _publisher.PublishAsync("customer.deleted", new CustomerDeletedEvent
            {
                CustomerId = customer.Id
            });

            return Ok();
        }
    }
}
