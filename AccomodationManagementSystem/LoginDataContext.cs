using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccomodationManagementSystem
{
    public class LoginDataContext:DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = loginFile.db");
        }

        public DbSet<loginInfo> m_loginInfo { get; set; } = null!;



    }
}
