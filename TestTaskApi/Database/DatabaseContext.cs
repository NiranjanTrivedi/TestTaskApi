using Microsoft.EntityFrameworkCore;
using TestTaskApi.DTOs;

namespace TestTaskApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
