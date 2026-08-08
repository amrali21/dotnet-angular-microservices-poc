using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using nextjs_backend_cust_service.Grpc;
using nextjs_backend_cust_service.Models;

namespace nextjs_backend_cust_service.Services
{
    public class CustomerLookupService : CustomerLookup.CustomerLookupBase
    {
        private readonly nextjstestContext _context;

        public CustomerLookupService(nextjstestContext context)
        {
            _context = context;
        }

        public override async Task<CustomerReply> GetCustomerById(CustomerIdRequest request, ServerCallContext context)
        {
            Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id);
            if (customer == null)
                return new CustomerReply { Found = false };

            return ToReply(customer);
        }

        public override async Task<CustomerListReply> GetCustomersByIds(CustomerIdListRequest request, ServerCallContext context)
        {
            List<Customer> customers = await _context.Customers
                .Where(c => request.Ids.Contains(c.Id))
                .ToListAsync();

            CustomerListReply reply = new();
            reply.Customers.AddRange(customers.Select(ToReply));
            return reply;
        }

        private static CustomerReply ToReply(Customer customer) => new()
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            ImageUrl = customer.ImageUrl,
            Found = true
        };
    }
}
