using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccomodationManagementSystem.VacancyDatabaseClasses
{
    public class AccomodationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = accommodation.db");
        }

        public DbSet<roomInfo> m_rooms { get; set; } = null!;
        public DbSet<bookingInfo> m_bookings { get; set; } = null!;


    }
}
