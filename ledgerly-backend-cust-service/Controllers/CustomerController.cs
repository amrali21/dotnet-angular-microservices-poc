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
        public CustomerController(ledgerlytestContext ledgerlytestContext, RabbitMqPublisher publisher)
        {
            _ledgerlytestContext = ledgerlytestContext;
            _publisher = publisher;
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
            catch
            {
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
            var output = await (from c in _ledgerlytestContext.Customers
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
        public async Task<IActionResult> fetchCustomerByID(string id)
        {
            var output = await (from c in _ledgerlytestContext.Customers
                                orderby c.Name
                                where c.Id == id
                                select new
                                {
                                    id = c.Id,
                                    name = c.Name,
                                    email = c.Email,
                                    image_url = c.ImageUrl
                                }).FirstOrDefaultAsync();

            return Ok(output);
        }

        [HttpGet]
        public async Task<IActionResult> fetchFilteredCustomers(string? query, int itemsPerPage, int offset)
        {
            var output = (from c in _ledgerlytestContext.Customers
                          orderby c.Name
                          where ((query == "" || query == null) || c.Name.StartsWith(query))
                          select new
                          {
                              id = c.Id,
                              name = c.Name,
                              email = c.Email,
                              image_url = c.ImageUrl
                          }).AsQueryable();

            return Ok(
                new
                {
                    data = await output.Skip(offset).Take(itemsPerPage).ToListAsync(),
                    count = output.Count()
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> fetchCustomerPages(string? query)
        {
            var output = await (from c in _ledgerlytestContext.Customers
                                orderby c.Name
                                where ((query == "" || query == null) || c.Name.StartsWith(query))
                                select new
                                {
                                    id = c.Id,
                                    name = c.Name,
                                    email = c.Email,
                                    image_url = c.ImageUrl
                                }).CountAsync();

            return Ok(output);
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
            catch
            {
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
            catch
            {
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
            catch
            {
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
