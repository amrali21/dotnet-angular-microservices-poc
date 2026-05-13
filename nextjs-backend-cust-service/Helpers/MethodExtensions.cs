using Microsoft.EntityFrameworkCore;
using nextjs_backend_cust_service.Models;

namespace nextjs_backend_cust_service.Helpers
{
    public static class MethodExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            List<User> users = new List<User>() {
                new User{
                    Id= "410544b2-4001-4271-9855-fec4b6a6442a",
                    Name= "User",
                    Email= "user@nextmail.com",
                    Password= "123456"
                }
            };

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

            List<Invoice> invoices = new List<Invoice> {
               new Invoice
               {
                   Id = "1",
                CustomerId= customers[0].Id,
                Amount= 15795,
                Status= "pending",
                Date= DateTime.Parse("2022-12-06"),
              },
              new Invoice{
                Id = "2",
                CustomerId= customers[1].Id,
                Amount= 20348,
                Status= "pending",
                Date= DateTime.Parse("2022-11-14"),
              },
              new Invoice{
                Id = "3",
                CustomerId= customers[4].Id,
                Amount= 3040,
                Status= "paid",
                Date= DateTime.Parse("2022-10-29"),
              },
             new Invoice {
                Id = "4",
                CustomerId= customers[3].Id,
                Amount= 44800,
                Status= "paid",
                Date= DateTime.Parse("2023-09-10"),
              },
              new Invoice{
                Id = "5",
                CustomerId= customers[5].Id,
                Amount= 34577,
                Status= "pending",
                Date= DateTime.Parse("2023-08-05"),
              },
             new Invoice {
                Id = "6",
                CustomerId= customers[7].Id,
                Amount= 54246,
                Status= "pending",
                Date= DateTime.Parse("2023-07-16"),
              },
             new Invoice {
                Id = "7",
                CustomerId= customers[6].Id,
                Amount= 666,
                Status= "pending",
                Date= DateTime.Parse("2023-06-27"),
              },
             new Invoice {
                Id = "8",
                CustomerId= customers[3].Id,
                Amount= 32545,
                Status= "paid",
                Date= DateTime.Parse("2023-06-09"),
              },
             new Invoice {
                Id = "9",
                CustomerId= customers[4].Id,
                Amount= 1250,
                Status= "paid",
                Date= DateTime.Parse("2023-06-17"),
              },
             new Invoice {
                Id = "10",
                CustomerId= customers[5].Id,
                Amount= 8546,
                Status= "paid",
                Date= DateTime.Parse("2023-06-07"),
              },
              new Invoice{
                Id = "11",
                CustomerId= customers[1].Id,
                Amount= 500,
                Status= "paid",
                Date= DateTime.Parse("2023-08-19"),
              },
             new Invoice {
                Id = "12",
                CustomerId= customers[5].Id,
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-06-03"),
              },
              new Invoice{
                Id = "13",
                CustomerId= customers[2].Id,
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-06-18"),
              },
             new Invoice {
                Id = "14",
                CustomerId= customers[0].Id,
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-10-04"),
              },
             new Invoice {
                Id = "15",
                CustomerId= customers[2].Id,
                Amount= 1000,
                Status= "paid",
                Date= DateTime.Parse("2022-06-05"),
              },
            };

            List<Revenue> revenues = new List<Revenue>()
            {
                 new Revenue { Month= "Jan", Revenue1= 2000 },
                 new Revenue { Month= "Feb", Revenue1= 1800 },
                 new Revenue { Month= "Mar", Revenue1= 2200 },
                 new Revenue { Month= "Apr", Revenue1= 2500 },
                 new Revenue { Month= "May", Revenue1= 2300 },
                 new Revenue { Month= "Jun", Revenue1= 3200 },
                 new Revenue { Month= "Jul", Revenue1= 3500 },
                 new Revenue { Month= "Aug", Revenue1= 3700 },
                 new Revenue { Month= "Sep", Revenue1= 2500 },
                 new Revenue { Month= "Oct", Revenue1= 2800 },
                 new Revenue { Month= "Nov", Revenue1= 3000 },
                 new Revenue { Month= "Dec", Revenue1= 4800 },
            };

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Customer>().HasData(customers);
            modelBuilder.Entity<Invoice>().HasData(invoices);
            modelBuilder.Entity<Revenue>().HasData(revenues);
        }
    }
}
