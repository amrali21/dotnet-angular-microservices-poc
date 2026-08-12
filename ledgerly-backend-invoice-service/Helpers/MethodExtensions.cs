using Microsoft.EntityFrameworkCore;
using ledgerly_backend.Models;

namespace ledgerly_backend.Helpers
{
    public static class MethodExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Customer ids below match the customers seeded in cust-service's
            // own database (ledgerly-customer) — kept in sync by hand since
            // invoice-service no longer owns the Customers table.
            List<Invoice> invoices = new List<Invoice> {
               new Invoice
               {
                   Id = 1,
                CustomerId= "3958dc9e-712f-4377-85e9-fec4b6a6442a", // Delba de Oliveira
                Amount= 15795,
                Status= "pending",
                Date= DateTime.Parse("2022-12-06"),
              },
              new Invoice{
                Id = 2,
                CustomerId= "3958dc9e-742f-4377-85e9-fec4b6a6442a", // Lee Robinson
                Amount= 20348,
                Status= "pending",
                Date= DateTime.Parse("2022-11-14"),
              },
              new Invoice{
                Id = 3,
                CustomerId= "3958dc9e-787f-4377-85e9-fec4b6a6442a", // Steph Dietz
                Amount= 3040,
                Status= "paid",
                Date= DateTime.Parse("2022-10-29"),
              },
             new Invoice {
                Id = 4,
                CustomerId= "50ca3e18-62cd-11ee-8c99-0242ac120002", // Steven Tey
                Amount= 44800,
                Status= "paid",
                Date= DateTime.Parse("2023-09-10"),
              },
              new Invoice{
                Id = 5,
                CustomerId= "76d65c26-f784-44a2-ac19-586678f7c2f2", // Michael Novotny
                Amount= 34577,
                Status= "pending",
                Date= DateTime.Parse("2023-08-05"),
              },
             new Invoice {
                Id = 6,
                CustomerId= "126eed9c-c90c-4ef6-a4a8-fcf7408d3c66", // Emil Kowalski
                Amount= 54246,
                Status= "pending",
                Date= DateTime.Parse("2023-07-16"),
              },
             new Invoice {
                Id = 7,
                CustomerId= "d6e15727-9fe1-4961-8c5b-ea44a9bd81aa", // Evil Rabbit
                Amount= 666,
                Status= "pending",
                Date= DateTime.Parse("2023-06-27"),
              },
             new Invoice {
                Id = 8,
                CustomerId= "50ca3e18-62cd-11ee-8c99-0242ac120002", // Steven Tey
                Amount= 32545,
                Status= "paid",
                Date= DateTime.Parse("2023-06-09"),
              },
             new Invoice {
                Id = 9,
                CustomerId= "3958dc9e-787f-4377-85e9-fec4b6a6442a", // Steph Dietz
                Amount= 1250,
                Status= "paid",
                Date= DateTime.Parse("2023-06-17"),
              },
             new Invoice {
                Id = 10,
                CustomerId= "76d65c26-f784-44a2-ac19-586678f7c2f2", // Michael Novotny
                Amount= 8546,
                Status= "paid",
                Date= DateTime.Parse("2023-06-07"),
              },
              new Invoice{
                Id = 11,
                CustomerId= "3958dc9e-742f-4377-85e9-fec4b6a6442a", // Lee Robinson
                Amount= 500,
                Status= "paid",
                Date= DateTime.Parse("2023-08-19"),
              },
             new Invoice {
                Id = 12,
                CustomerId= "76d65c26-f784-44a2-ac19-586678f7c2f2", // Michael Novotny
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-06-03"),
              },
              new Invoice{
                Id = 13,
                CustomerId= "3958dc9e-737f-4377-85e9-fec4b6a6442a", // Hector Simpson
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-06-18"),
              },
             new Invoice {
                Id = 14,
                CustomerId= "3958dc9e-712f-4377-85e9-fec4b6a6442a", // Delba de Oliveira
                Amount= 8945,
                Status= "paid",
                Date= DateTime.Parse("2023-10-04"),
              },
             new Invoice {
                Id = 15,
                CustomerId= "3958dc9e-737f-4377-85e9-fec4b6a6442a", // Hector Simpson
                Amount= 1000,
                Status= "paid",
                Date= DateTime.Parse("2022-06-05"),
              },
            };

            modelBuilder.Entity<Invoice>().HasData(
                invoices
            );
        }
    }
}
