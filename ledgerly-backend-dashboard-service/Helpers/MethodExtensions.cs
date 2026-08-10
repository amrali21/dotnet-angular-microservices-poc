using Microsoft.EntityFrameworkCore;
using ledgerly_backend_dashboard_service.Models;

namespace ledgerly_backend_dashboard_service.Helpers
{
    public static class MethodExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            int year = DateTime.UtcNow.Year;
            List<Revenue> revenues = new List<Revenue>()
            {
                 new Revenue { Month= "Jan", Year = year, Revenue1= 2000 },
                 new Revenue { Month= "Feb", Year = year, Revenue1= 1800 },
                 new Revenue { Month= "Mar", Year = year, Revenue1= 2200 },
                 new Revenue { Month= "Apr", Year = year, Revenue1= 2500 },
                 new Revenue { Month= "May", Year = year, Revenue1= 2300 },
                 new Revenue { Month= "Jun", Year = year, Revenue1= 3200 },
                 new Revenue { Month= "Jul", Year = year, Revenue1= 3500 },
                 new Revenue { Month= "Aug", Year = year, Revenue1= 3700 },
                 new Revenue { Month= "Sep", Year = year, Revenue1= 2500 },
                 new Revenue { Month= "Oct", Year = year, Revenue1= 2800 },
                 new Revenue { Month= "Nov", Year = year, Revenue1= 3000 },
                 new Revenue { Month= "Dec", Year = year, Revenue1= 4800 },
            };

            modelBuilder.Entity<Revenue>().HasData(revenues);
        }
    }
}
