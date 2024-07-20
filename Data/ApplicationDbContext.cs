using Microsoft.EntityFrameworkCore;
using RingoMediaTask.Models;

namespace RingoMediaTask.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
    }


}
