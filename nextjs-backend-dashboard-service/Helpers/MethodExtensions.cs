using Microsoft.EntityFrameworkCore;
using nextjs_backend_dashboard_service.Models;

namespace nextjs_backend_dashboard_service.Helpers
{
    public static class MethodExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Revenue>().HasData(revenues);
        }
    }
}
