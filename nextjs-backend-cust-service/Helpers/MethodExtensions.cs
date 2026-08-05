using Microsoft.EntityFrameworkCore;
using nextjs_backend_cust_service.Models;

namespace nextjs_backend_cust_service.Helpers
{
    public static class MethodExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            List<Customer> customers = new List<Customer> {
             new Customer {
                Id = "3958dc9e-712f-4377-85e9-fec4b6a6442a",
                Name = "Delba de Oliveira",
                Email = "delba@oliveira.com",
                ImageUrl = "/customers/delba-de-oliveira.png",
              },
              new Customer
              {
                Id = "3958dc9e-742f-4377-85e9-fec4b6a6442a",
                Name = "Lee Robinson",
                Email = "lee@robinson.com",
                ImageUrl = "/customers/lee-robinson.png",
              },
              new Customer
              {
                Id = "3958dc9e-737f-4377-85e9-fec4b6a6442a",
                Name = "Hector Simpson",
                Email = "hector@simpson.com",
                ImageUrl = "/customers/hector-simpson.png",
              },
             new Customer
             {
                Id = "50ca3e18-62cd-11ee-8c99-0242ac120002",
                Name = "Steven Tey",
                Email = "steven@tey.com",
                ImageUrl = "/customers/steven-tey.png",
              },
             new Customer
             {
                Id = "3958dc9e-787f-4377-85e9-fec4b6a6442a",
                Name = "Steph Dietz",
                Email = "steph@dietz.com",
                ImageUrl = "/customers/steph-dietz.png",
              },
            new Customer
            {
                Id = "76d65c26-f784-44a2-ac19-586678f7c2f2",
                Name = "Michael Novotny",
                Email = "michael@novotny.com",
                ImageUrl = "/customers/michael-novotny.png",
              },
             new Customer
             {
                Id = "d6e15727-9fe1-4961-8c5b-ea44a9bd81aa",
                Name = "Evil Rabbit",
                Email = "evil@rabbit.com",
                ImageUrl = "/customers/evil-rabbit.png",
              },
             new Customer
             {
                Id = "126eed9c-c90c-4ef6-a4a8-fcf7408d3c66",
                Name = "Emil Kowalski",
                Email = "emil@kowalski.com",
                ImageUrl = "/customers/emil-kowalski.png",
              },
            new Customer
            {
                Id = "CC27C14A-0ACF-4F4A-A6C9-D45682C144B9",
                Name = "Amy Burns",
                Email = "amy@burns.com",
                ImageUrl = "/customers/amy-burns.png",
              },
            new Customer
            {
                Id = "13D07535-C59E-4157-A011-F8D2EF4E0CBB",
                Name = "Balazs Orban",
                Email = "balazs@orban.com",
                ImageUrl = "/customers/balazs-orban.png",
              }
            };

            modelBuilder.Entity<Customer>().HasData(customers);
        }
    }
}
